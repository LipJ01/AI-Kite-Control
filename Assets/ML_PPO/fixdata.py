import pandas as pd
import argparse

# Set up command line argument parsing
parser = argparse.ArgumentParser(description='Process a CSV file.')
parser.add_argument('filename', type=str, help='The name of the CSV file to process')
args = parser.parse_args()

# Load the data
df = pd.read_csv(args.filename)

# Calculate the additional score
df['final_score'] += df['number_of_crashes'] * 400

# Save the updated dataframe back to csv
df.to_csv(args.filename, index=False)
