using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Register : MonoBehaviour
{
    public DatabaseBuilder databaseBuilder;

    [Header("Paineis")]
    public Animator VrfFieldsPanel; //ID 1
    public Animator VrfPswPanel;    //ID 2
    public Animator SuccessPanel;    //ID 3

    [Header("Entrada de Dados:")]
    public TMP_InputField UsernameField;
    public TMP_InputField EmailField;
    public TMP_InputField PasswordField;
    public TMP_InputField PasswordVrfField;

    public void BotaoRegistrar()
    {
        string username = UsernameField.text;
        string email = EmailField.text;
        string password = PasswordField.text;
        string passwordVrf = PasswordVrfField.text;

        if ((username == "") || (email == "") || (password == "") || (passwordVrf == ""))
        {
            VrfFieldsPanel.SetTrigger("open");
            Debug.Log("Favor preencher todos os campos!");
            return;
        }


        if (password != passwordVrf)
        {
            VrfPswPanel.SetTrigger("open");
            Debug.Log("As senhas não coincidem!");
            return;
        }

        List<Dictionary<string, string>> existingUsers = databaseBuilder.ReadTable("Players", $"Username='{username}'");

        if (existingUsers.Count > 0)
        {
            Debug.Log("Nome de usuário já existe.");
            return;
        }

        Dictionary<string, string> newUser = new Dictionary<string, string>
        {
            { "Username", username },
            { "Email", email },
            { "Password", password }
        };

        databaseBuilder.InsertIntoTable("Players", newUser);
        Debug.Log("Usuário registrado com sucesso!");
    }

    public void ClosePanelButton(int buttonId)
    {
        switch(buttonId)
        {
            case 1:
                VrfFieldsPanel.SetTrigger("close");
                break;
            case 2:
                PasswordField.text = "";
                PasswordVrfField.text = "";
                VrfPswPanel.SetTrigger("close");
                break;
            case 3:
                UsernameField.text = "";
                EmailField.text = "";
                PasswordField.text = "";
                PasswordVrfField.text = "";
                SuccessPanel.SetTrigger("close");
                break;
        }
    }
}