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


source /home/joshua/miniconda3/etc/profile.d/conda.sh
conda activate mlAgents && cd Assets/ML_PPO && mlagents-learn config/$config_path.yaml --run-id=$run_id $resume

# zsh -c "conda activate mlAgents && cd Assets/ML_PPO && mlagents-learn \"$config_path.yaml\" --run-id=\"$run_id\" $resume"
# /home/joshua/miniconda3/bin/conda init
# /home/joshua/miniconda3/bin/conda activate mlAgents && cd Assets/ML_PPO && mlagents-learn \"$config_path.yaml\" --run-id=\"$run_id\" $resume

# read no

# echo pwd
# gnome-terminal -- /bin/zsh -c "conda activate mlAgents && cd Assets/ML_PPO && mlagents-learn \"$config_path.yaml\" --run-id=\"$run_id\" $resume"
