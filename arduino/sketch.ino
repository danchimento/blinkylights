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

        Serial.print(input + "\n");

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
