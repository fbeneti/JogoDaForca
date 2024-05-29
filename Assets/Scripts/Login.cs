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


    void Awake()
    {
        instance = this;
    }


    void Start()
    {
        uiHandler = UIHandler.instance;
    }


    public void CheckPlayerLogin()
    {
        string username = UsernameField.text;
        string password = PasswordField.text;

        List<Dictionary<string, string>> players = databaseBuilder.ReadTable("Players", $"Username='{username}'");

        if (players.Count > 0)
        {
            var currentPlayer = players[0];
        
            Debug.Log("Player encontrado:");
            if (currentPlayer.ContainsKey("Password") && currentPlayer["Password"] == password)
            {
                Debug.Log("Usuário logado com sucesso!");
                uiHandler.SuccessCondition();
                uiHandler.SetLoginSuccess(true);
            }
            else
            {
                Debug.Log("Senha incorreta.");
                uiHandler.WrongCondition("Senha incorreta");
                uiHandler.SetLoginSuccess(false);
            }
        }
        else
        {
            Debug.Log("Nenhum jogador encontrado com o nome de usuário especificado.");
            uiHandler.WrongCondition("Nenhum jogador encontrado com o nome de usuário especificado.");
            uiHandler.SetLoginSuccess(false);
        }
    }
}