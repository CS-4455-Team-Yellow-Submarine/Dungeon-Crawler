﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class PlayerCharacter : Character
{
	// List of things in range that can be interacted with
	private List<Interactable> objectsInRange;

	// Most recent valid tile we stepped on
	private GameObject lastValidTile;

	// Health bar
	private Slider healthSlider;

	// Time of last attack
	private float lastAttackTime = -1f;

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
		base.FixedUpdate();

		// INTERACTION INPUT
		if(Input.GetMouseButtonDown(1) && objectsInRange.Count > 0){
			GetComponent<Animator>().SetBool("isInteracting", true);
		}

		// Update health bar
		healthSlider.value = health;

		if (health <= 0)
		{
			HandleDeadState();
		}
	}

	// Methods to get something added to or removed from the list of interactables in range
	public void AddToPlayerRange(Interactable obj){
		objectsInRange.Add(obj);
	}

	public void RemoveFromPlayerRange(Interactable obj){
		objectsInRange.Remove(obj);
	}

	// Returns the most plausible object for interaction
	private Interactable getObjectToInteractWith(){
		// Get the forward direction
		Vector3 forward = GetComponent<NewKnightController>().forward;
		// Determine which object is the most likely
		Interactable result = null;
		float maxDotProduct = -2f;
		foreach(Interactable obj in objectsInRange){
			// Direction to object
			Vector3 dirToObject = (obj.GetPosition() - transform.position).normalized;
			float dotProduct = Vector3.Dot(forward, dirToObject);
			if(dotProduct > maxDotProduct){
				maxDotProduct = dotProduct;
				result = obj;
			}
		}
		return result;
	}

	public void SetLastValidTile(GameObject obj){ 
		lastValidTile = obj;
	}

	// Fell off the map or something, take damage and return to last valid tile
	public void ReturnToLastValidTile(){
		GetComponent<CharacterController>().enabled = (false);
		transform.position = lastValidTile.transform.position + Vector3.up;
		GetComponent<CharacterController>().enabled = (true);
		//int damageToTake = health / 5; // Lose 20% of Health
		int damageToTake = 40;
		TakeDamage(damageToTake);
	}

	public void HandleAttack(Vector3 forward){
		// Check to make sure it's been at least 1 second since last attack
		if(Time.time - 1f > lastAttackTime){
			lastAttackTime = Time.time;
			// Launch projectile forward
			GameObject obj = Object.Instantiate(projectile) as GameObject;
			obj.transform.parent = transform;
			Projectile pr = obj.GetComponent<Projectile>();
			// Associate definitions with the projectile
			pr.Define(GetComponent<Rigidbody>(), transform.position + Vector3.up, null);
			// Override the direction of this projectile
			pr.SetMoveDirection(forward);
		}
	}

	public void DoAttack(){
		Debug.Log("Attacking");
		Vector3 forward = GetComponent<NewKnightController>().forward;
		lastAttackTime = Time.time;
		// Launch projectile forward
		GameObject obj = Object.Instantiate(projectile) as GameObject;
		obj.transform.parent = transform;
		Projectile pr = obj.GetComponent<Projectile>();
		// Associate definitions with the projectile
		pr.Define(GetComponent<Rigidbody>(), transform.position + Vector3.up, null);
		// Override the direction of this projectile
		pr.SetMoveDirection(forward);
	}

	new void HandleDeadState(){
		GameObject.Find("SystemMessage").GetComponent<Text>().text = "You have died...";
		Time.timeScale = 0f;
		base.HandleDeadState();
		SceneManager.LoadScene("LoseScene");
	}

	private void GoToStartMenu(){
		SceneManager.LoadScene("StartScene");
	}

	public void DoInteract(){
		// Interact with the first object in list (if any)
		if(objectsInRange.Count > 0){
			getObjectToInteractWith().OnInteraction(this.gameObject);
		}
	}

	public void OnInteractFinish(){
		GetComponent<Animator>().SetBool("isInteracting", false);
	}
}
