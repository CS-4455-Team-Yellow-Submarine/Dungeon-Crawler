using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Finite state machine basic definitions
public interface FSM_State{
	// Called when a state is entered
	void Start();
	// Called when a state is executing
	void Execute();
	// Called when a state is terminated
	void End();
	// Called when we want to name the current state
	string ToString();
}

public class FSM{
	// Current state
	FSM_State currentState;

	public FSM(){
		currentState = null;
	}

	public void SetState(FSM_State state){
		if(currentState != null)
			currentState.End();
		
		currentState = state;
		currentState.Start();
	}

	public void Update(){
		if(currentState != null)
			currentState.Execute();
	}

	public FSM_State GetState(){
		return currentState;
	}

	public string GetStateName(){
		if(currentState != null)
			return currentState.ToString();
		return "None";
	}
}

// End finite state machine basic definitions

// *****
// State machine states defined here
// *****

// Patrolling state
public class State_Patrol : FSM_State{
	private BasicEnemyCharacter enemy;
	private NavMeshAgent navAgent;
	private Animator anim;
	private List<Vector3> checkpoints;
	private List<int> ticksToReach;
	private int ticksLeft;
	private Vector3 moveVec;
	private int numCheckpoints;
	public int currentCheckpoint { get; set; }

	public State_Patrol(BasicEnemyCharacter ch){
		this.enemy = ch;
	}

	public void Start(){
		ticksLeft = 0;
	}

	public void Execute(){
		if(numCheckpoints == 0) return;
		// Check if we should go to next checkpoint
		if(navAgent.remainingDistance < 0.3f && !navAgent.pathPending && ticksLeft <= 0){
			currentCheckpoint = (currentCheckpoint + 1) % numCheckpoints;
			ticksLeft = ticksToReach[currentCheckpoint];
			navAgent.SetDestination(checkpoints[currentCheckpoint]);
		}
		/*
		if(ticksLeft == 0){
			currentCheckpoint = (currentCheckpoint + 1) % numCheckpoints;
			moveVec = (checkpoints[currentCheckpoint] - enemy.GetPosition()) / ticksToReach[currentCheckpoint];
			ticksLeft = ticksToReach[currentCheckpoint];
			if(moveVec.magnitude > 0.02f){
				Quaternion quat = Quaternion.LookRotation(moveVec.normalized);
				enemy.SetRotation(quat);
			}
		}
		*/
		// Move
		--ticksLeft;
		/*enemy.HandleMove(moveVec);
		// Set the animation accordingly
		if(moveVec.magnitude < 0.02f)
			anim.SetFloat("velocity", 0f);
		else
			anim.SetFloat("velocity", 1f);
			*/
		if(navAgent.velocity.magnitude < 0.02f)
			anim.SetFloat("velocity", 0f);
		else
			anim.SetFloat("velocity", 1f);
	}

	public void End(){
		enemy.lastCheckpoint = currentCheckpoint;
	}

	public void SetPatrolPoints(List<Vector3> points, List<int> ticks){
		checkpoints = points;
		ticksToReach = ticks;
		numCheckpoints = points.Count;
	}

	public void SetAnimator(Animator an){
		this.anim = an;
	}

	public void SetNavAgent(NavMeshAgent ag){
		this.navAgent = ag;
	}

	override public string ToString(){ return "Patrol"; }
}

// Chasing state
public class State_Chase : FSM_State{
	private BasicEnemyCharacter enemy;
	private Animator anim;
	private NavMeshAgent navAgent;
	private GameObject target;
	private float moveSpeed;
	private Vector3 moveVec;

	public State_Chase(BasicEnemyCharacter ch){
		this.enemy = ch;
	}

	public void SetChaseTarget(GameObject o){
		target = o;
	}

	public void SetSpeed(float f){
		moveSpeed = f;
	}

	public void SetAnimator(Animator an){
		this.anim = an;
	}

	public void SetNavAgent(NavMeshAgent ag){
		this.navAgent = ag;
	}

	public void Start(){
	}

	public void Execute(){
		// Chase after the player
		navAgent.SetDestination(target.transform.position);
		anim.SetFloat("velocity", 1f);
		/*
		moveVec = (target.transform.position - enemy.GetPosition()).normalized;
		enemy.SetRotation(Quaternion.LookRotation(moveVec));
		enemy.HandleMove(moveVec * moveSpeed / 50f);
		anim.SetFloat("velocity", 0.6f);
		*/
	}

	public void End(){
	}

	override public string ToString(){ return "Chase"; }
}

// Return to patrol point
public class State_Return : FSM_State{
	private BasicEnemyCharacter enemy;
	private Animator anim;
	private int ticksLeft;
	private Vector3 moveVec;
	private Vector3 destination;
	private float moveSpeed;

	public State_Return(BasicEnemyCharacter ch){
		this.enemy = ch;
	}

	public void SetSpeed(float f){
		moveSpeed = f;
	}

	public void SetAnimator(Animator an){
		this.anim = an;
	}

	public void SetDestination(Vector3 dst){
		destination = dst;
	}

	public void Start(){
	}

	public void Execute(){
		anim.SetFloat("velocity", 1f);
		// Determine move direction
		Vector3 heading = destination - enemy.GetPosition();
		// Move in that direction as appropriate
		if(heading.magnitude < moveSpeed / 50f){
			enemy.HandleMove(heading / 50f);
			enemy.OnArriveAtPatrolPoint();
		}
		else{
			enemy.HandleMove(heading.normalized * moveSpeed / 50f);
		}
	}

	public void End(){
		
	}

	override public string ToString(){ return "Return"; }
}


public class State_Attack : FSM_State{
	private BasicEnemyCharacter enemy;
	private Animator anim;
	private GameObject target;
	private Vector3 moveVec;
	private int attackDelay, attackCooldown; // Measured in game ticks!
	private GameObject bullet;
	private int damage;
	private float speed;
	private float distance;

	public State_Attack(BasicEnemyCharacter ch){
		this.enemy = ch;
	}

	public void SetTarget(GameObject dst){
		target = dst;
	}

	public void SetAnimator(Animator an){
		this.anim = an;
	}

	public void SetAttackDelay(float delay){
		attackDelay = (int)(50f * delay);
	}

	public void SetAttackCooldown(float cooldown){
		attackCooldown = (int)(50f * cooldown);
	}

	public void SetProjectile(GameObject o, int dmg = 10, float spd = 0f, float far = 1.2f){
		bullet = o;
		damage = dmg;
		speed = spd;
		distance = far;
	}

	public void Start(){
	}

	public void Execute(){
		// Can we attack?
		if(attackDelay > 0){
			--attackDelay;
			anim.SetBool("isAttacking", false);
			anim.SetFloat("velocity", 0f);
		}
		else if(attackDelay == 0){
			// Indicate an attack
			anim.SetBool("isAttacking", true);
			// Put attack on cooldown
			attackDelay = attackCooldown;
			// Fire a projectile as necessary
			GameObject obj = Object.Instantiate(bullet) as GameObject;
			obj.transform.parent = enemy.GetGameObject().transform;
			Projectile pr = obj.GetComponent<Projectile>();
			// Associate definitions with the projectile
			pr.Define(enemy.GetComponent<Rigidbody>(), enemy.transform.position, target, speed, distance);
		}
	}

	public void End(){
	}

	override public string ToString(){ return "Attack"; }
}

// End state definitions

// *****
// All boss state machines defined here
// *****

//this state is for boss attack
public class State_BossAttack : FSM_State
{
    private BossEnemyCharacter enemy;
    private Animator anim;
    private GameObject target;
    private Vector3 moveVec;
    private int attackDelay, attackCooldown; // Measured in game ticks!
    private GameObject bullet;
    private int damage;
    private float speed;
    private float distance;

    public State_BossAttack(BossEnemyCharacter ch)
    {
        this.enemy = ch;
    }

    public void SetTarget(GameObject dst)
    {
        target = dst;
    }

    public void SetAnimator(Animator an)
    {
        this.anim = an;
    }

    public void SetAttackDelay(float delay)
    {

        attackDelay = (int)(50f * delay);
    }

    public void SetAttackCooldown(float cooldown)
    {
        attackCooldown = (int)(50f * cooldown);
    }

    //the boss has higher damage than other enemys
    public void SetProjectile(GameObject o, int dmg = 15, float spd = 0f, float far = 1.8f)
    {
        bullet = o;
        damage = dmg;
        speed = spd;
        distance = far;
    }

    public void Start()
    {
    }

    public void Execute()
    {
        // Can we attack?
        if (attackDelay > 0)
        {
            --attackDelay;
            anim.SetBool("Boss is Attacking", false);
            anim.SetFloat("Boss velocity", 0f);
        }
        else if (attackDelay == 0)
        {
            // Indicate an attack
            anim.SetBool("Boss is Attacking", true);
            // Put attack on cooldown
            attackDelay = attackCooldown;
            // Fire a projectile as necessary
            GameObject obj = Object.Instantiate(bullet) as GameObject;
            obj.transform.parent = enemy.GetGameObject().transform;
            Projectile pr = obj.GetComponent<Projectile>();
            // Associate definitions with the projectile
            pr.Define(enemy.GetComponent<Rigidbody>(), enemy.transform.position, target, speed, distance);
        }
    }

    public void End()
    {
    }

    override public string ToString() { return "Boss Attack"; }
}

//this state is for boss being Relaxed (
public class State_BossRelax : FSM_State
{
    private BossEnemyCharacter enemy;
    private Animator anim;

    public State_BossRelax(BossEnemyCharacter ch)
    {
        this.enemy = ch;
    }
    public void SetAnimator(Animator an)
    {
        this.anim = an;
    }
    public void Start()
    {
    }

    public void Execute()
    {

    }
    public void End()
    {
    }

    override public string ToString() { return "Boss Relax"; }
}

//this state is for boss being Chased
public class State_BossChase : FSM_State
{
    private BossEnemyCharacter enemy;
    private Animator anim;
    private GameObject target;
    private float moveSpeed;
    private Vector3 moveVec;

    public State_BossChase(BossEnemyCharacter ch)
    {
        this.enemy = ch;
    }

    public void SetChaseTarget(GameObject o)
    {
        target = o;
    }

    public void SetSpeed(float f)
    {
        moveSpeed = f;
    }

    public void SetAnimator(Animator an)
    {
        this.anim = an;
    }

    public void Start()
    {
    }

    public void Execute()
    {
        // Chase after the player
        moveVec = (target.transform.position - enemy.GetPosition()).normalized;
        enemy.SetRotation(Quaternion.LookRotation(moveVec));
        //the boss is moving faster than normal enemys
        enemy.HandleMove(moveVec * moveSpeed / 30f);
        anim.SetFloat("velocity", 0.8f);
    }

    public void End()
    {
    }

    override public string ToString() { return "Boss Chase"; }
}


