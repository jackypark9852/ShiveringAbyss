using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public  class Enemy : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] protected string EnemyName = "Enemy";
    [SerializeField] protected float maxHealth; // 最大生命值
    [SerializeField] protected float health; // 当前生命值
    [SerializeField] protected float moveSpeed; //移动速度
    [SerializeField] protected float contactDamage; // 对玩家的触碰伤害
    float originalContactDamage; 
    [SerializeField] protected float attackDamage; // 另外攻击伤害
    [SerializeField] protected float oxygenDropAmount; // 氧气掉落量
    
    
    [Header("HUD")]
    [SerializeField] HealthBarBehavior healthBar; // 怪物头上UI 游戏对象的应用，之后用于怪物收到伤害时更新血条UI 

    [Header("Damaged Effect")] // 收到伤害暂时变成红色， 
    [SerializeField] float flashDuration; // 变色时长 : 怪物收到伤害后变成红色的时间
    [SerializeField] float contactDamageDisableDuration; // 控制时长 ： 怪物收到伤害后被短暂控制

    [Header("Death Effect")]
    [SerializeField] float deathDeactivateDelay; // 延迟时间长度 ——— 定义怪物在死亡后等待多久消失
    Color originalColor; // 用余怪物受伤变色后，重新返回原来颜色时用

    [SerializeField] AudioSource gotHitSound; 

    protected Transform moveTarget; // Move() 会让怪物往这个地点爬行移动

    // 怪物本体组件引用，因为继承此类的子类怪物有可能添加更多碰撞体，所以预先定义备用
    protected Rigidbody2D  myRigidBody2D; 
    protected CapsuleCollider2D myCapsuleCollider2D; 
    protected BoxCollider2D myBoxCollider2D; 
    protected CircleCollider2D myCircleCollider2D; 
    protected Animator myAnimator; 
    protected SpriteRenderer mySpriteRenderer; 

    // 用于击退方法（Knockback()）时利用玩家位置判定击退方向 
    protected PlayerMovement playerMovement; 

    // 怪物状态：
    protected bool isDead = false; //  怪物是否已死亡
    
    protected virtual void Awake() {
        myRigidBody2D = GetComponentInChildren<Rigidbody2D>();
        myCapsuleCollider2D = GetComponentInChildren<CapsuleCollider2D>();
        myBoxCollider2D = GetComponentInChildren<BoxCollider2D>();
        myCircleCollider2D = GetComponentInChildren<CircleCollider2D>();
        myAnimator = GetComponentInChildren<Animator>();
        mySpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        playerMovement = FindObjectOfType<PlayerMovement>();
    }
    protected virtual void Start() {
        health = maxHealth; 
        healthBar.setHealth(maxHealth, maxHealth);

        // 记录怪物初始数值
        originalColor = mySpriteRenderer.color;
        originalContactDamage = contactDamage;
    }
    //血量为零一下判定为死亡
    protected virtual void Update() {
        if(!isDead) {
            if(health <= 0) Die();
        }
    }

    protected virtual void Attack(){} // 执行怪物攻击的函数，在子类中重写
    protected virtual void Move(){} // 执行怪物移动的函数，在子类中重写
    protected virtual void Die()
    { // 执行怪物死亡程序
        // Debug.Log("[" + EnemyName + "] died!");
        isDead = true;
        GiveOxygenToPlayer(); // 将掉落的氧气值加给玩家
        healthBar.gameObject.SetActive(false); // 隐藏怪物血条

        myRigidBody2D.isKinematic = true;  //将可能有的碰撞体改为trigger 不在与玩家碰撞
        if(myCapsuleCollider2D != null) myCapsuleCollider2D.isTrigger = true; 
        if(myBoxCollider2D != null) myBoxCollider2D.isTrigger = true; 
        if(myCircleCollider2D != null) myCircleCollider2D.isTrigger = true;  

        // myRigidBody2D.isKinematic = true; //死亡后不再与玩家有物理碰撞

        Invoke("DeactivateSelf", deathDeactivateDelay);
        Debug.Log("Enemy Die() Called");  
    }

    private void DeactivateSelf()
    {
        gameObject.SetActive(false); // 移除怪物（进入 Deactivated 状态）
    }

    // 伤害源可以呼叫方法，对怪物造成伤害
    public virtual void TakeDamage(float damage){
        if(isDead) {return;}

        gotHitSound.Play();
        Debug.Log("Took " + damage + " damage.");
        health -= damage; 
        healthBar.setHealth(health, maxHealth);

        FlashColor(); // 怪物受伤短暂变成红色，给予玩家视觉反馈
        PauseContactDamge(); // 暂时取消怪物碰撞伤害，直到ResetContactDamage()恢复此能力
    }

    void PauseContactDamge() {
        contactDamage = 0f; 
        Invoke("ResetContactDamage", contactDamageDisableDuration); //计时结束恢复怪物碰撞伤害
    }

    void ResetContactDamage() { //恢复碰撞伤害
        contactDamage = originalContactDamage;
    }
    void PlayHitSound() {
        gotHitSound.Play();
    }
    void FlashColor() {
        Debug.Log("Flashed!");
        mySpriteRenderer.color = Color.red;  // 通过贴图渲染组件将怪物渲染成红色
        Invoke("ResetColor", flashDuration); // 计时结束 Invoke 恢复初始颜色
    }

    void ResetColor() { // 恢复怪物贴图初始颜色
        mySpriteRenderer.color = originalColor; 
    }

    // 与玩家碰撞时对玩家造成伤害
    protected virtual void OnCollisionEnter2D(Collision2D other) { 
        if(isDead) {return;} //怪物在死亡状态下无法伤害玩家

        if(other.gameObject.tag == "Player") { // 如碰撞体判定为“玩家”
            PlayerManager.Instance.TakeDamage(contactDamage); // 玩家扣除等于 contactDamage 的氧气量
            Debug.Log("Dealt " + contactDamage + " Damage!");
        }
    }
    protected virtual void GiveOxygenToPlayer() { // 给予玩家等于 oxygenDropAmount 的氧气量
        PlayerManager.Instance.AddOxygen(oxygenDropAmount);
    }

    public bool getIsDead() {
        return isDead; 
    }
}
