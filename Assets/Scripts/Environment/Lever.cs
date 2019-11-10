using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : Interactable
{
	// Time it takes to be pushed; 1 tick = 0.02 seconds
	private const int INTERACTION_TIME = 60;
	private int ticksUntilInteractionsAllowed = 0;
	public TriggeredObject[] objectsAffected;

	// Start is called before the first frame update
	new void Start()
	{
		base.Start();
	}

    // Update is called once per frame
    void Update()
	{
		if(ticksUntilInteractionsAllowed > 0){
			--ticksUntilInteractionsAllowed;
		}
    }

	// Interaction event
	override public void OnInteraction(GameObject player){
		// Make sure we can't spam interaction
		if(ticksUntilInteractionsAllowed > 0) return;
		// Pull animation
		GetComponent<Animator>().SetBool("isPulled", true);
		// Disable interaction for a while
		ticksUntilInteractionsAllowed = INTERACTION_TIME;
	}

	// Lever is pulled
	public void OnLeverPulled(){
		GetComponent<Animator>().SetBool("isPulled", false);
		// Trigger all objects
		foreach(TriggeredObject o in objectsAffected)
			o.getTriggered();
	}
}
