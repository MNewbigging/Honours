using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

// Receives data from ControllerListener, sends data to correct script for processing
public class InputMgr : MonoBehaviour {

    // Singleton
    public static InputMgr instance = null;
    // Reference to the current scene
    private string curScene;

    // References to scripts required to interact with
    private MenuMgr menuMgr; // Main menu control
    private PlayerInput playerInput; // Player control

    // List of input events (each int is active button id)
    List<int> inputEvents = new List<int>();

    // Keep this alive throughout
    private void Awake()
    {
        // Check if this instance already exists
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }


    // Add delegate method
    private void OnEnable() { SceneManager.sceneLoaded += GetCurrentSceneName; }
    
    // Delegate method to sceneLoaded - sets new scene name
    private void GetCurrentSceneName(Scene scene, LoadSceneMode mode)
    {
        curScene = scene.name;
        // Setup references to game objects
        if (curScene == "MainMenu")
        {
            menuMgr = GameObject.FindGameObjectWithTag("MenuMgr").GetComponent<MenuMgr>();
        }
        else if (curScene == "flatRiver")
        {
            playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
        }
    }
 
    

    // Called by ControllerListener, it passes event data to be added to events list
    public void ReceiveInputEvent(int inputData) {  inputEvents.Add(inputData); } 

    // Monitor input events
    private void Update()
    {
         // If there are any events to process
         if (inputEvents.Count > 0)
        {
            // Then process the first, removing it from list
            HandleInputEvents(); // Consider handling more events per frame, rather than 1pf as it is
        }
    }

    // Processes the first item in input events list; removes it once actioned
    private void HandleInputEvents()
    {
        // Get a copy of the first event in list
        int inputEvent = inputEvents[0];
       
        // Acting on input depends on current active scene
        if (curScene == "MainMenu")
        {
            menuMgr.HandleMainMenuInput(inputEvent);

        }
        else if (curScene == "flatRiver")
        {
            playerInput.HandlePlayerInput(inputEvent);
        }

        // Once input event is actioned, remove it from list
        inputEvents.RemoveAt(0);
    } // end handle input events

	
}
