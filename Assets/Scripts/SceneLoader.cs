using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SceneLoader : MonoBehaviour
{
    private UIHandler uiHandler;

    [Space]
    [Header("Player 1")]
    public TMP_Text player1Name;
    public Image player1Avatar;
    public TMP_Text player1Diamonds;
    public TMP_Text player1Coins;
    public TMP_Text player1Hints;
    public TMP_Text player1ExtraLifes;
    public TMP_Text player1StealTime;
    public TMP_Text player1Fogs;
    

    [Space]
    [Header("Player 2")]
    public TMP_Text player2Name;
    public Image player2Avatar;


    void Awake()
    {
        // Subscreve ao evento de cena carregada
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    void Start()
    {
        uiHandler = UIHandler.instance;
    }


    void OnDestroy()
    {
        // Remove a subscrição ao evento de cena carregada
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    // Função de callback que será chamada quando a cena for carregada
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Cena carregada: " + scene.name);
        AfterSceneLoaded();
    }


    // Função para executar após o carregamento da cena
    private void AfterSceneLoaded()
    {
        Debug.Log("Carregando dados do Player 1 na tela.");
                
        if (player1Name != null) player1Name.text = GlobalVariables.player1Name;
        else Debug.LogError("Player1 Name is null");
        
        //Save Player1's avatar RectTransform properties
        RectTransform rectTransform1 = player1Avatar.GetComponent<RectTransform>();
        Vector2 originalSizeDelta1 = rectTransform1.sizeDelta;
        Vector3 originalScale1 = rectTransform1.localScale;

        //Load Player1's avatar image
        string avatarPlayer1Path = "Avatars/avatar" + GlobalVariables.player1Avatar.ToString("D2");
        Sprite avatarPlayer1Sprite = Resources.Load<Sprite>(avatarPlayer1Path);
        Debug.Log("Avatar encontrado em: " + avatarPlayer1Path);

        if (avatarPlayer1Sprite != null)
        {
            player1Avatar.sprite = avatarPlayer1Sprite;

            //Apply RectTransform properties
            rectTransform1.sizeDelta = originalSizeDelta1;
            rectTransform1.localScale = originalScale1;
        }
        else
        {
            Debug.LogError("Avatar não encontrado em: " + avatarPlayer1Path);
        }

        if (player1Diamonds != null) player1Diamonds.text = GlobalVariables.player1Diamonds.ToString();
        else Debug.LogError("Player1 Diamonds is null");

        if (player1Coins != null) player1Coins.text = GlobalVariables.player1Coins.ToString();
        else Debug.LogError("Player1 Coins is null");

        if (player1Hints != null) player1Hints.text = GlobalVariables.player1Hints.ToString();
        else Debug.LogError("Player1 Hints is null");

        if (player1ExtraLifes != null) player1ExtraLifes.text = GlobalVariables.player1ExtraLifes.ToString();
        else Debug.LogError("Player1 Extra Lifes is null");

        if (player1StealTime != null) player1StealTime.text = GlobalVariables.player1StealTime.ToString();
        else Debug.LogError("Player1 Steal Time is null");

        if (player1Fogs != null) player1Fogs.text = GlobalVariables.player1Fogs.ToString();
        else Debug.LogError("Player1 Fogs is null");

        if (SceneManager.GetActiveScene().name == "New Game")
        { 
            Debug.Log("Carregando dados do Player 2 na tela.");
            if (player2Name != null)
            { 
                player2Name.text = GlobalVariables.player2Name;
                Debug.Log("Player2 Name: " + GlobalVariables.player2Name);
            }
            else Debug.LogError("Player2 Name is null");

            //Save Player2's avatar RectTransform properties
            RectTransform rectTransform2 = player2Avatar.GetComponent<RectTransform>();
            Vector2 originalSizeDelta2 = rectTransform2.sizeDelta;
            Vector3 originalScale2 = rectTransform2.localScale;

            //Load Player2's avatar image
            string avatarPlayer2Path = "Avatars/avatar" + GlobalVariables.player2Avatar.ToString("D2");
            Sprite avatarPlayer2Sprite = Resources.Load<Sprite>(avatarPlayer2Path);

            if (avatarPlayer2Sprite != null)
            {
                player2Avatar.sprite = avatarPlayer2Sprite;

                //Apply RectTransform properties
                rectTransform2.sizeDelta = originalSizeDelta2;
                rectTransform2.localScale = originalScale2;
            }
            else
            {
                Debug.LogError("Avatar não encontrado em: " + avatarPlayer2Path);
            }
        }
    }
}