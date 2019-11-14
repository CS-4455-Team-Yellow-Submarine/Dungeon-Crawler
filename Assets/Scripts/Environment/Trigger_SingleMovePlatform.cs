using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// On trigger: this object moves to the next platform
public class Trigger_SingleMovePlatform : TriggeredObject
{
	public Checkpoint[] checkpoints;
	private int[] ticksToReach;
	private int currentCheckpoint = -1;
	private int checkpointCount;
	private int ticksLeft;
	private Vector3 moveDirection;

	private List<GameObject> itemsToMove;
	private List<Vector3> itemOffsets;

	void Start(){
		// Initialize checkpoint and time to reach list
		checkpointCount = checkpoints.Length;
		if(checkpointCount > 0)
			ticksToReach = new int[checkpointCount];
		for(int i = 0; i < checkpointCount; i++){
			ticksToReach[i] = checkpoints[i].GetMoveTime();
		}
		itemsToMove = new List<GameObject>();
		itemOffsets = new List<Vector3>();
	}

	void FixedUpdate(){
		if(ticksLeft > 0){
			--ticksLeft;
			transform.position = transform.position + moveDirection;
			// Move items stuck on top of this thing
			int objectCount = itemsToMove.Count;
			for(int i = 0; i < objectCount; ++i){
				itemsToMove[i].transform.position = transform.position + itemOffsets[i];
			}
		}
	}

	// Trigger: Go to next checkpoint
	override public void getTriggered(){
		currentCheckpoint = (currentCheckpoint + 1) % checkpointCount;
		ticksLeft = ticksToReach[currentCheckpoint];
		moveDirection = (checkpoints[currentCheckpoint].gameObject.transform.position - transform.position) / ticksLeft;
	}

	// Add anything staying on top of this platform to list of items to move
	void OnTriggerStay(Collider col){
		GameObject obj = col.gameObject;
		if(!(obj.tag.Equals("Character") || obj.tag.Equals("Interactive"))) return;
		int index = itemsToMove.IndexOf(obj);
		if(index == -1){
			itemsToMove.Add(obj);
			itemOffsets.Add(obj.transform.position - transform.position);
		} else{
			itemOffsets[index] = obj.transform.position - transform.position;
		}
	}

	void OnTriggerExit(Collider col){
		GameObject obj = col.gameObject;
		if(!(obj.tag.Equals("Character") || obj.tag.Equals("Interactive"))) return;
		int index = itemsToMove.IndexOf(obj);
		if(index == -1)
			return;
		else{
			itemsToMove.RemoveAt(index);
			itemOffsets.RemoveAt(index);
		}
	}
}
