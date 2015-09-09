#include <MySensor.h>
#include <SPI.h>
#include <IRLib.h>


int RECV_PIN = 9;

#define CHILD_1  0  // childId

IRsend irsend;
IRrecv irrecv(RECV_PIN); 
IRdecode decoder;
//decode_results results;
unsigned int Buffer[RAWBUF];
MySensor gw;
MyMessage msg(CHILD_1, V_IR_RECEIVE);

void setup()
{
	irrecv.enableIRIn(); // Start the ir receiver
	decoder.UseExtnBuf(Buffer);
	gw.begin(incomingMessage);

	// Send the sketch version information to the gateway and Controller
	gw.sendSketchInfo("IR Sensor", "1.0");

	// Register a sensors to gw. Use binary light for test purposes.
	gw.present(CHILD_1, S_IR);
}


void loop()
{
	gw.process();
	if (irrecv.GetResults(&decoder)) {
		irrecv.resume();
		decoder.decode();
		decoder.DumpResults();
	
		char buffer[10];
		sprintf(buffer, "%08lx", decoder.value);
		// Send ir result to gw
		gw.send(msg.set(buffer));
	}
}



void incomingMessage(const MyMessage &message) {
	if (message.type == V_IR_SEND) {

		String codeHex = message.getString();

		long code = hexToDec(codeHex);
		irsend.send(RC6, code, 20);

		Serial.println(code);



		// Start receiving ir again...
		//irrecv.enableIRIn();
	}
}


long hexToDec(String hexString) {

	long decValue = 0;
	int nextInt;

	for (int i = 0; i < hexString.length(); i++) {

		nextInt = int(hexString.charAt(i));
		if (nextInt >= 48 && nextInt <= 57) nextInt = map(nextInt, 48, 57, 0, 9);
		if (nextInt >= 65 && nextInt <= 70) nextInt = map(nextInt, 65, 70, 10, 15);
		if (nextInt >= 97 && nextInt <= 102) nextInt = map(nextInt, 97, 102, 10, 15);
		nextInt = constrain(nextInt, 0, 15);

		decValue = (decValue * 16) + nextInt;
	}

	return decValue;
}