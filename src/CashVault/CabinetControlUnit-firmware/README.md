
## Release Notes

### Version 1.0.0
- **New Features:**
   - Added support for **Temperature Sensors**:
      - **DHT11** (default)
      - **DHT22**
      - **DS18B20**
   - Added support for **Vibration Sensor** SW-420.

- **Hardware Changes:**
   - Pin assignments for **Door Sensors** have been updated to accommodate the interrupt requirements for the **Vibration Sensor**.
   - Ensure to review and update your hardware connections accordingly.

- **Notes:**
   - For detailed pin configurations, refer to the source code or detailed documentation for soldering.

- **Known Issues:**
   - Vibration sensor sensitivity too high.

### Version 0.0.1 (Initial Implementation)
- Supported **Door Sensors** only on Arduino pins.
- This version was intended for initial testing and development purposes.

---

# Setting Up Visual Studio Code for PlatformIO

This guide will walk you through the steps to set up Visual Studio Code (VS Code) for development with PlatformIO, a development tool for embedded systems.

## Prerequisites

Before you start, make sure you have the following installed:
- [Visual Studio Code](https://code.visualstudio.com/)
- [Python 3.8 or later](https://www.python.org/downloads/)

## Step 1: Install the PlatformIO Extension

1. Open Visual Studio Code.
2. Click on the Extensions icon (or press `Ctrl+Shift+X`).
3. In the search field, type "PlatformIO IDE".
4. Click the `Install` button next to the "PlatformIO IDE".

---

# PlatformIO Project - Arduino Uno and Arduino Nano

This project includes two environments for developing firmware for Arduino Uno and Arduino Nano boards. Follow the steps below to open the project, select a platform, compile, and upload the firmware to the board.

## Steps

### 1. Opening the Project
1. Open Visual Studio Code.
2. Click on **File -> Open Folder** and select the directory where your project files are located.

### 2. Selecting an Environment
1. Once the project is open, check the **platformio.ini** file. Two environments are defined:
   - `uno` (for Arduino Uno)
   - `nano` (for Arduino Nano)
2. On left side of Visual Studio Code, click on **PlatformIO** icon to open PlatformIO extension.
3. Navigate to the **Project Tasks** section.
4. From the list of available environments, select the one you want to work with (`uno` or `nano`) or use Deafult environment
to build all available environments.

### 3. Compiling the Firmware
1. In the **PlatformIO** menu, navigate to the **Project Tasks** section.
2. Click on the desired environment (e.g., `uno` or `nano`).
3. Choose the **Build** option from **General** section to compile your code.

### 4. Uploading the Firmware
1. Connect the Arduino board to your computer via a USB cable.
2. In the **PlatformIO** menu, select the **Upload** option for the chosen environment. This will automatically compile and upload the firmware to your board.
3. Wait for the process to complete and verify that the board functions as expected.

---

If you encounter any issues, refer to the [PlatformIO documentation](https://docs.platformio.org/) or reach out to the PlatformIO community for support.

---
