using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PlayerInput : MonoBehaviour {

    // Refs to UI elements
    private GameObject pauseMenu;
    private GameObject gameOverScreen;
    // Ref to river mgr to reset game
    private RiverMgr riverMgr;
    private bool paused = false;
    private bool gameOver = false;
    // Ref to StatMgr to set player life timer
    private StatMgr statMgr;
    // Grab current boat movement vars from mgr
    private PlayerBoatMgr boatMgr;

    // Movement variables
    private float playerSpeed = 5.0f;
    private float cyclingTimer = 0.0f;
    private float turnSpeed = 80.0f;
    private float rotation;
    // Cycle countdown
    private float countdown = -1.0f;
    private const float countdownLimit = 5.0f; // 5 seconds
    // Pause button nav
    private int curSelectedPauseBtn = 0;
    private int curSelectedBoat = 1;
    private bool selectingBoat = false;

    Button exit;
    Dropdown boatSelect;


    // set pause menu obj ref, make inactive at start
    private void Start()
    {
        exit = GameObject.FindGameObjectWithTag("BtnExit").GetComponent<Button>();
        
        boatSelect = GameObject.FindGameObjectWithTag("BtnSelectBoat").GetComponent<Dropdown>();


        pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu");
        pauseMenu.SetActive(false);
        gameOverScreen = GameObject.FindGameObjectWithTag("GameOver");
        gameOverScreen.SetActive(false);
        riverMgr = GameObject.FindGameObjectWithTag("GameMgr").GetComponent<RiverMgr>();
        statMgr = GameObject.FindGameObjectWithTag("GameMgr").GetComponent<StatMgr>();
        boatMgr = GameObject.FindGameObjectWithTag("GameMgr").GetComponent<PlayerBoatMgr>();


    }

    // Move player, update countdown timer
    private void FixedUpdate()
    {
        // Reduce countdown clock
        countdown -= Time.deltaTime;

        // If still cycling, increment cycling timer
        if (countdown > 0)
            cyclingTimer += Time.deltaTime;
        // Otherwise, reset cycling timer
        else
            cyclingTimer = 0.0f;


        // Move player
        MovePlayer();

        // If player has sunk, game over
       if (transform.position.y < -0.13)
        {
            // Tell StatMgr to stop player life timer
            statMgr.StopLifeTimer();
            // Stop time, show end screen
            Time.timeScale = 0;
            gameOver = true;
            gameOverScreen.SetActive(true);
        }
    }

    // Player is always rotated/moved forward - rot and speed vars can be 0 at standstill
    private void MovePlayer()
    {
        // Rotate player
        //float rot = rotation * turnSpeed * Time.deltaTime;
        //transform.Rotate(0, rot, 0);


        //// Determine player speed based on elapsed time cycling
        //playerSpeed = 5 + (cyclingTimer / 10);
        //// Determine speed of rotation based on forward speed
        //turnSpeed = playerSpeed * 5;

        //// Apply player speed based on countdown (don't want to move if countdown expired)
        //float speed = (countdown < 0 ? 0.0f : playerSpeed);

        //// Move player forward based on speed
        //transform.position += transform.forward * speed * Time.deltaTime;



        // Determine player speed: base speed + timer / speed multiplier
        playerSpeed = boatMgr.GetBaseMoveSpeed() + (cyclingTimer / boatMgr.GetTimerMultiplier());

        // Determine total turn speed, based on total player speed above
        turnSpeed = playerSpeed * boatMgr.GetTurnSpeedMultiplier();

        // Rotation
        float rot = rotation * turnSpeed * Time.deltaTime;
        transform.Rotate(0, rot, 0);

        // Apply player speed based on countdown (don't move if countdown expired)
        float speed = (countdown < 0 ? 0.0f : playerSpeed);
        transform.position += transform.forward * speed * Time.deltaTime;

    }

    // Resets Game on player death
    private void ResetGame()
    {
        // Reset river
        riverMgr.SetupGame();
        // Reset player to origin
        Vector3 startPos = new Vector3(0, 0, 0);
        transform.position = startPos;
        transform.rotation = Quaternion.identity;
        // Reset timers
        cyclingTimer = 0;
        // Reset game timer to zero
        statMgr.StartLifeTimer();
        // Remove game over UI
        gameOverScreen.SetActive(false);
        // Remove pause if active
        pauseMenu.SetActive(false);
        paused = false;
        // Game no longer over
        gameOver = false;
    }

    private void HighlightButtons()
    {
        if (curSelectedPauseBtn == 0)
        {
            exit.image.color = Color.green;
            boatSelect.image.color = Color.white;
        }
        else if (curSelectedPauseBtn == 1)
        {
            exit.image.color = Color.white;
            boatSelect.image.color = Color.green;
        }
    }

    // Called when receiving input events from controller, sets player vars accordingly
    public void HandlePlayerInput(int inputEvent)
    {

        //Debug.Log("Player handling event: " + inputEvent);
        // Listen for select button press
        switch (inputEvent)
        {
            case 0: // Joystick idle
                rotation = inputEvent;
                break;
            case 1: // Joystick right
                if (selectingBoat)
                {
                    if (curSelectedBoat < 5)
                    {
                        curSelectedBoat++; 
                        pauseMenu.GetComponentInChildren<Dropdown>().value = curSelectedBoat - 1;
                    }
                        
                }
                else if (paused && !selectingBoat)
                {
                    // Navigate pause menu items
                    // Next button right (if there is one)
                    if (curSelectedPauseBtn < 1)
                    {
                        curSelectedPauseBtn++;
                        HighlightButtons();
                    }
                          
                }
                else
                {
                    // This is in game; set rotation
                    rotation = inputEvent;
                }
               
                break;
            case 2: // Joystick left
                if (selectingBoat)
                {
                    if (curSelectedBoat > 0)
                    {
                        curSelectedBoat--; 
                        pauseMenu.GetComponentInChildren<Dropdown>().value = curSelectedBoat - 1;
                    }
                        
                }
                if (paused && !selectingBoat)
                {
                    // Navigate pause menu
                    if (curSelectedPauseBtn > 0)
                    {
                        curSelectedPauseBtn--; Debug.Log(curSelectedPauseBtn);
                        HighlightButtons();
                    }
                      
                }
                else
                {
                    // This is in game; set rotation
                    rotation = -1;
                }
                
                break;
            case 3: // Joystick down
                
                break;
            case 4: // Joystick up
               
                break;
            case 5: // Magnet detected
                // user is pedalling - reset countdown
                countdown = countdownLimit;
                break;
            case 6: // Button 1 pressed
                if (selectingBoat)
                {
                    // Come out of dropdown
                    pauseMenu.GetComponentInChildren<Dropdown>().Hide();
                    // Reset game
                    ResetGame();
                    // Reload boat model
                    boatMgr.LoadBoatModel(curSelectedBoat);
                    // Allow game, and timers, to start
                    Time.timeScale = 1;

                    selectingBoat = false;
                }
                else if (paused && !selectingBoat)
                {
                    
                    if (curSelectedPauseBtn == 0)
                    {
                        // Quit game
                        //UnityEditor.EditorApplication.isPlaying = false;
                        Application.Quit();
                    }
                    else if (curSelectedPauseBtn == 1)
                    {
                        // Choose ship
                        pauseMenu.GetComponentInChildren<Dropdown>().Show();
                        selectingBoat = true;
                    }

                }
                else if (gameOver)
                {
                    ResetGame();
                    // Allow game, and timers, to start
                    Time.timeScale = 1;
                }

                break;
            case 7: // Button 2 pressed
                // Pause game
                if (Time.timeScale > 0)
                {
                    Time.timeScale = 0;
                    // Show pause menu
                    pauseMenu.SetActive(true);
                    paused = true;
                    HighlightButtons();
                   
                }
                // Unpause game 
                else
                {
                    // Hide pause menu
                    selectingBoat = false;
                    pauseMenu.SetActive(false);
                    Time.timeScale = 1;
                    paused = false;
                    curSelectedPauseBtn = 0;
                }
                break;
            default:
                Debug.Log("Default event: " + inputEvent);
                break;
        }

    }

    // Player collisions with sides, obstacles, crates etc
    private void OnCollisionEnter(Collision collision)
    {
        // If collided with obstacle
        if (collision.gameObject.tag == "Obstacle")
        {
            // Sink the player
            Vector3 sink = new Vector3(0, -0.01f, 0);
            transform.position += sink;
        }
        // If collided with crate
        else if (collision.gameObject.tag == "Crate")
        {
            // Destroy it
            Debug.Log("Picked up crate");
            Destroy(collision.gameObject);
            // Get some points
            statMgr.CrateScored();
        }

    }

}
