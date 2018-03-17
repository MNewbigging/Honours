using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Allows use of List<T>

// Manages river; holds list of in play river segments, adds/removes segments, spawns obstacles
public class RiverMgr : MonoBehaviour {

    // River segment definition
    struct RiverSegment
    {
        // Reference to river prefab 
        public GameObject riverPrefab;
        // Reference to obstacles on this river segment
        public List<GameObject> obstacles;
        // This segments type 
        public string type;

    }

    // List of river segments currently in play
    List<RiverSegment> river = new List<RiverSegment>();
    // Tracks direction; 1 up, 2 right, 3 down, 4 left
    public int direction;
    // Tracks river current speed
    private float riverCurrentSpeed = 1.2f;

    // Obstacle range min - increases as play continues
    private int obsRangeMin = 1;
    // Holds rock obstacle names as they appear in Resources folder
    private string[] rockNames = { "rock_a", "rock_b", "rock_c", "rock_d", "rock_e" };
    // Crate prefabs in resources
    private GameObject crate, barrel;


    void Start()
    {
        // Load prefabs
        crate = Resources.Load("Crate") as GameObject;
        barrel = Resources.Load("Barrel") as GameObject;
        // Initialise random engine
        Random.InitState(System.DateTime.Now.Millisecond);
        // Perform game setup
        SetupGame();
    }

    // Adds start segment, and two following segments randomly
    public void SetupGame()
    {
        // Clear from a previous run
        foreach (RiverSegment rs in river)
        {
            // Destroy river prefab go
            Destroy(rs.riverPrefab);
            // And all obstacle gos
            if(rs.obstacles != null)
            {
                foreach (GameObject obs in rs.obstacles)
                {
                    Destroy(obs);
                }
            }
        }
        // Ensure river is clear before starting
        river.Clear();

        // Set obstacle spawning range 
        obsRangeMin = 1;

        // Generate the first 3 river segments:

        // Start segment
        RiverSegment start = new RiverSegment();
        // Create prefab instance
        start.riverPrefab = (GameObject)Instantiate(Resources.Load("StartSegment"));
        // Set its position
        start.riverPrefab.transform.position = Vector3.zero;
        // Set its type
        start.type = "Sq1TL";
        // Add start to river list
        river.Add(start);

        // Going right
        direction = 2;

        // Add 2 more river segments
        AddRiverSegment();
        AddRiverSegment();
    }

    // Adds another random river segment to game river list
    public void AddRiverSegment()
    {
        // Increase min number of obstacles per each segment with every new segment
        obsRangeMin++;

        // First - add new tile
        // Look at direction to determine next available group of tiles
        switch(direction)
        {
            // Up
            case 1:
                {
                    int choice = Random.Range(1,3);
                
                    // Instantiate chosen tile
                    if (choice == 1)
                    {
                        RiverSegment Sq1TL = new RiverSegment();
                        Sq1TL.riverPrefab = (GameObject)Instantiate(Resources.Load("Sq1TL"));
                        // Set river current direction
                        Sq1TL.riverPrefab.GetComponentInChildren<RiverCurrent_UD>().ChangeCurrentDirection(riverCurrentSpeed);
                        Sq1TL.riverPrefab.GetComponentInChildren<RiverCurrentLR>().ChangeCurrentDirection(riverCurrentSpeed);
                        Sq1TL.riverPrefab.GetComponentInChildren<RiverCurrentTL>().ChangeCurrentDirection(riverCurrentSpeed);
                        // Set type
                        Sq1TL.type = "Sq1TL";
                        Vector3 pos = river[river.Count - 1].riverPrefab.transform.position;
                        pos.z += 60;
                        Sq1TL.riverPrefab.transform.position = pos;
                        // Add to river list
                        river.Add(Sq1TL);
                        // Set direction (right)
                        direction = 2;

                    }
                    else if (choice == 2)
                    {
                        RiverSegment Sq1TR = new RiverSegment();
                        Sq1TR.riverPrefab = (GameObject)Instantiate(Resources.Load("Sq1TR"));
                        // Set river direction
                        Sq1TR.riverPrefab.GetComponentInChildren<RiverCurrent_UD>().ChangeCurrentDirection(riverCurrentSpeed);
                        Sq1TR.riverPrefab.GetComponentInChildren<RiverCurrentLR>().ChangeCurrentDirection(-riverCurrentSpeed); // Right to left = neg
                        Sq1TR.riverPrefab.GetComponentInChildren<RiverCurrentTR>().ChangeCurrentDirection(-riverCurrentSpeed);
                        // Set type
                        Sq1TR.type = "Sq1TR";
                        Vector3 pos = river[river.Count - 1].riverPrefab.transform.position;
                        pos.z += 60;
                        Sq1TR.riverPrefab.transform.position = pos;
                        river.Add(Sq1TR);
                        // Set direction (left)
                        direction = 4;
                    }

                } break;
            // Right
            case 2:
                {
                    // Last tile was going right; can only choose from tiles in right river segments list
                    Random.InitState(System.DateTime.Now.Millisecond);
                    int choice = Random.Range(1, 3); // Only two options currently
                    // Grab last tile type
                    string lastType = river[river.Count - 1].type;
                    // Instantiate the next tile from this group
                    // Top Right
                    if (choice == 1)
                    {
                        // Instantiate next river segment
                        RiverSegment Sq1TR = new RiverSegment();
                        Sq1TR.riverPrefab = (GameObject)Instantiate(Resources.Load("Sq1TR"));
                        // Set current direction
                        Sq1TR.riverPrefab.GetComponentInChildren<RiverCurrent_UD>().ChangeCurrentDirection(-riverCurrentSpeed);
                        Sq1TR.riverPrefab.GetComponentInChildren<RiverCurrentLR>().ChangeCurrentDirection(riverCurrentSpeed);
                        Sq1TR.riverPrefab.GetComponentInChildren<RiverCurrentTR>().ChangeCurrentDirection(riverCurrentSpeed);
                        // Set its type
                        Sq1TR.type = "Sq1TR";
                        // Set its position - based on last tile type
                        if (lastType == "Sq1TL")
                        {
                            // In this case, add 60 to x for next tile pos
                            Vector3 pos = river[river.Count - 1].riverPrefab.transform.position;
                            pos.x += 60;
                            Sq1TR.riverPrefab.transform.position = pos;
                        }
                        else if (lastType == "Sq1BL")
                        {
                            // In this case, add 60 to x for next tile pos and subtract 40 from z
                            Vector3 pos = river[river.Count - 1].riverPrefab.transform.position;
                            pos.x += 60;
                            pos.z -= 40;
                            Sq1TR.riverPrefab.transform.position = pos;
                        }

                        // Add to river
                        river.Add(Sq1TR);
                        // Set direction (down)
                        direction = 3;
                    }
                    // Bottom Right
                    else if (choice == 2)
                    {
                        // Instantiate next river segment
                        RiverSegment Sq1BR = new RiverSegment();
                        Sq1BR.riverPrefab = (GameObject)Instantiate(Resources.Load("Sq1BR"));
                        Sq1BR.riverPrefab.GetComponentInChildren<RiverCurrent_UD>().ChangeCurrentDirection(riverCurrentSpeed);
                        Sq1BR.riverPrefab.GetComponentInChildren<RiverCurrentLR>().ChangeCurrentDirection(riverCurrentSpeed);
                        Sq1BR.riverPrefab.GetComponentInChildren<RiverCurrentBR>().ChangeCurrentDirection(-riverCurrentSpeed);
                        // Set its type
                        Sq1BR.type = "Sq1BR"; 
                        // Set its position - based on last tile type
                        if (lastType == "Sq1TL")
                        {
                            // In this case, add 60 to x for next tile pos and add 40 to z
                            Vector3 pos = river[river.Count - 1].riverPrefab.transform.position;
                            pos.x += 60;
                            pos.z += 40;
                            Sq1BR.riverPrefab.transform.position = pos;
                        }
                        else if (lastType == "Sq1BL")
                        {
                            // In this case, add 60 to x for next tile pos
                            Vector3 pos = river[river.Count - 1].riverPrefab.transform.position;
                            pos.x += 60;
                            Sq1BR.riverPrefab.transform.position = pos;
                        }

                        // Add to river
                        river.Add(Sq1BR);
                        // Set direction (up)
                        direction = 1;

                    }
                } break;
            // Down
            case 3:
                {
                    // Pick a random segment
                    Random.InitState(System.DateTime.Now.Millisecond);
                    int choice = Random.Range(1, 3); // Only two options currently
                    // Act on choice; instantiate chosen river segment
                    // Bottom Left
                    if (choice == 1)
                    {
                        // Instantiate prefab
                        RiverSegment Sq1BL = new RiverSegment();
                        Sq1BL.riverPrefab = (GameObject)Instantiate(Resources.Load("Sq1BL"));
                        // Set current direction
                        Sq1BL.riverPrefab.GetComponentInChildren<RiverCurrent_UD>().ChangeCurrentDirection(-riverCurrentSpeed);
                        Sq1BL.riverPrefab.GetComponentInChildren<RiverCurrentLR>().ChangeCurrentDirection(riverCurrentSpeed);
                        Sq1BL.riverPrefab.GetComponentInChildren<RiverCurrentBL>().ChangeCurrentDirection(-riverCurrentSpeed);
                        // Set its type
                        Sq1BL.type = "Sq1BL";
                        // Set its position
                        // In this case, -60 on z
                        Vector3 pos = river[river.Count - 1].riverPrefab.transform.position;
                        pos.z -= 60;
                        Sq1BL.riverPrefab.transform.position = pos;

                        // Add it to river list
                        river.Add(Sq1BL);
                        // Set direction (right)
                        direction = 2;
                    }
                    // Bottom Right
                    else if (choice == 2)
                    {
                        // Instantiate prefab
                        RiverSegment Sq1BR = new RiverSegment();
                        Sq1BR.riverPrefab = (GameObject)Instantiate(Resources.Load("Sq1BR"));
                        // Set current direction
                        Sq1BR.riverPrefab.GetComponentInChildren<RiverCurrent_UD>().ChangeCurrentDirection(-riverCurrentSpeed); // Up to down is negative
                        Sq1BR.riverPrefab.GetComponentInChildren<RiverCurrentLR>().ChangeCurrentDirection(-riverCurrentSpeed); // Right to left is negative
                        Sq1BR.riverPrefab.GetComponentInChildren<RiverCurrentBR>().ChangeCurrentDirection(riverCurrentSpeed);
                        // Set its type
                        Sq1BR.type = "Sq1BR";
                        // Set its position
                        // In this case, -60 on z
                        Vector3 pos = river[river.Count - 1].riverPrefab.transform.position;
                        pos.z -= 60;
                        Sq1BR.riverPrefab.transform.position = pos;

                        // Add segment to river list
                        river.Add(Sq1BR);
                        // Set direction (left)
                        direction = 4; 
                    }


                } break;
            // Left
            case 4:
                {
                    // Pick a random tile 
                    Random.InitState(System.DateTime.Now.Millisecond);
                    int choice = Random.Range(1,3);
                    // Grab last tile type
                    string lastType = river[river.Count - 1].type;

                    // Act on choice
                    // Top Left
                    if (choice == 1)
                    {
                        RiverSegment Sq1TL = new RiverSegment();
                        Sq1TL.riverPrefab = (GameObject)Instantiate(Resources.Load("Sq1TL"));
                        // Set current direction
                        Sq1TL.riverPrefab.GetComponentInChildren<RiverCurrent_UD>().ChangeCurrentDirection(-riverCurrentSpeed); // Going down so neg
                        Sq1TL.riverPrefab.GetComponentInChildren<RiverCurrentLR>().ChangeCurrentDirection(-riverCurrentSpeed); // Right to left so neg
                        Sq1TL.riverPrefab.GetComponentInChildren<RiverCurrentTL>().ChangeCurrentDirection(-riverCurrentSpeed); // Negate
                        Sq1TL.type = "Sq1TL";
                        // Set position based on last tile type
                        if (lastType == "Sq1TR")
                        {
                            Vector3 pos = river[river.Count - 1].riverPrefab.transform.position;
                            pos.x -= 60;
                            Sq1TL.riverPrefab.transform.position = pos;
                        }
                        else if (lastType == "Sq1BR")
                        {
                            Vector3 pos = river[river.Count - 1].riverPrefab.transform.position;
                            pos.x -= 60;
                            pos.z -= 40;
                            Sq1TL.riverPrefab.transform.position = pos;
                        }
                        // Add it to river list
                        river.Add(Sq1TL);
                        // Set direction (down)
                        direction = 3;

                    }
                    // Bottom Left
                    else if (choice == 2)
                    {
                        RiverSegment Sq1BL = new RiverSegment();
                        Sq1BL.riverPrefab = (GameObject)Instantiate(Resources.Load("Sq1BL"));
                        // Set river current direction
                        Sq1BL.riverPrefab.GetComponentInChildren<RiverCurrent_UD>().ChangeCurrentDirection(riverCurrentSpeed);
                        Sq1BL.riverPrefab.GetComponentInChildren<RiverCurrentLR>().ChangeCurrentDirection(-riverCurrentSpeed); // Right to left
                        Sq1BL.riverPrefab.GetComponentInChildren<RiverCurrentBL>().ChangeCurrentDirection(riverCurrentSpeed);
                        // Set type
                        Sq1BL.type = "Sq1BL";
                        // Set position based on last tile type
                        if (lastType == "Sq1BR")
                        {
                            Vector3 pos = river[river.Count - 1].riverPrefab.transform.position;
                            pos.x -= 60;
                            Sq1BL.riverPrefab.transform.position = pos;
                        }
                        else if (lastType == "Sq1TR")
                        {
                            Vector3 pos = river[river.Count - 1].riverPrefab.transform.position;
                            pos.x -= 60;
                            pos.z += 40;
                            Sq1BL.riverPrefab.transform.position = pos;
                        }
                        // Add it to river list
                        river.Add(Sq1BL);
                        // Set direction (up)
                        direction = 1;

                    }

                } break;

            default: break;
        } // end add tile switch on dir

        // Now add obstacles to this river segment
        AddRiverObstacles();
       
    } 

    // Adds river obstacles to the last placed river segment
    private void AddRiverObstacles()
    {
        // First iteration - just add one rock to each river segment, in a random legal position

        // Copy the last river segment in river list
        RiverSegment riverSeg = river[river.Count - 1]; 
        // Instantiate it's obstacles list
        riverSeg.obstacles = new List<GameObject>();

        // Determine how many obstacles on this river segment
        int obsCount = Random.Range(obsRangeMin, obsRangeMin + 3);

        for (int i = 0; i < obsCount; i++)
        {
            // Randomise location of obstacle - based on type of river segment (each has unique area coords)
            int x = 0, z = 0;
            // Top left 
            if (riverSeg.type == "Sq1TL")
            {
                x = Random.Range(-9, 30);
                if (x > 10)
                    z = Random.Range(41, 60);
                else
                    z = Random.Range(1, 60);
            }
            // Top right
            else if (riverSeg.type == "Sq1TR")
            {

                x = Random.Range(-30, 9);
                if (x < -10)
                    z = Random.Range(41, 60);
                else
                    z = Random.Range(1, 60);
            }
            // Bottom left
            else if (riverSeg.type == "Sq1BL")
            {
                x = Random.Range(-9, 30);
                if (x > 10)
                    z = Random.Range(1, 20);
                else
                    z = Random.Range(1, 60);
            }
            // Bottom right
            else if (riverSeg.type == "Sq1BR")
            {
                x = Random.Range(-30, 9);
                if (x < -10)
                    z = Random.Range(1, 20);
                else
                    z = Random.Range(1, 60);
            }
            else
            {
                Debug.Log("Type error!");
            }

            // Then create a random rock game object
            string chosenRock = rockNames[Random.Range(0, rockNames.Length)];
            Debug.Log("Rock: " + chosenRock);
            GameObject rock = (GameObject)Instantiate(Resources.Load(chosenRock));
            // Set its position - relative to the segment's center pos
            Vector3 pos = new Vector3(riverSeg.riverPrefab.transform.position.x + x, 0, riverSeg.riverPrefab.transform.position.z + z);
            rock.transform.position = pos;
            // Add it to obstacles list
            riverSeg.obstacles.Add(rock);

        } // end obstacle for loop

        // Finally, overwrite last river segment with the copy that's been changed
        river[river.Count - 1] = riverSeg;

    }

    // Spawns more crates
    private void SpawnCrates()
    {
        // Another method - get center of all tiles that make up segment, spawn 3 or so spread from that center
        for (int i = 1; i < 5; i++) // Each segment has 1 TZ then 4 tiles
        {
            // The center point of this tile
            Vector3 spawnPointCenter = river[1].riverPrefab.transform.GetChild(i).transform.position;
            
            // Randomise spawn positions within this tile
            for (int j = 0; j < 6; j++)
            {
                float x = Random.Range(-10, 10);
                float z = Random.Range(-10, 10);
                // Find new spawn point relative to center 
                Vector3 sp = new Vector3(x + spawnPointCenter.x, 0, z + spawnPointCenter.z);
                // Instantiate a crate or barrel
                switch(Random.Range(0, 2))
                {
                    case 0:
                        Instantiate(crate, sp, Quaternion.identity);
                        break;
                    case 1:
                        //var rotation = Quaternion.Euler(0, 0, 90);
                        Instantiate(crate, sp, Quaternion.identity);
                        
                        break;
                    default: Debug.Log("Crate instantiating error"); break;
                }    
            }  //end crate spawning   
        } // end tile iterator
    }

    // Deletes the first (oldest) river segment from game river list - also deleting all attached obstacles
    public void RemoveRiverSegment()
    {

        // First destroy the river segment prefab
        Destroy(river[0].riverPrefab);
        // And all obstacles
        if (river[0].obstacles != null)
        {
            foreach (GameObject obs in river[0].obstacles)
            {
                Destroy(obs);
            }
        }

        // Then remove the segment from the game list
        river.RemoveAt(0);

        // Spawn some crates
        SpawnCrates();
    }





}
