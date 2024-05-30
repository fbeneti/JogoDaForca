using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LetterButton : MonoBehaviour
{
    string letter;

    public void SetButton(string _letter)
    {  
        letter = _letter; 
    }


    //BUTTON INPUT OR HINT
    public void Sendletter(bool isThatAHint)
    {
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
