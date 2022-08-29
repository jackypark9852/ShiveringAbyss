using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolEnemy : Enemy
{
    // 怪物在巡逻状态下会利用 waypoints 设置行走目标地点
    // 可以在Unity 引擎中选择游戏对象作为 waypoint
    // 素材中有 waypoint prefab，可用作为定义 waypoint 的游戏对象
    [SerializeField] Transform waypoint1; 
    [SerializeField] Transform waypoint2; 


    [SerializeField] float waypointReachThreshold; // 判定怪物到达 waypoint  
    [SerializeField] float knockBackHeightMultiplier; // 击退效果的垂直受力参数； 参数越高，怪物就飞的越高

    protected bool isMovementDisabled; // 数值为真实时，怪物无法移动
    protected override void Start() {
        base.Start(); 
        moveTarget = waypoint1; 
    }

    protected override void Update() {
        base.Update(); // Checks for death

        if(!isMovementDisabled) {
            // Switch patrol destination once reached
            if(Vector2.Distance(transform.position, waypoint1.position) < waypointReachThreshold) {
                moveTarget = waypoint2; 
            }
            else if(Vector2.Distance(transform.position, waypoint2.position) < waypointReachThreshold) {
                moveTarget = waypoint1; 
            }
            Move();
        }
    }
    protected override void Move() { // 向目标点 moveTarget 移动
        float dir = Mathf.Sign(moveTarget.position.x - transform.position.x);  
        myRigidBody2D.velocity = new Vector3(moveSpeed * dir, myRigidBody2D.velocity.y, 0);
    }

    public override void TakeDamage(float damage) {
        base.TakeDamage(damage);
        KnockBack(playerMovement.getKnockbackForce(), playerMovement.transform); // 从玩家 playerMovement脚本中得到击退强度， 位置信息
    }

    protected virtual void KnockBack(float knockBackForce, Transform damageSourcePosition) { // 击退强度和伤害来源位置；决定击退距离和方向
        myRigidBody2D.velocity = Vector2.zero;
        float dir = Mathf.Sign(transform.position.x - damageSourcePosition.position.x); 
        DisableMovement(playerMovement.getKnockbackDuration()); // 怪物在击退状态下受控制，无法移动
        myRigidBody2D.AddForce((Vector2.right *dir + Vector2.up*2) *knockBackForce);
    }

    protected void DisableMovement(float duration){ // 怪物处于控制状态下， 无法移动
        isMovementDisabled = true; 
        Invoke("EnableMovement", duration); // 计时解除控制
    }

    protected void EnableMovement(){ 
        isMovementDisabled = false; 
    }

    protected void OnDrawGizmos() { // 绘制源泉提示怪物到达waypoint的判定范围
        Gizmos.color = Color.blue; 
        Gizmos.DrawWireSphere(transform.position, waypointReachThreshold);
    }
} 
