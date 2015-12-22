<img alt="" src="https://raw.githubusercontent.com/derwish-pro/MyNetSensors/master/icons/MyNetSensors_banner2.png" >

Official website with detailed documentation: <a href="http://www.mynetsensors.com"> MyNetSensors.com </a><br />

## What is this app for?

With this program, you can easily create your smart devices, automate them, schedule them, monitor state changes of other devices and respond to them, as well as generate graph charts for visualising activity of various sensors.

<img alt="" src="https://raw.githubusercontent.com/derwish-pro/MyNetSensors/master/WebController/Screen1.png" >

All devices are connected over the wireless network. No wires!

The program automatically recognizes devices and allows to control them through a web interface.

This can be a part of your smart home, with many automated systems - lighting, windows, doors, climate control or just the control panel for the simplest device on the Arduino.

## What you need to know/be able to run this system?

All you need to know is how to connect Arduino with a sensor or device that you want to automate, and flash the firmware in Arduino.

Assembling device takes as less as 5 minutes.

## What hardware is needed?

The minimum hardware required to make it work - two Arduino boards and two radio modules NRF24L01+.

<img alt="" src="https://raw.githubusercontent.com/derwish-pro/MyNetSensors/master/SerialGateway.Device/Screen1.png" >

One set is connected to the computer, and the second is the basis of your smart device.

By the way, minimum smart device (Arduino Pro Mini + NRF24L01+) should cost about $3.

Assembly is very simple, see the instructions.

Assembly is very simple, see the <a class="fadebefore" href="http://www.mynetsensors.com/Hardware/Gateway">instructions</a>.


## How it works?

All devices communicate through the gateway.

<img alt="" src="https://raw.githubusercontent.com/derwish-pro/MyNetSensors/master/SerialGateway.Device/Screen2.png" >

The gateway handles network routing, records nodes state to the database, keeps statistics.

Devices can relay messages for each other, providing wireless connectivity at a great distance from the gateway.

The gateway managed by the computer, where MyNetSensors running. Separately, run the web server, which allows you to control devices over the Internet.

## Is it free?

This program is open source (GNU General Public License), you can download and use it absolutely for free.

Read more on <a href="http://www.mynetsensors.com"> MyNetSensors.com </a>

Mail to info@mynetsensors.com

