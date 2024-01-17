
# JADE

Creative Computing and AI Master's: first semester's project prototype.

<a href="url"><img src="https://github.com/vitorianavi/jade-project/blob/master/jade.png?raw=true" align="left" height="300" width="300"></a>

**JADE's primary goal is to encourage practitioners to express themselves, perform, and create, exploring pole dance as an art form.**



It allows pole dancers to:

- Build showcases with interactive media that respond to pole moves/figures.

- Visualize the interactions in a virtual simulation.

- Create choreographies with other pole dancers with a challenge game.

  

## How to install and run

  > P.S.: It currently only works for MacOX

1. Starting with the server for pose recognition:

- Install the Python requirements in `server/requirements.txt` (it requires Metal for MacOX)

- Run:

```
cd server/
uvicorn server:app --host 0.0.0.0 --port 8080 --ws-ping-timeout=200
```

2. Visual projections:
- Install [Isadora](https://troikatronix.com/)
- Open `isadora/projections.izz`
- Configure OSC to listen to port `6448` and channels `pose/inputs` and `pose/output`

3. Arduino ESP32
> It requires a PIR sensor and a DMX light connected to the board
- Install Arduino IDE
- Compile and upload `arduino/jade.ino` to the board

5. Application
- Install Unity's `2022.3 LTS` release
- Open `unity/PoleStage` project
- Set up [Syphon for Unity](https://github.com/keijiro/KlakSyphon) (it requires `OBS Studio` installation)
- Play the main scene
``