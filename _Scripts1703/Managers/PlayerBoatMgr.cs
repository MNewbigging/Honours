using UnityEngine;
using System.Collections;

public class PlayerBoatMgr : MonoBehaviour {

    // Boat variables
    private float baseMoveSpeed; // Boat always at least this fast (+ current)
    private float timerMoveSpeedMultiplier; // Timer / multiplier is added to base speed (lower the better here)
    private float turnSpeed; // Base turn speed - always this fast
    private float turnSpeedMultiplier; // multi * player speed (as above) - higher the better
    // Getters
    public float GetBaseMoveSpeed() { return baseMoveSpeed; }
    public float GetTimerMultiplier() { return turnSpeedMultiplier; }
    public float GetTurnSpeed() { return turnSpeed; }
    public float GetTurnSpeedMultiplier() { return turnSpeedMultiplier; }

    // The boat prefabs
    private GameObject pirateShip, superSail, speedy, birdShip, fang;
    // Player prefab which parents the boat prefabs
    private Transform player;
    // The current boat prefab
    private GameObject boat;


	void Start () {
        // Grab player
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        // Load prefabs
        pirateShip = Resources.Load("pirate_ship") as GameObject;
        superSail = Resources.Load("super_sail") as GameObject;
        speedy = Resources.Load("speedy") as GameObject;
        birdShip = Resources.Load("bird_ship") as GameObject;
        fang = Resources.Load("fang") as GameObject;

        // Instantiate player boat
        LoadBoatModel(1);
	}
	
    // Handles model swapping, takes in selected boat choice from PlayerInput when paused
    public void LoadBoatModel(int selectedBoat)
    {
        // Remove any previously existing boat
        Destroy(boat);
        // Instantiate at origin
        Vector3 spawn = new Vector3(0, 0, 0);
        var rotation = Quaternion.Euler(0, 90, 0);
        // Setup boat movement vars and instantiate model
        switch(selectedBoat)
        {
            case 1:
                PirateShip();
                boat = Instantiate(pirateShip, spawn, rotation) as GameObject;
                break;
            case 2:
                SuperSail();
                boat = Instantiate(superSail, spawn, rotation) as GameObject;
                break;
            case 3:
                Speedy();
                boat = Instantiate(speedy, spawn, rotation) as GameObject;
                break;
            case 4:
                BirdShip();
                boat = Instantiate(birdShip, spawn, rotation) as GameObject;
                break;
            case 5:
                Fang();
                boat = Instantiate(fang, spawn, rotation) as GameObject;
                break;
        }
        
        // Make player object parent of this boat model
        boat.transform.parent = player;

    }

    // Set boat vars to that of pirate ship
	private void PirateShip()
    {
        baseMoveSpeed = 2.5f;
        timerMoveSpeedMultiplier = 10;
        turnSpeed = 20.0f;
        turnSpeedMultiplier = 4;
    }

    // Set boat vars to that of super sail
    private void SuperSail()
    {
        baseMoveSpeed = 2.5f;
        timerMoveSpeedMultiplier = 8;
        turnSpeed = 20.0f;
        turnSpeedMultiplier = 5;
    }

    // Set boat vars to that of speedy boat
    private void Speedy()
    {
        baseMoveSpeed = 5.0f;
        timerMoveSpeedMultiplier = 10;
        turnSpeed = 30.0f;
        turnSpeedMultiplier = 6;
    }

    // Set boat vars to that of bird boat
    private void BirdShip()
    {
        baseMoveSpeed = 3.5f;
        timerMoveSpeedMultiplier = 6;
        turnSpeed = 20.0f;
        turnSpeedMultiplier = 8;
    }

    // Set boat vars to that of fang boat
    private void Fang()
    {
        baseMoveSpeed = 4.0f;
        timerMoveSpeedMultiplier = 5;
        turnSpeed = 30.0f;
        turnSpeedMultiplier = 5;
    }



    void Update () {
	
	}
}
