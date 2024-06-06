using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Login : MonoBehaviour
{
    public static Login instance;
    public DatabaseBuilder databaseBuilder;
    
    [Header("Entrada de Dados:")]
    public TMP_InputField UsernameField;
    public TMP_InputField PasswordField;

    private UIHandler uiHandler;
    private bool loginSuccess = false;      // Flag to track if player is logged
    private bool messageDisplayed = false;  // Flag to track if a message is displayed


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


    void Start()
    {
        uiHandler = UIHandler.instance;

        // Adicione os eventos para fechar o painel de mensagem
        UsernameField.onSelect.AddListener(delegate { OnInputFieldSelect(); });
        UsernameField.onValueChanged.AddListener(delegate { OnInputFieldEdit(); });
        PasswordField.onSelect.AddListener(delegate { OnInputFieldSelect(); });
        PasswordField.onValueChanged.AddListener(delegate { OnInputFieldEdit(); });
    }


    public void CheckPlayerLogin()
    {
        string username = UsernameField.text;
        string password = PasswordField.text;

        List<Dictionary<string, string>> players = databaseBuilder.ReadTable("Players", $"Username='{username}'");

        if (loginSuccess)
        {
            uiHandler.BackToMenu("4-Menu");
            return;
        }

        if (players.Count > 0)
        {
            var currentPlayer = players[0];
        
            Debug.Log("Player encontrado:");
            if (currentPlayer.ContainsKey("Password") && currentPlayer["Password"] == password)
            {
                Debug.Log("Usuário logado com sucesso!");
                GlobalVariables.playerName = username;
                GlobalVariables.playerAvatar = int.Parse(currentPlayer["Avatar"]);
                GlobalVariables.playerVictories = int.Parse(currentPlayer["Victories"]);
                GlobalVariables.playerLosses = int.Parse(currentPlayer["Losses"]);
                GlobalVariables.playerDiamonds = int.Parse(currentPlayer["Diamonds"]);
                GlobalVariables.playerCoins = int.Parse(currentPlayer["Coins"]);                
                GlobalVariables.playerHints = int.Parse(currentPlayer["Hints"]);
                GlobalVariables.playerExtraLifes = int.Parse(currentPlayer["ExtraLifes"]);
                GlobalVariables.playerStealTime = int.Parse(currentPlayer["StealTime"]);
                GlobalVariables.playerFogs = int.Parse(currentPlayer["Fogs"]);
                uiHandler.MessageCondition("AVISO", "Usuário logado com sucesso!");
                uiHandler.SetLoginSuccess(true);
                messageDisplayed = true;
                loginSuccess = true;
            }
            else
            {
                Debug.Log("Senha incorreta.");
                uiHandler.MessageCondition("ATENÇÃO", "Senha incorreta");
                uiHandler.SetLoginSuccess(false);
                messageDisplayed = true;
            }
        }
        else
        {
            Debug.Log("Nenhum jogador encontrado com o nome de usuário especificado.");
            uiHandler.MessageCondition("ATENÇÃO", "Nenhum jogador encontrado com o nome de usuário especificado.");
            uiHandler.SetLoginSuccess(false);
            messageDisplayed = true;
        }
    }


    void OnInputFieldSelect()
    {
        if (messageDisplayed)
        {
            // Fecha o painel de mensagem ao selecionar um campo de entrada
            uiHandler.ClosePanelButton(8);
            messageDisplayed = false;
        }
    }


    void OnInputFieldEdit()
    {
        if (messageDisplayed)
        {
            // Fecha o painel de mensagem ao editar um campo de entrada
            uiHandler.ClosePanelButton(8);
            messageDisplayed = false;
        }
    }
}