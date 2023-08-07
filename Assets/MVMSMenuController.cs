using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class MVMSMenuController : MonoBehaviour
{
    public Button backButton;
    // Start is called before the first frame update
    void Start()
    {
        backButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Exit";
        backButton.onClick.AddListener(BackButtonClicked);
    }

    public void BackButtonClicked() 
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
