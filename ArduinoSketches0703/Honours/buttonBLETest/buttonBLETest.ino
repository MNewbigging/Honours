
#include "Boards.h"
#include <RBL_nRF8001.h>



int buttonState = 0;
bool buttonPressed = false;
String message = "";

void setup() {
  
  pinMode(8, INPUT);
  Serial.begin(9600);

  // Set searchable name
  //ble_set_name("Blend Micro");
  
  // Init and start library
  //ble_begin();
}

void loop() {
  
  buttonState = digitalRead(8);

  if(buttonState == LOW)
  {
    buttonPressed = true;
  }
  // Button released
  else
  {
    // Check if it was just pressed
    if (buttonPressed)
    {
      // Send message
      Serial.println("BTN01");
      // Flip switch
      buttonPressed = false;
    }
  }

  
 
  
  
}
