#include <Arduino.h>
#include "pwmWrite.h"
Pwm servo = Pwm();

int ch1, ch2;
float ch1Intense, ch2Intense, prech1Intense, prech2Intense;
float maxIntense = 250;
int myTimeout = 150;

void setup() {
    Serial.print("Ch1: ");
  Serial.begin(115200);

  // init signal pinout
  ch1 = 9;
  ch2 = 8;

  // reset all servos to original angle
  resetServo();
}

void resetServo() {
  servo.writeServo(ch1, 0);
  servo.writeServo(ch2, 0);
  delay(30);
  Serial.setTimeout(myTimeout);
}


void loop() {
  // servo.writeServo(ch1, 100);
  //   servo.writeServo(ch2, 180);
  // servo.writeServo(ch1, 0);
  //   servo.writeServo(ch2, 0);
  if (Serial.available()) {
    String command = Serial.readStringUntil('\n');
    int idx = command.indexOf(',');
    prech1Intense = ch1Intense;
    prech2Intense = ch2Intense;

    if (command.substring(0, idx).toFloat() != -1) {
      ch1Intense = 13.0 * command.substring(0, idx).toFloat() / 18.0;
    }
    if (command.substring(idx + 1).toFloat() != -1) {
      ch2Intense = 13.0 * command.substring(idx + 1).toFloat() / 18.0;
    }

    ch1Intense = ch1Intense > maxIntense ? maxIntense : ch1Intense;
    ch2Intense = ch2Intense > maxIntense ? maxIntense : ch2Intense;
    prech1Intense = ch1Intense;
    prech2Intense = ch2Intense;

    servo.writeServo(ch1, ch1Intense);
    servo.writeServo(ch2, ch2Intense);

    Serial.print("Ch1: ");
    Serial.print(ch1Intense);
    Serial.print(", Ch2: ");
    Serial.println(ch2Intense);

    // give delay
    // 目前討論是在unity端控制每個command的delay,
    // 馬達每轉動1度，耗時2.68ms
    // 下面是一個小靈感
    // 紀錄兩顆馬達分別上一個轉到的角度以及現在要轉到的角度，
    // 乘上2.68後找最大的那個，就是這輪馬達會需要的最少轉動時間

     //float delMax = max(abs(prech1Intense - ch1Intense), abs(prech2Intense - ch2Intense));
     //Serial.print("delay: ");
     //Serial.println(delMax);
    // delay(delMax * 2.68);
    // prech1Intense = ch1Intense; prech2Intense = ch2Intense;
  }
}