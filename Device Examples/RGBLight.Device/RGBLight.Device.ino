// Example sketch showing how to control an RGB Led Strip.
// This example will not remember the last rgb color set after power failure.

#include <MySensor.h>
#include <SPI.h>

#define RED  3  // Arduino PWM pin for Red
#define GREEN 9 // Arduino PWM pin for Green
#define BLUE 10  // Arduino PWM pin for Blue
#define strip 1  // sensor number needed in the custom devices set up

int RGB_pins[3] = { RED,GREEN,BLUE };
long RGB_values[3] = { 0,0,0 };

MySensor gw;

void setup()
{
	// Initialize library and add callback for incoming messages
	gw.begin(incomingMessage);

	// Send the sketch version information to the gateway and Controller
	gw.sendSketchInfo("RGB Node", "1.0");

	// Register the sensor to gw
	gw.present(strip, S_RGB_LIGHT);

	// Set the rgb pins in output mode
	for (int i = 0; i<3; i++) {
		pinMode(RGB_pins[i], OUTPUT);
	}
}


void loop()
{
	// Alway process incoming messages whenever possible
	gw.process();
}

void incomingMessage(const MyMessage &message) {
	// We only expect one type of message from controller. But we better check anyway.
	if (message.type == V_RGB) {
		// starting to process the hex code
		String hexstring = message.getString(); //here goes the hex color code coming from Pidome through MySensors (like FF9A00)
		long number = (long)strtol(&hexstring[0], NULL, 16);
		RGB_values[0] = number >> 16;
		RGB_values[1] = number >> 8 & 0xFF;
		RGB_values[2] = number & 0xFF;

		for (int i = 0; i<3; i++) {
			analogWrite(RGB_pins[i], RGB_values[i]);
		}

		// Write some debug info
		Serial.print("Red is ");
		Serial.println(RGB_values[0]);
		Serial.print("Green is ");
		Serial.println(RGB_values[1]);
		Serial.print("Blue is ");
		Serial.println(RGB_values[2]);


	}
}