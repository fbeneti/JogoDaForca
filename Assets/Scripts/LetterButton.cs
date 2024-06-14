using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LetterButton : MonoBehaviour
{
    public string letter { get; private set; }

    public void SetButton(string _letter)
    {  
        letter = _letter; 
    }


    //Button input or Hint
    public void Sendletter(bool isThatAHint)
    {
        //Check if thats the player's turn
        if (TurnManager.instance.IsMyTurn())
        {
            Debug.Log("My letter is: " + letter);
            GameManager.instance.InputFromButton(letter, isThatAHint);
            ButtonCreator.instance.RemoveLetter(this);
            GetComponent<Button>().interactable = false;
        }
        else
        {
            return;
        }
    }
}
