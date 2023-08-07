using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EvaluationSceneMenuController : MonoBehaviour
{
    public Button backButton;
    
    public Button switchCameraButton;

    public GameObject droneCamera;
    public GameObject shipCamera;
    void Start()
    {
        backButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Exit";
        backButton.onClick.AddListener(BackButtonClicked);

        switchCameraButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Switch Camera";
        switchCameraButton.onClick.AddListener(SwitchCameraButtonClicked);



        droneCamera.SetActive(false);
        shipCamera.SetActive(true);
    }

    public void BackButtonClicked() 
    {
        Debug.Log("Back button clicked");
        SceneManager.LoadScene("MainMenuScene");
    }

    public void SwitchCameraButtonClicked() 
    {
        if (droneCamera.activeSelf)
        {
            droneCamera.SetActive(false);
            shipCamera.SetActive(true);
        }
        else
        {
            droneCamera.SetActive(true);
            shipCamera.SetActive(false);
        }
    }
}
