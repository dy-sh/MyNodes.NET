#include <SPI.h>
#include <MySensor.h>  
#include <DHT.h>  

bool debugEnabled = true;

#define DHT_PIN 2
#define PHOTORESISTOR_PIN A1
#define BATTERY_PIN A0
unsigned long SLEEP_TIME = 298000;//sleep 5 min (2000ms DHT waiting)


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

int batteryLast = 0;




void setup()
{
	// use the 1.1 V internal reference
#if defined(__AVR_ATmega2560__)
	analogReference(INTERNAL1V1);
#else
	analogReference(INTERNAL);
#endif

	gw.begin();
	dht.setup(DHT_PIN);

	gw.sendSketchInfo("Weather Station", "1.0");

	gw.present(DHT_HUM_ID, S_HUM);
	gw.present(DHT_TEMP_ID, S_TEMP);
	gw.present(PHOTORESISTOR_ID, S_LIGHT_LEVEL);

	pinMode(PHOTORESISTOR_PIN, INPUT);
}





void SendDHT() {
	//waiting for collect DHT11 data
	gw.wait(2000);

	float temperature = dht.getTemperature();

	if (isnan(temperature)) {
		if (debugEnabled)
			Serial.println("Failed reading temperature from DHT");
	}
	else if (temperature != lastTemp) {
		lastTemp = temperature;
		gw.send(msgTemp.set(temperature, 1));
		if (debugEnabled) {
			Serial.print("T: ");
			Serial.println(temperature);
		}
	}

	float humidity = dht.getHumidity();
	if (isnan(humidity)) {
		if (debugEnabled)
			Serial.println("Failed reading humidity from DHT");
	}
	else if (humidity != lastHum) {
		lastHum = humidity;
		gw.send(msgHum.set(humidity, 1));
		if (debugEnabled) {
			Serial.print("H: ");
			Serial.println(humidity);
		}
	}
}

void SendLight()
{
	//int state = (1023 - analogRead(PHOTORESISTOR_PIN)) / 10.23;
	int state = (analogRead(PHOTORESISTOR_PIN)) / 10.23;
	if (state != lastLight) {
		gw.send(msgLight.set(state));
		if (debugEnabled) {
			Serial.print("Light: ");
			Serial.println(state);
		}
		lastLight = state;
	}
}

void SendBattery()
{
	int val = analogRead(BATTERY_PIN);

	// 1M, 470K divider across battery and using internal ADC ref of 1.1V
	// Sense point is bypassed with 0.1 uF cap to reduce noise at that point
	// ((1e6+470e3)/470e3)*1.1 = Vmax = 3.44 Volts
	// 3.44/1023 = Volts per bit = 0.003363075
	float batteryV = val * 0.003363075;
	//int batteryPcnt = val / 10;

	//remap from min/max voltage
	int batteryPcnt = map(val, 500, 900, 0, 100);
	batteryPcnt = constrain(batteryPcnt, 0, 100);




	//to prevent noise
	if (batteryPcnt - batteryLast > 0 && batteryPcnt - batteryLast < 5)
		batteryPcnt = batteryLast;

	//if (batteryLast != batteryPcnt) {
	gw.sendBatteryLevel(batteryPcnt);
	batteryLast = batteryPcnt;
	//}

	if (debugEnabled) {
		Serial.print("Battery: ");
		Serial.println(batteryLast);
	}
}


void loop()
{


	SendDHT();
	SendLight();
	SendBattery();

	gw.sleep(SLEEP_TIME);
}