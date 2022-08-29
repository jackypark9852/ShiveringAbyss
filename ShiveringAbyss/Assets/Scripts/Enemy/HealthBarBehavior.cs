using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class HealthBarBehavior : MonoBehaviour
{ 
    [SerializeField] Image healthImage; 
    [SerializeField] Image EffectImage;

    float health; 
    float maxHealth; 

    [SerializeField] float hurtSpeed = 0.005f; 

    void Start()
    {
        health = 1; 
        maxHealth = 1; 
    }

    private void Update() {
        healthImage.fillAmount = health/ maxHealth;

        if(EffectImage.fillAmount > healthImage.fillAmount) { 
            EffectImage.fillAmount -= hurtSpeed; 
        } 
        else {
            EffectImage.fillAmount = healthImage.fillAmount;  
        }
    }   
    public void setHealth(float health, float maxHealth) {
        // Debug.Log("Health set:" + health + ", " + maxHealth);
        this.health = health; 
        this.maxHealth = maxHealth;
    }
}
