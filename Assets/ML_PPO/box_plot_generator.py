import pandas as pd
import plotly.graph_objs as go
import os
import re  # to use regular expressions

def generate_graph_data(results_dir):
    # Prepare an empty list to store all dataframes
    all_data_runs = []
    all_data_evaluations = []
    all_data = []

    # Walk through the results directory and its subdirectories
    for root, dirs, files in os.walk(results_dir):
        for file in files:
            # Check if the file is evaluation.csv
            if file == 'evaluation.csv':
                # Construct the full file path
                file_path = os.path.join(root, file)

                # Read the data from the csv file into a DataFrame
                data = pd.read_csv(file_path)

                # Add this data to the all_data list
                all_data.append(data)

    # Concatenate all dataframes
    all_data = pd.concat(all_data)

    # First, group the data by model_name
    grouped = all_data.groupby('model_name')
                
    # filter data by model_name
    for name, group in grouped:
        # Check if file name matches LLC_FFF_C0FF_X format
        if re.match("^LLC_FFF_C0FF_\d+$", name):
            all_data_runs.append(data)
        else:
            all_data_evaluations.append(data)
      
    # Concatenate all dataframes
    all_data_runs = pd.concat(all_data_runs)
    all_data_evaluations = pd.concat(all_data_evaluations)

    # Create box plots
    box_data_runs = [go.Box(y=all_data_runs['final_score'] / all_data_runs['time_elapsed'], name="All Runs")]
    box_data_evaluations = [go.Box(y=all_data_evaluations['final_score'] / all_data_evaluations['time_elapsed'], name="All Evaluations")]

    return box_data_runs, box_data_evaluations

generate_graph_data('results')