using UnityEngine;
using System.Collections;

public class CrateKillZone : MonoBehaviour {

    // This destroys anything that touches it
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Crate"))
            Debug.Log("Crate destroyed");

        Destroy(other);

    }
}
