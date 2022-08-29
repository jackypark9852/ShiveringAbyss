using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkNestEnemy : MonoBehaviour
{
    private Enemy enemy; 
    private Animator myAnimator; 

    bool deathAnimationPlayed = false; 

    [SerializeField] AudioSource breakingSound; 
    private void Awake() {
        enemy = GetComponent<Enemy>(); 
        myAnimator = GetComponent<Animator>();
    }

    private void Update() {
        if(enemy.getIsDead() && !deathAnimationPlayed) { // 如果怪物已经死亡，播放动画
            deathAnimationPlayed = true; 
            myAnimator.Play("DarkNest_dying"); 
        }
    }

    void PlayBreakingSound() {
        breakingSound.Play(); 
    }
}
