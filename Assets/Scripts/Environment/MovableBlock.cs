using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableBlock : Ground
{
	private int numCheckpoints;
	private int currentCheckpoint;
	private int ticksLeft;
	private Vector3 moveVec;
	private List<Vector3> checkpoints;
	private List<int> ticksToReach;

	private List<GameObject> itemsToMove;
	private List<Vector3> itemOffsets;

    // Start is called before the first frame update
    new void Start()
    {
		base.Start();
		// Init list of checkpoints
		checkpoints = new List<Vector3>();
		ticksToReach = new List<int>();
		numCheckpoints = 0;
		ticksLeft = 0;
		currentCheckpoint = 0;
      	// Get the list of checkpoints
		Transform movingPoints = transform.Find("Movepoints").gameObject.transform;
		int numChildren = movingPoints.childCount;
		for(int i = 0; i < numChildren; ++i){
			// Get each checkpoint
			GameObject goalPoint = movingPoints.GetChild(i).gameObject;
			Checkpoint cp = goalPoint.GetComponent(typeof(Checkpoint)) as Checkpoint;
			if(cp != null){
				numCheckpoints++;
				checkpoints.Add(goalPoint.transform.position);
				ticksToReach.Add(cp.GetMoveTime());
			}
		}
		// Set first checkpoint as goal
		ticksLeft = ticksToReach[0];
		moveVec = (checkpoints[0] - transform.position) / ticksToReach[0];
		// Other initializations
		itemsToMove = new List<GameObject>();
		itemOffsets = new List<Vector3>();
    }

    // Update is called once per frame
    new void FixedUpdate()
	{
		base.FixedUpdate();
		// Check if we should go to next checkpoint
		if(ticksLeft == 0){
			currentCheckpoint = (currentCheckpoint + 1) % numCheckpoints;
			ticksLeft = ticksToReach[currentCheckpoint];
			moveVec = (checkpoints[currentCheckpoint] - transform.position) / ticksToReach[currentCheckpoint];
		}
		// Move
		--ticksLeft;
		transform.position = transform.position + moveVec;
		// Move items stuck on top of this thing
		int objectCount = itemsToMove.Count;
		for(int i = 0; i < objectCount; ++i){
			itemsToMove[i].transform.position = transform.position + itemOffsets[i];
		}
	}

	// Add anything staying on top of this platform to list of items to move
	void OnTriggerStay(Collider col){
		GameObject obj = col.gameObject;
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
		int index = itemsToMove.IndexOf(obj);
		if(index == -1)
			return;
		else{
			itemsToMove.RemoveAt(index);
			itemOffsets.RemoveAt(index);
		}
	}

	override public GROUND_TYPE GetGroundType(){
		return GROUND_TYPE.Ground;
	}
}
