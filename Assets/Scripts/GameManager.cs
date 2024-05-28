using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    List<string> solvedList = new List<string>();
    string[] unsolvedWord;

    [Header("Letters")]
    public GameObject letterPrefab;
    public Transform letterHolder;
    List<TMP_Text> letterHolderList = new List<TMP_Text>();

    [Header("Categories")]
    public Category[] categories;
    public TMP_Text categoryText;

    [Header("Timer")]
    public TMP_Text timerText;
    int playTime;

    [Header("Hints")]
    public int maxHints = 3;

    [Header("Mistakes")]
    [Space]
    public Animator[] petalList;

    [SerializeField]
    int maxMistakes;
    int currentMistakes;

    bool gameOver;

    public DatabaseBuilder databaseBuilder;

    void Awake()
    {
        instance = this;
    }

    
    // Start is called before the first frame update
    void Start()
    {
        maxMistakes = petalList.Length;
        Initialize();
        StartCoroutine(Timer());
    }


    void Initialize()
    {
        //LOAD CATEGORIES FROM DATABASE
        List<Dictionary<string, string>> categories = databaseBuilder.ReadTable("Categories");
        if (categories.Count == 0)
        {
            Debug.LogError("Nenhuma categoria encontrada no banco de dados.");
            return;
        }

        //PICK A CATEGORY FROM THE LIST
        int cIndex = Random.Range(0, categories.Count);
        var selectedCategory = categories[cIndex];
        categoryText.text = selectedCategory["Categoria"];

        //LOAD WORDS FROM SELECTED CATEGORY 
        int categoryId = int.Parse(selectedCategory["Id"]);
        Debug.Log("Categoria Id = " + selectedCategory["Id"]);
        List<Dictionary<string, string>> words = databaseBuilder.ReadTable("Words", $"Categoria = {categoryId}");
        if (words.Count == 0)
        {
            Debug.LogError("Nenhuma palavra encontrada para a categoria selecionada.");
            return;
        }

        //PICK A WORD FROM THE LIST
        int wIndex = Random.Range(0, words.Count);
        string pickedWord = words[wIndex]["Nome"];
        Debug.Log("Palavra: " + pickedWord);

        //SPLIT THE WORD INTO SINGLE LETTERS
        string[] splittedWord = pickedWord.Select(l => l.ToString()).ToArray();
        unsolvedWord = new string[splittedWord.Length];
        foreach (string letter in splittedWord) 
        {
            solvedList.Add(letter);
        }

        //CREATE THE VISUAL
        for (int i = 0; i < solvedList.Count; i++)
        {
            GameObject tempLetter = Instantiate(letterPrefab, letterHolder, false);
            letterHolderList.Add(tempLetter.GetComponent<TMP_Text>());
        }
    }


    public void InputFromButton(string requestedLetter, bool isThatAHint)
    {
        //CHECK IF THE GAME IS NOT GAME OVER YET


        //SEARCH MECHANIC FOR SOLVED LIST
        CheckLetter(requestedLetter, isThatAHint);

    }


    void CheckLetter(string requestedLetter, bool isThatAHint)
    {
        if (gameOver)
        {
            return;
        }
        
        bool letterFound = false;

        //FIND THE LETTER IN THE SOLVED LIST
        for (int i = 0;i < solvedList.Count;i++)
        {
            if (solvedList[i].Equals(requestedLetter, System.StringComparison.OrdinalIgnoreCase))
            {
                letterHolderList[i].text = requestedLetter;
                unsolvedWord[i] = requestedLetter;
                letterFound = true;
            }
        }

        if (!letterFound && !isThatAHint)
        {
            //MISTAGE STUFF - GRAPHICAL REPRESENTATION
            petalList[currentMistakes].SetTrigger("miss");
            currentMistakes++;

            if (currentMistakes == maxMistakes)
            {
                //DO GAME OVER
                UIHandler.instance.LoseCondition(playTime);
                gameOver = true;
                return;
            }


        }

        //CHECK IF GAME WON
        gameOver = CheckIfWon();
        if (gameOver)
        {
            //SHOW UI
            UIHandler.instance.WinCondition(playTime);
        }
    }


    bool CheckIfWon()
    {
        //CHECK MECHANICK
        for (int i = 0; i < unsolvedWord.Length; i++)
        {
            if (string.IsNullOrEmpty(unsolvedWord[i]) || !unsolvedWord[i].Equals(solvedList[i], System.StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        return true;
    }

    public bool GameOver()
    {
        return gameOver;
    }
    
    IEnumerator Timer()
    {
        int seconds = 0;
        int minutes = 0;
        timerText.text = minutes.ToString("D2") + ":" + seconds.ToString("D2");

        while (!gameOver)
        {
            yield return new WaitForSeconds(1);
            playTime++;

            seconds = playTime % 60;
            minutes = playTime / 60 % 60;

            timerText.text = minutes.ToString("D2") + ":" + seconds.ToString("D2");
        }
    }
}
