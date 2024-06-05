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
    public TMP_Text player1Diamonds;
    public Image player1Avatar;

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

        // Chame aqui a função que você quer executar após o carregamento da cena
        AfterSceneLoaded();
    }

    // Função para executar após o carregamento da cena
    private void AfterSceneLoaded()
    {
        Debug.Log("Função executada após o carregamento da cena.");
        player1Name.text = GlobalVariables.playerName;
        player1Diamonds.text = GlobalVariables.playerDiamonds.ToString();

        //Save RectTransform properties
        RectTransform rectTransform = player1Avatar.GetComponent<RectTransform>();
        Vector2 originalSizeDelta = rectTransform.sizeDelta;
        Vector3 originalScale = rectTransform.localScale;

        //Load avatar image
        string avatarPath = "Avatars/avatar" + GlobalVariables.playerAvatar.ToString("D2");
        Sprite avatarSprite = Resources.Load<Sprite>(avatarPath);

        if (avatarSprite != null)
        {
            player1Avatar.sprite = avatarSprite;

            //Apply RectTransform properties
            rectTransform.sizeDelta = originalSizeDelta;
            rectTransform.localScale = originalScale;
        }
        else
        {
            Debug.LogError("Avatar não encontrado em: " + avatarPath);
        }
    }
}