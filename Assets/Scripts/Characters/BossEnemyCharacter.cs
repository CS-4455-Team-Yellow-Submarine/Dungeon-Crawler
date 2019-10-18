using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(Animator))]
public class BossEnemyCharacter : Character
{
    // Private controllers
    private Animator anim;
    private ConeCollider visionRange;
    private SphereCollider attackCollider;

    // Other variables
    private FSM stateMachine;
    //private List<Vector3> checkpoints;
    //private List<int> ticksToReach;

    //public int lastCheckpoint { get; set; }

    // Character specific
    public float moveSpeed;
    public bool playerInRange;
    public float attackDelay = 1f; // How much time before the attack animation should start
    public float attackCooldown = 2.5f; // How much time before we can attack again
    public float attackRange = 2f; // How far ahead we can atta

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        // Get references
        anim = GetComponent<Animator>();
        if (anim == null)
            Debug.Log("Boss Animator could not be found");
        visionRange = GetComponent<ConeCollider>();
        if (visionRange == null)
            Debug.Log("Could not find Boss vision range box collider");
        attackCollider = GetComponent<SphereCollider>();
        if (attackCollider == null)
            Debug.Log("Could not find Boss attack range box collider");
        // Currently not attacking, it observed if player is
        attackCollider.enabled = false;
        visionRange.enabled = true;

        // Init state machine,
        //the boss intially is in Relax state
        stateMachine = new FSM();
        State_BossRelax state = new State_BossRelax(this);
        state.SetAnimator(anim);

        stateMachine.SetState(state);


    }

    new void FixedUpdate()
    {
        base.FixedUpdate();
        // Update state machine
        stateMachine.Update();
    }

    void OnTriggerEnter(Collider col)
    {
        // Check if it was the player
        if (col.attachedRigidbody && col.attachedRigidbody.gameObject.name.Equals("Player"))
        {
            // Check the state
            if (stateMachine.GetStateName().Equals("Boss Relax"))
            {
                visionRange.enabled = false;
                attackCollider.enabled = true;
                // Change to Boss Stand state
                State_BossChase state = new State_BossChase(this);
                state.SetAnimator(anim);
                state.SetSpeed(moveSpeed);
                state.SetChaseTarget(col.attachedRigidbody.gameObject);

                stateMachine.SetState(state);
            }

            else if (stateMachine.GetStateName().Equals("Boss Chase"))
            {
                playerInRange = true;
                // Change to attack state
                State_BossAttack state = new State_BossAttack(this);
                state.SetAnimator(anim);
                state.SetTarget(col.attachedRigidbody.gameObject);
                state.SetAttackDelay(attackDelay);
                state.SetAttackCooldown(attackCooldown);
                state.SetProjectile(projectile, attackDamage, 0, attackRange);

                stateMachine.SetState(state);
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        // Check if it was the player
        if (col.attachedRigidbody && col.attachedRigidbody.gameObject.name.Equals("Player"))
        {
            // Check the state
            if (stateMachine.GetStateName().Equals("Boss Attack"))
            {
                playerInRange = false;
                // Change to chase state
                State_BossChase state = new State_BossChase(this);
                state.SetAnimator(anim);
                state.SetSpeed(moveSpeed);
                state.SetChaseTarget(col.attachedRigidbody.gameObject);

                stateMachine.SetState(state);
            }
        }
    }
}
