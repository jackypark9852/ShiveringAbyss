using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour
{
    protected bool isDead = false; 
    protected ParticleSystem deathEffect; 
    protected Rigidbody2D myRigidBody; 
    Vector2 deathKick = new Vector2(20f, 20f);
    public virtual void Awake() {
        deathEffect = GetComponentInChildren<ParticleSystem>();
        myRigidBody = GetComponent<Rigidbody2D>(); 
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public virtual void Die(bool kick = true) {

    }

    protected void ClampVelocity(float maxVelocity) {
        myRigidBody.velocity = Vector2.ClampMagnitude(myRigidBody.velocity, maxVelocity);
    }
}
