using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StoreManager : MonoBehaviour
{
    public static StoreManager instance;
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI diamondsText;
    public TextMeshProUGUI hintsText;
    public TextMeshProUGUI extraLifesText;
    public TextMeshProUGUI stealTimeText;
    public TextMeshProUGUI fogsText;
    private int coinsCount = GlobalVariables.player1Coins;
    private int diamondsCount = GlobalVariables.player1Diamonds;
    private int hintsCount = GlobalVariables.player1Hints;
    private int extraLifesCount = GlobalVariables.player1ExtraLifes;
    private int stealTimeCount = GlobalVariables.player1StealTime;
    private int fogsCount = GlobalVariables.player1Fogs;
    private DatabaseBuilder databaseBuilder;


    // Start is called before the first frame update
    void Start()
    {
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

        instance = this;
    }


    public void AddCoins(int coinsToAdd)
    {
        coinsCount += coinsToAdd;
        coinsText.text = coinsCount.ToString();
        GlobalVariables.player1Coins = coinsCount;
        UpdateCoinsOnDatabase();
    }


    public void AddDiamonds(int price)
    {
        if (price <= coinsCount)
        {
            coinsCount -= price;
            coinsText.text = coinsCount.ToString();
            GlobalVariables.player1Coins = coinsCount;
            UpdateCoinsOnDatabase();

            if (price == 250) diamondsCount += 1;
            if (price == 2000) diamondsCount += 10;
            if (price == 4000) diamondsCount += 25;
            diamondsText.text = diamondsCount.ToString();
            GlobalVariables.player1Diamonds = diamondsCount;
            UpdateDiamondsOnDatabase();
        }
        else
        {
            Debug.Log("Você não tem moedas suficientes...");
        }
    }


    public void AddHints(int price)
    {
        if (price <= diamondsCount)
        {
            diamondsCount -= price;
            diamondsText.text = diamondsCount.ToString();
            GlobalVariables.player1Diamonds = diamondsCount;
            UpdateDiamondsOnDatabase();

            hintsCount++;
            hintsText.text = hintsCount.ToString();
            GlobalVariables.player1Hints = hintsCount;
            UpdateHintsOnDatabase();
        }
        else
        {
            Debug.Log("Você não tem diamantes suficientes...");
        }
    }


    public void AddExtraLifes(int price)
    {
        if (price <= diamondsCount)
        {
            diamondsCount -= price;
            diamondsText.text = diamondsCount.ToString();
            GlobalVariables.player1Diamonds = diamondsCount;
            UpdateDiamondsOnDatabase();

            extraLifesCount++;
            extraLifesText.text = extraLifesCount.ToString();
            GlobalVariables.player1ExtraLifes = extraLifesCount;
            UpdateHintsOnDatabase();
        }
        else
        {
            Debug.Log("Você não tem diamantes suficientes...");
        }
    }


    public void AddStealTime(int price)
    {
        if (price <= diamondsCount)
        {
            diamondsCount -= price;
            diamondsText.text = diamondsCount.ToString();
            GlobalVariables.player1Diamonds = diamondsCount;
            UpdateDiamondsOnDatabase();

            stealTimeCount++;
            stealTimeText.text = stealTimeCount.ToString();
            GlobalVariables.player1StealTime = stealTimeCount;
            UpdateHintsOnDatabase();
        }
        else
        {
            Debug.Log("Você não tem diamantes suficientes...");
        }
    }


    public void AddFogs(int price)
    {
        if (price <= diamondsCount)
        {
            diamondsCount -= price;
            diamondsText.text = diamondsCount.ToString();
            GlobalVariables.player1Diamonds = diamondsCount;
            UpdateDiamondsOnDatabase();

            fogsCount++;
            fogsText.text = fogsCount.ToString();
            GlobalVariables.player1Fogs = fogsCount;
            UpdateHintsOnDatabase();
        }
        else
        {
            Debug.Log("Você não tem diamantes suficientes...");
        }
    }


    public void UpdateCoinsOnDatabase()
    {
        Dictionary<string, string> player1Coins = new Dictionary<string, string>
        {
            {"Coins", coinsCount.ToString()}
        };

        databaseBuilder.UpdateTable("Players", player1Coins, $"Username = '{GlobalVariables.player1Name}'");
        GlobalVariables.player1Coins = coinsCount;
        Debug.Log("Moedas compradas com sucesso");
    }


    public void UpdateDiamondsOnDatabase()
    {
        Dictionary<string, string> player1Diamonds = new Dictionary<string, string>
        {
            {"Diamonds", diamondsCount.ToString()}
        };

        databaseBuilder.UpdateTable("Players", player1Diamonds, $"Username = '{GlobalVariables.player1Name}'");
        GlobalVariables.player1Diamonds = diamondsCount;
        Debug.Log("Diamantes comprados com sucesso");
    }


    public void UpdateHintsOnDatabase()
    {
        Dictionary<string, string> player1Hints = new Dictionary<string, string>
        {
            {"Hints", hintsCount.ToString()}
        };

        databaseBuilder.UpdateTable("Players", player1Hints, $"Username = '{GlobalVariables.player1Name}'");
        GlobalVariables.player1Hints = hintsCount;
        Debug.Log("Dicas extras compradas com sucesso");
    }


    public void UpdateExtraLifesOnDatabase()
    {
        Dictionary<string, string> player1ExtraLifes = new Dictionary<string, string>
        {
            {"ExtraLifes", extraLifesCount.ToString()}
        };

        databaseBuilder.UpdateTable("Players", player1ExtraLifes, $"Username = '{GlobalVariables.player1Name}'");
        GlobalVariables.player1ExtraLifes = extraLifesCount;
        Debug.Log("Vidas Extras compradas com sucesso");
    }


    public void UpdateStealTimeOnDatabase()
    {
        Dictionary<string, string> player1StealTime = new Dictionary<string, string>
        {
            {"StealTime", stealTimeCount.ToString()}
        };

        databaseBuilder.UpdateTable("Players", player1StealTime, $"Username = '{GlobalVariables.player1Name}'");
        GlobalVariables.player1StealTime = stealTimeCount;
        Debug.Log("Rouba-tempo comprado com sucesso");
    }


    public void UpdateFogsOnDatabase()
    {
        Dictionary<string, string> player1Fogs = new Dictionary<string, string>
        {
            {"Fogs", fogsCount.ToString()}
        };

        databaseBuilder.UpdateTable("Players", player1Fogs, $"Username = '{GlobalVariables.player1Name}'");
        GlobalVariables.player1Fogs = fogsCount;
        Debug.Log("Neblinas compradas com sucesso");
    }
}
