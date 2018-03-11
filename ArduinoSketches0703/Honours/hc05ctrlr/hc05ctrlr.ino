
#include "Boards.h"
#include <RBL_nRF8001.h>
#include <SPI.h>
#include <EEPROM.h>
#include <SoftwareSerial.h>

// Button variables
int button1State = 0;
bool button1Pressed = false;
int button2State = 0;
bool button2Pressed = false;

// Magnetic sensor - reed switch variables
int reedSwitchState = 0;
bool magnetDetected = false;

// Joystick variables
int joystickState = 0;
int x, y;
bool left, right;

// Pairing variables
bool paired = false;
String message = "";
int pairConfirmation = 0;

// HC-05 bluetooth comms
SoftwareSerial bluetooth(0, 1);

void setup() {
  // Button pins
  pinMode(8, INPUT);
  pinMode(5, INPUT);
  // Reed switch pin
  pinMode(9, INPUT_PULLUP);

  // Start serial - baud rate must match in c# script
  Serial.begin(9600);

  bluetooth.begin(9600);
  
}

void loop() {
  
    // Act on joystick state
    detect_joystick();

    // Act on button 1 state
    detect_button_1();

    // Act on button 2 state
    detect_button_2();

    // Detect magnet on pedal; sends info out if detected
    detect_magnet();

   
} // end main loop


// Reads joystick values, sends out direction to game
void detect_joystick()
{
  // Read in x axis value
  x = analogRead(3);

  // If closer to 1023 above midpoint 800 then going left
  if (x > 950 && joystickState != -1)
  {
    joystickState = -1;
    bluetooth.write("2");
  }
  // On other side, going right
  else if (x < 700 && joystickState != 1)
  {
     joystickState = 1;
     bluetooth.write("1");
  }
  // If within normal range, not going left or right
  else if (x > 750 && x < 900)
  {
    if (joystickState != 0)
    {
       bluetooth.write("0"); 
       joystickState = 0; 
    }
  }

  // Read in y axis value
  //y = analogRead(3);
}

// Reads magnetic sensor, sends out data if magnet detected
void detect_magnet()
{
  // Check for reed switch state
  reedSwitchState = digitalRead(9);
  // LOW when magnetic field nearby
  if (reedSwitchState == LOW)
  {
    // Check if this is first detection
    if (!magnetDetected)
    {
      // Send message
      bluetooth.write("5");
      // Magnet is now detected - don't do this again
      magnetDetected = true;
    }
  } 
  // HIGH when no magnetic field nearby
  else
  {
    // Flip flag
    magnetDetected = false;
  }

} // end detect_magnet()




// Reads button 1 state, sends info out
void detect_button_1()
{
   // Check for button 1 state
  button1State = digitalRead(8);
  // If it's pressed
  if(button1State == LOW)
  {
    // Check if it this is first press
    if (!button1Pressed)
    {
      // Send message 
      bluetooth.write("6"); 
      
      // Flip switch
      button1Pressed = true;
    }
    
  }
  // Button 1 released
  else
  {
    // Check if it was just pressed
    if (button1Pressed)
    {
      // Send message
      //Serial.println("0");
      // Flip switch
      button1Pressed = false;
    }
  }
} // end detect_button_1()




// Check button 2 state, send out data
void detect_button_2()
{
  // Check for button 2 state
  button2State = digitalRead(5);
  // If it's pressed
  if (button2State == LOW)
  {
    // Check if this is first press
    if (!button2Pressed)
    {
      // Send message
      bluetooth.write("7");
      // Flip switch
      button2Pressed = true;
    }
  }
  else // Btn 2 not pressed
  {
    // Check if it was just pressed
    if (button2Pressed)
    {
      // Send message
      //Serial.println("0");
      // Flip switch
      button2Pressed = false;
    }
  }
} // end detect_button_2()

