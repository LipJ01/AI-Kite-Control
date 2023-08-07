@echo off

REM Check if three arguments are provided
IF "%~3"=="" (
    echo Usage: %0 ^<config_path^> ^<run_id^> ^<resume^>
    exit /b 1
)

REM Assign the arguments to variables
SET "config_path=%~1"
SET "run_id=%~2"
IF /I "%~3"=="true" (
    SET "resume=--resume"
) ELSE (
    SET "resume="
)

REM Run the commands in a new Command Prompt window
REM start "" /wait cmd.exe /k "cd C:\Users\lipb1\Documents\Unity Projects\3D\Kite\Assets\ML_PPO && conda activate unity_ml_agents && mlagents-learn \%config_path%.yaml\ --run-id=\"%run_id%\" %resume%"
@REM start "" /wait cmd.exe /k "cd \"C:\Users\lipb1\Documents\UnityProjects\3D\Kite\Assets\ML_PPO\" && conda activate unity_ml_agents && mlagents-learn \"%config_path%.yaml\" --run-id=\"%run_id%\" %resume%"
@REM start "" /wait cmd.exe /k "cd \"C:\Users\lipb1\Documents\UnityProjects\3D\Kite\Assets\ML_PPO\" && conda activate unity_ml_agents"
start "" cmd.exe /k "cd C:\Users\lipb1\Documents\UnityProjects\3D\Kite\Assets\ML_PPO\ && conda activate unity_ml_agents && mlagents-learn "config/%config_path%.yaml" --run-id="%run_id%" %resume% --force"
```

