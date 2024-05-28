using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIHandler : MonoBehaviour
{
    public static UIHandler instance;

    private DataService _ds;

    public Animator GameOverPanel; //ID 1
    public Animator SettingsPanel; //ID 2
    public Animator StatsPanel;    //ID 3
    public Animator WinPanel;      //ID 4
    public Animator LoginPanel;    //ID 5
    public Animator RegisterPanel; //ID 6

    [Header("STATS")]
    public TMP_Text statsText;
    [SerializeField] SCR_BaseStats saveFile;

    void Awake()
    {
        instance = this;
    }


    void Start()
    {
        UpdateStatsText();
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


    void UpdateStatsText()
    {
        List<int> statsList = saveFile.GetStats();
        statsText.text =
            "N° de Vitórias:    " + statsList[0] + "\n" +
            "N° de Derrotas:    " + statsList[1] + "\n" +
            "Total de Jogos:    " + statsList[3] + "\n" +
            "Taxa de Vitória:   " + statsList[2] + "% \n" +
            "Menor Tempo:       " + statsList[4] + "segundos \n";
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
        }
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

}
