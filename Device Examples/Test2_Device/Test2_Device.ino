
#include <SPI.h>
#include <MySensor.h>  
#include <DHT.h>  

#define DHT_PIN 2
#define PHOTORESISTOR_PIN A0
unsigned long SLEEP_TIME = 2000;


#define DHT_HUM_ID 0
#define DHT_TEMP_ID 1
#define PHOTORESISTOR_ID 2

MySensor gw;

DHT dht;
float lastTemp;
float lastHum;
float lastLight;
MyMessage msgHum(DHT_HUM_ID, V_HUM);
MyMessage msgTemp(DHT_TEMP_ID, V_TEMP);
MyMessage msgLight(PHOTORESISTOR_ID, V_LIGHT_LEVEL);




void setup()
{
	gw.begin();
	dht.setup(DHT_PIN);

	gw.sendSketchInfo("Arduino Uno", "1.0");

	gw.present(DHT_HUM_ID, S_HUM);
	gw.present(DHT_TEMP_ID, S_TEMP);

	gw.present(PHOTORESISTOR_ID, S_LIGHT_LEVEL);
}

void loop()
{
	gw.wait(SLEEP_TIME);

	SendDHT();
	SendLight();


}

void SendDHT() {
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
	int lightLevel = (1023 - analogRead(PHOTORESISTOR_PIN)) / 10.23;
	if (lightLevel != lastLight) {
		gw.send(msgLight.set(lightLevel));
		Serial.print("Light: ");
		Serial.println(lightLevel);
		lastLight = lightLevel;
	}
}