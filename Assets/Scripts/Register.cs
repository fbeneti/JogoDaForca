using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Register : MonoBehaviour
{
    public static Register instance;
    private DatabaseBuilder databaseBuilder;

    [Header("Entrada de Dados:")]
    public TMP_InputField UsernameField;
    public TMP_InputField EmailField;
    public TMP_InputField PasswordField;
    public TMP_InputField PasswordVrfField;

    [Space]
    [Header("Audios")]
    public AudioSource audioWriting;
    
    private UIHandler uiHandler;
    private bool messageDisplayed = false;  // Flag to track if a message is displayed


    void Awake()
    {
        instance = this;
    }


    void Start()
    {
        uiHandler = UIHandler.instance;

        // If DatabaseBuilder wasn't instantiated, instantiate and initiate it
        if (DatabaseBuilder.instance == null)
        {
           GameObject dbGameObject = new GameObject("DatabaseBuilder");
            databaseBuilder = dbGameObject.AddComponent<DatabaseBuilder>();
            databaseBuilder.Initialize();
        }
        else
        {
            databaseBuilder = DatabaseBuilder.instance;
        }

        // Adicione os eventos para fechar o painel de mensagem
        UsernameField.onSelect.AddListener(delegate { OnInputFieldSelect(); });
        UsernameField.onValueChanged.AddListener(delegate { OnInputFieldEdit(); });
        UsernameField.onValueChanged.AddListener(OnInputValueChanged);
        EmailField.onSelect.AddListener(delegate { OnInputFieldSelect(); });
        EmailField.onValueChanged.AddListener(delegate { OnInputFieldEdit(); });
        EmailField.onValueChanged.AddListener(OnInputValueChanged);
        PasswordField.onSelect.AddListener(delegate { OnInputFieldSelect(); });
        PasswordField.onValueChanged.AddListener(delegate { OnInputFieldEdit(); });
        PasswordField.onValueChanged.AddListener(OnInputValueChanged);
        PasswordVrfField.onSelect.AddListener(delegate { OnInputFieldSelect(); });
        PasswordVrfField.onValueChanged.AddListener(delegate { OnInputFieldEdit(); });
        PasswordVrfField.onValueChanged.AddListener(OnInputValueChanged);
    }

    
    public void RegisterPlayer()
    {
        string username = UsernameField.text;
        string email = EmailField.text;
        string password = PasswordField.text;
        string passwordVrf = PasswordVrfField.text;

        if ((username == "") || (email == "") || (password == "") || (passwordVrf == ""))
        {
            uiHandler.MessageCondition("ATENÇÃO", "Favor preencher todos os campos!");
            Debug.Log("Favor preencher todos os campos!");
            messageDisplayed = true;
            return;
        }


        if (password != passwordVrf)
        {
            uiHandler.MessageCondition("ATENÇÃO", "As senhas não coincidem!");
            Debug.Log("As senhas não coincidem!");
            messageDisplayed = true;
            return;
        }

        List<Dictionary<string, string>> existingUsersByUsername = databaseBuilder.ReadTable("Players", $"Username='{username}'");

        if (existingUsersByUsername.Count > 0)
        {
            uiHandler.MessageCondition("ATENÇÃO", "Nome de usuário já cadastrado!");
            Debug.Log("Nome de usuário já cadastrado!");
            messageDisplayed = true;
            return;
        }

        List<Dictionary<string, string>> existingUsersByEmail = databaseBuilder.ReadTable("Players", $"Email='{email}'");

        if (existingUsersByEmail.Count > 0)
        {
            uiHandler.MessageCondition("ATENÇÃO", "E-mail já cadastrado!");
            Debug.Log("E-mail já cadastrado!");
            messageDisplayed = true;
            return;
        }

        Dictionary<string, string> newUser = new Dictionary<string, string>
        {
            { "Username", username },
            { "Email", email },
            { "Password", password }
        };
        databaseBuilder.InsertIntoTable("Players", newUser);
        uiHandler.MessageCondition("AVISO", "Usuário cadastrado com sucesso!");
        Debug.Log("Usuário cadastrado com sucesso!");
        uiHandler.SetRegisterSuccess(true);
        messageDisplayed = true;
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


        void OnInputValueChanged(string value)
    {
        audioWriting.Play();
    }
}