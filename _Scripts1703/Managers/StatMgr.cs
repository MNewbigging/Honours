using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

// Handles all timing data for the game, writes to file

public class StatMgr : MonoBehaviour {

    // Total game length timer
    private float totalSessionLength = 0.0f;
    private Text gameTimerText;
    // Current life timer
    private float lifeTimer;
    private Text lifeTimerText;
    // Previous life timer
    private float prevLifeTimer;
    private Text prevLifeTimerText;
    // Best life timer
    private float bestLifeTime = 0;
    private Text bestLifeTimeText;
    // Score 
    private float gameScore = 0;
    private Text gameScoreText;
    // Score setter
    public void CrateScored()
    {
        gameScore += 100;
        // Set UI text
        gameScoreText.text = "Game score - " + gameScore.ToString();
    }


    // Setup text ref objs
    private void Start()
    {
        gameTimerText = GameObject.FindGameObjectWithTag("GameTimer").GetComponent<Text>();
        lifeTimerText = GameObject.FindGameObjectWithTag("LifeTimer").GetComponent<Text>();
        prevLifeTimerText = GameObject.FindGameObjectWithTag("PrevLifeTimer").GetComponent<Text>();
        bestLifeTimeText = GameObject.FindGameObjectWithTag("BestLifeTimer").GetComponent<Text>();
        gameScoreText = GameObject.FindGameObjectWithTag("GameScore").GetComponent<Text>();
    }
    
    // Formats timers for printing to UI
    private string[] TimerFormat(float timer)
    {
        // Two strings returned; minutes and seconds
        string[] formattedTime = new string[2];
        // Find minutes
        formattedTime[0] = Mathf.Floor(timer / 60).ToString("00");
        // Find second
        formattedTime[1] = Mathf.Floor(timer % 60).ToString("00");
        // Done, send back formatted time strings
        return formattedTime;
    }

    // Stop player life timer
    public void StopLifeTimer()
    {
        // Set player prev life timer text to life timer's current value
        string[] timeStrings = TimerFormat(lifeTimer);
        prevLifeTimerText.text = "Prev life time - " + timeStrings[0] + ":" + timeStrings[1];
        // Was this the best life time this session?
        if (lifeTimer > bestLifeTime)
        {
            // Update best life time if so
            bestLifeTime = lifeTimer;
            // Set UI
            bestLifeTimeText.text = "Best life time - " + timeStrings[0] + ":" + timeStrings[1];
        }
    }

    // Start player life timer
    public void StartLifeTimer()
    {
        // Reset the life timer now that player begins new life
        lifeTimer = 0;
    }

    // Keep clocks ticking
    private void FixedUpdate()
    {
        // Update total game length timer
        totalSessionLength += Time.deltaTime;
        // Format into strings
        string[] timeStrings = TimerFormat(totalSessionLength);
        // Set game timer UI text
        gameTimerText.text = "Game time - " + timeStrings[0] + ":" + timeStrings[1];

        // Update player life timer
        lifeTimer += Time.deltaTime;
        // Format
        timeStrings = TimerFormat(lifeTimer);
        // Set UI
        lifeTimerText.text = "Cur life time - " + timeStrings[0] + ":" + timeStrings[1];
    }


    // On game end, write data to file
    private void OnDisable()
    {
        // The location of file 
        string path = "Assets/Saves/saves.txt";
        // Write data to file
        StreamWriter writer = new StreamWriter(path, true); // true appends
        writer.WriteLine(gameTimerText.text);
        writer.Close();
   
    }








}
