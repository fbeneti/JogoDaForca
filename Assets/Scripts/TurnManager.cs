using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TurnManager : MonoBehaviourPunCallbacks
{
    public static TurnManager instance;

    public delegate void TurnAction();
    public static event TurnAction OnTurnChanged;

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
    }

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SetInitialTurn();
        }
    }

    private void SetInitialTurn()
    {
        var players = PhotonNetwork.CurrentRoom.Players.Values.ToList();
        System.Random random = new System.Random();
        int randomIndex = random.Next(players.Count);
        int randomPlayerActorNumber = players[randomIndex].ActorNumber;

        ExitGames.Client.Photon.Hashtable initialProps = new ExitGames.Client.Photon.Hashtable
        {
            { "CurrentTurn", randomPlayerActorNumber }
        };

        PhotonNetwork.CurrentRoom.SetCustomProperties(initialProps);

        photonView.RPC("UpdateTurnUIRPC", RpcTarget.All);
    }

    public void EndTurn()
    {
        int nextPlayer = GetNextPlayer();
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
            { "CurrentTurn", nextPlayer }
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);

        // Chamando RPC após a mudança de turno ser sincronizada
        StartCoroutine(DelayedUIUpdate());
    }

    // Método para aguardar um curto atraso antes de chamar a atualização da UI
    private IEnumerator DelayedUIUpdate()
    {
        yield return new WaitForSeconds(0.5f); // Aguarda 0.5 segundos para garantir a sincronização
        photonView.RPC("UpdateTurnUIRPC", RpcTarget.All);
    }

    private int GetNextPlayer()
    {
        int currentTurn = GetCurrentPlayer();
        Player currentPlayer = PhotonNetwork.CurrentRoom.GetPlayer(currentTurn);
        Player nextPlayer = GetNextPlayerInRoom(currentPlayer);
        return nextPlayer.ActorNumber;
    }

    private Player GetNextPlayerInRoom(Player currentPlayer)
    {
        Player[] players = PhotonNetwork.PlayerList;
        int currentIndex = players.ToList().IndexOf(currentPlayer);
        int nextIndex = (currentIndex + 1) % players.Length;
        return players[nextIndex];
    }

    public int GetCurrentPlayer()
    {
        return (int)PhotonNetwork.CurrentRoom.CustomProperties["CurrentTurn"];
    }

    public bool IsMyTurn()
    {
        return PhotonNetwork.LocalPlayer.ActorNumber == GetCurrentPlayer();
    }

    [PunRPC]
    private void UpdateTurnUIRPC()
    {
        OnTurnChanged?.Invoke();
        GameManager.instance.UpdateTurnUI();
    }
}
