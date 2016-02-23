/*
* Arduino Led Library
*
* Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
* License: http://opensource.org/licenses/MIT
*/


//#include <Arduino.h>
#include "Led.h"


class LedStrip
{
public:
	LedStrip(int r_pin, int g_pin, int b_pin, bool invertPWM = false);
	void SetColor(int r, int g, int b);
	void SetColor(uint32_t);
	void FadeToColor(uint8_t r, uint8_t g, uint8_t b, unsigned int fadeTime);
	void Strobe(unsigned int on_duration, unsigned int off_duration, unsigned int times);
	void StopStrobe();
	void Loop();
	void TurnOn();
	void TurnOff();
	void TurnOn(unsigned int fadeTime);
	void TurnOff(unsigned int fadeTime);
	int Get_r();
	int Get_g();
	int Get_b();
	bool IsOn();
private:
	Led led_r, led_g, led_b;
};


