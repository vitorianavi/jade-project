#include <esp_dmx.h>
#include <WiFi.h>
#include <AsyncTCP.h>
#include <HTTPClient.h>
#include <ESPAsyncWebSrv.h>

/*
  PIR SENSOR CONTEXT
*/
const int pir_pin = 3;

/*
  DMX LIGHTS CONTEXT
*/
enum channels_t {
  GLOBAL_DIMMER = 1,
  R_DIMMER = 2,
  G_DIMMER = 3,
  B_DIMMER = 4,
  ASTRO = 5,
  EFFECTS = 6,
  SPEED = 7,
  N_CHANNELS
};

enum effect_offsets_t {
  CONST_BRIGHTNESS = 0, // range: 0-4
  COLOR_JUMP = 5, // range: 0-40
  COLOR_GRADIENT = 46, // range: 0-40
  COLOR_PULSE = 87, // range: 0-40
  STROBE = 128, // range: 0-40
  SOUND_CONTROL_1 = 169, // range: 0-40
  SOUND_CONTROL_2 = 210 // range: 0-45
};

enum light_setups_t {
  MODE_BLUE, // blue light
  MODE_PINK, // pink light
  MODE_YELLOW, // pink light
  MODE_BLUE_ASTRO, // blue light with astro
  MODE_OFF, // blue light with astro
  N_MODES
};

const dmx_port_t dmx_port = DMX_NUM_0;
const int light_offset = 0;

// set the communication pins
const int tx_pin = 43;
const int rx_pin = 44;
const int rts_pin = 0;

// global color setup
int color_r = 0;
int color_g = 0;
int color_b = 0;
int intensity = 0;
int astro = 0;
bool pulse = false;
int delay_ms = 200;
int light_on = true;

int start_time = 0;
int current_mode = MODE_YELLOW;

/*
  HTTP CLIENT CONTEXT
*/
String serverPath = "http://10.72.80.228:8081/pir/1";
String serverURLOn = serverPath + "?status=on";
String serverURLOff = serverPath + "?status=off";

bool lastSensorState = false;

/*
  WEBSOCKET SERVER CONTEXT
*/

char* ssid = "Visitors";
char* password = "";

AsyncWebServer server(8081);
AsyncWebSocket ws("/dmx");

void setup() {
  Serial.begin(9200);

  /* 
    SOCKET SERVER SETUP
  */
  connectToWifi(ssid, password);

  ws.onEvent(on_message_received);
  server.addHandler(&ws);
  server.begin();

  /*
    PIR SENSOR SETUP
  */
  pinMode(pir_pin, INPUT);

  /*
    LIGHTS SETUP
  */
  // use the default DMX configuration
  dmx_config_t config = DMX_CONFIG_DEFAULT;

  // install the DMX driver
  dmx_driver_install(dmx_port, &config, DMX_INTR_FLAGS_DEFAULT);
  dmx_set_pin(dmx_port, tx_pin, rx_pin, rts_pin);
}

void loop() {
  /*
    PRESENCE SENSOR CONTROL
  */
  bool isSensorActivated = digitalRead(pir_pin);
    
  if(WiFi.status() == WL_CONNECTED) {
    if(isSensorActivated && lastSensorState==false) {
      bool success = sendGETRequest(serverURLOn);
      if (success) lastSensorState = true;
    
    } else if(!isSensorActivated && lastSensorState==true) {
      bool success = sendGETRequest(serverURLOff);
      if (success) lastSensorState = false;
    }
  }

  /*
    LIGHT CONTROL
  */
  if (pulse == true) {
    if (millis() - start_time >= delay_ms) {
      light_on = (light_on == true) ? false : true;
      start_time = millis();
    }
  } else {
    light_on = true;
  }

  if (light_on) {
    write_light(dmx_port, light_offset, color_r, color_g, color_b, intensity, astro);
  } else {
    write_light(dmx_port, light_offset, color_r, color_g, color_b, 0, 0);
  }
  dmx_send(dmx_port, DMX_PACKET_SIZE);
  dmx_wait_sent(dmx_port, DMX_TIMEOUT_TICK);
}

void connectToWifi(char* ssid, char* pswd) {
    // Connect to wifi
    WiFi.begin(ssid, password);

    // Wait some time to connect to wifi
    for(int i = 0; i < 15 && WiFi.status() != WL_CONNECTED; i++) {
        Serial.print(".");
        delay(1000);
    }
    Serial.println();
    Serial.println("WiFi connected.");
    Serial.print("IP address: ");
    Serial.println(WiFi.localIP());
}

bool sendGETRequest(String url) {
    HTTPClient http;
    Serial.print("GET ");
    Serial.println(url);

    http.begin(url.c_str());
    int response = http.GET();
    if (response < 200) {
      Serial.println(http.errorToString(response).c_str());
      return false;
    }
    http.end();
    return true;
}

void on_message_received(AsyncWebSocket *server, AsyncWebSocketClient *client, AwsEventType type, void *arg, uint8_t *data, size_t len) {
    switch (type) {
      case WS_EVT_CONNECT:
        Serial.printf("WebSocket client #%u connected from %s\n", client->id(), client->remoteIP().toString().c_str());
        break;
      case WS_EVT_DISCONNECT:
        Serial.printf("WebSocket client #%u disconnected\n", client->id());
        break;
      case WS_EVT_DATA:
        color_r = data[0];
        color_g = data[1];
        color_b = data[2];
        intensity = data[3];
        astro = data[4];
        if (data[5] == 1) {
          pulse = true;
          delay_ms = data[6];
        } else {
          pulse = false;
        }
        break;
      case WS_EVT_PONG:
      case WS_EVT_ERROR:
        break;
  }
}

void write_light(int port, int offset, int r, int g, int b, int intensity, int astro) {
  uint8_t data[DMX_PACKET_SIZE] = {0};

  data[GLOBAL_DIMMER] = intensity;
  data[R_DIMMER] = r;
  data[G_DIMMER] = g;
  data[B_DIMMER] = b;
  data[ASTRO] = astro;

  dmx_write_offset(port, offset, data, N_CHANNELS);
}
