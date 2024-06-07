using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Mathematics;


public class Conn : MonoBehaviourPunCallbacks

{
    public static Conn instance;

    [SerializeField]
    private GameObject painelL, painelS, roomPanel, lobbyPanel;
    [SerializeField]
    private InputField nomeJogador, nomeSala;
    [SerializeField]
    public Text roomName;
    public Text txtNick;
    [SerializeField]
    private GameObject jogador;


void Awake()
    {
        instance = this;
    }


    void Start()
    {

    }


    public void Login()
    {
        PhotonNetwork.NickName = GlobalVariables.player1Name;
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
        painelL.SetActive(false);
        painelS.SetActive(true);
    }


    public void CriarSala(InputField nomeSala)
    {
        PhotonNetwork.CreateRoom(nomeSala.text);
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


    public void JoinRoom(InputField nomeSala)
    {
        PhotonNetwork.JoinRoom(nomeSala.text);
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


    private bool aguardandoSegundoJogador = false;


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
        }
        else
        {
            // Posição do segundo jogador mais longe do primeiro
            Vector3 posicaoSegundoJogador = new Vector3(-184.0f, -120.0f, 0.0f);
            PhotonNetwork.Instantiate("Player", posicaoSegundoJogador, Quaternion.identity, 0);
        }
        StartCoroutine(IniciarJogo());
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        Debug.Log("Novo jogador entrou na sala: " + newPlayer.NickName);

        // Apenas o MasterClient deve enviar os dados do Player1 para o Player2
        if (PhotonNetwork.IsMasterClient)
        {
            // Dados do Player1 são enviados para o Player2
            photonView.RPC("SendPlayer1Data", newPlayer,
                GlobalVariables.player1Name,
                GlobalVariables.player1Avatar,
                GlobalVariables.player1Victories,
                GlobalVariables.player1Losses,
                GlobalVariables.player1Diamonds,
                GlobalVariables.player1Coins,
                GlobalVariables.player1Hints,
                GlobalVariables.player1ExtraLifes,
                GlobalVariables.player1StealTime,
                GlobalVariables.player1Fogs);
        }
    }

    [PunRPC]
    void SendPlayer1Data(string name, int avatar, int victories, int losses, int diamonds, int coins, int hints, int extraLifes, int stealTime, int fogs)
    {
        // Atualiza os dados do Player2 no GlobalVariables
        GlobalVariables.player2Name = name;
        GlobalVariables.player2Avatar = avatar;
        GlobalVariables.player2Victories = victories;
        GlobalVariables.player2Losses = losses;
        GlobalVariables.player2Diamonds = diamonds;
        GlobalVariables.player2Coins = coins;
        GlobalVariables.player2Hints = hints;
        GlobalVariables.player2ExtraLifes = extraLifes;
        GlobalVariables.player2StealTime = stealTime;
        GlobalVariables.player2Fogs = fogs;
        
        Debug.Log("Dados do Player2 recebidos:");
        Debug.Log("Nome: " + GlobalVariables.player2Name);
        Debug.Log("Avatar: " + GlobalVariables.player2Avatar);
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

            // Inicia o jogo e carrega a cena "Game"
            Debug.Log("Iniciando o jogo e carregando a cena 'Game'...");
            PhotonNetwork.LoadLevel("New Scene");
        }
    }
}
