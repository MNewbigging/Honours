
#include "Boards.h"
#include <RBL_nRF8001.h>

// 21/01
// 2 button controller; provides left/right rotation in game
// Last Edit: 
// Changed outgoing messages to numbers, so that it can be parsed easily in c#

int button1State = 0;
int button2State = 0;
bool button1Pressed = false;
bool button2Pressed = false;

String message = "";

void setup() {
  // Button pins
  pinMode(8, INPUT);
  pinMode(5, INPUT);

  // Start serial
  Serial.begin(9600);

  // Set searchable name
  //ble_set_name("Blend Micro");
  
  // Init and start library
  //ble_begin();
}

void loop() {

  // Check for button 1 state
  button1State = digitalRead(8);
  // If it's pressed
  if(button1State == LOW)
  {
    // Check if it this is first press
    if (!button1Pressed)
    {
      // Send message
      Serial.println("BTN 1 DOWN");
      
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
      Serial.println("BTN 1 UP");
      // Flip switch
      button1Pressed = false;
    }
  }

  // Check for button 2 state
  button2State = digitalRead(5);
  // If it's pressed
  if (button2State == LOW)
  {
    // Check if this is first press
    if (!button2Pressed)
    {
      // Send message
      Serial.println("BTN 2 DOWN");
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
      Serial.println("BTN 2 UP");
      // Flip switch
      button2Pressed = false;
    }
  }
  
 
  
  
}
