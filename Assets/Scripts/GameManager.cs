using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;


public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;
    public DatabaseBuilder databaseBuilder;

    List<string> solvedList = new List<string>();
    string[] unsolvedWord;
    
    [Header("Letters")]
    [Space]
    public GameObject letterPrefab;
    public Transform letterHolder;
    List<TMP_Text> letterHolderList = new List<TMP_Text>();

    [Header("Categories")]
    public TMP_Text categoryText;

    [Header("Timer")]
    [Space]
    public TMP_Text timerText;
    public TMP_Text playerTurnText;
    int playTime;
    int maxTurnTime = 15;

    [Header("Hints")]
    [Space]
    public int maxHints = 3;

    [Header("Mistakes")]
    [Space]
    public Animator[] petalList;

    [SerializeField]
    int maxMistakes;
    int currentMistakes;

    private bool gameOver;
    private Coroutine timerCoroutine;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        TurnManager.OnTurnChanged += () => UpdateTurnUI();
    }


    // Start is called before the first frame update
    private void Start()
    {
        maxMistakes = petalList.Length;
        Initialize();
        StartCoroutine(Timer());
    }


    private void Initialize()
    {
        if (PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnectedAndReady)
        {
            SetInitialWord();
            Timer();
        }
    }
    
    
    public void InputFromButton(string requestedLetter, bool isThatAHint)
    {
        // check if the game is not over yet

        // Search mechanic for solved list
        CheckLetter(requestedLetter, isThatAHint);
    }


    private void CheckLetter(string requestedLetter, bool isThatAHint)
    {
        if (gameOver)
        {
            return;
        }

        bool letterFound = false;
        string normalizedRequestedLetter = RemoveAccents(requestedLetter.ToLower());

        // find the letter in the solved list
        for (int i = 0; i < solvedList.Count; i++)
        {
            //Ignore special characters
            if (solvedList[i] == "-" || solvedList[i] == " ")
            {
                continue; 
            }

            string normalizedSolvedLetter = RemoveAccents(solvedList[i].ToLower());
            if (normalizedSolvedLetter == normalizedRequestedLetter)
            {
                letterHolderList[i].text = solvedList[i];
                unsolvedWord[i] = solvedList[i];
                letterFound = true;
            }
        }


        if (!letterFound && !isThatAHint)
        {
            //Mistake stuff - Graphical representation
            petalList[currentMistakes].SetTrigger("miss");
            currentMistakes++;

            if (currentMistakes == maxMistakes)
            {
                //Do game over
                UIHandler.instance.LoseCondition(playTime);
                gameOver = true;
                return;
            }
        }

        //Check if game won
        gameOver = CheckIfWon();
        if (gameOver)
        {
            // SHOW UI
            UIHandler.instance.WinCondition(playTime);
        }
    }

    private bool CheckIfWon()
    {
        //Check mechanick
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


    private IEnumerator Timer()
    {
        int seconds = 0;
        int minutes = 0;
        timerText.text = minutes.ToString("D2") + ":" + seconds.ToString("D2");

        // Wait for 5 seconds before starting the timer
        yield return new WaitForSeconds(5);

        // Start the timer after 5 seconds
        while (!gameOver)
        {
            yield return new WaitForSeconds(1);
            playTime++;

            seconds = playTime % 60;
            minutes = playTime / 60 % 60;

            timerText.text = minutes.ToString("D2") + ":" + seconds.ToString("D2");
        }
    }


    private void ResetAndSyncTimer()
    {
        playTime = maxTurnTime;
        UpdateTimerUI(playTime);
        photonView.RPC("SyncTime", RpcTarget.AllBuffered, playTime);
    }


    [PunRPC]
    private void SyncTime(int time)
    {
        // Update time received from all clients
        playTime = time;

        // Update the time counter on the user interface
        UpdateTimerUI(playTime);
    }


    private void UpdateTimerUI(int seconds)
    {
        timerText.text = seconds.ToString("D2");
    }


    private void SetInitialWord()
    {
        //Load categories from database
        List<Dictionary<string, string>> categories = databaseBuilder.ReadTable("Categories");
        if (categories.Count == 0)
        {
            Debug.LogError("Nenhuma categoria encontrada no banco de dados.");
            return;
        }

        //Pick a category from the list
        int cIndex = Random.Range(0, categories.Count);
        var selectedCategory = categories[cIndex];
        string categoryName = selectedCategory["Categoria"];

        //Load words from selected category on the words list 
        int categoryId = int.Parse(selectedCategory["Id"]);
        Debug.Log("Categoria Id = " + selectedCategory["Id"]);
        List<Dictionary<string, string>> words = databaseBuilder.ReadTable("Words", $"Categoria = {categoryId}");
        if (words.Count == 0)
        {
            Debug.LogError("Nenhuma palavra encontrada para a categoria selecionada.");
            return;
        }

        //Pick a word from the list
        int wIndex = Random.Range(0, words.Count);
        string pickedWord = words[wIndex]["Nome"];
        Debug.Log("Palavra: " + pickedWord);

        if (PhotonNetwork.IsConnectedAndReady)
        {
            photonView.RPC("SyncInitialWord", RpcTarget.AllBuffered, categoryName, pickedWord);
        }
        else
        {
            SyncInitialWord(categoryName, pickedWord);
        }
    }


    [PunRPC]
    private void SyncInitialWord(string categoryName, string pickedWord)
    {
        Debug.Log($"SyncInitialWord called with categoryName: {categoryName} and pickedWord: {pickedWord}");

        categoryText.text = CapitalizeFirstLetter(categoryName);
        Debug.Log($"Formatted categoryName: {categoryText.text}");

        string[] splittedWord = pickedWord.Select(l => l.ToString()).ToArray();
        unsolvedWord = new string[splittedWord.Length];
        solvedList = new List<string>(splittedWord);

        //Create the Visual
        for (int i = 0; i < solvedList.Count; i++)
        {
            GameObject tempLetter = Instantiate(letterPrefab, letterHolder, false);
            TMP_Text letterText = tempLetter.GetComponent<TMP_Text>();

            if (letterText != null)
            {
                if (solvedList[i] == "-" || solvedList[i] == " ")
                {
                    // Mostrar caracteres especiais diretamente
                    letterText.text = solvedList[i];
                    unsolvedWord[i] = solvedList[i];
                }
                else
                {
                    letterText.GetComponent<TMP_Text>();
                }
                letterHolderList.Add(letterText);
            }
        }
    }


    [PunRPC]
    private void SyncPlayerNickname(string newNickname)
    {
        playerTurnText.text = "Vez do Jogador: " + newNickname;


    }


    public void UpdateTurnUI()
    {
        int currentTurnActorNumber = TurnManager.instance.GetCurrentPlayer();
        Player currentPlayer = PhotonNetwork.CurrentRoom.GetPlayer(currentTurnActorNumber);
        if (currentPlayer != null)
        {
            photonView.RPC("SyncPlayerNickname", RpcTarget.AllBuffered, currentPlayer.NickName);
        }
    }


    [PunRPC]
    private void SyncGameOver()
    {
        if (TurnManager.instance.IsMyTurn())
        {
            playerTurnText.gameObject.SetActive(false);
            UIHandler.instance.WinCondition(playTime);
        }
        else
        {
            playerTurnText.gameObject.SetActive(false);
            UIHandler.instance.LoseCondition(playTime);
        }
    }


    [PunRPC]
    private void SyncGameOverLose()
    {
        if (!TurnManager.instance.IsMyTurn())
        {
            playerTurnText.gameObject.SetActive(false);
            UIHandler.instance.WinCondition(playTime);
        }
        else
        {
            playerTurnText.gameObject.SetActive(false);
            UIHandler.instance.LoseCondition(playTime);
        }
    }


    public string CapitalizeFirstLetter(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        input = input.ToLower(); 
        return char.ToUpper(input[0]) + input.Substring(1);
    }


    public static string RemoveAccents(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return text;

        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }
}