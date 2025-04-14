using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStartMenu : MonoBehaviour
{
    [Header("UI Pages")]
    public GameObject mainMenu;
    public GameObject options;
    public GameObject about;
    public GameObject EMSSetup;

    [Header("Main Menu Buttons")]
    public Button startButton;
    public Button optionButton;
    public Button aboutButton;
    public Button quitButton;
    public Button SelectButton; 
    public Button EMSConfirmButton;

    public List<Button> returnButtons;
    public TMPro.TMP_Dropdown EMSDropdown;

    // Start is called before the first frame update
    void Start()
    {
        EnableMainMenu();

        //Hook events
        startButton.onClick.AddListener(StartGame);
        optionButton.onClick.AddListener(EnableOption);
        aboutButton.onClick.AddListener(EnableAbout);
        quitButton.onClick.AddListener(QuitGame);
        SelectButton.onClick.AddListener(EnableEMSSetup);
        EMSConfirmButton.onClick.AddListener(EnableEMSValueSet);

        foreach (var item in returnButtons)
        {
            item.onClick.AddListener(EnableMainMenu);
        }
    }

    public void QuitGame()
    {
        SceneLoaderManager.instance.Exit();
    }

    public void StartGame()
    {
        HideAll();
        SceneLoaderManager.instance.LoadStage1();
    }

    public void HideAll()
    {
        mainMenu.SetActive(false);
        options.SetActive(false);
        about.SetActive(false);
    }

    public void EnableMainMenu()
    {
        mainMenu.SetActive(true);
        options.SetActive(false);
        about.SetActive(false);
    }
    public void EnableOption()
    {
        mainMenu.SetActive(false);
        options.SetActive(true);
        about.SetActive(false);
    }
    public void EnableAbout()
    {
        mainMenu.SetActive(false);
        options.SetActive(false);
        about.SetActive(true);
    }
    public void EnableEMSSetup()
    {
        mainMenu.SetActive(false);
        options.SetActive(false);
        about.SetActive(false);
        EMSSetup.SetActive(true);
    }
    public void EnableEMSValueSet()
    {
        string value = EMSDropdown.options[EMSDropdown.value].text.Replace("no set", "Done");
        GameDataManager.instance.EMS[EMSDropdown.value] = ServoDeviceControl.instance.ampl;
        EMSDropdown.options[EMSDropdown.value].text = value;
        EMSDropdown.RefreshShownValue();
        options.SetActive(true);
        EMSSetup.SetActive(false);
    }
}
