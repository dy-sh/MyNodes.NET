#include <MySensor.h>
#include <SPI.h>

#include "LedStrip.h"
#include <EEPROM.h>



#define LED_STRIP_INVERT_PWM true

LedStrip ledStrip(3, 9, 10, LED_STRIP_INVERT_PWM);

#define ButtonPin 2
#define TurnOnOffFadeTime 2000

#define POWER_ID 1
#define RGB_ID 2


bool lastButtonState;
bool buttonState;
long lastDebounceTime = 0;
#define DebounceDelay 50
bool buttonSwitch;


uint8_t preset_r, preset_g, preset_b;

MySensor gw;

MyMessage powerMessage(POWER_ID, V_STATUS);
MyMessage rgbMessage(RGB_ID, V_RGB);


unsigned long lastSaveTime = 0;


void sendCurrentRGB() {
	//long RGB = ((long)preset_r << 16L) | ((long)preset_g << 8L) | (long)preset_b;
	long RGB = ((long)ledStrip.Get_r() << 16L) | ((long)ledStrip.Get_g() << 8L) | (long)ledStrip.Get_b();
	char charBuf[6];
	String(RGB, HEX).toCharArray(charBuf, 7);

	gw.send(rgbMessage.set(charBuf));
}

void sendCurrentPower() {
	gw.send(powerMessage.set(buttonSwitch));
}


void resetEEPROMTimer() {
	lastSaveTime = millis();
}



void incomingMessage(const MyMessage &message) {
	if (message.type == V_RGB) {
		resetEEPROMTimer();

		String hexstring = message.getString();
		long val = (long)strtol(&hexstring[0], NULL, 16);

		ledStrip.SetColor(val >> 16, val >> 8 & 0xFF, val & 0xFF);

		Serial.print("Set color: ");
		Serial.println(hexstring);
	}
	if (message.type == V_STATUS) {
		buttonSwitch = message.getBool();

		if (buttonSwitch)
			ledStrip.TurnOn(TurnOnOffFadeTime);
		else
			ledStrip.TurnOff(TurnOnOffFadeTime);

		Serial.print("Set power: ");
		Serial.println(buttonSwitch);
	}
}



void switchLedState()
{
	buttonSwitch = !buttonSwitch;

	if (buttonSwitch)
		ledStrip.TurnOn(TurnOnOffFadeTime);
	else
		ledStrip.TurnOff(TurnOnOffFadeTime);

	sendCurrentPower();

	Serial.print("Switch: ");
	Serial.println(buttonSwitch);
}

void readButton()
{
	bool reading = digitalRead(ButtonPin);


	if (reading != lastButtonState)
		lastDebounceTime = millis();


	if ((millis() - lastDebounceTime) > DebounceDelay) {
		if (reading != buttonState) {
			buttonState = reading;

			if (buttonState)
				switchLedState();
		}
	}


	lastButtonState = reading;
}


void storePreset() {
	Serial.println("Preset stored in EEPROM");
	gw.saveState(0, preset_r);
	gw.saveState(1, preset_g);
	gw.saveState(2, preset_b);

	sendCurrentPower();
	sendCurrentRGB();
}

void readPreset() {
	preset_r = gw.loadState(0);
	preset_g = gw.loadState(1);
	preset_b = gw.loadState(2);
}


void saveStateToEEPROM() {
	if (millis() - lastSaveTime > 600000) {
		resetEEPROMTimer();

		byte r = ledStrip.Get_r();
		byte g = ledStrip.Get_g();
		byte b = ledStrip.Get_b();

		if (r != preset_r || g != preset_g || b != preset_b)
		{
			preset_r = r;
			preset_g = g;
			preset_b = b;
			storePreset();
		}
	}
}



void setup() {

	// Initialize library and add callback for incoming messages
	gw.begin(incomingMessage);

	// Send the sketch version information to the gateway and Controller
	gw.sendSketchInfo("RGB Node", "2.0");

	// Register the sensor to gw
	gw.present(POWER_ID, S_BINARY);
	gw.present(RGB_ID, S_RGB_LIGHT);


	pinMode(ButtonPin, INPUT_PULLUP);

	lastButtonState = digitalRead(ButtonPin);
	buttonState = lastButtonState;
	buttonSwitch = false;


	//set last color
	ledStrip.TurnOff();
	readPreset();
	ledStrip.SetColor(preset_r, preset_g, preset_b);
	//	switchLedState();

	//send current color to gateway
	sendCurrentPower();
	sendCurrentRGB();

	Serial.println("Led strip is ready");
}


void loop() {

	ledStrip.Loop();
	readButton();

	gw.process();

	saveStateToEEPROM();
}
