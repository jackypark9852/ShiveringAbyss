using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class WormEnemy : Enemy
{
    // 怪物在巡逻状态下会利用 waypoints 设置行走目标地点
    // 可以在Unity 引擎中选择游戏对象作为 waypoint
    // 素材中有 waypoint prefab，可用作为定义 waypoint 的游戏对象
    

    [Header("移动AI")]
    [SerializeField] Transform moveDestination; 

    [Header("巡逻AI")]
    [SerializeField] Transform waypoint1; // 巡逻点1
    [SerializeField] Transform waypoint2; // 巡逻点2
    [SerializeField] float waypointReachThreshold; // 判定怪物到达 waypoint
    [SerializeField] float newPatrolDestinationDelay; // 幼虫到达目的地后前往新的目标点之前的延迟时间

    [Header("追杀范围")]
    [SerializeField] Transform chaseRadiusCenter; //追杀范围中心点
    [SerializeField] float chaseRadius; //追杀范围半径


    [SerializeField] float knockBackHeightMultiplier; // 击退效果的垂直受力参数； 参数越高，怪物就飞的越高
    

    protected bool isMovementDisabled; // 数值为真实时，怪物无法移动

    bool isHostile = false; //是否处于追杀状态

    protected override void Start() {
        base.Start(); 
        moveDestination.position = GetNewPatrolDestination(); //指定初始行走目标地点
    }

    protected override void Update() {
        base.Update(); // Checks for death

        if(!isMovementDisabled && !isDead) {
            if(isHostile) { //如果处于追杀状态
                moveDestination.position = playerMovement.transform.position; 

                if(Vector2.Distance(moveDestination.position, chaseRadiusCenter.position) > chaseRadius) {
                    isHostile = false; 
                }
            }
            else { //如果处于正常巡逻状态
                // 如果到达行走目标地点
                if(Mathf.Abs(transform.position.x - moveDestination.position.x) < waypointReachThreshold) {
                    //停止两秒
                    DisableMovement(newPatrolDestinationDelay);
                    //指定新的目标定点
                    moveDestination.position = GetNewPatrolDestination(); 
                    return; //提前返回终结Update(), 下一帧怪物再进行移动
                }
            }
            FlipComponents(); //根据移动方向调整怪物面向 
        }

        if(isDead) {
            myAnimator.Play("Worm_dying");
        }
    }

    private void FixedUpdate() {
        if(!isMovementDisabled && !isDead) { 
            Move(); // 向 moveDestination 移动
        }
    }
    void FlipComponents() {
        bool enemyHasHorizontalSpeed = Mathf.Abs(myRigidBody2D.velocity.x) > Mathf.Epsilon;  //判定怪物是否在横向移动
        if(enemyHasHorizontalSpeed) {
            float enemyDir = Mathf.Sign(myRigidBody2D.velocity.x); //根据敌人的速度向量判定横向运动方向
            transform.localScale = new Vector2(-enemyDir, 1f); // localScale.x = 1 ：贴图初始方向 localScale.x = -1 ：贴图左右镜像翻面
        }
    }

    private Vector2 GetNewPatrolDestination()
    {
        // 在两个waypoint 之间寻找一个与怪物同样y值的点作为新目标地点
        Vector2 newDest = new Vector2(UnityEngine.Random.Range(waypoint1.position.x, waypoint2.position.x), transform.position.y);
        // Debug.Log(newDest);
        return newDest;
    }

    protected override void Move() { // 向目标点 moveDestination 移动
        myAnimator.Play("Worm_walking");
        float dir = Mathf.Sign(moveDestination.position.x - transform.position.x);  
        myRigidBody2D.velocity = new Vector3(moveSpeed * dir, myRigidBody2D.velocity.y, 0);
    }

    public override void TakeDamage(float damage) {
        base.TakeDamage(damage);
        KnockBack(playerMovement.getKnockbackForce(), playerMovement.transform); // 从玩家 playerMovement脚本中得到击退强度， 位置信息
        isHostile = true; //受伤
    }

    protected virtual void KnockBack(float knockBackForce, Transform damageSourcePosition) { // 击退强度和伤害来源位置；决定击退距离和方向
        myRigidBody2D.velocity = Vector2.zero;
        float dir = Mathf.Sign(transform.position.x - damageSourcePosition.position.x); 
        DisableMovement(playerMovement.getKnockbackDuration()); // 怪物在击退状态下受控制，无法移动
        myRigidBody2D.AddForce((Vector2.right *dir + Vector2.up*2) *knockBackForce);
    }

    protected void DisableMovement(float duration){ // 怪物处于控制状态下， 无法移动
        myAnimator.Play("Worm_idling");
        isMovementDisabled = true; 
        Invoke("EnableMovement", duration); // 计时解除控制
    }
    protected void EnableMovement(){ 
        isMovementDisabled = false; 
    }

    protected void OnDrawGizmos() { 
        Gizmos.color = Color.blue; // 绘制源泉提示怪物到达waypoint的判定范围
        Gizmos.DrawWireSphere(transform.position, waypointReachThreshold);

        Gizmos.color = Color.green; // 绘制幼虫行走目标地点
        Gizmos.DrawWireSphere(moveDestination.position, .3f); 

        Gizmos.color = Color.red; //绘制追杀范围
        Gizmos.DrawWireSphere(chaseRadiusCenter.position, chaseRadius); 
    }
} 
