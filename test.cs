//test update
#include "s4d_breadboard.h"
int pass_number = analogRead( POTENTIOMETER );
int initialMagnetValue;
int passcode[3] = {1,2,3};
int code[3] = {0,0,0};
bool alarm;

void setup() {
  initializeBreadboard();
  initialMagnetValue = analogRead(MAGNETSENSOR);
  alarm = false;
}

//Mayhem
int buzzer(){
  for(int i = 0; i<200; i++){
     digitalWrite(LED_GREEN, HIGH); //mijn alarm is knipper light ipv geluid.
     delay(50);
     digitalWrite(LED_GREEN, LOW);
     delay(50);
   }
}

//max functie
int max_f(int x, int y){
  if(x>y){
    return x;
  }
  return y;
}


//invoer van key
void keycode(bool interrupt){
  lampjes(LED_YELLOW);
  int t = floor(millis()/1000);
  bool too_slow = false;
  for(int i = 0; i<3; i++){
    while(!digitalRead(BUTTON1) && !too_slow){
      code[i] = keypad();
      int t_div = max_f(0,20-( floor( (millis() /1000) )-t));
      lcd_keypad(code, t_div, interrupt);
      too_slow = t_div==0 && interrupt;
    }
    delay(200);
  }
}




// regelt de alarm knoppen
bool alarm_button1(){ //okay
  if( digitalRead(BUTTON1) ) {
     return true;
    } else {
    return false;
    }
}

bool alarm_button2(){ //cancel
    if( digitalRead(BUTTON2) ) {
      return true;
    } else {
    return false;
    }
}

// regelt de deur
bool doorSwitch(){
  int currentMagnetValue = analogRead(MAGNETSENSOR);
  int difMagnetValue = abs(currentMagnetValue - initialMagnetValue);
  if(difMagnetValue<60){
    difMagnetValue = 0;
  }
  return ( difMagnetValue >= 60);
}

//input van de keypad
int keypad() {
  pass_number = floor(analogRead( POTENTIOMETER )/100);
  if (pass_number > 9){
    pass_number = 9;
  }
  return pass_number;
}

//regelt de lampjes
void lampjes(int color){
  digitalWrite(LED_RED, LOW);
  digitalWrite(LED_GREEN, LOW);
  digitalWrite(LED_YELLOW, LOW);
  digitalWrite(LED_BLUE, LOW);

  digitalWrite(color, HIGH);//door && alarm
}

//regelt de displays
void lcd_keypad(int* arraykeycode, int t, bool interrupt){
  if(interrupt){
    OLED.printTop( String(arraykeycode[0])+" | "+String(arraykeycode[1])+" | "+String(arraykeycode[2])+" time left: "+String(t));//door && !alarm
  }
  else{
    OLED.printTop( String(arraykeycode[0])+" | "+String(arraykeycode[1])+" | "+String(arraykeycode[2])+" insert new");
  }
}


//loop
void loop() {
  OLED.printTop("");
  if(doorSwitch()){
    lampjes(LED_BLUE);
  }
  else{
    lampjes(LED_GREEN);
  }

  if(digitalRead(BUTTON2)){

    keycode(false);
    passcode[0] = code[0];
    passcode[1] = code[1];
    passcode[2] = code[2];

  }

  if(digitalRead(BUTTON1)){
    lampjes(LED_RED);
    while(!doorSwitch()){
      delay(200);
    }
    keycode(true);
    if(passcode[0] != code[0] || passcode[1] != code[1] || passcode[2] != code[2]){
      while(true){
        buzzer();
      }
    }
  }
}
