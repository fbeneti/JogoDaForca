using SQLite4Unity3d;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Registration : MonoBehaviour
{
    private DataService _ds;
    private SQLiteConnection _connection;
    private string DatabaseName = "existing.db";

    [Header("Paineis")]
    public Animator VrfFieldsPanel; //ID 1
    public Animator VrfPswPanel;    //ID 2
    public Animator SuccessPanel;    //ID 3

    [Header("InputFields")]
    public TMP_InputField emailField;
    public TMP_InputField usernameField;
    public TMP_InputField passwordField;
    public TMP_InputField passwordVerifyField;

    bool passwordVerified = false;
    bool fieldsVerified = false;

    public Registration(){

        #if UNITY_EDITOR
            var dbPath = string.Format(@"Assets/StreamingAssets/{0}", DatabaseName);
        #else
            // check if file exists in Application.persistentDataPath
            var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);

            if (!File.Exists(filepath))
            {
                Debug.Log("Database not in Persistent path");
                // if it doesn't ->
                // open StreamingAssets directory and load the db ->

                #if UNITY_ANDROID 
                    var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + DatabaseName);  // this is the path to your StreamingAssets in android
                    while (!loadDb.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
                    // then save to Application.persistentDataPath
                    File.WriteAllBytes(filepath, loadDb.bytes);
                #elif UNITY_IOS
                    var loadDb = Application.dataPath + "/Raw/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                    // then save to Application.persistentDataPath
                    File.Copy(loadDb, filepath);
                #elif UNITY_WP8
                    var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                    // then save to Application.persistentDataPath
                    File.Copy(loadDb, filepath);
                #elif UNITY_WINRT
                    var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                    // then save to Application.persistentDataPath
                    File.Copy(loadDb, filepath);
                #elif UNITY_STANDALONE_OSX
                    var loadDb = Application.dataPath + "/Resources/Data/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                    // then save to Application.persistentDataPath
                    File.Copy(loadDb, filepath);
                #else
                    var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                    // then save to Application.persistentDataPath
                    File.Copy(loadDb, filepath);
                #endif
                    Debug.Log("Database written");
            }
            var dbPath = filepath;
        #endif

     _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        Debug.Log("Final PATH: " + dbPath);  
    }

    public void Register() 
    {
        VerifyInputs();
        VerifyPassword();
        if (fieldsVerified)
        { 
            if (passwordVerified)
            {
                _connection.CreateTable<Players> ();
                _connection.InsertAll(new[]{
        			new Players{
	        			Username = usernameField.text,
		        		Email = emailField.text,
			        	Password = passwordField.text
    			    }
                });
            SuccessPanel.SetTrigger("open");
            }
        }
    }

    public void VerifyPassword()
    {
        if (passwordField.text != passwordVerifyField.text)        
        {
            passwordVerified = false;
            VrfPswPanel.SetTrigger("open");
        }
        else
        { 
            passwordVerified = true;
        }
    }

    public void VerifyInputs()
    {
        if ((emailField.text.Length <= 10) && (usernameField.text.Length <= 4) && (passwordField.text.Length <= 4))
        {
            fieldsVerified = false;
            VrfFieldsPanel.SetTrigger("open");
        }
        else
        { 
            fieldsVerified = true;
        }
    }

    public void ClosePanelButton(int buttonId)
    {
        switch(buttonId)
        {
            case 1:
                VrfFieldsPanel.SetTrigger("close");
                break;
            case 2:
                passwordField.text = "";
                passwordVerifyField.text = "";
                VrfPswPanel.SetTrigger("close");
                break;
            case 3:
                emailField.text = "";
                usernameField.text = "";
                passwordField.text = "";
                passwordVerifyField.text = "";
                SuccessPanel.SetTrigger("close");
                break;
        }
    }
}

