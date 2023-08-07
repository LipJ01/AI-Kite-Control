import os
import pandas as pd
import dash
from dash import dcc
from dash import html
from dash.dependencies import Input, Output
import plotly.graph_objs as go
from flask import Flask

# Directory where results are stored
results_dir = 'results'

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

# First, group the data by model_name
grouped = all_data.groupby('model_name')

# Prepare data for the graph
graph_data = []
for name, group in grouped:
    graph_data.append(go.Scatter(x=group['steps'], y=group['final_score'], mode='markers', name=name))

# Initialize Flask server
server = Flask(__name__)

# Initialize Dash app
app = dash.Dash(__name__, server=server)

# Define Dash layout
app.layout = html.Div(children=[
    html.H1(children='Final Score from each Model'),

    dcc.Graph(
        id='model-graph',
        figure={
            'data': graph_data,
            'layout': go.Layout(
                xaxis={'title': 'Compute (steps)'},
                yaxis={'title': 'Final Score'},
                hovermode='closest'
            )
        }
    )
])

if __name__ == '__main__':
    app.run_server(debug=True)
