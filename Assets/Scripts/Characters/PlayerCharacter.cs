using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCharacter : Character
{
	// 1 tick = 0.02s
	private int JUMP_COOLDOWN = 60;
	private int ticks_until_jump = 0;

	// List of things in range that can be interacted with
	private List<Interactable> objectsInRange;

	// Most recent valid tile we stepped on
	private GameObject lastValidTile;

	// Health bar
	private Slider healthSlider;

    // Start is called before the first frame update
    new void Start()
    {
		base.Start();
		// Initializations
		objectsInRange = new List<Interactable>();
		rb = GetComponent<Rigidbody>();
		healthSlider = GameObject.Find("HealthSlider").GetComponent<Slider>();
    }

    // Update is called once per frame
    new void FixedUpdate()
	{
		/*
		if(ticks_until_jump > 0) --ticks_until_jump;
		base.FixedUpdate();
		// Check for key inputs
		// MOVEMENT INPUT
		float h = Input.GetAxis("Horizontal") * 2;
		float v = Input.GetAxis("Vertical") * 2;
		Vector3 vel = rb.velocity;
		if (Input.GetKeyDown("space") && ticks_until_jump == 0)
		{
			vel.y = Physics.gravity.y * -0.5f;
			ticks_until_jump = JUMP_COOLDOWN;
		}
		vel.x = h;
		vel.z = v;
		rb.velocity = vel;

		// INTERACTION INPUT
		if(Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)){
			// Interact with the first object in list (if any)
			if(objectsInRange.Count > 0){
				//Debug.Log("Interacting with " + objectsInRange[0].name);
				objectsInRange[0].OnInteraction(this.gameObject);
			}
		}
		*/

		// Update health bar
		//healthSlider.value = health;
	}

	// Methods to get something added to or removed from the list of interactables in range
	public void AddToPlayerRange(Interactable obj){
		//Debug.Log("Added " + obj.name + " to list");
		objectsInRange.Add(obj);
	}

	public void RemoveFromPlayerRange(Interactable obj){
		//Debug.Log("Removed " + obj.name + " from list");
		objectsInRange.Remove(obj);
	}

	public void SetLastValidTile(GameObject obj){ 
		lastValidTile = obj;
	}

	// Fell off the map or something, take damage and return to last valid tile
	public void ReturnToLastValidTile(){
		transform.position = lastValidTile.transform.position + Vector3.up;
		int damageToTake = health / 5; // Lose 20% of Health
		TakeDamage(damageToTake);
	}
}
