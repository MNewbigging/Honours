int hallEffectState = 0;

void setup() {
  // Declare pins to be used
  pinMode(5, INPUT_PULLUP);

  Serial.begin(9600);
  pinMode(2, INPUT);
}

void loop() {
  // Read sensor state
  hallEffectState = digitalRead(2);

  Serial.println(hallEffectState);

}


