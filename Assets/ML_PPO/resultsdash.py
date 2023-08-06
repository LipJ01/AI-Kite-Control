import importlib
import os
from flask import Flask
import dash
from dash import dcc
from dash import html
from dash.dependencies import Input, Output
import plotly.graph_objs as go
import dash_table
import pandas as pd

# Directory where results are stored
results_dir = 'results'

# Dynamically import the graph_generator module
graph_generator = importlib.import_module('graph_generator')

# Use the function in the module to generate the data for the graph
all_data = graph_generator.generate_all_data(results_dir)
graph_data, best_model_scores = graph_generator.generate_all_graph_data(all_data)
crash_data = graph_generator.generate_crash_graph_data(all_data)

# Initialize Flask server
server = Flask(__name__)

# Initialize Dash app
app = dash.Dash(__name__, server=server)

# Define Dash layout# Define Dash layout
app.layout = html.Div(children=[
  html.H1(id='model-title'), # Give the title an id

  dcc.Graph(
    id='model-graph',
    figure={
      'data': graph_data,
      'layout': go.Layout(
        #...
      )
    }
  ),
  html.H2(children='Model Variances'),
  html.Div(id='variance-div'),
  html.Div(id='overall_var_text'),

  html.H2(children='Crashes graph'),
  dcc.Graph(
    id='crash-graph',
    figure={
      'data': crash_data,
      'layout': go.Layout(
        #...
      )
    }
  ),
  html.H2(children='Best Scores Distribution'),

  dcc.Graph(id='best-scores-graph'),

  html.Button('Refresh', id='refresh-button', n_clicks=0),

  # New DataTable component to display the best scores, with width and centering
  html.Div(
    dash_table.DataTable(
      id='table',
      columns=[{"name": i, "id": i} for i in best_model_scores.columns],
      data=best_model_scores.to_dict('records'),
    ),
    style={
      'maxWidth': '600px',
      'margin': 'auto',
    },
  )
])

def compute_variances(all_data):
    # Group by both model_name and steps
    grouped = all_data.groupby(['model_name', 'steps'])
    
    step_variances = []

    # For each group of same model_name and steps
    for (name, steps), group in grouped:
        if len(group) > 1:  # Check if the model has been run more than once for this step value
            scores = group['final_score'] / group['time_elapsed']
            variance = scores.var()
            step_variances.append(variance)

    if step_variances:
        avg_step_variance = sum(step_variances) / len(step_variances)
    else:
        avg_step_variance = 0

    # Existing code to compute the overall variance of variances based on model_name only
    model_grouped = all_data.groupby('model_name')
    model_variances = []
    for name, group in model_grouped:
        scores = group['final_score'] / group['time_elapsed']
        variance = scores.var()
        model_variances.append((name, variance))
    
    overall_variance_of_variances = pd.DataFrame(model_variances, columns=['model_name', 'variance'])['variance'].var()
    ordered_by_variance_model_variances = pd.DataFrame(model_variances, columns=['model_name', 'variance']).sort_values(by='variance', ascending=False)

    return ordered_by_variance_model_variances, overall_variance_of_variances, avg_step_variance




@app.callback(
  [Output('model-title', 'children'),
   Output('model-graph', 'figure'),
   Output('best-scores-graph', 'figure'),
   Output('table', 'data'),
   Output('variance-div', 'children'),
   Output('overall_var_text', 'children'),
   Output('crash-graph', 'figure')],
  [Input('refresh-button', 'n_clicks')]
)
def update_graph(n_clicks):
  global graph_data
  all_data = graph_generator.generate_all_data(results_dir)
  graph_data, best_model_scores = graph_generator.generate_all_graph_data(all_data)
  crash_data = graph_generator.generate_crash_graph_data(all_data)

  # Generate histogram for best scores
  best_scores_graph_data = [go.Histogram(x=best_model_scores['best_score'])]

  # Calculate number of models and create title
  num_models = len(graph_data) # This assumes that one data element corresponds to one model
  title = 'Final Score from each Model - {} Models'.format(num_models)

  model_variances, overall_variance_of_variances, avg_variances = compute_variances(all_data)

  # Additional DataTable to display variances
  var_table = dash_table.DataTable(
      id='variance-table',
      columns=[{"name": i, "id": i} for i in model_variances.columns],
      data=model_variances.to_dict('records'),
  )

  overall_var_text = html.Div(f"Variance of Variances: {avg_variances:.2f}")
  

  return title, {
    'data': graph_data,
    'layout': go.Layout(
      yaxis={'title': 'Evaluation score'},  # Add your x-axis title here
      xaxis={'title': 'Steps'},  # Add your y-axis title here
    )
  }, {
    'data': best_scores_graph_data,
    'layout': go.Layout(
      title='Best Scores Distribution',
      xaxis={'title': 'Best Score'},
      yaxis={'title': 'Count'},
    ),
  }, best_model_scores.to_dict('records') , var_table, overall_var_text, {
    'data': crash_data,
    'layout': go.Layout(
      yaxis={'title': 'Number of crashes'},  # Add your x-axis title here
      xaxis={'title': 'Steps'},  # Add your y-axis title here
    )
  }

if __name__ == '__main__':
  app.run_server(debug=True, host='0.0.0.0')
