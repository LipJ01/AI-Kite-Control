import pandas as pd
import plotly.graph_objs as go
import os

def generate_all_data(results_dir):
    # Prepare an empty list to store all dataframes
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
    

    # save the data to a csv file
    all_data.to_csv('all_data.csv')

    return all_data

# def generate_all_graph_data(all_data):

#     grouped = all_data.groupby('model_name')
#     # Prepare data for the graph
#     graph_data = []

#     # Prepare a list to store model names and their best scores
#     models_best_scores = []

#     for name, group in grouped:
#         yData = group['final_score'] / group['time_elapsed'] - group['number_of_crashes'] * 10
#         # limit the minimum of yData to 0
#         yData = yData.apply(lambda x: max(x, 0))
#         graph_data.append(go.Scatter(x=group['steps'], y=yData, mode='markers', name=name))

#         # Calculate best score of the current model and append to the list
#         best_score = max(yData)
#         models_best_scores.append((name, best_score))

#     # Convert the list into a DataFrame and sort it by best scores in descending order
#     models_best_scores = pd.DataFrame(models_best_scores, columns=['model_name', 'best_score']).sort_values('best_score', ascending=False)

#     return graph_data, models_best_scores

def generate_all_graph_data(all_data):

    # Create a function to bin steps into ranges (same as before)
    def bin_steps(step):
        # Define the step ranges
        bins = [0, 100000, 200000, 300000, 400000, 500000, 600000, 700000, 800000, 900000, 1000000]
        labels = ['100k', '200k', '300k', '400k', '500k', '600k', '700k', '800k', '900k', '1000k']
        return pd.cut([step], bins=bins, labels=labels)[0]

    # Apply the binning function to the steps column
    all_data['step_range'] = all_data['steps'].apply(bin_steps)

    # Group by model_name and step_range, then calculate average for yData
    grouped = all_data.groupby(['model_name', 'step_range']).agg({
        'final_score': 'mean',
        'time_elapsed': 'mean',
        'number_of_crashes': 'mean'
    }).reset_index()

    graph_data = []

    # Iterate through models and add their data to the graph
    for model in grouped['model_name'].unique():
        model_data = grouped[grouped['model_name'] == model]
        yData = model_data['final_score'] / model_data['time_elapsed'] - model_data['number_of_crashes'] * 10
        yData = yData.apply(lambda x: max(x, 0))
        graph_data.append(go.Scatter(x=model_data['step_range'], y=yData, mode='markers', name=model))

    # Calculate best score for each model
    models_best_scores = []
    for model in all_data['model_name'].unique():
        model_data = all_data[all_data['model_name'] == model]
        yData = model_data['final_score'] / model_data['time_elapsed'] - model_data['number_of_crashes'] * 10
        yData = yData.apply(lambda x: max(x, 0))
        best_score = max(yData)
        models_best_scores.append((model, best_score))

    # Convert the list into a DataFrame and sort it by best scores in descending order
    models_best_scores = pd.DataFrame(models_best_scores, columns=['model_name', 'best_score']).sort_values('best_score', ascending=False)

    return graph_data, models_best_scores


def generate_crash_graph_data(all_data):
    # Create a function to bin steps into ranges
    def bin_steps(step):
        # Define the step ranges
        bins = [0, 100000, 200000, 300000, 400000, 500000, 600000, 700000, 800000, 900000, 1000000]
        labels = ['0-100k', '100k-200k', '200k-300k', '300k-400k', '400k-500k', '500k-600k', '600k-700k', '700k-800k', '800k-900k', '900k-1000k']
        return pd.cut([step], bins=bins, labels=labels)[0]

    # Apply the binning function to the steps column
    all_data['step_range'] = all_data['steps'].apply(bin_steps)

    # Group by the step range only, then calculate the average crashes across all models
    grouped = all_data.groupby('step_range').agg({'number_of_crashes': 'mean'}).reset_index()

    # Prepare data for the graph
    graph_data = go.Scatter(x=grouped['step_range'], y=grouped['number_of_crashes'], mode='markers', name='Average Crashes')

    return [graph_data]
