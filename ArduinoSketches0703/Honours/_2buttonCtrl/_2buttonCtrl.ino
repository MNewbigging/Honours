
#include "Boards.h"
#include <RBL_nRF8001.h>

// 21/01
// 2 button controller; provides left/right rotation in game
// Last Edit: 
// Changed outgoing messages to numbers, so that it can be parsed easily in c# 

int button1State = 0;
bool button1Pressed = false;

int button2State = 0;
bool button2Pressed = false;

int reedSwitchState = 0;

bool paired = false;

String message = "";
int pairConfirmation = 0;

void setup() {
  // Button pins
  pinMode(8, INPUT);
  pinMode(5, INPUT);
  // Reed switch pin
  pinMode(9, INPUT_PULLUP);

  // Start serial - baud rate must match in c# script
  Serial.begin(9600);

  // Set searchable name
  //ble_set_name("Blend Micro");
  
  // Init and start library
  //ble_begin();
}





void loop() {
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
  if (paired)
  {
    // Detect magnet on pedal; sends info out if detected
    detect_magnet();

    // Act on button 1 state
    detect_button_1();

    // Act on button 2 state
    detect_button_2();
 
  } // end if paired
} // end main loop




// Reads magnetic sensor, sends out data if magnet detected
void detect_magnet()
{
  // Check for reed switch state
  reedSwitchState = digitalRead(9);
  // HIGH when no magnetic field nearby
  if (reedSwitchState == LOW)
  {
    Serial.println("2");
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
      Serial.println("-1");
      
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
      Serial.println("0");
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
      Serial.println("1");
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
      Serial.println("0");
      // Flip switch
      button2Pressed = false;
    }
  }
} // end detect_button_2()

