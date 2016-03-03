
#include <MySensors/MySensor.h>
#include <Bounce2/Bounce2.h>

#define COUNTER_ID 1
#define LED_ID 10
#define LED_PIN  10  // Arduino pin for led

MySensor gw;
MyMessage msg(COUNTER_ID, V_TRIPPED);
int value = 0;


void incomingMessage(const MyMessage &message) {
	if (message.type == V_TRIPPED) {
		bool val = message.getBool();
		digitalWrite(LED_PIN, val);
		//Serial.print("Led changed. new state: ");
		//Serial.println(val);
	}
}


void setup()
{
	gw.begin(incomingMessage);

	pinMode(LED_PIN, OUTPUT);

	gw.present(COUNTER_ID, S_CUSTOM);
	gw.present(LED_ID, S_DOOR);
}


void loop()
{
	gw.process();

	gw.send(msg.set(value));

	value++;
	//gw.wait(100);
}

