
#include <MySensor.h>
#include <SPI.h>
#include <Bounce2.h>

#define CHILD_ID1 3
#define CHILD_ID2 4
#define BUTTON_PIN1  3  // Arduino Digital I/O pin for button/reed switch
#define BUTTON_PIN2  4  // Arduino Digital I/O pin for button/reed switch

MySensor gw;
Bounce debouncer1 = Bounce();
Bounce debouncer2 = Bounce();
int oldValue1 = -1;
int oldValue2 = -1;
bool val;

// Change to V_LIGHT if you use S_LIGHT in presentation below
MyMessage msg1(CHILD_ID1, V_TRIPPED);
MyMessage msg2(CHILD_ID2, V_TRIPPED);



#define RELAY_1  3  // Arduino Digital I/O pin number for first relay (second on pin+1 etc)
#define NUMBER_OF_RELAYS 2 // Total number of attached relays
#define RELAY_ON 1  // GPIO value to write to turn on attached relay
#define RELAY_OFF 0 // GPIO value to write to turn off attached relay


void setup()
{
	gw.begin(incomingMessage, AUTO, true);



	// Setup the button
	pinMode(BUTTON_PIN1, INPUT_PULLUP);
	pinMode(BUTTON_PIN2, INPUT_PULLUP);
	// Activate internal pull-up


	// After setting up the button, setup debouncer
	debouncer1.attach(BUTTON_PIN1);
	debouncer2.attach(BUTTON_PIN2);
	debouncer1.interval(50);
	debouncer2.interval(50);

	// Register binary input sensor to gw (they will be created as child devices)
	// You can use S_DOOR, S_MOTION or S_LIGHT here depending on your usage. 
	// If S_LIGHT is used, remember to update variable type you send in. See "msg" above.
	gw.present(CHILD_ID1, S_DOOR, "Button1");

	gw.present(CHILD_ID2, S_DOOR, "Button2");
	gw.sendSketchInfo("Firmware1", "v1.1");




	// Fetch relay status
	for (int sensor = 1, pin = RELAY_1; sensor <= NUMBER_OF_RELAYS; sensor++, pin++) {
		// Register all sensors to gw (they will be created as child devices)
		gw.present(sensor, S_LIGHT);
	}
}


//  Check if digital input has changed and send in new value
void loop()
{
	gw.process();

	debouncer1.update();
	debouncer2.update();
	// Get the update value
	int value = debouncer1.read();

	if (value != oldValue1) {
		// Send in the new value
		gw.send(msg1.set(value));
		oldValue1 = value;
	}

	value = debouncer2.read();

	if (value != oldValue2) {
		// Send in the new value
		gw.send(msg2.set(value));
		oldValue2 = value;
	}

	//	val = !val;
	//	gw.send(msg2.set(val));
}





void incomingMessage(const MyMessage &message) {
	// We only expect one type of message from controller. But we better check anyway.
	if (message.type == V_LIGHT) {
		// Change relay state

		// Write some debug info
		Serial.print("Incoming change for sensor:");
		Serial.print(message.sensor);
		Serial.print(", New status: ");
		Serial.println(message.getBool());
	}
}
