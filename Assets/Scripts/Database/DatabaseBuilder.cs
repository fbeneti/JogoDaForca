using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;


public class DatabaseBuilder : MonoBehaviour
{
    public static DatabaseBuilder instance;

    [Header("Banco de Dados:")]
    public string databaseName;
    protected string databasePath;
    protected SqliteConnection Connection => new SqliteConnection($"Data source = {this.databasePath};");

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


    public void Initialize()
    {
        this.databaseName = "existing.db";

        if (string.IsNullOrEmpty(this.databaseName))
        {
            Debug.LogError("Database name is empty!");
            return;
        }
        
        CopyDatabaseFileIfNotExists();
    }


    #region Create DataBase
    private void CopyDatabaseFileIfNotExists()
    {
        this.databasePath = Path.Combine(Application.persistentDataPath, this.databaseName);

        if(File.Exists(this.databasePath))
            return;
        
        var originDatabasePath = string.Empty;
        var isAndroid = false;

#if UNITY_EDITOR || UNITY_WP8 || UNITY_WINRT || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX
        originDatabasePath = Path.Combine(Application.streamingAssetsPath, this.databaseName);

#elif UNITY_STANDALONE_OSX
        originDatabasePath = Path.Combine(Application.dataPath, "/Resources/Data/StreamingAssets/", this.databaseName);
        
#elif UNITY_IOS
        originDatabasePath = Path.Combine(Application.dataPath, "Raw", this.databaseName);        

#elif UNITY_ANDROID
        isAndroid = true;
        originDatabasePath = "jar:file://" + Application.dataPath + "!/assets/" + this.databaseName;
        StartCoroutine(GetInternalFileAndroid(originDatabasePath));

#endif

        if (!isAndroid)
        {
            Debug.LogWarning($"COPY FILE: {originDatabasePath} to {this.databasePath}");
            File.Copy(originDatabasePath, this.databasePath);
        }
    }


    protected IEnumerator GetInternalFileAndroid(string path)
    {
        var request = UnityWebRequest.Get(path);
        yield return request.SendWebRequest();

        if (request.isHttpError || request.isNetworkError)
        {
            Debug.LogError($"Error reading android file!: {request.error}");
            throw new Exception($"Error reading android file!: { request.error }");
        }
        else
        {
            File.WriteAllBytes(this.databasePath, request.downloadHandler.data);
            Debug.Log("File copied! ->" + this.databasePath);
        }
    }
    #endregion

    #region Create Table
    protected void CreateTablePlayers()
    {
        var commandText = $"CREATE TABLE Players " +
            "(" +
            "  Username TEXT PRIMARY KEY, " +
            "  Email TEXT NOT NULL," +
            "  Password TEXT NOT NULL," +
            "  Avatar INTEGER," +
            "  Victories INTEGER," +
            "  Losses INTEGER," +
            "  Diamonds INTEGER" +
            ");";
        
        using(var connection = Connection)
        { 
            connection.Open();
            using(var command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                Debug.Log("CreateTablePlayers");
            }
        }
    }
    #endregion

    #region Insert Data
    public void InsertIntoTable(string tableName, Dictionary<string, string> data)
    {
        var columns = new List<string>();
        var values = new List<string>();

        foreach (var pair in data)
        {
            columns.Add(pair.Key);
            values.Add($"'{pair.Value}'");
        }

        var query = $"INSERT INTO {tableName} ({string.Join(", ", columns)}) VALUES ({string.Join(", ", values)});";

        using (var connection = Connection)
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = query;
                command.ExecuteNonQuery();
            }
        }
    }
    #endregion

    #region Read Data
    public List<Dictionary<string, string>> ReadTable(string tableName, string whereClause = "")
    {
        var query = $"SELECT * FROM {tableName}";

        if (!string.IsNullOrEmpty(whereClause))
        {
            query += $" WHERE {whereClause}";
        }

        var result = new List<Dictionary<string, string>>();
        
        using(var connection = Connection)
        {
            connection.Open();
            using(var command = connection.CreateCommand())
            {
                command.CommandText = query;
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var row = new Dictionary<string, string>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row[reader.GetName(i)] = reader[i].ToString();
                        }
                        result.Add(row);
                    }
                }
            }
        }

        return result;
    }
    #endregion

    #region Update Data
    public void UpdateTable(string tableName, Dictionary<string, string> data, string whereClause)
    {
        var setClauses = new List<string>();

        foreach (var pair in data)
        {
            setClauses.Add($"{pair.Key} = '{pair.Value}'");
        }

        var query = $"UPDATE {tableName} SET {string.Join(", ", setClauses)} WHERE {whereClause};";

        using(var connection = Connection)
        {
            connection.Open();
            using(var command = connection.CreateCommand())
            {
                command.CommandText = query;
                command.ExecuteNonQuery();
            }
        }
    }
    #endregion

    #region Delete Data
    public void DeleteFromTable(string tableName, string whereClause)
    {
        var query = $"DELETE FROM {tableName} WHERE {whereClause};";

        using(var connection = Connection)
        {
            connection.Open();
            using(var command = connection.CreateCommand())
            {
                command.CommandText = query;
                command.ExecuteNonQuery();
            }
        }
    }
    #endregion
}
