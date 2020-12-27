int pins[6] = {LOW, LOW, LOW, LOW, LOW, LOW };

void setup()
{
    Serial.begin(9600);

    pinMode(2, OUTPUT);
    pinMode(3, OUTPUT);
    pinMode(4, OUTPUT);
    pinMode(5, OUTPUT);
    pinMode(6, OUTPUT);
    pinMode(7, OUTPUT);
    pinMode(13, OUTPUT);
    digitalWrite(13, LOW);
}

void loop()
{
    for (int i = 2; i <= 7; i++)
    {
        digitalWrite(i, pins[i - 2]);
    }

    while (Serial.available() > 0)
    {
        String input = Serial.readString();

        int start = input.indexOf("[[") + 2;
        int end = input.indexOf("]]");

        if (start < 0 || end < start) {
            continue;
        }

        String pinString = input.substring(start, end);
        int pinNumber = pinString.substring(0, 1).toInt();
        int pinValue = pinString.substring(2, 3).toInt();

        pins[pinNumber - 1] = pinValue;
        break;
    }
}
