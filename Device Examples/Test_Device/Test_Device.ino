
#include <MySensor.h>
#include <SPI.h>
#include <Bounce2.h>

#define BUTTON1_ID 3
#define BUTTON2_ID 4
#define LED1_ID 1

#define BUTTON_PIN1  3  // Arduino Digital I/O pin for button/reed switch
#define BUTTON_PIN2  4  // Arduino Digital I/O pin for button/reed switch
#define LED1_PIN 9

MySensor gw;
Bounce debouncer1 = Bounce();
Bounce debouncer2 = Bounce();
int oldValue1 = -1;
int oldValue2 = -1;

//bool val;

// Change to V_LIGHT if you use S_LIGHT in presentation below
MyMessage msg1(BUTTON1_ID, V_TRIPPED);
MyMessage msg2(BUTTON2_ID, V_TRIPPED);



void setup()
{
	pinMode(LED1_PIN, OUTPUT);
	
	gw.begin(incomingMessage);
	

	pinMode(BUTTON_PIN1, INPUT_PULLUP);
	pinMode(BUTTON_PIN2, INPUT_PULLUP);


	debouncer1.attach(BUTTON_PIN1);
	debouncer2.attach(BUTTON_PIN2);
	debouncer1.interval(50);
	debouncer2.interval(50);


	gw.present(BUTTON1_ID, S_DOOR);
	gw.present(BUTTON2_ID, S_DOOR, "Button2");

	gw.present(LED1_ID, S_DIMMER);

}



void loop()
{
	gw.process();
	debouncer1.update();
	debouncer2.update();

	int value = debouncer1.read();
	if (value != oldValue1) {
		gw.send(msg1.set(value));
		oldValue1 = value;
	}

	value = debouncer2.read();
	if (value != oldValue2) {
		gw.send(msg2.set(value));
		oldValue2 = value;
	}

	//	val = !val;
	//	gw.send(msg2.set(val));
}





void incomingMessage(const MyMessage &message) {

	if (message.isAck()) {
		Serial.println("Received ack from gateway");
	}

	if (message.type == V_DIMMER) {
		if (message.sensor == LED1_ID) {
			int state = message.getByte();
			state = map(state, 0, 100, 0, 255);
			analogWrite(LED1_PIN, state);
		}


	}

	Serial.print("Received change steate for sensor ");
	Serial.print(message.sensor);
	Serial.print(", new state: ");
	Serial.println(message.data);
}
