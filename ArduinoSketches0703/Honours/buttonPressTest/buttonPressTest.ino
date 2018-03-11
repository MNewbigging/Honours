
#include "Boards.h"
#include <RBL_nRF8001.h>

int buttonLState = 0; 
int buttonRState = 0;
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
  
  buttonLState = digitalRead(8);
  
  if (buttonLState == LOW)
  {
    Serial.println("BTN01");
  }

  buttonRState = digitalRead(5);

  if (buttonRState == LOW)
  {
    Serial.println("BTN02");
  }
}
