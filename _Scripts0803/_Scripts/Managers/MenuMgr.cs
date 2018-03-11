using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

// Loads game, quits application. Handles input taken from InputMgr
public class MenuMgr : MonoBehaviour {


    // Tracks currently selected item
    // 0 - Start
    // 1 - Quit
    private int curSelected = 0;


    // Close game routine
    IEnumerator QuitGame()
    {
        // Wait 
        yield return new WaitForSeconds(2);
        // Quit game
        UnityEditor.EditorApplication.isPlaying = false;
        // Application.Quit();
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
                }
                break;
            case 2: // Joystick left
                // Highlight item to left (if there is one)
                if (curSelected > 0)
                {
                    curSelected--;
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
