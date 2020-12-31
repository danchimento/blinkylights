# Blinky Lights

Blink Lights is an Arduino/.NET project. Software runs on the host PC that hits the Brightwell /health endpoint. Lights on the Blinky Lights box are turned on for any systems that are unavailable. 

# Installation
1. Plug in the Blinky Lights box.
2. Go the the [Releases](https://github.com/danchimento/blinkylights/releases) page of the github project.
3. Choose the latest release and download the .zip appropriate for your machine. 
4. Extract the downloaded zip file anywhere you want the application to live. 
5. MAC ONLY: Copy the `blinkconfig.json` file to your Users/[USERNAME] directory.
6. Double click the `blinkylights` application to start it.
7. Switch from readiness to production (see Production section below)
7. OPTIONAL: Set this application to run on startup. Somehow. Good luck.


# Production

For security reasons, Blinky Lights is configured for the readiness environment by default. To connect to production instead, edit the blinkconfig.json file and update all the appropriate values. (All values can be retrieved from Postman. Ask a developer for help if necessary).


# Customization

By default, the lights are mapped as follows:

- Light 1: CardProcessor.FIS_North
- Light 2: CardProcessor.FIS_South
- Light 3: OnDemand.Cambridge
- Light 4: OnDemand.WUBS
- Light 5: OnDemand.MoneyGram
- Light 6: OnDemand.Transfast

This mapping can be changed by modifying the mapping values in the `blinkconfig.json` file.


```
-------------------
| 1 2 3 4 5 6     |
|                 |
|                 == USB
|                 |
-------------------
```
