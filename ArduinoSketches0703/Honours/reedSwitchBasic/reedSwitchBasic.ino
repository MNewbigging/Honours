// This test turns LED off on button press. Largely just to find pin numbers!


int switchState = 0;

void setup() {

  pinMode(5, INPUT_PULLUP);
  pinMode(8, OUTPUT);
  // serial begin here
  Serial.begin(9600);
  Serial.println("hello");
}
  // pulse width modulation
  // analogue input digital input pin
  // check if blend has pull up resistor on board



  
void loop() {


// Read sensor pin (HIGH when no magnetic field detected)
switchState = digitalRead(5);

if (switchState == LOW)
{
  digitalWrite(8, HIGH);
}
else
{
  digitalWrite(8, LOW);
}



 
}
