int pins[6] = {LOW, LOW, LOW, LOW, LOW, LOW};

void setup()
{
    Serial.begin(9600);

    pinMode(2, OUTPUT);
    pinMode(4, OUTPUT);
    pinMode(6, OUTPUT);
    pinMode(8, OUTPUT);
    pinMode(10, OUTPUT);
    pinMode(12, OUTPUT);

    digitalWrite(2, HIGH);
    delay(100);
    digitalWrite(2, LOW);
    digitalWrite(4, HIGH);
    delay(100);
    digitalWrite(4, LOW);
    digitalWrite(6, HIGH);
    delay(100);
    digitalWrite(6, LOW);
    digitalWrite(8, HIGH);
    delay(100);
    digitalWrite(8, LOW);
    digitalWrite(10, HIGH);
    delay(100);
    digitalWrite(10, LOW);
    digitalWrite(12, HIGH);
    delay(100);
    digitalWrite(12, LOW);
    digitalWrite(10, HIGH);
    delay(100);
    digitalWrite(8, HIGH);
    digitalWrite(10, LOW);
    delay(100);
    digitalWrite(8, LOW);
    digitalWrite(6, HIGH);
    delay(100);
    digitalWrite(6, LOW);
    digitalWrite(4, HIGH);
    delay(100);
    digitalWrite(4, LOW);
    digitalWrite(2, HIGH);
    delay(100);
    digitalWrite(2, LOW);
    delay(100);
    digitalWrite(2, LOW);
    digitalWrite(4, LOW);
    digitalWrite(6, LOW);
    digitalWrite(8, LOW);
    digitalWrite(10, LOW);
    digitalWrite(12, LOW);
    delay(500);
    digitalWrite(2, HIGH);
    digitalWrite(4, HIGH);
    digitalWrite(6, HIGH);
    digitalWrite(8, HIGH);
    digitalWrite(10, HIGH);
    digitalWrite(12, HIGH);
    delay(500);
    digitalWrite(2, LOW);
    digitalWrite(4, LOW);
    digitalWrite(6, LOW);
    digitalWrite(8, LOW);
    digitalWrite(10, LOW);
    digitalWrite(12, LOW);
    delay(500);
    digitalWrite(2, HIGH);
    digitalWrite(4, HIGH);
    digitalWrite(6, HIGH);
    digitalWrite(8, HIGH);
    digitalWrite(10, HIGH);
    digitalWrite(12, HIGH);
    delay(500);
    digitalWrite(2, LOW);
    digitalWrite(4, LOW);
    digitalWrite(6, LOW);
    digitalWrite(8, LOW);
    digitalWrite(10, LOW);
    digitalWrite(12, LOW);
}

void loop()
{
    for (int i = 1; i < 7; i++)
    {
        digitalWrite(i * 2, pins[i - 1]);
    }

    if (Serial.available())
    {
        String input = Serial.readString();

        if (input.indexOf("BLINKY") > -1) 
        {
            pins[0] = HIGH;
            Serial.print("LIGHTS\n");
        }
        else
        {
            Serial.print("Received: " + input + "\n");
        }

        int start = input.indexOf("[[") + 2;
        int end = input.indexOf("]]");

        if (start < 0 || end < start) {
            return;
        }

        String pinString = input.substring(start, end);
        int pinNumber = pinString.substring(0, 1).toInt();
        int pinValue = pinString.substring(2, 3).toInt();

        pins[pinNumber - 1] = pinValue;
    }
}
