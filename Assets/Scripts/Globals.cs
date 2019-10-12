using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globals : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

	void OnCollisionEnter(Collision collision){
		Collider col = collision.collider;
		// Check if there is a rigidbody attached
		if(col.attachedRigidbody == null) return;
		// Check if it's the player
		GameObject obj = col.attachedRigidbody.gameObject;
		if(!obj.name.Equals("Player")) return;
		// Report this as the most recent valid tile the player stepped on
		PlayerCharacter pc = obj.GetComponent<PlayerCharacter>() as PlayerCharacter;
		pc.ReturnToLastValidTile();
	}
}
