using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : Ground
{
	// How often this block should do damage. 1 tick = 0.02 seconds
	const int ATTACK_RATE = 5;
	// Amount of damage this block should deal
	int DAMAGE_INFLICTED = 6;

	private int attack_cooldown = 0;

    // Start is called before the first frame update
    new void Start()
    {
		base.Start();
    }

    // Update is called once per frame
    new void FixedUpdate()
    {
		if(attack_cooldown > 0) --attack_cooldown;
		base.FixedUpdate();
    }

	// Damage whatever enters here
	void OnTriggerEnter(Collider col){
		if(attack_cooldown > 0) return;
		// Check if there is a rigidbody attached
		if(col.attachedRigidbody == null) return;
		// Check if it's something that can be damaged
		GameObject obj = col.attachedRigidbody.gameObject;
		if(!obj.tag.Equals("Character")) return;
		// Damage it if possible
		Component[] comps = obj.GetComponents(typeof(Component)) as Component[];
		Character c = null;
		// Need to cast types since the thing being damaged is a derived class
		foreach(Component comp in comps)
			if(comp is Character)
				c = comp as Character;
		if(c == null) return;
		c.TakeDamage(DAMAGE_INFLICTED);
		attack_cooldown = ATTACK_RATE;
	}

	// Continue damaging whatever is in here
	void OnTriggerStay(Collider col){
		if(attack_cooldown > 0) return;
		// Check if there is a rigidbody attached
		if(col.attachedRigidbody == null) return;
		// Check if it's something that can be damaged
		GameObject obj = col.attachedRigidbody.gameObject;
		if(!obj.tag.Equals("Character")) return;
		// Damage it if possible
		Component[] comps = obj.GetComponents(typeof(Component)) as Component[];
		Character c = null;
		// Need to cast types since the thing being damaged is a derived class
		foreach(Component comp in comps)
			if(comp is Character)
				c = comp as Character;
		if(c == null) return;
		c.TakeDamage(DAMAGE_INFLICTED);
		attack_cooldown = ATTACK_RATE;
	}

	override public GROUND_TYPE GetGroundType(){
		return GROUND_TYPE.Lava;
	}
}
