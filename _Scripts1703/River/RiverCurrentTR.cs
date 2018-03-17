using UnityEngine;
using System.Collections;

public class RiverCurrentTR : MonoBehaviour {

    // Current speed
    private float currentSpeed = 1.2f;
    // Change current direction
    public void ChangeCurrentDirection(float newCurrentSpeed) { currentSpeed = newCurrentSpeed; }

    // Current - move everything on this river piece
    private void OnTriggerStay(Collider other)
    {
        // Move it downstream - corner moves on x and z
        Vector3 downstream = new Vector3((currentSpeed * Time.deltaTime), 0, -(currentSpeed * Time.deltaTime));
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
