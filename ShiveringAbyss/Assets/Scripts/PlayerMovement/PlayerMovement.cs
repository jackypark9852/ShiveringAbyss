using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class PlayerMovement : LivingEntity
{
    [Header("Velocity")]
    [SerializeField] float movementSpeed = 5f; 
    [SerializeField] float jumpSpeed = 5f; 
    [SerializeField] float maxVelocity = 22f; 



    [Header("Gravity Scale")]
    [SerializeField] float defaultGravtiy = 10f; 



    [Header("State Machine")]
    [SerializeField] float isMovingThreshold; 
    [SerializeField] float isFallingThreshold = -0.1f; 
    [SerializeField] bool isControlLocked; 
    [SerializeField] bool isOnGround; 
    [SerializeField] bool isMoving; 
    [SerializeField] bool isFalling; 
    [SerializeField] bool isDashing;
    bool playedMaskAnimation = false; 
    


    [Header("Animation")]
    float uninterruptableTime = 0; 
    

    [Header("Attack")] 
    [SerializeField] float attackDamage; 
    [SerializeField] float startAttackCD;
    [SerializeField] float attackAnimSpeed; 
    [SerializeField] float attackMoveSpeed; 
    [SerializeField] Transform attackPosA; 
    [SerializeField] Transform attackPosB; 
    [SerializeField] LayerMask enemyLayerMask; 
    [SerializeField] float knockBackForce; 
    [SerializeField] float knockBackDuration; 
    float attackDuration; 
    float attackCD = 0; 



    [Header("Dash")] 
    [SerializeField] float dashSpeed;
    [SerializeField] float startDashDuration; 
    [SerializeField] float dashDuration;
    [SerializeField] float startDashCD;
    [SerializeField] float dashCD;
    [SerializeField] int playerLayerIdx;
    [SerializeField] int enemyLayerIdx;

    // [SerializeField] float attackAnimationLength; 

    Vector2 movementInput; 
    CapsuleCollider2D myCapsuleCollider; 
    BoxCollider2D myFootCollider; 
    Animator myAnimator; 
    LayerMask enemiesLayerMask; 

    // ------------------------------------音频Audio Source------------------------------------------
    [SerializeField] AudioSource audioFootstep; 
    [SerializeField] AudioSource attackSound; 
    [SerializeField] AudioSource jumpsound; 
    [SerializeField] AudioSource landingSound; 
    [SerializeField] AudioSource dashSound;
 
    public override void Awake() {
        base.Awake(); 
        myRigidBody = GetComponent<Rigidbody2D>(); 
        myCapsuleCollider = GetComponent<CapsuleCollider2D>(); 
        myFootCollider = GetComponent<BoxCollider2D>(); 
        myAnimator = GetComponent<Animator>(); 

        enemiesLayerMask = LayerMask.GetMask("Enemy");
    }
    void Start()
    {
        myRigidBody.gravityScale = defaultGravtiy; 
        isControlLocked = false; 
        attackDuration = GetClipLength(myAnimator, "Skull_attacking", attackAnimSpeed);
        if(!playedMaskAnimation && GameManager.Instance.getCurrentLevel() == 1) {
            myAnimator.Play("Skull_mask"); 
            uninterruptableTime = 3f; 
        }
    }
    void FixedUpdate() {
        if(!isDead) {
            // Dash is completely uninterrutable 
            // TODO: Try input.ActionMap.Disable() implementation to limit player control()
            UpdateState(); 
            if(isDashing) {
                Dash(); 
            }
            else {
                if(uninterruptableTime <= 0) {
                    Move();
                    FlipComponents(); 
                    UpdateAnimationState(); 
                }
            }
        }
    }
    void Update()
    {
        if(!isDead) {
            UpdateCoolDowns();
            UpdateSpeed();
        }
    } 
    void UpdateCoolDowns(){
        attackCD -= Time.deltaTime; 
        uninterruptableTime -= Time.deltaTime;
        dashDuration -= Time.deltaTime;
        dashCD -= Time.deltaTime;
    }

    void UpdateSpeed()
    {
        if (movementSpeed > maxVelocity)
        {
            movementSpeed = maxVelocity;
        }
        // switch (PlayerManager.Instance.getOxygenAmount())
        // {
        //     case 0:
        //         {
        //             GameManager.Instance.ProcessPlayerDeath();
        //         }
        //         break;
        //     case 50:
        //         {
        //             movementSpeed = 2f;
        //         }
        //         break;
        //     case 75:
        //         {
        //             movementSpeed = 4f;
        //         }
        //         break;
        //     case 100:
        //         {
        //             movementSpeed = 6f;
        //         }
        //         break;
        //     case 150:
        //         {
        //             movementSpeed = 6f;
        //         }
        //         break;

        // }
    }
// ------------------------------------玩家控制方法 ------------------------------------------
    void OnMove(InputValue val) {
        if(isDead) { return;}
        movementInput = val.Get<Vector2>();
    }
    void OnJump(InputValue val)
    {
        if(isDead) {return;}
        if(isOnGround) {
            // Debug.Log("Jump Succeeded!");
            // Debug.Log(isOnGround);
            Jump();
        }
    }
    void OnAttack(InputValue val) {
        if(attackCD <= 0) {
            attackCD = startAttackCD;
            Attack();
        }
    }
    void OnDash(InputValue val) {
        if(dashCD <= 0) {
            // myRigidBody.gravityScale = 0f;
            dashCD = startDashCD;
            uninterruptableTime = startDashDuration; 
            dashDuration = startDashDuration; 
        }
    }

// -----------------------------------移动方法 ------------------------------------------
    void Jump()
    {
        myRigidBody.velocity = new Vector2(myRigidBody.velocity.x, jumpSpeed);
    }
    void Move()
    {
        myRigidBody.velocity =
                new Vector2(movementInput.x * movementSpeed, 
                            myRigidBody.velocity.y);
        myAnimator.SetBool("isRunning", movementInput.x != 0);
    }
    void Dash() {
        // myRigidBody.gravityScale = 0f;
        //TODO: 氧气消耗
        Vector2 dashVelocity= Vector2.right * dashSpeed * transform.localScale.x;
        myRigidBody.velocity = dashVelocity; 
        myAnimator.Play("Skull_dashing"); 
    }
    void Attack() {
        // 防止取消不可取消的动画
        if(uninterruptableTime > 0) {return;}

        //TODO：Include hit detection in update
        Collider2D[] enemiesToDamage = Physics2D.OverlapAreaAll(attackPosA.position, attackPosB.position, enemyLayerMask); 
        for(int i = 0; i < enemiesToDamage.Length; ++i) {
            enemiesToDamage[i].SendMessageUpwards("TakeDamage", attackDamage);
        }
        myAnimator.Play("Skull_attacking"); 

        // Create uninterruptable duration 
        uninterruptableTime = attackDuration;
 
        //Make a small dash  
        Vector2 forwardForce = Vector2.right * attackMoveSpeed * transform.localScale.x;
        myRigidBody.velocity += forwardForce;
    }
    // ------------------------------------状态器方法 ------------------------------------------
    void UpdateState() {
        CheckIfMoving(); 
        CheckIfOnGround(); 
        CheckIfFalling(); 
        CheckIfDashing();
    }

    void CheckIfDashing()
    {
        if(!isDashing && dashDuration > 0) {
            myRigidBody.gravityScale = 0f; 
            Physics2D.IgnoreLayerCollision(playerLayerIdx, enemyLayerIdx, true);
            isDashing = true; 
        }
        else if(isDashing && dashDuration <= 0) {
            myRigidBody.gravityScale = defaultGravtiy; 
            Physics2D.IgnoreLayerCollision(playerLayerIdx, enemyLayerIdx, false);
            isDashing = false; 
        }
    }

    void CheckIfMoving() {
        isMoving = (myRigidBody.velocity.magnitude >= isMovingThreshold);
    }
    void CheckIfOnGround()
    {
        bool originalIsOnGround = isOnGround; 
        isOnGround = myFootCollider.IsTouchingLayers(LayerMask.GetMask("Platform")); 
        if(myFootCollider.IsTouchingLayers(LayerMask.GetMask("Enemy"))) {
            isOnGround = true; 
        }
        if(!originalIsOnGround && isOnGround) jumpsound.Play();
    }
    void CheckIfFalling()
    {
        isFalling = myRigidBody.velocity.y < isFallingThreshold; 
    }
    void UpdateAnimationState() {
        if(isMoving) {
            if(isOnGround) {
                myAnimator.Play("Skull_walking");
                
            }
            else {
                if(isFalling) {
                    myAnimator.Play("Skull_falling");
                }
                else {
                    myAnimator.Play("Skull_jumping");
                }
            }
        }
        else {
            myAnimator.Play("Skull_idling");
        }
    }
// ------------------------------------BUFF------------------------------------------
    void OnBuff_addOxygen(InputValue val)
    {
        print("Oxygen added:" + PlayerManager.Instance.getOxygenAmount());
        if (!GetComponent<Buff_OxygenRecharge>())
        {
            gameObject.AddComponent<Buff_OxygenRecharge>().SetUp(GetComponent<PlayerMovement>(), 0f).Launch();

        }
    }
    void OnBuff_MoveQuick(InputValue val)
    {
        if (!GetComponent<Buff_MoveSlow>())
        {
            gameObject.AddComponent<Buff_MoveSlow>().SetUp(GetComponent<PlayerMovement>(), 3f).Launch();
        }
    }
// ------------------------------------音频------------------------------------------
    void PlayFootStep() {
        audioFootstep.Play();
    }

    void PlayAttackSound() {
        attackSound.Play(); 
    }

    void PlayJumpSound() {
        jumpsound.Play();
    }

    void PlayLandingSound() {
        landingSound.Play();
    }

    void PlayDashingSound() {
        dashSound.Play();
    }
// ------------------------------------其他------------------------------------------
    // TODO: Move flip components into onMove
    void FlipComponents() {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon; 
        if(playerHasHorizontalSpeed) {
            float playerDir = Mathf.Sign(myRigidBody.velocity.x);
            transform.localScale = new Vector2(playerDir, 1f);
        }
    }
    float GetClipLength(Animator animator, string v, float animSpeed)
    {
        var attackClip = animator.runtimeAnimatorController.animationClips.First(a => a.name == "Skull_attacking");
        return attackClip.length/animSpeed;
    }
    public float getAttackDamage() {
        return attackDamage;
    }
    public float getAttackDuration() {
        return attackDuration;
    }
    public float getMovementSpeed()
    {
        return movementSpeed;
    }
    public void setMovementSpeed(float speed)
    {
        movementSpeed = speed;
    }
    public float getKnockbackForce() {
        return  knockBackForce;
    }
    public float getKnockbackDuration() {
        return  knockBackDuration;
    }

    public 
    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        // float width = Mathf.Abs(attackPosB.position.x - attackPosA.position.x); 
        float width = attackPosB.position.x - attackPosA.position.x; 
        // float height = Mathf.Abs(attackPosB.position.y - attackPosA.position.y); 
        float height = attackPosB.position.y - attackPosA.position.y; 
        var rect = new Rect(attackPosA.position.x, attackPosA.position.y, width, height);
         Gizmos.DrawLine(new Vector3(rect.x, rect.y), new Vector3(rect.x + rect.width, rect.y ));
         Gizmos.DrawLine(new Vector3(rect.x, rect.y), new Vector3(rect.x , rect.y + rect.height));
         Gizmos.DrawLine(new Vector3(rect.x + rect.width, rect.y + rect.height), new Vector3(rect.x + rect.width, rect.y));
         Gizmos.DrawLine(new Vector3(rect.x + rect.width, rect.y + rect.height), new Vector3(rect.x, rect.y + rect.height));
    }
}

