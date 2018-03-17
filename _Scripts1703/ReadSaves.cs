using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

// This reads the saved game info and displays on main menu

public class ReadSaves : MonoBehaviour {

    // Reference to UI obj
    private Text previousSessions;


	// Use this for initialization
	void Start () {
        previousSessions = GameObject.FindGameObjectWithTag("PrevSessions").GetComponent<Text>();
        ReadFile();
	}

    private void ReadFile()
    {
        // The location of file 
        string path = "Assets/Saves/saves.txt";
        StreamReader reader = new StreamReader(path);
        string data = "Previous sessions:\n";
        while(!reader.EndOfStream)
        {
            string line = reader.ReadLine();
            data += line + "\n";
        }

        //Debug.Log(reader.ReadToEnd());
        previousSessions.text = data;
        reader.Close();
    }

}
