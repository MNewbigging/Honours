
#include "Boards.h"
#include <RBL_nRF8001.h>
#include <SPI.h>
#include <EEPROM.h>
#include <SoftwareSerial.h>

#define BLUETOOTH_SPEED 38400

// 31/01
// 2 button controller; provides left/right rotation in game
// Last Edit: 
// Changed button state call order to set magnet last (always sends multiple messages on one magnet field pickup) 

int button1State = 0;
bool button1Pressed = false;

int button2State = 0;
bool button2Pressed = false;

int reedSwitchState = 0;
bool magnetDetected = false;

bool paired = false;

String message = "";
int pairConfirmation = 0;

int joystickState = 0;
int x, y;
bool left, right;



void setup() {
  // Button pins
  pinMode(8, INPUT);
  pinMode(5, INPUT);
  // Reed switch pin
  pinMode(9, INPUT_PULLUP);

 

  
  // Start serial - baud rate must match in c# script
  Serial.begin(9600);

  ble_set_pins(3,2);
  // Set searchable name
  ble_set_name("Blend Micro");
  
  // Init and start library
  ble_begin();

}





void loop() {

 
  // Broadcast availability to pair

   if (ble_connected())
   {
    //Serial.println("1");
    ble_write('1');
   }
   else
   {
    //Serial.println("0");
    ble_write('0');
   }
    ble_do_events();
  // Perform pairing check once data available to read
  if (Serial.available() > 0)
  {
    // Read in a byte of data 
    pairConfirmation = Serial.read();
    // Check we received a P
    if (pairConfirmation == 80)
    {
      paired = true;
      Serial.println("Paired");
    }
  }

  // Once paired, perform main loop
  while (paired)
  {

    detect_joystick();

    // Act on button 1 state
    detect_button_1();

    // Act on button 2 state
    detect_button_2();

    // Detect magnet on pedal; sends info out if detected
    detect_magnet();

    // Listen for unpairing message
    if (Serial.available() > 0)
    {
      // Read in a byte of data
      pairConfirmation = Serial.read();
      if (pairConfirmation == 85)
      {
        Serial.println("Unpaired");
        // Unpair - break out of main loop
        paired = false;
      }
    }
 
  } // end if paired
} // end main loop


// Reads joystick values, sends out direction to game
void detect_joystick()
{
  // Read in x axis value
  x = analogRead(3);

  // If closer to 1023 above midpoint 800 then going left
  if (x > 900 && joystickState != -1)
  {
    joystickState = -1;
    Serial.println("-1");
  }
  // On other side, going right
  else if (x < 700 && joystickState != 1)
  {
     joystickState = 1;
     Serial.println("1");
  }
  // If within normal range, not going left or right
  else if (x > 700 && x < 900)
  {
    if (joystickState != 0)
    {
       Serial.println("0"); 
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
      Serial.println("2");
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
      Serial.println("3"); 
      
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
      Serial.println("4");
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

