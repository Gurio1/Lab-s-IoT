# IoT Temperature Monitoring and Door Control System

This project involves two Internet of Things (IoT) systems:
1. **Temperature Monitoring**: It uses three temperature thermometers to measure and monitor the temperature in real-time.
2. **Door Control**: An IoT system to control a door in the laboratory, allowing it to be opened remotely.

## Features

- **Temperature Monitoring System**:
  - Uses three thermometers to monitor the temperature in various parts of the laboratory.
  - Real-time temperature updates sent via MQTT to a web-based interface.
  - Display data in charts for visualization.

- **Door Control System**:
  - Controls a door in the laboratory.
  - Can open the door remotely based on user input.
- **Authentication**:
  - No registration
  - Will be used existing system:
    - Active Directory
    - Google Active Directory(Gmail) P.S Im not sure
## Technologies Used

- **Hardware**:
  - ESP32 microcontroller
  - Arduino Uno(It just already existed and it needs to be used) and ENC28J60 Ethernet module
  - Relay for door control
  - Temperature sensors (thermometers)

- **Software**:
  - **Frontend**: Angular for the user interface.
  - **Backend**: ASP.NET Core with MQTT communication for device control and data collection.
  - **MQTT Broker** for messaging between devices and the backend(Broker is inside ASP.NET).
  - **Mongo DB** - Not implemented yet
  - **SignalR** - for real-time updates of temperature charts.

---
