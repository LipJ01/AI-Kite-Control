#!/bin/zsh

# Check if two arguments are provided
if [ $# -ne 3 ]; then
    echo "Usage: $0 <config_path> <run_id> <resume>"
    exit 1
fi


# Assign the arguments to variables
config_path="$1"
run_id="$2"
if [ "$3" = "true" ]; then
    resume="--resume"
else
    resume=""
fi

# Open a new iTerm window and run the commands then stay open
osascript -e '
    tell application "iTerm"
        create window with default profile
        tell current session of current window
            write text "z ML_PPO"
            write text "mfpy unity-mla"
            write text "mlagents-learn \"'config/$config_path'.yaml\" --run-id=\"'$run_id'\" '$resume'"
        end tell
    end tell
'
