using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIHandler : MonoBehaviour
{
    public static UIHandler instance;
    private Login login;
    private Register register;
    private bool loginSuccess = false;  

    public Animator GameOverPanel;  //ID 1
    public Animator SettingsPanel;  //ID 2
    public Animator StatsPanel;     //ID 3
    public Animator WinPanel;       //ID 4
    public Animator WrongPanel;     //ID 5
    public TMP_Text WrongText;      //Texto do painel
    public Animator SuccessPanel;   //ID 6
    public Animator WarningPanel;   //ID 7
    public TMP_Text WarningText;    //Texto do painel
    

    [Header("STATS")]
    public TMP_Text statsText;
    [SerializeField] SCR_BaseStats saveFile;

    void Awake()
    {
        instance = this;
    }


    void Start()
    {
        login = Login.instance;
        register = Register.instance;
        if (statsText != null)
        { 
            UpdateStatsText();
        }
    }


    //TOP LEFT CORNER BUTTON
    public void SettingsButton()
    {
        SettingsPanel.SetTrigger("open");
    }


    //TOP LEFT CORNER BUTTON
    public void StatsButton()
    {
        StatsPanel.SetTrigger("open");
        UpdateStatsText();
    }


    //Go to check login. If thats OK, enter the game
    public void PlayGameButton()
    {
        login.CheckPlayerLogin();
    }


    //Go to registration function
    public void RegisterButton()
    {
        register.RegisterPlayer();
    }


    public void LoginSuccessOkButton()
    {
        if (loginSuccess)
        { 
            SceneManager.LoadScene("Menu");
        }
    }


    void UpdateStatsText()
    {
        List<int> statsList = saveFile.GetStats();
        statsText.text =
            "N� de Vit�rias:    " + statsList[0] + "\n" +
            "N� de Derrotas:    " + statsList[1] + "\n" +
            "Total de Jogos:    " + statsList[3] + "\n" +
            "Taxa de Vit�ria:   " + statsList[2] + "% \n" +
            "Menor Tempo:       " + statsList[4] + "segundos \n";
    }


    //COULD PASS IN MISTAKES USED AND TIME USED
    public void WinCondition(int playTime)
    {
        saveFile.SaveStats(true, playTime);
        WinPanel.SetTrigger("open");
    }


    //COULDN'T PASS IN MISTAKES USED AND TIME USED
    public void LoseCondition(int playTime)
    {
        saveFile.SaveStats(false, playTime);
        GameOverPanel.SetTrigger("open");
    }


    //Open Wrong Panel and set message
    public void WrongCondition(string message)
    {
        WrongPanel.SetTrigger("open");
        WrongText.text = message;
    }


    //Open Success Panel
    public void SuccessCondition()
    {
        SuccessPanel.SetTrigger("open");
    }


    //Open Warning panel and set message
    public void WarningCondition(string message)
    {
        WarningPanel.SetTrigger("open");
        WarningText.text = message;
    }


    public void SetLoginSuccess(bool success)
    {
        loginSuccess = success;
    }


    public void BackToMenu(string levelToLoad)
    {
        SceneManager.LoadScene(levelToLoad);
    }


    public void RestartGame()
    {
        //RELOAD THE CURRENT OPEN SCENE
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    public void ExitGame()
    {
        Application.Quit();
    }


        public void ClosePanelButton(int buttonId)
    {
        switch(buttonId)
        {
            case 1:
                GameOverPanel.SetTrigger("close");
                break;
            case 2:
                SettingsPanel.SetTrigger("close");
                break;
            case 3:
                StatsPanel.SetTrigger("close");
                break;
            case 4:
                WinPanel.SetTrigger("close");
                break;
            case 5:
                WrongPanel.SetTrigger("close");
                break;
            case 6:
                SuccessPanel.SetTrigger("close");
                break;
            case 7:
                WarningPanel.SetTrigger("close");
                break;
        }
    }
}
