#include <SPI.h>
#include <MySensor.h>  
#include <DHT.h>  

#define MOTION_PIN 5
#define DHT_PIN 2
#define PHOTORESISTOR_PIN A0
unsigned long SLEEP_TIME = 100;


#define DHT_HUM_ID 0
#define DHT_TEMP_ID 1
#define PHOTORESISTOR_ID 2
#define MOTION_ID 3

MySensor gw;

DHT dht;
float lastTemp;
float lastHum;
float lastLight;
unsigned long lastDHTTime;
int lastMotion;
MyMessage msgHum(DHT_HUM_ID, V_HUM);
MyMessage msgTemp(DHT_TEMP_ID, V_TEMP);
MyMessage msgLight(PHOTORESISTOR_ID, V_LIGHT_LEVEL);
MyMessage msgMotion(MOTION_ID, V_TRIPPED);



void setup()
{
	gw.begin();
	dht.setup(DHT_PIN);

	gw.sendSketchInfo("Arduino Uno", "1.0");

	gw.present(DHT_HUM_ID, S_HUM);
	gw.present(DHT_TEMP_ID, S_TEMP);
	gw.present(PHOTORESISTOR_ID, S_LIGHT_LEVEL);
	gw.present(MOTION_ID, S_MOTION);

	lastDHTTime = millis();

	pinMode(MOTION_PIN, INPUT);
	pinMode(PHOTORESISTOR_PIN, INPUT);
}

void loop()
{
	gw.wait(SLEEP_TIME);

	SendDHT();
	SendLight();
	SendMotion();

}

void SendDHT() {
	if (millis() - lastDHTTime < 2000)
		return;

	lastDHTTime = millis();

	float temperature = dht.getTemperature();

	if (isnan(temperature)) {
		Serial.println("Failed reading temperature from DHT");
	}
	else if (temperature != lastTemp) {
		lastTemp = temperature;
		gw.send(msgTemp.set(temperature, 1));
		Serial.print("T: ");
		Serial.println(temperature);
	}

	float humidity = dht.getHumidity();
	if (isnan(humidity)) {
		Serial.println("Failed reading humidity from DHT");
	}
	else if (humidity != lastHum) {
		lastHum = humidity;
		gw.send(msgHum.set(humidity, 1));
		Serial.print("H: ");
		Serial.println(humidity);
	}
}

void SendLight()
{
	int state = (1023 - analogRead(PHOTORESISTOR_PIN)) / 10.23;
	if (state != lastLight) {
		gw.send(msgLight.set(state));
		Serial.print("Light: ");
		Serial.println(state);
		lastLight = state;
	}
}

void SendMotion()
{
	int state = digitalRead(MOTION_PIN); 
	if (state != lastMotion) {
		gw.send(msgMotion.set(state ? "1" : "0"));
		Serial.print("Motion: ");
		Serial.println(state);
		lastMotion = state;
	}
}
