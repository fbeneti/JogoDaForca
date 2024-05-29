using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Register : MonoBehaviour
{
    public static Register instance;
    public DatabaseBuilder databaseBuilder;

    [Header("Entrada de Dados:")]
    public TMP_InputField UsernameField;
    public TMP_InputField EmailField;
    public TMP_InputField PasswordField;
    public TMP_InputField PasswordVrfField;

    private UIHandler uiHandler;

    void Awake()
    {
        instance = this;
    }


    void Start()
    {
        uiHandler = UIHandler.instance;
    }

    
    public void RegisterPlayer()
    {
        string username = UsernameField.text;
        string email = EmailField.text;
        string password = PasswordField.text;
        string passwordVrf = PasswordVrfField.text;

        if ((username == "") || (email == "") || (password == "") || (passwordVrf == ""))
        {
            uiHandler.WarningCondition("Favor preencher todos os campos!");
            Debug.Log("Favor preencher todos os campos!");
            return;
        }


        if (password != passwordVrf)
        {
            uiHandler.WarningCondition("As senhas não coincidem!");
            Debug.Log("As senhas não coincidem!");
            return;
        }

        List<Dictionary<string, string>> existingUsersByUsername = databaseBuilder.ReadTable("Players", $"Username='{username}'");

        if (existingUsersByUsername.Count > 0)
        {
            uiHandler.WarningCondition("Nome de usuário já cadastrado!");
            Debug.Log("Nome de usuário já cadastrado!");
            return;
        }

        List<Dictionary<string, string>> existingUsersByEmail = databaseBuilder.ReadTable("Players", $"Email='{email}'");

        if (existingUsersByEmail.Count > 0)
        {
            uiHandler.WarningCondition("E-mail já cadastrado!");
            Debug.Log("E-mail já cadastrado!");
            return;
        }

        Dictionary<string, string> newUser = new Dictionary<string, string>
        {
            { "Username", username },
            { "Email", email },
            { "Password", password }
        };
        databaseBuilder.InsertIntoTable("Players", newUser);
        uiHandler.SuccessCondition();
        Debug.Log("Usuário cadastrado com sucesso!");
    }
}