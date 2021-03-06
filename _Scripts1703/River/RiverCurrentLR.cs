﻿using UnityEngine;
using System.Collections;

public class RiverCurrentLR : MonoBehaviour {


    // Current strength
    private float currentSpeed = 1.2f;
    // Setter
    public void ChangeCurrentDirection(float newCurrentSpeed) { currentSpeed = newCurrentSpeed; }


    // Current - move everything on this river piece
    private void OnTriggerStay(Collider other)
    {
        // Move it downstream - LR moves on x axis
        Vector3 downstream = new Vector3((currentSpeed * Time.deltaTime), 0, 0);
        // If it's the player, target parent
        if (other.transform.parent != null && other.transform.parent.tag == "Player")
        {
            other.transform.parent.position += downstream;
        }
        else if (other.tag == "Crate")
        {
            // Adjust speed for crate - slower than player
            downstream *= 0.8f;
            other.transform.position += downstream;
        }
    }
}
