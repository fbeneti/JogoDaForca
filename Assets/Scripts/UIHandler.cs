using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class UIHandler : MonoBehaviourPun
{
    public static UIHandler instance;
    private Conn conn;
    private Login login;
    private Register register;
    private bool loginSuccess = false;
    private bool registerSuccess = false;

    [Header("PANELS")]
    public Animator GameOverPanel;       //ID 1
    public Animator SettingsPanel;       //ID 2
    public Animator StatsPanel;          //ID 3
    public Animator WinPanel;            //ID 4
    public Animator WrongPanel;          //ID 5
    public TMP_Text WrongText;           //Texto do painel
    public Animator SuccessPanel;        //ID 6
    public Animator WarningPanel;        //ID 7
    public TMP_Text WarningText;         //Texto do painel
    public Animator MessagePanel;        // ID 8
    public TMP_Text MessagePanelTitle;   //   Message Title
    public TMP_Text MessagePanelText;    //   Message Text
    [Header("GamePanels")]
    public GameObject[] hangPanels;

    [Space]
    [Header("STATS")]
    public TMP_Text statsText;
    [SerializeField] SCR_BaseStats saveFile;
  

    void Awake()
    {
        instance = this;
    }


    void Start()
    {
        conn = Conn.instance;
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
        Login.instance.CheckPlayerLogin();
    }


    //Go to registration function
    public void RegisterButton()
    {
        register.RegisterPlayer();
    }


    public void MessagePanelClick()
    {
        if (loginSuccess)
        { 
            SceneManager.LoadScene("4-Menu");
        }
        else if (registerSuccess)
        {
            SceneManager.LoadScene("2-Login");
        }
        else
        { 
            MessagePanel.SetTrigger("close");
        }
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
            case 8:
                MessagePanel.SetTrigger("close");
                break;
        }
    }


    public void ActivateGamePanel(int categoryId)
    {
        // Formatar o categoryId como uma string de 2 dígitos
        string panelName = $"HangPanel{categoryId:D2}";

        // Desativar todos os painéis
        foreach (GameObject panel in hangPanels)
        {
            panel.SetActive(false);
        }

        // Ativar o painel correspondente à categoria
        bool panelFound = false;
        foreach (GameObject panel in hangPanels)
        {
            if (panel.name == panelName)
            {
                panel.SetActive(true);
                panelFound = true;
                break;
            }
        }

        // Se nenhum painel correspondente for encontrado, ativar o painel padrão
        if (!panelFound && hangPanels.Length > 0)
        {
            hangPanels[0].SetActive(true);
        }
    }


    public void MultiplayerButton()
    {
        PhotonNetwork.NickName = GlobalVariables.player1Name;
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
        SceneManager.LoadScene("6-Lobby");
    }
    
    
    void UpdateStatsText()
    {
        List<int> statsList = saveFile.GetStats();
        statsText.text =
            "No de Vitórias:    " + statsList[0] + "\n" +
            "No de Derrotas:    " + statsList[1] + "\n" +
            "Total de Jogos:    " + statsList[3] + "\n" +
            "Taxa de Vitória:   " + statsList[2] + "% \n" +
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

    //Open Message Panel
    public void SuccessCondition()
    {
        SuccessPanel.SetTrigger("open");
    }


    //Open Message Panel
    public void MessageCondition(string panelTitle, string message)
    {
        MessagePanel.SetTrigger("open");
        MessagePanelTitle.text = panelTitle;
        MessagePanelText.text = message;
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


    public void SetRegisterSuccess(bool success)
    {
        registerSuccess = success;
    }


    public void BackToMenu(string levelToLoad)
    {
        SceneManager.LoadScene(levelToLoad);
    }


    public void RestartGame()
    {
        // Check if PhotonView are found and log a message
        if (photonView != null)
        {
            Debug.Log("PhotonView encontrado. Enviando RPC para reiniciar o jogo para todos os jogadores.");
            photonView.RPC("RPC_RestartGame", RpcTarget.All);
        }
        else
        {
            Debug.LogError("PhotonView não encontrado! Certifique-se de que o componente PhotonView está anexado ao objeto.");
            //Reload the current open scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }


    [PunRPC]
    public IEnumerator RPC_RestartGame()
    {
        Debug.Log("RPC_RestartGame chamado. Recarregando a cena.");
        yield return new WaitForSeconds(1f);
        // Adiciona um atraso de 1 segundo
        SceneManager.LoadScene("7-Game");
    }


    public void ExitGame()
    {
        Application.Quit();
    }
}
