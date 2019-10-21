using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ConeCollider), typeof(CapsuleCollider), typeof(Animator))]
public class BasicEnemyCharacter : Character
{
	// Private controllers
	private Animator anim;
	private ConeCollider visionRange;
	private CapsuleCollider attackCollider;

	// Other variables
	private FSM stateMachine;
	private List<Vector3> checkpoints;
	private List<int> ticksToReach;
	public int lastCheckpoint { get; set; }

	// Character specific
	public float moveSpeed;
	public bool playerInRange;
	public float attackDelay = 1f; // How much time before the attack animation should start
	public float attackCooldown = 4f; // How much time before we can attack again
	public float attackRange = 1.2f; // How far ahead we can attack

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
		attackCollider = GetComponent<CapsuleCollider>();
		if(attackCollider == null)
			Debug.Log("Could not find capsule collider");
		// Set the attack capsule's range
		attackCollider.radius = attackRange;
		attackCollider.isTrigger = true;
		// Currently not attacking
		attackCollider.enabled = false;
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

		if(health <= 0){
			GameObject.Find("EnemyHealthSlider").GetComponent<Slider>().value = 0;
			GameObject.Find("Enemy_Health").GetComponent<Text>().text = "";
		}
    }

	void OnTriggerEnter(Collider col){
		// Check if player is in vision range
		if(col.attachedRigidbody && col.attachedRigidbody.gameObject.name.Equals("Player")){
			// Determine which state we should change to
			if(stateMachine.GetStateName().Equals("Patrol") || stateMachine.GetStateName().Equals("Return")){
				visionRange.enabled = false;
				attackCollider.enabled = true;
				// Change to chase state
				State_Chase state = new State_Chase(this);
				state.SetAnimator(anim);
				state.SetSpeed(moveSpeed);
				state.SetChaseTarget(col.attachedRigidbody.gameObject);

				stateMachine.SetState(state);
				attackCollider.radius = attackRange;
				return;
			}
			else if(stateMachine.GetStateName().Equals("Chase")){
				playerInRange = true;
				// Change to attack state
				State_Attack state = new State_Attack(this);
				state.SetAnimator(anim);
				state.SetTarget(col.attachedRigidbody.gameObject);
				state.SetAttackDelay(attackDelay);
				state.SetAttackCooldown(attackCooldown);
				state.SetProjectile(projectile, attackDamage, 0, attackRange);

				stateMachine.SetState(state);
				// Keep the attack more persistent
				attackCollider.radius = attackRange * 1.25f;
			}
		}
	}

	/*
	void OnTriggerStay(Collider col){
		// Check if it was the player
		if(col.attachedRigidbody && col.attachedRigidbody.gameObject.name.Equals("Player")){
			// Check the state
			if(stateMachine.GetStateName().Equals("Attack") || stateMachine.GetStateName().Equals("Chase")){
				playerInRange = true;
				// Change to attack state if we're not already there
				if(!stateMachine.GetStateName().Equals("Attack")){
					State_Attack state = new State_Attack(this);
					state.SetAnimator(anim);
					state.SetTarget(transform.position + (col.attachedRigidbody.gameObject.transform.position - transform.position).normalized * attackRange);
					state.SetAttackDelay(attackDelay);
					state.SetAttackCooldown(attackCooldown);
					state.SetProjectile(projectile, attackDamage);

					stateMachine.SetState(state);
				}
			}
		}
	}
	*/

	void OnTriggerExit(Collider col){
		// Check if it was the player
		if(col.attachedRigidbody && col.attachedRigidbody.gameObject.name.Equals("Player")){
			// Check the state
			if(stateMachine.GetStateName().Equals("Attack")){
				playerInRange = false;
				// Change to chase state
				State_Chase state = new State_Chase(this);
				state.SetAnimator(anim);
				state.SetSpeed(moveSpeed);
				state.SetChaseTarget(col.attachedRigidbody.gameObject);

				stateMachine.SetState(state);
				attackCollider.radius = attackRange;
			}
		}
	}

	// Tell this character to return to patrolling
	public void ForceReturnToPatrol(){
		// No need to force return if we're already patrolling
		if(stateMachine.GetStateName().Equals("Patrol")) return;

		visionRange.enabled = true;
		attackCollider.enabled = false;

		State_Return state = new State_Return(this);
		state.SetAnimator(anim);
		state.SetSpeed(moveSpeed);
		state.SetDestination(checkpoints[lastCheckpoint]);

		stateMachine.SetState(state);
	}

	// We have returned to where we were previously patrolling
	public void OnArriveAtPatrolPoint(){
		// Continue patrolling
		State_Patrol state = new State_Patrol(this);
		state.SetAnimator(anim);
		state.SetPatrolPoints(checkpoints, ticksToReach);
		state.currentCheckpoint = lastCheckpoint;

		stateMachine.SetState(state);
	}

	// Used for updating the health info
	new public void TakeDamage(int amount){
		base.TakeDamage(amount);
		GameObject.Find("EnemyHealthSlider").GetComponent<Slider>().value = health;
		GameObject.Find("Enemy_Health").GetComponent<Text>().text = unitName;
	}
}
