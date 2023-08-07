using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class EvaluationMenuController : MonoBehaviour 
{
    public Dropdown modelDropdown; // Assuming a dropdown is used for model selection
    public Button playButton;

    public bool runUnevaluatedModel = true;
    public Button backButton;
    void Start()
    {
        // add listeners
        playButton.onClick.AddListener(PlayButtonClicked);
        backButton.onClick.AddListener(BackButtonClicked);

        // set text
        playButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Play";
        backButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Back";

        // set dropdown options
        string modelPath = "Assets/ML_PPO/results/";
        List<string> modelDirectories = Directory.GetDirectories(modelPath).ToList();
        bool selectedModelFound = false;
        foreach (string modelDirectory in modelDirectories)
        {
            string modelName = Path.GetFileName(modelDirectory);
            Debug.Log(modelName);
            string onnxPath = Path.Combine(modelDirectory, "flying.onnx");
            string configFilePath = Path.Combine(modelDirectory, "configuration.yaml");
            if (File.Exists(onnxPath) && File.Exists(configFilePath))
            {
                string evaluationFilePath = Path.Combine(modelDirectory, "evaluation.csv");
                modelDropdown.options.Add(new Dropdown.OptionData() { text = modelName + (File.Exists(evaluationFilePath) ? " (evaluated)" : "") });
                if (!selectedModelFound && !File.Exists(evaluationFilePath))
                {
                    modelDropdown.value = modelDropdown.options.Count - 1;
                    // selectedModelFound = true;
                }
            }
        }
        // if PlayerPrefs.GetString("model_name") != null and PlayerPrefs.GetString("model_name") is in modelDropdown.options, set modelDropdown.value to the index of PlayerPrefs.GetString("model_name")
        if (PlayerPrefs.GetString("model_name") != null && modelDropdown.options.Any(option => option.text == PlayerPrefs.GetString("model_name")))
        {
            modelDropdown.value = modelDropdown.options.FindIndex(option => option.text == PlayerPrefs.GetString("model_name"));
        }
        if (!selectedModelFound)
        {
            modelDropdown.value = modelDropdown.options.FindIndex(option => option.text == "LLC_FFF_C0FF_10");
        }
        if (runUnevaluatedModel && selectedModelFound)
        {
            PlayButtonClicked();
        }
        // find the first combination of model and environment config that hasn't been run.
    }

    public void PlayButtonClicked() 
    {
        Debug.Log("Play button clicked");
        string modelNameFromDropdown = modelDropdown.options[modelDropdown.value].text.Split(' ')[0];
        PlayerPrefs.SetString("model_name", modelNameFromDropdown);
        SceneManager.LoadScene("EvaluationScene");
    }

    public void BackButtonClicked() 
    {
        Debug.Log("Back button clicked");
        SceneManager.LoadScene("MainMenuScene");
    }
}
