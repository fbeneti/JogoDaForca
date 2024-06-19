using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ButtonCreator : MonoBehaviour
{
    public static ButtonCreator instance;
    private DatabaseBuilder databaseBuilder;
    private StoreManager storeManager;
    private GameManager gameManager;

    [Header("Painel Alerta")]
    public Animator alertPanel;
    public TMP_Text alertMessage;
    public TMP_Text yesButton;
    public TMP_Text noButton;

    [Space]
    [Header("Dica")]
    public TMP_Text hintText;

    [Space]
    [Header("Botões")]
    public GameObject buttonPrefab;
    public Transform buttonholder1;
    public Transform buttonholder2;
    public Transform buttonholder3;
    private bool exists = false;
    private bool freeHintsOver = false;
    private int countHint = 1;
    private bool hintsBtn = false;
    private bool extraLifesBtn = false;

    string[] lettersToUse = new string[27] 
    {
        "Q","W","E","R","T","Y","U","I","O","P",
        "A","S","D","F","G","H","J","K","L","�",
        "Z","X","C","V","B","N","M"
    };

    List<LetterButton> letterList = new List<LetterButton>();

    void Awake()
    {
        instance = this;

        gameManager = GameManager.instance;

    }

    // Start is called before the first frame update
    void Start()
    {
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
        
        storeManager = StoreManager.instance;
        hintText.text = "";
        PopulateKeyboard();
    }

    // Update is called once per frame
    void PopulateKeyboard()
    {
        for (int i = 0; i < lettersToUse.Length; i++)
        {
            if (i < 10)
            {
                GameObject newButton = Instantiate(buttonPrefab, buttonholder1, false);
                newButton.GetComponentInChildren<TMP_Text>().text = lettersToUse[i];
                LetterButton myLetter = newButton.GetComponent<LetterButton>();
                myLetter.SetButton(lettersToUse[i]);

                letterList.Add(myLetter);
            } else if (i < 20)
            {
                GameObject newButton = Instantiate(buttonPrefab, buttonholder2, false);
                newButton.GetComponentInChildren<TMP_Text>().text = lettersToUse[i];
                LetterButton myLetter = newButton.GetComponent<LetterButton>();
                myLetter.SetButton(lettersToUse[i]);

                letterList.Add(myLetter);
            } else
            {
                GameObject newButton = Instantiate(buttonPrefab, buttonholder3, false);
                newButton.GetComponentInChildren<TMP_Text>().text = lettersToUse[i];
                LetterButton myLetter = newButton.GetComponent<LetterButton>();
                myLetter.SetButton(lettersToUse[i]);

                letterList.Add(myLetter);
            }
        }
    }


    public void RemoveLetter(LetterButton theButton)
    {
        letterList.Remove(theButton);
    }


    //FROM THE HINT BUTTON
    public void UseHint()
    {
        if (GameManager.instance.GameOver() || GameManager.instance.maxHints <= 0)
        {
            if (!freeHintsOver)
            { 
                StartCoroutine(ShowAlertFreeHintsOver());
                freeHintsOver = true;
                return;
            }
            if (GameManager.instance.GameOver() || GlobalVariables.player1Hints <= 0)
            { 
                ShowAlertHintsOver();
                return;
            }
            switch(countHint)
            {
                case 1:
                    hintText.text = GlobalVariables.wordHint1;
                    GlobalVariables.player1Hints--;
                    countHint++;
                    UpdateHintOnDatabase();
                    break;
                case 2:
                    hintText.text = GlobalVariables.wordHint2;
                    GlobalVariables.player1Hints--;
                    countHint++;
                    UpdateHintOnDatabase();
                    break;
                case 3:
                    hintText.text = GlobalVariables.wordHint3;
                    GlobalVariables.player1Hints--;
                    countHint++;
                    UpdateHintOnDatabase();
                    break;
                case 4:
                    StartCoroutine(ShowAlertNoMoreHints());
                    break;
            }
            return;
        }
        GameManager.instance.maxHints--;
        StartCoroutine(SortLetter());
    }


    public IEnumerator ShowAlertFreeHintsOver()
    {
        Debug.Log("Voce não tem mais dicas!");
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
        alertPanel.SetTrigger("Show");
        alertMessage.text = "Acabaram as dicas gratuítas!";
        yield return new WaitForSeconds(3);
        alertPanel.SetTrigger("Hide");
    }


    public IEnumerator ShowAlertNoMoreHints()
    {
        Debug.Log("Acabaram todas as dicas! Agora é com você...");
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
        alertPanel.SetTrigger("Show");
        alertMessage.text = "Acabaram todas as dicas! Agora é com você...";

        yield return new WaitForSeconds(3);
        alertPanel.SetTrigger("Hide");
    }


    public void ShowAlertHintsOver()
    {
        Debug.Log("Voce não tem mais dicas!");
        alertPanel.SetTrigger("Show");
        alertMessage.text = "Acabaram as dicas! Deseja comprar mais?";
        hintsBtn = true;
        yesButton.gameObject.SetActive(true);
        noButton.gameObject.SetActive(true);
    }


    public void AlertPanelNoButton()
    {
        alertPanel.SetTrigger("Hide");
        hintsBtn = false;
    }


    public void AlertPanelYesButton()
    {
        if (GlobalVariables.player1Diamonds > 0)
        {
            storeManager.AddHints(1);
            alertMessage.text = "Dica comprada com sucesso!";
            hintsBtn = false;

            StartCoroutine(CloseAlertPanel());
        }
        else
        { 
            alertMessage.text = "Você não tem diamantes suficientes...";
        
            StartCoroutine(CloseAlertPanel());
        }
    }


    public IEnumerator CloseAlertPanel()
    {
        yield return new WaitForSeconds(2);
        
        alertPanel.SetTrigger("Hide");
    }


    public void UpdateHintOnDatabase()
    {
        Dictionary<string, string> player1 = new Dictionary<string, string>
        {
            {"Hints", GlobalVariables.player1Hints.ToString()}
        };
        databaseBuilder.UpdateTable("Players", player1, $"Username = '{GlobalVariables.player1Name}'");
    }

   
    public IEnumerator SortLetter()
    {
        while (!exists)
        { 
            int randomIndex = Random.Range(0, GlobalVariables.solvedList.Count);    
            string selectedLetter = GlobalVariables.solvedList[randomIndex];

            exists = letterList.Exists(button => button.letter == selectedLetter);

            if (exists)
            {
                LetterButton letterButton = letterList.Find(button => button.letter == selectedLetter);
                if (letterButton != null)
                {
                    letterButton.Sendletter(true);
                }
            }
            yield return null;
        }
    }
}
