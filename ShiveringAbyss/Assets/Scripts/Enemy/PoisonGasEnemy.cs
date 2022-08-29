using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonGasEnemy : MonoBehaviour
{
    [Header("玩家侦测 (黄色范围）")]
    [SerializeField] float playerDetectionRadius; // 玩家侦测范围
    [SerializeField] Vector3 playerDetectionPosOffset; // 玩家侦测范围偏移 

    [Header("攻击 （红色范围）")]
    [SerializeField] float attackDelay;  // 从怪物侦测到玩家到攻击的时间延迟
    [SerializeField] float attackRadius; // 攻击伤害半径
    [SerializeField] Vector3 attackPosOffset; // 攻击范围偏移
    [SerializeField] float attackDamage; // 攻击每秒伤害
    [SerializeField] float attackDuration; 
    [SerializeField] float startAttackCD; // 攻击CD设置, 攻击状态开始倒计时
    [SerializeField] float attackCD = 0; // 目前攻击CD，少于0 处于可攻击状态
    [SerializeField] LayerMask playerLayerMask; // 玩家图层，用于侦测玩家
    private Enemy enemy; // 敌人类组件，存有敌人存活状态信息
    private Animator myAnimator; 

    public bool isPreparingAttack; // 以侦测到玩家，准备攻击 
    public bool isAttacking; // 处于攻击状态，玩家走进范围会收到伤害

    bool ranDeathSequence = false; //用于确保死亡后事件仅运行一次 
    
    [SerializeField] AudioSource damagedSound; 
    [SerializeField] AudioSource attackingSound;

//------------------------------------------更新函数----------------------------------------------------------------------------
    private void Awake() {
        enemy = GetComponent<Enemy>(); 
        myAnimator = GetComponent<Animator>();
    }

    private void Update() {
        if(enemy.getIsDead() && !ranDeathSequence) {  //死亡事件仅运行一次
            ranDeathSequence = true; 
            myAnimator.Play("PoisonGasEnemy_dying");
        }
        attackCD -= Time.deltaTime; 
    }

    private void FixedUpdate() {
        if(enemy.getIsDead()){return;}

        if(isPreparingAttack) { // 攻击预备状态
            // TODO: 播放准备攻击动画
        }
        else if(isAttacking) { // 攻击状态 
            // TODO: 播放攻击动画
            Attack(); // 侦测范围内的玩家，并对其造成伤害
        }
        else { // 闲置状态
            // TODO: 播放闲置动画
            DetectPlayer(); 
        }
    }
//------------------------------------------范围判定函数----------------------------------------------------------------------------
    private void Attack(){ //判定攻击范围内有没有玩家
        Collider2D playerColliderInRadius = Physics2D.OverlapCircle(transform.position + attackPosOffset, attackRadius, playerLayerMask); 
        if(playerColliderInRadius != null) { //范围内没有玩家碰撞体会返还null
            PlayerManager.Instance.TakeDamage(attackDamage * Time.deltaTime); // 伤害量 = 每秒伤害*时间
        }
    }


    void DetectPlayer() { // 寻找侦察范围内的玩家
        // 以怪物为中心，检测侦察范围 playerDetectionRadius 内的玩家碰撞体
        Collider2D playerColliderInRadius = Physics2D.OverlapCircle(transform.position + playerDetectionPosOffset, playerDetectionRadius, playerLayerMask); 
        if(playerColliderInRadius != null) { // 如范围内有玩家，
            if(attackCD <= 0) { // 并且攻击CD结束
                SetPreparingState(); 
            } 
        }
    }

//------------------------------------------状态设置函数----------------------------------------------------------------------------
    void SetPreparingState(){
        myAnimator.Play("PoisonGasEnemy_attacking");
        isPreparingAttack = true; 
        Invoke("SetAttackingState", attackDelay); // 进入攻击预备状态后，倒计时invoke 攻击状态
        // Debug.Log("Preparing to attack!");
    }
    void ResetPreparingState(){
        isPreparingAttack = false; 
    }
    void SetAttackingState(){
        if (enemy.getIsDead()){return;} //如果怪物在攻击准备过程中死亡，取消攻击

        attackingSound.Play(); 
        myAnimator.Play("PoisonGasEnemy_attacking2");
        attackCD = startAttackCD; // 开始攻击，攻击CD初始化
        // Debug.Log("Attacking!");
        ResetPreparingState(); // 关闭攻击预备状态
        isAttacking = true; 
        Invoke("ResetAttackingState", attackDuration); // 设定攻击持续时间，倒计时invoke 结束攻击
    }
    void ResetAttackingState(){
        attackingSound.Stop();
        myAnimator.Play("PoisonGasEnemy_idling");
        isAttacking = false; 
    }

//------------------------------------------其他函数----------------------------------------------------------------------------
    private void OnDrawGizmos() { // 在Unity 编辑窗中绘攻击范围
        Gizmos.color = Color.red; 
        Gizmos.DrawWireSphere(transform.position + attackPosOffset, attackRadius); // 攻击范围
        
        Gizmos.color = Color.yellow; 
        Gizmos.DrawWireSphere(transform.position + playerDetectionPosOffset, playerDetectionRadius); // 攻击范围
    }
//------------------------------------------音频----------------------------------------------------------------------------
    void PlayAttackingSound() {
        attackingSound.Play(); 
    }
}
 