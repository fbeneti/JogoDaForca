using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Login : MonoBehaviour
{
    public DatabaseBuilder databaseBuilder;

    [Header("Paineis")]
    public Animator WrongPanel;      //ID 1
    public Animator SuccessPanel;    //ID 2
    
    [Header("Entrada de Dados:")]
    public TMP_InputField UsernameField;
    public TMP_InputField PasswordField;

    public void BotaoJogar()
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
                SuccessPanel.SetTrigger("open");
            }
            else
            {
                Debug.Log("Senha incorreta.");
                WrongPanel.SetTrigger("open");
            }
        }
        else
        {
            Debug.Log("Nenhum jogador encontrado com o nome de usuário especificado.");
            WrongPanel.SetTrigger("open");
        }
    }

        public void ClosePanelButton(int buttonId)
    {
        switch(buttonId)
        {
            case 1:
                PasswordField.text = "";
                WrongPanel.SetTrigger("close");
                break;
            case 2:
                UsernameField.text = "";
                PasswordField.text = "";
                SuccessPanel.SetTrigger("close");
                break;
        }
    }
}