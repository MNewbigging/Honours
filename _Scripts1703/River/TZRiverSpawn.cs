using UnityEngine;
using System.Collections;

// This should be attached to the river segment trigger zones. It calls AddRiverSegment from river manager script

public class TZRiverSpawn : MonoBehaviour {

    // Reference to the rivermgr script to call its functions
    public GameObject gameMgr;
    private RiverMgr riverMgrScript;

    // Only run the collision code once
    private bool completed = false;


    void Start()
    {
        // Get reference to GameMgr game object in scene
        if (gameMgr == null)
        {
            gameMgr = GameObject.FindWithTag("GameMgr");
        }
        // Then a reference to that gameMgr's riverMgr attached script
        riverMgrScript = gameMgr.GetComponent<RiverMgr>();
    }

    // Perform once player enters only
    private void OnTriggerEnter(Collider other)
    {
        // Confirm it's the player triggering the collision
        if (other.transform.parent != null && other.transform.parent.tag == "Player" && completed == false)
        {
            // Add another random river segment to river
            riverMgrScript.AddRiverSegment();
            // Delete the old one
            riverMgrScript.RemoveRiverSegment();
            // Flip switch to prevent this from reoccurring
            completed = true;

            Debug.Log("Added to river");
        }
    }
}
