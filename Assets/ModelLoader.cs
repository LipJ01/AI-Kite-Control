
using Unity.MLAgents.Policies;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class ModelLoader : MonoBehaviour
{
    public int numberOfKites;

    public StatsCollector statsCollector;
    private BehaviorParameters[] behaviorParameters;

    private List<string> orderedCheckpoints;
    private int currentModelIndex;

    public void LoadNextModel()
    {
        if (currentModelIndex < orderedCheckpoints.Count - 1)
        {
            currentModelIndex++;
            LoadModel(orderedCheckpoints[currentModelIndex]);
        }
    }

    public void LoadPreviousModel()
    {
        if (currentModelIndex > 0)
        {
            currentModelIndex--;
            LoadModel(orderedCheckpoints[currentModelIndex]);
        }
    }

    public bool LoadModelAtIndex(int index)
    {
        if (index >= 0 && index < orderedCheckpoints.Count)
        {
            currentModelIndex = index;
            LoadModel(orderedCheckpoints[currentModelIndex]);
            return true;
        }
        return false;
    }

    private void LoadModel(string modelPath)
    {
        #if UNITY_EDITOR
            Unity.Barracuda.NNModel nnModel = AssetDatabase.LoadAssetAtPath<Unity.Barracuda.NNModel>(modelPath);
        #endif

        if (nnModel == null)
        {
            Debug.Log("Failed to load model");
            return;
        }

        foreach (var behaviorParameter in behaviorParameters)
        {
            behaviorParameter.Model = nnModel;
        }
    }

    private int ExtractStepNumber(string filepath)
    {
        string filename = Path.GetFileNameWithoutExtension(filepath);
        string numberString = filename.Split('-')[1];
        return int.Parse(numberString);
    }

    void Start()
    {
        behaviorParameters = GetComponentsInChildren<BehaviorParameters>();
        numberOfKites = behaviorParameters.Length;
        InitializeModels();
    }

    private void InitializeModels()
    {
        string modelName = PlayerPrefs.GetString("model_name");
        if (modelName == "")
        {
            Debug.Log("No model name");
            return;
        }

        string modelDirectory = "Assets/ML_PPO/results/" + modelName + "/Flying/";

        if (!Directory.Exists(modelDirectory))
        {
            Debug.Log("No model directory");
            Debug.Log(modelDirectory);
            return;
        }

        var checkpointFiles = Directory.GetFiles(modelDirectory, "*.onnx");
        orderedCheckpoints = checkpointFiles.OrderBy(path => ExtractStepNumber(path)).ToList();

        // Load the first model
        if (orderedCheckpoints.Count > 0)
        {
            LoadModelAtIndex(0);
        }
    }
}
