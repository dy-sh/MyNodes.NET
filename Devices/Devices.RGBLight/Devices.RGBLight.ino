#include <MySensor.h>
#include <SPI.h>

#include "LedStrip.h"
#include <EEPROM.h>
#include <Bounce2.h>


#define LED_STRIP_INVERT_PWM true

LedStrip ledStrip(3, 9, 10, LED_STRIP_INVERT_PWM);

#define BUTTON_PIN 2
#define LIGHT_FADE_TIME 2000

#define POWER_ID 1
#define RGB_ID 2


Bounce debouncer = Bounce();
bool lightEnabled;

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
	gw.send(powerMessage.set(lightEnabled));
}


void resetEEPROMTimer() {
	lastSaveTime = millis();
}

long RGB_values[3] = { 0,0,0 };

void incomingMessage(const MyMessage &message) {
	if (message.type == V_RGB) {
		resetEEPROMTimer();

		String hexstring = message.getString();

		hexstring[6] = '\0';

		//long number = (long)strtol(&hexstring[0], NULL, 16);
		//RGB_values[0] = number >> 16;
		//RGB_values[1] = number >> 8 & 0xFF;
		//RGB_values[2] = number & 0xFF;
		//Serial.print("Red is ");
		//Serial.println(RGB_values[0]);
		//Serial.print("Green is ");
		//Serial.println(RGB_values[1]);
		//Serial.print("Blue is ");
		//Serial.println(RGB_values[2]);

		long val = (long)strtol(&hexstring[0], NULL, 16);

		ledStrip.SetColor(val >> 16, val >> 8 & 0xFF, val & 0xFF);

		Serial.print("Set color: ");
		Serial.println(hexstring);


	}
	if (message.type == V_STATUS) {
		lightEnabled = message.getBool();

		if (lightEnabled)
			ledStrip.TurnOn(LIGHT_FADE_TIME);
		else
			ledStrip.TurnOff(LIGHT_FADE_TIME);

		Serial.print("Set power: ");
		Serial.println(lightEnabled);
	}
}



void switchLedState()
{
	lightEnabled = !lightEnabled;

	Serial.print("Switch: ");
	Serial.println(lightEnabled);

	if (lightEnabled)
		ledStrip.TurnOn(LIGHT_FADE_TIME);
	else
		ledStrip.TurnOff(LIGHT_FADE_TIME);

	sendCurrentPower();

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

	gw.begin(incomingMessage);

	Serial.begin(115200);
	Serial.println("Starting RGB Node...");

	//set last color
	ledStrip.TurnOff();
	readPreset();
	ledStrip.SetColor(preset_r, preset_g, preset_b);

	pinMode(BUTTON_PIN, INPUT_PULLUP);

	lightEnabled = false;
	debouncer.attach(BUTTON_PIN);
	debouncer.interval(25);



	Serial.println("Presenting...");


	gw.sendSketchInfo("RGB Node", "2.0");

	gw.present(POWER_ID, S_BINARY);
	gw.present(RGB_ID, S_RGB_LIGHT);

	sendCurrentPower();
	sendCurrentRGB();

	Serial.println("Ready");

}


void loop() {
	gw.process();

	ledStrip.Loop();

	//read button
	debouncer.update();
	if (debouncer.fell()) {
		Serial.println(millis());
		switchLedState();
	}

	saveStateToEEPROM();
}
