/*
* Arduino Led Library
*
* Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
* License: http://opensource.org/licenses/MIT
*/

#include "Arduino.h"

#ifndef Led_H_
#define Led_H_

class Led {
public:
	Led(uint8_t pin=13, bool invertPWM =false);
	void Loop();
	void SetBrightness(uint8_t brightness);
	uint8_t GetBrightness();
	uint8_t GetCurrentValue();
	void TurnOn();
	void TurnOff();
	bool IsOn();



	//STROBE
	void Strobe(unsigned int on_duration, unsigned int off_duration, unsigned int times);
	void StopStrobe();
	bool IsStrobing();

	//FADE
	void Fade(uint8_t target_brightness, unsigned int fade_time);
	void StopFade();
	bool IsFading();
	uint8_t GetFadeTargetBrightness();
	unsigned int GetFadeTimeRamain();

	//FADE ON/OFF
	void TurnOn(unsigned int fade_time);
	void TurnOff(unsigned int fade_time);
	bool IsOnOffFading();


private:
	uint8_t pin;
	uint8_t brightness;
	uint8_t current_value;
	bool isOn;
	void WritePWM();
	bool invertPWM;

	//STROBE
	bool isStrobing;
	bool strobe_current_brightness;
	bool strobe_isOn_before;
	unsigned int strobe_on_duration;
	unsigned int strobe_off_duration;
	unsigned int strobe_times_remain;
	unsigned long strobe_last_time;
	void CalculateStrobe();

	//FADE
	uint8_t fade_target_brightness;
	unsigned int fade_time_remain;
	unsigned long fade_last_step_time;
	unsigned int fade_step_duration;
	void CalculateFade();

	//FADE ON/OFF
	uint8_t fadeOnOff_current_brightness;
	uint8_t fadeOnOff_target_brightness;
	unsigned int fadeOnOff_time_remain;
	unsigned long fadeOnOff_last_step_time;
	unsigned int fadeOnOff_step_duration;
	void CalculateFadeOnOff();
	
};

#endif /* Led_H_ */
