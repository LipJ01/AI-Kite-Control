using UnityEngine;
using System.IO;

public class ExportMenuController : MonoBehaviour 
{
    public void ExportButtonClicked() 
    {
        // Sample code to write evaluation results to a text file
        string path = "path_to_your_directory/EvaluationResults.txt";
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine("your_evaluation_results");
        writer.Close();
    }
}
