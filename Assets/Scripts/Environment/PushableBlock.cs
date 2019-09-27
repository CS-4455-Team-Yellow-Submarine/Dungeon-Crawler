using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableBlock : Interactable
{
	// Time it takes to be pushed; 1 tick = 0.02 seconds
	private const int INTERACTION_TIME = 50;
	private bool interactionDisabled = false;
	private int ticksUntilInteractionsAllowed = 0;
	private List<Vector3> testVecs = new List<Vector3>{Vector3.forward, Vector3.back, Vector3.left, Vector3.right};
	private Vector3 pushVector = Vector3.zero;

    // Start is called before the first frame update
    new void Start()
    {
		base.Start();
    }

    // Update is called once per frame
    new void FixedUpdate()
    {
		if(ticksUntilInteractionsAllowed > 0){
			--ticksUntilInteractionsAllowed;
			transform.position = transform.position + pushVector;
			if(ticksUntilInteractionsAllowed == 0){
				pushVector = Vector3.zero;
				interactionDisabled = false;
			}
		}
		if(ticksUntilInteractionsAllowed == 0){
			if(GetComponent<Rigidbody>().velocity.y == 0f)
				GetComponent<Rigidbody>().isKinematic = true;
		}
    }

	// Block should be pushed by player
	override public void OnInteraction(){
		if(interactionDisabled) return;
		// Get pushed
		Vector3 relativePosition = transform.position - player.transform.position;
		// Test which cardinal direction this should be pushed
		Vector3 pushDir = Vector3.zero;
		float maxVal = -2048f;
		foreach(Vector3 vec in testVecs){
			float dotProduct = Vector3.Dot(vec, relativePosition);
			if(dotProduct > maxVal){
				maxVal = dotProduct;
				pushDir = vec;
			}
		}
		// Note that block is 2x2x2; move it 1 unit
		pushVector = pushDir / INTERACTION_TIME;
		interactionDisabled = true;
		ticksUntilInteractionsAllowed = INTERACTION_TIME;
		GetComponent<Rigidbody>().isKinematic = false;
	}
}
