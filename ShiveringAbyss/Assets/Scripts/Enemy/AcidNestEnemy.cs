using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidNestEnemy : MonoBehaviour
{
    [SerializeField] Vector3 explosionOffset; // 怪物爆炸偏离
    [SerializeField] float explosionDelay;  // 怪物死亡后爆炸延迟
    [SerializeField] float explosionRadius; // 爆炸伤害半径
    [SerializeField] float explosionDamage; // 爆炸伤害
    [SerializeField] LayerMask playerLayerMask; // 玩家图层，用于爆炸半径内过滤出玩家碰撞体，判定玩家是否在爆炸半径内

    [Header("Sound Effect")]
    [SerializeField] AudioSource breakingSound; 
    [SerializeField] AudioSource explodingSound; 
    private Enemy enemy; // 敌人类组件，存有敌人存活状态信息
    private Animator myAnimator; 

    bool ranDeathSequence = false; //用于确保死亡后事件仅运行一次 
    private void Awake() {
        enemy = GetComponent<Enemy>(); 
        myAnimator = GetComponent<Animator>();
    }

    private void Update() {
        if(enemy.getIsDead() && !ranDeathSequence) {  //死亡事件仅运行一次
            ranDeathSequence = true; 
            myAnimator.Play("AcidNest_dying"); 
            Invoke("Explode", explosionDelay); // 爆炸造成伤害
        }
    }

    private void Explode(){
        // 获取爆炸半径（explosion radius） 内所有属 Player 图层的碰撞体 
        myAnimator.Play("AcidNest_exploding");
        Collider2D[] collidersInExplosionRadius = Physics2D.OverlapCircleAll(transform.position + explosionOffset, explosionRadius, playerLayerMask);
        if(collidersInExplosionRadius.Length > 0) { //如有碰撞体， 说明玩家在爆炸范围内
            PlayerManager.Instance.TakeDamage(explosionDamage);
        }
    }

    private void OnDrawGizmos() { // 在Unity 编辑窗中绘制爆炸范围
        Gizmos.color = Color.red; 
        Gizmos.DrawWireSphere(transform.position + explosionOffset, explosionRadius);
    }

    void PlayBreakingSound() {
        breakingSound.Play();
    }
    void PlayExplodingSound() {
        explodingSound.Play();
    }
}
