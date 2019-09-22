using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : Ground
{
	// How often this block should do damage. 1 tick = 0.02 seconds
	const int ATTACK_RATE = 5;
	// Amount of damage this block should deal
	int DAMAGE_INFLICTED = 6;

    // Start is called before the first frame update
    new void Start()
    {
		base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
		base.Update();
    }

	// Damage whatever enters here
	void OnTriggerEnter(Collider col){
		// Check if there is a rigidbody attached
		if(col.attachedRigidbody == null) return;
		// Check if it's something that can be damaged
		GameObject obj = col.attachedRigidbody.gameObject;
		if(!obj.tag.Equals("Character")) return;
		// Damage it if possible
		Character c = obj.GetComponent<Character>();
		if(c == null) return;
		c.TakeDamage(DAMAGE_INFLICTED, ATTACK_RATE);
	}

	// Continue damaging whatever is in here
	void OnTriggerStay(Collider col){
		// Check if there is a rigidbody attached
		if(col.attachedRigidbody == null) return;
		// Check if it's something that can be damaged
		GameObject obj = col.attachedRigidbody.gameObject;
		if(!obj.tag.Equals("Character")) return;
		// Damage it if possible
		Character c = obj.GetComponent<Character>();
		if(c == null) return;
		c.TakeDamage(DAMAGE_INFLICTED, ATTACK_RATE);
	}
}
