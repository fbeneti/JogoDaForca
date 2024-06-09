using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class Conn : MonoBehaviourPunCallbacks

{
    public static Conn instance;
    public DatabaseBuilder databaseBuilder;


    public TMP_InputField createRoomName;
    public TMP_InputField joinRoomName;
    [SerializeField]
    private GameObject painelL, painelS, roomPanel, lobbyPanel;
    [SerializeField]
    private InputField nomeJogador;
    [SerializeField]
    public Text roomName;
    public Text txtNick;
    [SerializeField]
    private GameObject jogador;

    public string selectedDifficulty;
    public string selectedCategory;
    public string selectedWord;
    public string selectedWordHint1;
    public string selectedWordHint2;
    public string selectedWordHint3;
    public int selectedCategoryId;
    private string nomeSalaCriar;
    private string nomeSalaEntrar;


void Awake()
    {
        instance = this;
    }


    void Start()
    {
        nomeSalaCriar = createRoomName.text;
        nomeSalaEntrar = joinRoomName.text;
    }


    public void Login()
    {
        PhotonNetwork.NickName = GlobalVariables.player1Name;
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
        painelL.SetActive(false);
        painelS.SetActive(true);
    }


    public void CriarSala()
    {
        PhotonNetwork.CreateRoom(nomeSalaCriar);
    }


    public override void OnConnectedToMaster() // verifica se esta conectado
    {
        Debug.Log("Conectado");
        PhotonNetwork.JoinLobby();  // Entrar no Lobby
    }


    public override void OnJoinedLobby()
    {
        Debug.Log("Conectado");
    }


    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(nomeSalaEntrar);
    }
    
    
    public void JoinRandom()
    {
        PhotonNetwork.JoinRandomRoom();
    }
    
    
    public override void OnDisconnected(DisconnectCause cause) // verify if is connected
    {
        Debug.Log("Conexão perdida");
    }


    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Não entrou em nenhuma sala: " + message);  // if don't find any room
    }


    public override void OnJoinedRoom()
    {
        Debug.Log("Entrou em uma sala");
        Debug.Log("Nome da sala: " + PhotonNetwork.CurrentRoom.Name);
        Debug.Log("Quantidade de jogadores na sala: " + PhotonNetwork.CurrentRoom.PlayerCount);
        Debug.Log("Apelido do jogador: " + PhotonNetwork.NickName);

        txtNick.text = PhotonNetwork.NickName;

        painelS.SetActive(false);
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);
        roomName.text = "Nome da Sala: " + PhotonNetwork.CurrentRoom.Name;

        if (PhotonNetwork.IsMasterClient)
        {
            // Posição do primeiro jogador
            Vector3 posicaoPrimeiroJogador = new Vector3(-184.0f, -16.0f, 0.0f);
            PhotonNetwork.Instantiate("Player", posicaoPrimeiroJogador, Quaternion.identity, 0);

            // Sort category and word
            SelectCategoryAndWord();

            // Envia a categoria e a palavra para todos os jogadores
            photonView.RPC("SyncCategoryAndWord", RpcTarget.AllBuffered, selectedDifficulty, selectedCategory, selectedCategoryId, selectedWord);
        }
        else
        {
            // Posição do segundo jogador mais longe do primeiro
            Vector3 posicaoSegundoJogador = new Vector3(-184.0f, -120.0f, 0.0f);
            PhotonNetwork.Instantiate("Player", posicaoSegundoJogador, Quaternion.identity, 0);
        }
        
        StartCoroutine(IniciarJogo());
    }


    private void SelectCategoryAndWord()
    {
        // Load categories from database
        List<Dictionary<string, string>> categories = databaseBuilder.ReadTable("Categories");
        if (categories.Count == 0)
        {
            Debug.LogError("Nenhuma categoria encontrada no banco de dados.");
            return;
        }

        // Pick a category from the list
        int cIndex = Random.Range(0, categories.Count);
        var selectedCategoryDict = categories[cIndex];
        selectedCategory = selectedCategoryDict["Categoria"];
        selectedCategoryId = int.Parse(selectedCategoryDict["Id"]);
        Debug.Log("Categoria Id = " + selectedCategoryDict["Id"]);

        // Load words from picked category on the words list
        List<Dictionary<string, string>> words = databaseBuilder.ReadTable("Words", $"Categoria = {selectedCategoryId}");
        if (words.Count == 0)
        {
            Debug.LogError("Nenhuma palavra encontrada para a categoria selecionada.");
            return;
        }

        // Pick a word from the list
        int wIndex = Random.Range(0, words.Count);
        selectedWord = words[wIndex]["Nome"];
        selectedWordHint1 = words[wIndex]["Dica1"];
        selectedWordHint2 = words[wIndex]["Dica2"];
        selectedWordHint3 = words[wIndex]["Dica3"];
        Debug.Log("Palavra: " + selectedWord);

        int difficultyId = int.Parse(words[wIndex]["Dificuldade"]);

        // Load dificulty with the selected Id
        List<Dictionary<string, string>> difficulties = databaseBuilder.ReadTable("Dificulties", $"Id = {difficultyId}");
        if (difficulties.Count == 0)
        {
            Debug.LogError("Nenhuma dificuldade encontrada para o Id selecionado.");
            return;
        }

        // Pick a dificulty from the list
        var selectedDifficultyDict = difficulties[0];
        selectedDifficulty = selectedDifficultyDict["Dificuldade"];
        Debug.Log($"Dificuldade: {selectedDifficulty}");
    }


    [PunRPC]
    private void SyncCategoryAndWord(string difficulty, string category, int categoryId, string word)
    {
        selectedDifficulty = difficulty;
        selectedCategory = category;
        selectedCategoryId = categoryId;
        selectedWord = word;
        Debug.Log($"Sincronizado: {category} - {word}");
    }


    private IEnumerator IniciarJogo()
    {
        // Aguarda até que o segundo jogador entre na sala
        while (PhotonNetwork.CurrentRoom.PlayerCount < 2)
        {
            Debug.Log("Aguardando Segundo Jogador");
            yield return null;
        }

        // Quando o segundo jogador entrar na sala, aguarda 10 segundos antes de iniciar o jogo
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            yield return new WaitForSeconds(5f);

            // Inicia o jogo e carrega a cena da categoria correspondente
            string sceneName = $"8-Game{selectedCategoryId:D2}";
            Debug.Log($"Iniciando o jogo e carregando a cena {sceneName}");
            PhotonNetwork.LoadLevel(sceneName);
        }
    }
}
