
#include <SPI.h>
#include <MySensor.h>  
#include <DHT.h>  

#define HUM_ID 0
#define TEMP_ID 1
#define DHT_PIN 2
unsigned long SLEEP_TIME = 3000;

MySensor gw;
DHT dht;
float lastTemp;
float lastHum;
boolean metric = true;
MyMessage msgHum(HUM_ID, V_HUM);
MyMessage msgTemp(TEMP_ID, V_TEMP);


void setup()
{
	gw.begin();
	dht.setup(DHT_PIN);

	gw.sendSketchInfo("Temperature sensor", "1.0");

	gw.present(HUM_ID, S_HUM);
	gw.present(TEMP_ID, S_TEMP);

	metric = gw.getConfig().isMetric;
}

void loop()
{
	gw.sleep(SLEEP_TIME); //sleep a bit

	float temperature = dht.getTemperature();
	if (isnan(temperature)) {
		Serial.println("Failed reading temperature from DHT");
	}
	else if (temperature != lastTemp) {
		lastTemp = temperature;
		if (!metric) {
			temperature = dht.toFahrenheit(temperature);
		}
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

