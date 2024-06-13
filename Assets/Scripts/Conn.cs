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
    public List<PlayerItem> playerItemsList = new List<PlayerItem>();
    public PlayerItem playerItemPrefab;
    public Transform playerItemParent;

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

    }


    void Start()
    {
        nomeSalaCriar = createRoomName.text;
        nomeSalaEntrar = joinRoomName.text;

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
    }


    public void Login()
    {
        PhotonNetwork.NickName = GlobalVariables.player1Name;
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
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


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }


    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
    }


    void UpdatePlayerList()
    {
        foreach(PlayerItem item in playerItemsList)
        {
            Destroy(item.gameObject);
        }
        playerItemsList.Clear();

        if(PhotonNetwork.CurrentRoom == null)
        {
            return;
        }
        foreach(KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            PlayerItem newPlayerItem = Instantiate(playerItemPrefab, playerItemParent);
            newPlayerItem.SetPlayerInfo(player.Value);
            
            playerItemsList.Add(newPlayerItem);
        }
    }


    public override void OnJoinedRoom()
    {
        Debug.Log("Entrou em uma sala");
        Debug.Log("Nome da sala: " + PhotonNetwork.CurrentRoom.Name);
        Debug.Log("Quantidade de jogadores na sala: " + PhotonNetwork.CurrentRoom.PlayerCount);
        Debug.Log("Apelido do jogador: " + PhotonNetwork.NickName);

        painelS.SetActive(false);
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);
        roomName.text = "Nome da Sala: " + PhotonNetwork.CurrentRoom.Name;

         if (PhotonNetwork.IsMasterClient)
        {
            // Posição do primeiro jogador
            Vector3 posicaoPrimeiroJogador = new Vector3(-184.0f, -16.0f, 0.0f);
            PhotonNetwork.Instantiate("Player", posicaoPrimeiroJogador, Quaternion.identity, 0);
        }
        else
        {
            // Posição do segundo jogador mais longe do primeiro
            Vector3 posicaoSegundoJogador = new Vector3(-184.0f, -120.0f, 0.0f);
            PhotonNetwork.Instantiate("Player", posicaoSegundoJogador, Quaternion.identity, 0);
        }

        UpdatePlayerList();
        StartCoroutine(IniciarJogo());
    }


    private IEnumerator IniciarJogo()
    {
        int cID;
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
            if (GlobalVariables.actualCategoryId == 0)
            {
                cID = Random.Range(0, GlobalVariables.countCategory);
                GlobalVariables.actualCategoryId = cID;
            }
            else
            {
                cID = GlobalVariables.actualCategoryId;
            }

            // Load categories from database
            List<Dictionary<string, string>> categories = databaseBuilder.ReadTable("Categories", $"Id = {cID}");
            if (categories.Count == 0)
            {
                Debug.LogError("Nenhuma categoria encontrada no banco de dados.");
                yield return null;
            }

            // Pick a category from the list
            int cIndex = Random.Range(0, categories.Count);
            var selectedCategory = categories[cIndex];
            string categoryName = selectedCategory["Categoria"];
            Debug.Log("Categoria = " + categoryName);

            // Load words from selected category on the words list 
            int categoryId = int.Parse(selectedCategory["Id"]);
            Debug.Log("Categoria Id = " + selectedCategory["Id"]);
            List<Dictionary<string, string>> words = databaseBuilder.ReadTable("Words", $"Categoria = {categoryId}");
            if (words.Count == 0)
            {
                Debug.LogError("Nenhuma palavra encontrada para a categoria selecionada.");
                yield return null;
            }

            // Pick a word from the list
            int wIndex = Random.Range(0, words.Count);
            string pickedWord = words[wIndex]["Nome"];
            int difficultyId = int.Parse(words[wIndex]["Dificuldade"]);
            Debug.Log("Palavra = " + pickedWord);

            // Load dificulty with the selected Id
            List<Dictionary<string, string>> difficulties = databaseBuilder.ReadTable("Dificulties", $"Id = {difficultyId}");
            if (difficulties.Count == 0)
            {
                Debug.LogError("Nenhuma dificuldade encontrada para o Id selecionado.");
                yield return null;
            }

            // Pick a dificulty from the list
            int dIndex = Random.Range(0, difficulties.Count);
            var selectedDifficulty = difficulties[dIndex];
            string difficultyName = selectedDifficulty["Dificuldade"];

            GlobalVariables.actualCategoryName = categoryName;
            GlobalVariables.actualDifficulty = difficultyName;
            GlobalVariables.actualWord = pickedWord;

            string sceneName = $"8-Game{cID:D2}";
            Debug.Log($"Iniciando o jogo e carregando a cena {sceneName}");
            PhotonNetwork.LoadLevel(sceneName);
        }
    }


    
}
