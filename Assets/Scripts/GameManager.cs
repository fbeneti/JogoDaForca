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
    
    [Space]
    [Header("Dificulties")]
    public TMP_Text difficultyText;

    [Space]
    [Header("Categories")]
    public TMP_Text categoryText;

    [Header("Letters")]
    public GameObject keyboardSection;
    public GameObject letterPrefab;
    public Transform letterHolder;
    List<TMP_Text> letterHolderList = new List<TMP_Text>();

    [Space]
    [Header("Timer")]
    public TMP_Text timerText;
    public TMP_Text playerTurnText;
    public TMP_Text feedbackText;
    int playTime;
    int maxTurnTime = 25;

    [Space]
    [Header("Free Hints")]
    public int maxHints = 1;

    [Space]
    [Header("Parts")]
    public Animator[] partList;

    [SerializeField]
    int maxMistakes;
    public int currentMistakes;

    [Space]
    [Header("Hearts")]
    public Animator[] heartList;

    [Space]
    [Header("Audio")]
    public AudioSource audioAcertou1;
    public AudioSource audioAcertou2;
    public AudioSource audioErrou;
    public AudioSource audioGanhou;
    public AudioSource audioPerdeu1;
    public AudioSource audioPerdeu2;
    public AudioSource audioComecar;

    [Space]
    [Header("Vitao")]
    public Animator hostPanel;

    private Conn conn;
    private SceneLoader sceneLoader;
    private UIHandler uiHandler;
    private bool gameOver;

    private Coroutine timerCoroutine;


    private void Awake()
    {
        instance = this;
        TurnManager.OnTurnChanged += () => UpdateTurnUI();
    }


    // Start is called before the first frame update
    private void Start()
    {
        conn = Conn.instance;
        sceneLoader = SceneLoader.instance;
        uiHandler = UIHandler.instance;
        maxMistakes = partList.Length;

        Initialize();
        StartCoroutine(Timer());
    }


    private void Initialize()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SetInitialWord();
            Timer();
        }
    }
    
    
    public void InputFromButton(string requestedLetter, bool isThatAHint)
    {
        // Search mechanic for solved list
        CheckLetter(requestedLetter, isThatAHint);
    }


    private void CheckLetter(string requestedLetter, bool isThatAHint)
    {
        if (gameOver) return;

        bool letterFound = false;

        // remove accents from the word
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
                VitaoAcertou();
                string hitMessage = PhotonNetwork.LocalPlayer.NickName + " Acertou!";
                photonView.RPC("ShowFeedbackMessage", RpcTarget.All, hitMessage);
            }
        }

        if (!letterFound && !isThatAHint)
        {
            //Mistake stuff - Graphical representation
            partList[currentMistakes].SetTrigger("miss");
            heartList[currentMistakes].SetTrigger("hide");
            currentMistakes++;
            VitaoErrou();
            string mistakeMessage = PhotonNetwork.LocalPlayer.NickName + " errou!";
            photonView.RPC("ShowFeedbackMessage", RpcTarget.All, mistakeMessage);


            if (currentMistakes == maxMistakes)
            {
                //Do game over
                photonView.RPC("SyncGameOverLose", RpcTarget.All);
                VitaoLose();
                gameOver = true;
                return;
            }
        }

        //Check if game won
        if (!gameOver && CheckIfWon())
        {
            gameOver = true;
            photonView.RPC("SyncGameOver", RpcTarget.All);
            VitaoWin();
        }
        TurnManager.instance.EndTurn();
        ResetAndSyncTimer();
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


    public void VitaoAcertou()
    {
        hostPanel.SetTrigger("Show");
        int iAcertou = Random.Range(1, 3);
        switch(iAcertou)
        {
            case 1:
                StartCoroutine(PlayAcertou1());
                break;
            case 2:
                StartCoroutine(PlayAcertou2());
                break;
        }
    }


    private IEnumerator PlayAcertou1()
    {
        audioAcertou1.Play();
        yield return new WaitForSeconds(4);
        hostPanel.SetTrigger("Hide");
    }


    private IEnumerator PlayAcertou2()
    {
        audioAcertou2.Play();
        yield return new WaitForSeconds(3);
        hostPanel.SetTrigger("Hide");
    }


    public void VitaoErrou()
    {
        StartCoroutine(PlayErrou());
    }


    private IEnumerator PlayErrou()
    {
        hostPanel.SetTrigger("Show");
        audioErrou.Play();
        yield return new WaitForSeconds(3);
        hostPanel.SetTrigger("Hide");
    }


    public void VitaoWin()
    {
        StartCoroutine(PlayGanhou());
    }


    private IEnumerator PlayGanhou()
    {
        yield return new WaitForSeconds(3);
        audioGanhou.Play();
    }


    public void VitaoLose()
    {
        int iPerdeu = Random.Range(1, 3);
        switch(iPerdeu)
        {
            case 1:
                StartCoroutine(PlayPerdeu1());
                break;
            case 2:
                StartCoroutine(PlayPerdeu2());
                break;
        }
    }


    private IEnumerator PlayPerdeu1()
    {
        yield return new WaitForSeconds(3);
        audioPerdeu1.Play();
    }


    private IEnumerator PlayPerdeu2()
    {
        yield return new WaitForSeconds(3);
        audioPerdeu2.Play();
    }


    private IEnumerator Timer()
    {
        playTime = maxTurnTime;
        UpdateTimerUI(playTime);

        yield return new WaitForSeconds(5);

        while (!gameOver)
        {
            yield return new WaitForSeconds(1f);
            playTime--;
            UpdateTimerUI(playTime);
            if (playTime <= 0)
            {
                string timeMessage = PhotonNetwork.LocalPlayer.NickName + " Acabou o tempo!";
                photonView.RPC("ShowFeedbackMessage", RpcTarget.All, timeMessage);
                TurnManager.instance.EndTurn();
                ResetAndSyncTimer();
            }
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
        difficultyText.text = CapitalizeFirstLetter(GlobalVariables.actualDifficulty);
        Debug.Log($"Dificuldade = {difficultyText.text}");
        categoryText.text = CapitalizeFirstLetter(GlobalVariables.actualCategoryName);
        Debug.Log($"Categoria = {categoryText.text}");
        Debug.Log($"Palavra = {GlobalVariables.actualWord}");

        // Sync Word, Category and Dificulty between players
        photonView.RPC("SyncInitialWord", RpcTarget.AllBuffered, GlobalVariables.actualDifficulty, GlobalVariables.actualCategoryName, GlobalVariables.actualCategoryId, GlobalVariables.actualWord, GlobalVariables.wordHint1, GlobalVariables.wordHint2, GlobalVariables.wordHint3);
    }


    [PunRPC]
    private void SyncInitialWord(string difficultyName, string categoryName, int categoryId, string pickedWord, string wordHint1, string wordHint2, string wordHint3)
    {
        difficultyText.text = CapitalizeFirstLetter(difficultyName);
        categoryText.text = CapitalizeFirstLetter(categoryName);
        pickedWord = pickedWord.ToUpper();

        GlobalVariables.actualCategoryName = categoryName;
        GlobalVariables.actualDifficulty = difficultyName;
        GlobalVariables.actualWord = pickedWord;
        GlobalVariables.wordHint1 = wordHint1;
        GlobalVariables.wordHint2 = wordHint2;
        GlobalVariables.wordHint3 = wordHint3;

        string[] splittedWord = pickedWord.Select(l => l.ToString()).ToArray();
        unsolvedWord = new string[splittedWord.Length];
        solvedList = new List<string>(splittedWord);
        GlobalVariables.solvedList = solvedList;

        // Clear letters list to avoid memory leak
        foreach (var letter in letterHolderList)
        {
            Destroy(letter.gameObject);
        }
        letterHolderList.Clear();

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
            feedbackText.gameObject.SetActive(false);
            keyboardSection.gameObject.SetActive(false);
            UIHandler.instance.WinCondition(playTime);
        }
        else
        {
            playerTurnText.gameObject.SetActive(false);
            feedbackText.gameObject.SetActive(false);
            keyboardSection.gameObject.SetActive(false);
            UIHandler.instance.LoseCondition(playTime);
        }
    }


    [PunRPC]
    private void SyncGameOverLose()
    {
        if (!TurnManager.instance.IsMyTurn())
        {
            playerTurnText.gameObject.SetActive(false);
            feedbackText.gameObject.SetActive(false);
            keyboardSection.gameObject.SetActive(false);
            UIHandler.instance.WinCondition(playTime);
        }
        else
        {
            playerTurnText.gameObject.SetActive(false);
            feedbackText.gameObject.SetActive(false);
            keyboardSection.gameObject.SetActive(false);
            UIHandler.instance.LoseCondition(playTime);
        }
    }


    [PunRPC]
    private void ShowFeedbackMessage(string message)
    {
        feedbackText.text = message;
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

    //private IEnumerator TotalGameTimer()
    //{
    //    int seconds = 0;
    //    int minutes = 0;
    //    timerText.text = minutes.ToString("D2") + ":" + seconds.ToString("D2");

        // Wait for 5 seconds before starting the timer
    //    yield return new WaitForSeconds(5);

        // Start the timer after 5 seconds
    //    while (!gameOver)
    //    {
    //        yield return new WaitForSeconds(1);
    //        playTime++;

    //        seconds = playTime % 60;
    //        minutes = playTime / 60 % 60;

    //        timerText.text = minutes.ToString("D2") + ":" + seconds.ToString("D2");
    //    }
    //}
