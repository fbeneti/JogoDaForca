using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Mathematics;


public class Conn : MonoBehaviourPunCallbacks

{
    [SerializeField]
    private GameObject painelL, painelS;
    [SerializeField]
    private InputField nomeJogador, nomeSala;
    [SerializeField]
    public Text txtNick;
    [SerializeField]
    private GameObject jogador;


    void Start()
    {

    }


    public void Login()
    {
        PhotonNetwork.NickName = nomeJogador.text;
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
        painelL.SetActive(false);
        painelS.SetActive(true);
    }


    public void CriarSala()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InLobby)
            PhotonNetwork.JoinOrCreateRoom(nomeSala.text, new RoomOptions(), TypedLobby.Default);
        else
            Debug.LogError("O cliente Photon não está pronto para operações. Aguardando conexão...");
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


    public override void OnDisconnected(DisconnectCause cause) // verificar se esta disconectado
    {
        Debug.Log("Conexão perdida");
    }


    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Não entrou em nenhuma sala");  //se nao estiver nenhuma sala
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
            PhotonNetwork.LoadLevel("6-Game");
        }
    }
}
