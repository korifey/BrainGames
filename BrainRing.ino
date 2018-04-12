int BLINK_PERIOD = 2000;

int ledPin = 13;
//pins
int input[2];
int indicator[2];

int globalState;
int STATE_FREE = 0;
int STATE_QUESTION = 1;
int STATE_FALSESTARTED = 2;
int STATE_STARTED = 3;
int STATE_ANSWERING = 4;
int STATE_ENDED = 5;

bool blocked[2];
int actor;
long time;

void reset() {
  setState(STATE_FREE);
  digitalWrite(indicator[0], LOW);
  digitalWrite(indicator[1], LOW);
  
  blocked[0] = false;
  blocked[1] = false;
  
  time = 0;
}

void setup() {
  Serial.begin(9600);
  
  // put your setup code here, to run once:
  pinMode(ledPin, OUTPUT);
  
  input[0] = 2;
  pinMode(input[0], INPUT);
  
  input[1] = 3;
  pinMode(input[1], INPUT);
  
  indicator[0] = 4;
  pinMode(indicator[0], OUTPUT);
  
  indicator[1] = 5;
  pinMode(indicator[1], OUTPUT);
 
  reset();
}

void setState(int state) {
  globalState = state;
  Serial.println(state);
}


void handlePress(int i) {
  //Serial.print("handle ");
  //Serial.println(i);
  if (blocked[i]) return;
  
  if (globalState == STATE_FREE) {
    digitalWrite(indicator[i], HIGH);
  } else if (globalState == STATE_QUESTION) {
    setState(STATE_FALSESTARTED);
    actor = i;
    time = millis();
    blocked[i] = true;    
    
  } else if (globalState == STATE_STARTED) {
    setState(STATE_ANSWERING);
    time = millis();
    actor = i;
    blocked[i] = true;
  }  
}

void showIndicator() {
  if (globalState == STATE_FALSESTARTED) {
    if ( (((millis() - time) / (BLINK_PERIOD/2)) % 2) == 0) {
      digitalWrite(indicator[actor], HIGH);
    } else {
      digitalWrite(indicator[actor], LOW);
    }
  } else if (globalState == STATE_ANSWERING) {
    digitalWrite(indicator[actor], HIGH);
    
  } else if (globalState == STATE_STARTED || globalState == STATE_ENDED) {
    digitalWrite(indicator[0], LOW);
    digitalWrite(indicator[1], LOW);
  }
}

void loop() {
  if (Serial.available() > 0) {
    int c = Serial.read();
    if (c == 'f') //start
    {
      reset();      
      
    } else if (c == 'q') { //question is questioning, can be falsestart
      setState(STATE_QUESTION);
      
    } else if (c == 's') { //no falsestart
      if (globalState != STATE_FALSESTARTED) {
        setState(STATE_STARTED);
      }
      
    } else if (c == 'c') { //continue after action
      setState(STATE_STARTED);
        
    } else if (c == 'e') { //game ended, button doesn't work
      setState(STATE_ENDED);
    }
  }
  
  
  showIndicator();
  if (globalState == STATE_FALSESTARTED || globalState == STATE_ENDED) return; //do not apply user input
  
  if (digitalRead(input[0]) == LOW) handlePress(0);
  if (digitalRead(input[1]) == LOW) handlePress(1);  
}
