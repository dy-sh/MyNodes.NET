/*
* Arduino Led Library
*
* Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
* License: http://opensource.org/licenses/MIT
*/

#include "LedStrip.h"


LedStrip::LedStrip(int r_pin, int g_pin, int b_pin,bool invertPWM)
{
	led_r = Led(r_pin, invertPWM);
	led_g = Led(g_pin, invertPWM);
	led_b = Led(b_pin, invertPWM);
}


void LedStrip::SetColor(int r, int g, int b) {
	led_r.SetBrightness(r);
	led_g.SetBrightness(g);
	led_b.SetBrightness(b);
}

void LedStrip::SetColor(uint32_t color) {
	SetColor((color & 0xFF0000) >> 16, (color & 0x00FF00) >> 8, color & 0x0000FF);
}

void LedStrip::FadeToColor(uint8_t r, uint8_t g, uint8_t b, unsigned int fadeTime) {
	led_r.StopFade();
	led_g.StopFade();
	led_b.StopFade();
	led_r.Fade(r, fadeTime);
	led_g.Fade(g, fadeTime);
	led_b.Fade(b, fadeTime);
}

void LedStrip::Strobe(unsigned int on_duration, unsigned int off_duration, unsigned int times) {
	led_r.Strobe(on_duration, off_duration, times);
	led_g.Strobe(on_duration, off_duration, times);
	led_b.Strobe(on_duration, off_duration, times);
}

void LedStrip::StopStrobe() {
	led_r.StopStrobe();
	led_g.StopStrobe();
	led_b.StopStrobe();
}

void LedStrip::Loop()
{

	led_r.Loop();
	led_g.Loop();
	led_b.Loop();
}

int LedStrip::Get_r()
{
	return led_r.GetBrightness();
}

int LedStrip::Get_g()
{
	return led_g.GetBrightness();
}
int LedStrip::Get_b()
{
	return led_b.GetBrightness();
}


void LedStrip::TurnOn() {
	led_r.TurnOn();
	led_g.TurnOn();
	led_b.TurnOn();
}

void LedStrip::TurnOff() {
	led_r.TurnOff();
	led_g.TurnOff();
	led_b.TurnOff();
}

void LedStrip::TurnOn(unsigned int fadeTime) {
	led_r.TurnOn(fadeTime);
	led_g.TurnOn(fadeTime);
	led_b.TurnOn(fadeTime);
}

void LedStrip::TurnOff(unsigned int fadeTime) {
	led_r.TurnOff(fadeTime);
	led_g.TurnOff(fadeTime);
	led_b.TurnOff(fadeTime);
}

bool LedStrip::IsOn()
{
	return led_r.IsOn()
		&& led_g.IsOn()
		&& led_b.IsOn();
}

