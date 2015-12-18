#include <MySensors\MySensor.h>
#include <SPI\SPI.h>

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

void loop() {

	ledStrip.Loop();
	readButton();

	gw.process();

	saveStateToEEPROM();
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


unsigned long lastSaveTime = 0;
void saveStateToEEPROM() {
	if (millis() - lastSaveTime > 600000) {
		resetEEPROMTimer();

		byte r = ledStrip.Get_r();
		byte g = ledStrip.Get_g();
		byte b = ledStrip.Get_b();

		if (r != preset_r || g != preset_g  || b != preset_b)
		{			
			preset_r = r;
			preset_g = g;
			preset_b = b;
			storePreset();
		}
	}
}

void resetEEPROMTimer(){
	lastSaveTime = millis();
}

/*
void LED_on() {
	Serial.print("Led ON: ");

	char *arg;
	unsigned int fade_time;

	arg = serialCommand.next();
	if (arg != NULL) {
		fade_time = atoi(arg);
		Serial.println(fade_time);
		ledStrip.TurnOn(fade_time);
	}
	else
	{
		Serial.println("OK");
		ledStrip.TurnOn();
	}

	buttonSwitch = true;
}


void LED_off() {
	Serial.print("Led OFF: ");

	char *arg;
	unsigned int fade_time;

	arg = serialCommand.next();
	if (arg != NULL) {
		fade_time = atoi(arg);
		Serial.println(fade_time);
		ledStrip.TurnOff(fade_time);
	}
	else
	{
		Serial.println("OK");
		ledStrip.TurnOff();
	}

	buttonSwitch = false;
}



void fade_to_preset()
{
	readPreset();

	Serial.print("Fade to preset: ");

	char *arg;
	unsigned int fade_time;

	arg = serialCommand.next();
	if (arg != NULL) {
		fade_time = atoi(arg);
		Serial.println(fade_time);
	}
	else
	{
		Serial.println("bad arguments");
		return;
	}

	Serial.println(preset_r);
	Serial.println(preset_g);
	Serial.println(preset_b);

	ledStrip.FadeToColor(preset_r, preset_g, preset_b, fade_time);
}








void getState() {
	Serial.print("State: ");
	Serial.print(ledStrip.Get_r());
	Serial.print(" ");
	Serial.print(ledStrip.Get_g());
	Serial.print(" ");
	Serial.print(ledStrip.Get_b());
	Serial.print(" ");
	Serial.println(ledStrip.IsOn());
}

void setcolor() {

	Serial.print("Set color: ");

	int r = 0, g = 0, b = 0;
	char *arg;

	arg = serialCommand.next();
	if (arg != NULL) {
		r = atoi(arg);
	}
	else
	{
		Serial.println("bad arguments");
		return;
	}

	arg = serialCommand.next();
	if (arg != NULL) {
		g = atoi(arg);
	}
	else
	{
		Serial.println("bad arguments");
		return;
	}

	arg = serialCommand.next();
	if (arg != NULL) {
		b = atoi(arg);
	}
	else
	{
		Serial.println("bad arguments");
		return;
	}

	Serial.print(r);
	Serial.print(" ");
	Serial.print(g);
	Serial.print(" ");
	Serial.println(b);

	preset_r = r;
	preset_g = g;
	preset_b = b;

	ledStrip.SetColor(r, g, b);
}


void fadecolor() {

	Serial.print("Fade color: ");

	int r = 0, g = 0, b = 0, time = 0;
	char *arg;

	arg = serialCommand.next();
	if (arg != NULL) {
		r = atoi(arg);
	}
	else
	{
		Serial.println("bad arguments");
		return;
	}

	arg = serialCommand.next();
	if (arg != NULL) {
		g = atoi(arg);
	}
	else
	{
		Serial.println("bad arguments");
		return;
	}

	arg = serialCommand.next();
	if (arg != NULL) {
		b = atoi(arg);
	}
	else
	{
		Serial.println("bad arguments");
		return;
	}

	arg = serialCommand.next();
	if (arg != NULL) {
		time = atoi(arg);
	}
	else
	{
		Serial.println("bad arguments");
		return;
	}

	Serial.print(r);
	Serial.print(" ");
	Serial.print(g);
	Serial.print(" ");
	Serial.print(b);
	Serial.print(" ");
	Serial.println(time);

	preset_r = r;
	preset_g = g;
	preset_b = b;

	ledStrip.FadeToColor(r, g, b, time);
}



void strobe() {

	Serial.print("Strobe: ");

	unsigned int on_duration, off_duration, times;

	char *arg;

	arg = serialCommand.next();
	if (arg != NULL) {
		on_duration = atoi(arg);
	}
	else
	{
		Serial.println("bad arguments");
		return;
	}

	arg = serialCommand.next();
	if (arg != NULL) {
		off_duration = atoi(arg);
	}
	else
	{
		Serial.println("bad arguments");
		return;
	}

	arg = serialCommand.next();
	if (arg != NULL) {
		times = atoi(arg);
	}
	else
	{
		Serial.println("bad arguments");
		return;
	}


	Serial.print(on_duration);
	Serial.print(" ");
	Serial.print(off_duration);
	Serial.print(" ");
	Serial.println(times);


	ledStrip.Strobe(on_duration, off_duration, times);
}

void stopstrobe() {
	Serial.print("Stop strobe");
	ledStrip.StopStrobe();
}



void unrecognized(const char *command) {
	Serial.println("Available commands:");
	Serial.println("on - turn on leds");
	Serial.println("off - turn off leds");
	Serial.println("on 2000 - turn on leds, fade time = 2000");
	Serial.println("off 2000 - turn off leds, fade time = 2000");
	Serial.println("fade 150 100 50 1000 - fade color to R:150,G:100,B:50, fade time = 1000");
	Serial.println("fadetopreset 1000 - fade to color stored in EEPROM, fade time = 1000");
	Serial.println("color 150 100 50 - change color to R:150,G:100,B:50");
	Serial.println("strobe 100 900 3 - strobe 3 times, 100 ms - on, 900 ms - off");
	Serial.println("stopstrobe - stop strobing");
	Serial.println("storepreset - store current color to EEPROM");
	Serial.println("state - get leds state (R,G,B,isOn)");

}



*/


