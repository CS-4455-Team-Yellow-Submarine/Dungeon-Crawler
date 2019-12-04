using System.Collections;
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
		if(GameObject.Find("StartLocation") != null)
			transform.position = GameObject.Find("StartLocation").gameObject.transform.position;
		objectsInRange = new List<Interactable>();
		rb = GetComponent<Rigidbody>();
		healthSlider = GameObject.Find("HealthSlider").GetComponent<Slider>();
    }

    // Update is called once per frame
    new void FixedUpdate()
	{
		base.FixedUpdate();

		// INTERACTION INPUT
		if((Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.LeftShift)) && objectsInRange.Count > 0){
			GetComponent<Animator>().SetBool("isInteracting", true);
		}

		if(transform.position.z > 37){
			GameObject.Find("StartLocation").GetComponent<DoNotDestroy>().SetCheckpointLocation(new Vector3(0, 5, 38));
		}

		// Update health bar
		healthSlider.value = health;
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
		int damageToTake = health / 5; // Lose 20% of Health or 10 health
		if(damageToTake < 10) damageToTake = 10;
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
		// Set the damage of the projectile
		pr.damage = attackDamage;
	}

	new void OnCharacterDeath(){
		GameObject.Find("SystemMessage").GetComponent<Text>().text = "You have died...";
		Time.timeScale = 0f;
		if(SceneManager.GetActiveScene().name.Equals("TutorialScene"))
			SceneManager.LoadScene("LoseScene");
		else
			SceneManager.LoadScene("LoseScene2");
	}

	private void GoToStartMenu(){
		if(SceneManager.GetActiveScene().name.Equals("TutorialScene"))
			SceneManager.LoadScene("StartScene");
		else
			SceneManager.LoadScene("StartScene2");
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
