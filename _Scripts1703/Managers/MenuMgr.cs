using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Loads game, quits application. Handles input taken from InputMgr
public class MenuMgr : MonoBehaviour {


    // Tracks currently selected item
    // 0 - Start
    // 1 - Quit
    private int curSelected = 0;

    List<Button> buttons = new List<Button>();

    // Setup buttons
    private void Start()
    {
        Button start = GameObject.FindGameObjectWithTag("BtnStart").GetComponent<Button>();
        Button quit = GameObject.FindGameObjectWithTag("BtnQuit").GetComponent<Button>();
        buttons.Add(start);
        buttons.Add(quit);

        // Set initial colours
        HighlightButtons();
    }

    // Close game routine
    IEnumerator QuitGame()
    {
        // Wait 
        yield return new WaitForSeconds(2);
        // Quit game
        //UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }

    // Change button background colours according to what's been selected
    private void HighlightButtons()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            if (i == curSelected)
                buttons[i].image.color = Color.green;
            else
                buttons[i].image.color = Color.white;
        }
    }

    // Handle input for main menu
    public void HandleMainMenuInput(int inputEvent)
    {
        Debug.Log("Main menu handling event: " + inputEvent);
        // Listen for select button press
        switch (inputEvent)
        {
            case 0: // Joystick idle
                // do nothing
                break;
            case 1: // Joystick right
                 // Highlight item to right (if there is one)
                if (curSelected < 1)
                {
                    curSelected++;
                    HighlightButtons();
                }
                break;
            case 2: // Joystick left
                // Highlight item to left (if there is one)
                if (curSelected > 0)
                {
                    curSelected--;
                    HighlightButtons();
                }
                break;
            case 3: // Joystick down
                // do nothing
                break;
            case 4: // Joystick up
               
                break;
            case 5: // Magnet detected

                break;
            case 6: // Button 1 pressed
                // Run click event of curSelected button
                if (curSelected == 0)
                {
                    // Load game
                    SceneManager.LoadScene(1);
                }
                else if (curSelected == 1)
                {
                    Debug.Log("Exiting game");
                    StartCoroutine(QuitGame());
                }
                break;
            case 7: // Button 2 pressed

                break;
            default:
                Debug.Log("Default event: " + inputEvent);
                break;
        }
    }
    

}
