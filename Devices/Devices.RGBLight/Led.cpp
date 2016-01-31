/*
* Arduino Led Library
*
* Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
* License: http://opensource.org/licenses/MIT
*/


#include "Led.h"



Led::Led(uint8_t pin, bool invertPWM) {
	this->invertPWM = invertPWM;
	this->pin = pin;

	isOn = false;


	strobe_isOn_before = false;
	strobe_times_remain = 0;
	isStrobing = false;

	fade_time_remain = 0;
	current_value = 0;

	fadeOnOff_current_brightness = 0;
	fadeOnOff_time_remain = 0;

	SetBrightness(brightness);
}


void Led::TurnOn() {
	fadeOnOff_current_brightness = 100;
	fadeOnOff_time_remain = 0;

	if (IsStrobing()) {
		strobe_isOn_before = true;
		StopStrobe();
	}
	else
	{
		isOn = true;
		WritePWM();
	}
}


void Led::TurnOff() {
	fadeOnOff_current_brightness = 0;
	fadeOnOff_time_remain = 0;

	if (IsStrobing()) {
		strobe_isOn_before = false;
		StopStrobe();
	}
	else
	{
		isOn = false;
		WritePWM();
	}
}


bool Led::IsOn() {
	return isOn;
}


void Led::WritePWM()
{

	//	Serial.print(fadeOnOff_current_brightness);
	//	Serial.print(" ");


	if (!isOn)
		current_value = 0;
	else {
		current_value = brightness;

		//		Serial.print(current_value);
		//		Serial.print(" ");
		current_value = (uint32_t)(current_value*fadeOnOff_current_brightness / 100);
		//		Serial.print(current_value);
		//		Serial.println(" ");
	}

	//!!!! INVERTED PWM VALUES !!!!
	if (invertPWM)
		analogWrite(pin, 255 - current_value);
	else
		analogWrite(pin, current_value);

}


void Led::SetBrightness(uint8_t brightness) {

	if (IsFading())
		StopFade();

	brightness = constrain(brightness, 0, 255);
	this->brightness = brightness;

	WritePWM();
}

uint8_t Led::GetBrightness() {
	return brightness;
}

uint8_t Led::GetCurrentValue() {
	return current_value;
}

void Led::Loop() {
	CalculateFade();
	CalculateStrobe();
	CalculateFadeOnOff();

	WritePWM();
}






///------------------------FADE------------------------


void Led::Fade(uint8_t target_brightness, unsigned int fade_time) {

	fade_time_remain = 0;

	if (fade_time == 0) {
		SetBrightness(target_brightness);
		return;
	}

	target_brightness = constrain(target_brightness, 0, 255);

	fade_time_remain = fade_time;
	fade_last_step_time = millis();
	fade_target_brightness = target_brightness;


	int bright_diff = abs(brightness - fade_target_brightness);
	if (bright_diff == 0) {
		fade_time_remain = 0;
		return;
	}
	fade_step_duration = round(fade_time_remain / bright_diff);
}


void Led::StopFade() {
	fade_time_remain = 0;
}


bool Led::IsFading() {
	return fade_time_remain > 0;
}

uint8_t Led::GetFadeTargetBrightness() {
	return fade_target_brightness;
}

unsigned int Led::GetFadeTimeRamain() {
	return fade_time_remain;
}


void Led::CalculateFade()
{
	if (!IsFading())
		return;

	unsigned long now = millis();
	unsigned int time_diff = now - fade_last_step_time;


	if (time_diff < fade_step_duration)
		return;

	float percent = (float)time_diff / (float)fade_time_remain;

	if (percent >= 1) {

		brightness = fade_target_brightness;
		fade_time_remain = 0;
		return;
	}

	int bright_diff = fade_target_brightness - brightness;
	int increment = round(bright_diff * percent);

	brightness += increment;
	brightness = constrain(brightness, 0, 255);

	fade_time_remain -= time_diff;
	fade_last_step_time = millis();
}



///------------------------END FADE------------------------








///------------------------ON/OFF FADE------------------------



void Led::TurnOn(unsigned int fade_time) {
	if (IsStrobing()) {
		strobe_isOn_before = true;
		StopStrobe();
	}

	if (fade_time == 0) {
		TurnOn();
		return;
	}

	isOn = true;

	fadeOnOff_time_remain = fade_time;
	fadeOnOff_last_step_time = millis();
	fadeOnOff_target_brightness = 100;

	int bright_diff = abs(fadeOnOff_current_brightness - fadeOnOff_target_brightness);
	if (bright_diff == 0) {
		fadeOnOff_time_remain = 0;
		return;
	}
	fadeOnOff_step_duration = round(fadeOnOff_time_remain / bright_diff);
}

void Led::TurnOff(unsigned int fade_time) {
	if (IsStrobing()) {
		strobe_isOn_before = true;
		StopStrobe();
	}

	if (fade_time == 0) {
		TurnOff();
		return;
	}

	fadeOnOff_time_remain = fade_time;
	fadeOnOff_last_step_time = millis();
	fadeOnOff_target_brightness = 0;

	int bright_diff = abs(fadeOnOff_current_brightness - fadeOnOff_target_brightness);
	if (bright_diff == 0) {
		fadeOnOff_time_remain = 0;
		return;
	}
	fadeOnOff_step_duration = round(fadeOnOff_time_remain / bright_diff);
}



bool Led::IsOnOffFading() {
	return fadeOnOff_time_remain > 0;
}



void Led::CalculateFadeOnOff()
{
	if (!IsOnOffFading())
		return;

	unsigned long now = millis();
	unsigned int time_diff = now - fadeOnOff_last_step_time;


	if (time_diff < fadeOnOff_step_duration)
		return;

	float percent = (float)time_diff / (float)fadeOnOff_time_remain;

	if (percent >= 1) {
		fadeOnOff_current_brightness = fadeOnOff_target_brightness;
		fadeOnOff_time_remain = 0;

		if (fadeOnOff_target_brightness == 0)
			isOn = false;
	}

	int bright_diff = fadeOnOff_target_brightness - fadeOnOff_current_brightness;
	int increment = round(bright_diff * percent);

	fadeOnOff_current_brightness += increment;
	fadeOnOff_current_brightness = constrain(fadeOnOff_current_brightness, 0, 100);

	fadeOnOff_time_remain -= time_diff;
	fadeOnOff_last_step_time = millis();
}



///------------------------END ON/OFF FADE------------------------






///------------------------STROBE------------------------
//times = 0 means unlimited strobing
void Led::Strobe(unsigned int on_duration, unsigned int off_duration, unsigned int times) {
	if (!IsStrobing()) {
		isStrobing = true;
		strobe_last_time = millis();
		strobe_isOn_before = isOn;


	}

	strobe_on_duration = on_duration;
	strobe_off_duration = off_duration;
	strobe_times_remain = times;

	fadeOnOff_current_brightness = fadeOnOff_target_brightness;
	fadeOnOff_time_remain = 0;
}

void Led::StopStrobe() {
	isStrobing = false;
	strobe_times_remain = 0;
	isOn = strobe_isOn_before;

	WritePWM();
}

bool Led::IsStrobing() {
	return isStrobing;
}

void Led::CalculateStrobe()
{
	if (!IsStrobing())
		return;

	unsigned long now = millis();
	unsigned int time_diff = now - strobe_last_time;

	strobe_current_brightness = true;

	if (time_diff >= strobe_on_duration)
	{
		strobe_current_brightness = false;
	}

	isOn = strobe_current_brightness;

	if (time_diff >= strobe_on_duration + strobe_off_duration)
	{
		strobe_last_time = millis();

		//0 means unlimited strobing
		if (strobe_times_remain > 0) {
			strobe_times_remain -= 1;
			if (strobe_times_remain == 0)
				StopStrobe();
		}
	}


}

///------------------------END STROBE------------------------