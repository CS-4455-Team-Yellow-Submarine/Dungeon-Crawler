using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ConeCollider), typeof(CapsuleCollider), typeof(Animator))]
public class BasicEnemyCharacter : Character
{
	// Private controllers
	private Animator anim;
	private ConeCollider visionRange;
	private CapsuleCollider attackRange;

	// Other variables
	private FSM stateMachine;
	private List<Vector3> checkpoints;
	private List<int> ticksToReach;

	// Start is called before the first frame update
	new void Start()
	{
		base.Start();
		// Get references
		anim = GetComponent<Animator>();
		if (anim == null)
			Debug.Log("Animator could not be found");
		visionRange = GetComponent<ConeCollider>();
		if(visionRange == null)
			Debug.Log("Could not find cone collider");
		attackRange = GetComponent<CapsuleCollider>();
		if(attackRange == null)
			Debug.Log("Could not find capsule collider");
		// Currently not attacking
		attackRange.enabled = false;
		visionRange.enabled = true;

		// Init list of checkpoints
		checkpoints = new List<Vector3>();
		ticksToReach = new List<int>();
		// Get the list of checkpoints
		Transform movingPoints = transform.Find("Movepoints").gameObject.transform;
		int numChildren = movingPoints.childCount;
		for(int i = 0; i < numChildren; ++i){
			// Get each checkpoint
			GameObject goalPoint = movingPoints.GetChild(i).gameObject;
			Checkpoint cp = goalPoint.GetComponent(typeof(Checkpoint)) as Checkpoint;
			if(cp != null){
				checkpoints.Add(goalPoint.transform.position);
				ticksToReach.Add(cp.GetMoveTime());
			}
		}

		// Init state machine
		stateMachine = new FSM();
		State_Patrol state = new State_Patrol(this);
		state.SetAnimator(anim);
		state.SetPatrolPoints(checkpoints, ticksToReach);
		state.currentCheckpoint = -1;

		stateMachine.SetState(state);
    }


	new void FixedUpdate()
	{
		base.FixedUpdate();
		// Update state machine
		stateMachine.Update();
    }

	void OnTriggerEnter(Collider col){
		if(col.attachedRigidbody && col.attachedRigidbody.gameObject.tag.Equals("Character"))
			Debug.Log("Entered range of some collider");
	}
}
