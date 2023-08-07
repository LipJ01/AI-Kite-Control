using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
// files
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization;
public class TrainingMenuController : MonoBehaviour 
{
    public Dropdown modelDropdown; // Assuming a dropdown is used for model selection
    public Dropdown configDropdown; // Assuming an InputField is used for model configuration

    public Dropdown envConfigDropdown;

    public Button trainButton;
    public InputField newModelInputField;

    public Button backButton;

    public ScriptRunner scriptRunner;

    public Slider progressBar;


    private List<string> modelDirectories;
    private List<string> modelNames = new List<string>();

    void Start()
    {
        // add listeners
        trainButton.onClick.AddListener(TrainButtonClicked);
        trainButton.interactable = false;
        newModelInputField.interactable = false;
        backButton.onClick.AddListener(BackButtonClicked);
        newModelInputField.onValueChanged.AddListener(delegate {
            NewModelInputFieldValueChanged(newModelInputField.text);
        });


        // set text
        trainButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Train";

        backButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Back";

        // get dropdown options (config)
        // search through directory for all model configurations (Assets/ML_PPO/config/[config_name].yaml)
        // add each config name
        string configPath = "Assets/ML_PPO/config/";
        string[] configFiles = Directory.GetFiles(configPath, "*.yaml");
        foreach (string configFile in configFiles)
        {
            string configName = Path.GetFileNameWithoutExtension(configFile);
            configDropdown.options.Add(new Dropdown.OptionData() { text = configName });
        }
        // get dropdown options (environment)
        configPath = "Assets/ML_PPO/envConfig/";
        configFiles = Directory.GetFiles(configPath, "*.yaml");
        foreach (string configFile in configFiles)
        {
            string configName = Path.GetFileNameWithoutExtension(configFile);
            envConfigDropdown.options.Add(new Dropdown.OptionData() { text = configName });
        }

        // search through directory for all models (Assets/ML_PPO/results/[model_name]/flying.onnx)
        // also search through directory for all model configurations (Assets/ML_PPO/results/[model_name]/config.yaml)
        // add each model name
        string modelPath = "Assets/ML_PPO/results/";
        modelDirectories = Directory.GetDirectories(modelPath).ToList();
        foreach (string modelDirectory in modelDirectories)
        {
            string modelName = Path.GetFileName(modelDirectory);
            modelNames.Add(modelName);
            // Debug.Log(modelName);
            string onnxPath = Path.Combine(modelDirectory, "flying.onnx");
            string configFilePath = Path.Combine(modelDirectory, "configuration.yaml");
            if (File.Exists(onnxPath) && File.Exists(configFilePath))
            {
                modelDropdown.options.Add(new Dropdown.OptionData() { text = modelName });
            }
        }
        // Debug.Log("model names contains 'config12_1': " + modelNames.Contains("config12_1"));

        // set dropdown options
        modelDropdown.value = 0;
        configDropdown.value = 0;


        // Check if directories exist for all possible combinations
        string pattern = @"^.{5}[^T][^T].*$";
        Match match;

        for(int i = 1; i < configDropdown.options.Count - 1; i++)
        {
            for(int j = 1; j < envConfigDropdown.options.Count - 1; j++)
            {
                int i_platform = i;
                int j_platform = j;
                if (Application.platform == RuntimePlatform.LinuxEditor)
                {
                    i_platform = configDropdown.options.Count - i;
                    j_platform = envConfigDropdown.options.Count - j;
                }
                int count = 1;
                while (Directory.Exists($"Assets/ML_PPO/results/{configDropdown.options[i_platform].text}_{envConfigDropdown.options[j_platform].text}_{count}"))
                {
                    Debug.Log($"Assets/ML_PPO/results/{configDropdown.options[i_platform].text}_{envConfigDropdown.options[j_platform].text}_{count} already exists");
                    count++;
                }
                if (count == 1) // directory doesn't exist for this combination
                {
                    match = Regex.Match(configDropdown.options[i_platform].text, pattern);
                    if(!match.Success)
                    {
                        Debug.Log($"Assets/ML_PPO/results/{configDropdown.options[i_platform].text}_{envConfigDropdown.options[j_platform].text}_{count} Skipping for compute saving");
                        continue;
                    }
                    Debug.Log($"Assets/ML_PPO/results/{configDropdown.options[i_platform].text}_{envConfigDropdown.options[j_platform].text}_{count} Ready to train");
                    configDropdown.value = i_platform;
                    envConfigDropdown.value = j_platform;
                    newModelInputField.interactable = true;
                    modelDropdown.interactable = false;
                    trainButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Start Training";
                    string defaultModelName = configDropdown.options[configDropdown.value].text;
                    string defaultEnvConfigName = envConfigDropdown.options[envConfigDropdown.value].text;
                    newModelInputField.text = defaultModelName + "_" + defaultEnvConfigName + "_1";
                    break;
                }
            }
            if (envConfigDropdown.value == i && configDropdown.value != 0)
            {
                break;
            }
        }
        // add listeners for dropdowns
        modelDropdown.onValueChanged.AddListener(delegate {
            ModelDropdownValueChanged(modelDropdown);
        });
        configDropdown.onValueChanged.AddListener(delegate {
            ModelConfigDropdownValueChanged(configDropdown);
        });
        envConfigDropdown.onValueChanged.AddListener(delegate {
            EnvConfigDropdownValueChanged(envConfigDropdown);
        });
    }

    public void TrainButtonClicked() 
    {
        // Logic to initialize training using selected model and its configuration
        if (modelDropdown.value == 0 && configDropdown.value == 0)
        {
            Debug.Log("No model or config selected");
            return;
        }
        else if (modelDropdown.value == 0)
        {
            Debug.Log("Starting new training run with config: " + configDropdown.options[configDropdown.value].text);
            if(Application.platform == RuntimePlatform.WindowsEditor | Application.platform == RuntimePlatform.WindowsPlayer)
            {
                // make dir for new model (same name as text in input field)
                Directory.CreateDirectory("Assets/ML_PPO/results/" + newModelInputField.text);
                Directory.CreateDirectory("Assets/ML_PPO/results/" + newModelInputField.text + "/run_logs");
                Directory.CreateDirectory("Assets/ML_PPO/results/" + newModelInputField.text + "/Flying");
                scriptRunner.RunScript(configDropdown.options[configDropdown.value].text, newModelInputField.text, false);
            }
            else if(Application.platform == RuntimePlatform.OSXEditor | Application.platform == RuntimePlatform.OSXPlayer)
            {
                scriptRunner.RunScript(configDropdown.options[configDropdown.value].text, newModelInputField.text, false);
            }
            else if(Application.platform == RuntimePlatform.LinuxEditor | Application.platform == RuntimePlatform.LinuxPlayer)
            {
                scriptRunner.RunScript(configDropdown.options[configDropdown.value].text, newModelInputField.text, false);
            }
            else
            {
                Debug.LogException(new System.Exception("Unsupported platform"));
            }
            StartCoroutine(SleepWithProgressBar(6, () => {
                Debug.Log("Loading scene");
                SceneManager.LoadScene("MVMS");
            }));
        }
        else if (configDropdown.value == 0)
        {
            Debug.Log("Resuming training with model: " + modelDropdown.options[modelDropdown.value].text);
            scriptRunner.RunScript("results/" + modelDropdown.options[modelDropdown.value].text + "/configuration", modelDropdown.options[modelDropdown.value].text, true);
            StartCoroutine(SleepWithProgressBar(6, () => {
                Debug.Log("Loading scene");
                SceneManager.LoadScene("MVMS");
            }));
        }
        else
        {
            Debug.LogException(new System.Exception("Cannot train with both model and config"));   
        }
    }

    public void BackButtonClicked() 
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    void ModelDropdownValueChanged(Dropdown change)
    {
        Debug.Log("Model dropdown changed");
        // grey out config dropdown if model is selected
        if (modelDropdown.value == 0)
        {
            newModelInputField.interactable = true;
            configDropdown.interactable = true;
        }
        else
        {
            newModelInputField.interactable = false;
            configDropdown.interactable = false;
            trainButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Continue Training";
        }
        trainButton.interactable = (modelDropdown.value != 0 || configDropdown.value != 0);
    }

    void ModelConfigDropdownValueChanged(Dropdown change)
    {
        Debug.Log("Config dropdown changed");
        // grey out model dropdown if config is selected
        if (configDropdown.value == 0)
        {
            newModelInputField.interactable = false;
            modelDropdown.interactable = true;
        }
        else
        {
            newModelInputField.interactable = true;
            modelDropdown.interactable = false;
            trainButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Start Training";
            string defaultModelName = configDropdown.options[configDropdown.value].text;
            string defaultEnvConfigName = envConfigDropdown.options[envConfigDropdown.value].text;
            int i = 1;
            while (modelNames.Contains(defaultModelName + "_" + defaultEnvConfigName + "_"+ i.ToString()))
            {
                i++;
            }
            newModelInputField.text = defaultModelName + "_" + defaultEnvConfigName + "_" + i.ToString();
        }
        trainButton.interactable = ((modelDropdown.value != 0 || configDropdown.value != 0 ) && newModelInputField.text != "" && envConfigDropdown.value != 0);

    }

    void EnvConfigDropdownValueChanged(Dropdown change) 
    {
        Debug.Log("Env config dropdown changed");
        // grey out model dropdown if config is selected
        if (envConfigDropdown.value == 0)
        {
            newModelInputField.interactable = false;
        }
        else
        {
            newModelInputField.interactable = true;
            trainButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Start Training";
            string defaultModelName = configDropdown.options[configDropdown.value].text;
            string defaultEnvConfigName = envConfigDropdown.options[envConfigDropdown.value].text;
            int i = 1;
            while (modelNames.Contains(defaultModelName + "_" + defaultEnvConfigName + "_"+ i.ToString()))
            {
                i++;
            }
            newModelInputField.text = defaultModelName + "_" + defaultEnvConfigName + "_" + i.ToString();
        }
        string envConfigPath = "Assets/ML_PPO/envConfig/" + envConfigDropdown.options[envConfigDropdown.value].text + ".yaml";
        string envConfigText = File.ReadAllText(envConfigPath);
        var input = new StringReader(envConfigText);
        var deserializer = new DeserializerBuilder().Build();
        var envConfig = deserializer.Deserialize<Dictionary<object, object>>(input);
        if (envConfig.ContainsKey("rewardMode"))
        {
            PlayerPrefs.SetString("rewardMode", envConfig["rewardMode"].ToString());
        }
        if (envConfig.ContainsKey("varyWind"))
        {
            PlayerPrefs.SetString("varyWind", envConfig["varyWind"].ToString());
        }
        if (envConfig.ContainsKey("varyPhysics"))
        {
            PlayerPrefs.SetString("varyPhysics", envConfig["varyPhysics"].ToString());
        }
        if (envConfig.ContainsKey("carOrPunching"))
        {
            PlayerPrefs.SetString("carOrPunching", envConfig["carOrPunching"].ToString());
        }
        trainButton.interactable = ((modelDropdown.value != 0 || configDropdown.value != 0 ) && newModelInputField.text != "" && envConfigDropdown.value != 0);

    }


    void NewModelInputFieldValueChanged(string change)
    {
        Debug.Log("New model input field changed");
        trainButton.interactable = ((modelDropdown.value != 0 || configDropdown.value != 0 ) && newModelInputField.text != "" && envConfigDropdown.value != 0);
    }

    private IEnumerator<float> SleepWithProgressBar(float seconds, System.Action onComplete)
    {
        float elapsedTime = 0;
        while (elapsedTime < seconds)
        {
            elapsedTime += Time.deltaTime;
            progressBar.value = Mathf.Clamp01(elapsedTime / seconds); // Update progress bar
            yield return 0;
        }
        progressBar.value = 1; // Ensure progress bar gets to 100%
        onComplete?.Invoke(); // Call the completion action
    }

}