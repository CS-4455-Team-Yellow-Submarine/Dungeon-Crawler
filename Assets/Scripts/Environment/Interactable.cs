using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    // Start is called before the first frame update
    protected void Start()
    {
        
    }

    // Update is called once per frame
	protected void FixedUpdate()
    {
        
    }

	// Notify the player this thing can be interacted (if it is the player)
	void OnTriggerEnter(Collider col){
		// Get the game object associated with this if it exists
		if(col.attachedRigidbody == null) return;
		GameObject obj = col.attachedRigidbody.gameObject;
		if(obj.name.Equals("Player")){
			Component[] comps = obj.GetComponents(typeof(Component)) as Component[];
			PlayerCharacter player = null;
			// Need to cast types since the thing being damaged is a derived class
			foreach(Component comp in comps)
				if(comp is Character)
					player = comp as PlayerCharacter;
			if(player == null) return;
			player.AddToPlayerRange(this);
		}
	}

	// Remove from the player's interactable list (if it is the player)
	void OnTriggerExit(Collider col){
		// Get the game object associated with this if it exists
		if(col.attachedRigidbody == null) return;
		GameObject obj = col.attachedRigidbody.gameObject;
		if(obj.name.Equals("Player")){
			Component[] comps = obj.GetComponents(typeof(Component)) as Component[];
			PlayerCharacter player = null;
			// Need to cast types since the thing being damaged is a derived class
			foreach(Component comp in comps)
				if(comp is Character)
					player = comp as PlayerCharacter;
			if(player == null) return;
			player.RemoveFromPlayerRange(this);
		}
	}

	public abstract void OnInteraction(GameObject player);
}
