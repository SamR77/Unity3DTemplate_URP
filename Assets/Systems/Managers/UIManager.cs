using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    [Header("UI Menu Objects")]
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject gameplayUI;


    private static UIManager instance;

    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UIManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("UIManager");
                }
            }
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void ShowGameplayUI()
    {
        disableAllMenus();
        gameplayUI.SetActive(true);
    }
    public void ShowMainMenu()
    {
        disableAllMenus();
        mainMenuUI.SetActive(true);
    }
    public void ShowPauseMenu()
    {
        disableAllMenus();
        pauseMenuUI.SetActive(true);
    }
    public void disableAllMenus()
    {
        pauseMenuUI.SetActive(false);
        mainMenuUI.SetActive(false);
        gameplayUI.SetActive(false);
    }
}
