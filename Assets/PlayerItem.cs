using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerItem : MonoBehaviourPunCallbacks
{
    public Text playerName;
    public Image playerAvatar;
    UIHandler uI;
    public Sprite[] avatars;
    Player player;
    ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();
    public void SetPlayerInfo(Player _player)
    {
        playerName.text = _player.NickName;
        player = _player;
        UpdatePlayerItem(player);

        
    }
   

    void UpdatePlayerItem(Player player)
    {
        if(player.CustomProperties.ContainsKey("playerAvatar"))
        {
            playerAvatar.sprite = avatars[GlobalVariables.player1Avatar];
        }
    }
   
   
    

    
}
