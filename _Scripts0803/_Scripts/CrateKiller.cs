using UnityEngine;
using System.Collections;

public class CrateKiller : MonoBehaviour {

	
	void FixedUpdate () {
	    // Destroy this if falling off level
        if (transform.position.y < -50)
        {
            Debug.Log("Destroying crate");
            Destroy(gameObject);
        }
	}
}
