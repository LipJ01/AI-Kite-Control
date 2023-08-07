using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenuController : MonoBehaviour 
{
    public Button trainingButton;
    public Button evaluationButton;

    public Button exitButton;

    void Start()
    {
        // add listeners
        trainingButton.onClick.AddListener(TrainingButtonClicked);
        evaluationButton.onClick.AddListener(EvaluationButtonClicked);
        exitButton.onClick.AddListener(ExitButtonClicked);

        // set text
        trainingButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Training";
        evaluationButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Evaluation";
        exitButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Exit";
        
        
    }


    public void TrainingButtonClicked() 
    {
        SceneManager.LoadScene("TrainingMenuScene");
    }

    public void EvaluationButtonClicked() 
    {
        SceneManager.LoadScene("EvaluationMenuScene");
    }

    public void ExitButtonClicked() 
    {
        Application.Quit();
    }
}
