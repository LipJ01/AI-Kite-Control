# Flying virtual kites in a virtual world

<!-- demo gif -->
![](https://github.com/LipJ01/AI-Kite-Control/blob/main/demoGIF.gif)

## Setup
If stuck with issues following below instructions. See Unity's Documentation [Install ML-Agents](https://github.com/Unity-Technologies/ml-agents/blob/develop/docs/Installation.md).
### Python Environment (Required for training)

This project uses Python 3.9
Create a virtual environment, conda is recommended.
For installation on Arm64, see [here](https://fathinah.medium.com/installing-data-science-libraries-in-mac-m1-using-miniforge-2e7378d73e9c). TLDR: Create the environment with miniforge.

```bash
pip install -r requirements.txt
```

### Unity Project
Clone the project and open it in Unity. The project is tested with Unity 2021.3.20f1.

### Testing the project
Open the scene `Assets/Scenes/Evaluation.unity` in Unity. Press play to start the simulation. The kites will be controlled by the trained model.

### Training a model
Start the python server using ml-agents. The server will listen for observations from Unity and send back the actions to take whilst training.

```bash
mlagents-learn config/trainer_config.yaml --run-id=[model_name]
```
This should output something like this:
```bash
2021-10-18 16:38:00 INFO [learn.py:274] run1: run_id set to run1
2021-10-18 16:38:00 INFO [environment.py:100] Listening on port 5004. Start training by pressing the Play button in the Unity Editor.
```

Then open the scene `Assets/Scenes/MVMS.unity` in Unity. Press play to start the training.

Happy kiting!