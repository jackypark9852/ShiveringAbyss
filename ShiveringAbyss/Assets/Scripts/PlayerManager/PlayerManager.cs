using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerManager : MonoBehaviour
{
    public static  PlayerManager Instance;


    [Header("Oxygen")]
    [SerializeField] float originOxygenAmount = 100f;
    [SerializeField] float currentOxygenAmount = 100f;
    [SerializeField] float maxOxygenNormal = 100f;
    [SerializeField] float maxOxygenUpgrated = 200f;

    //public float oxygenLevel;
    float totalTime;
    bool isDead = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        if(isDead) {return;}
        
        if(currentOxygenAmount > maxOxygenNormal)
        {
            currentOxygenAmount = maxOxygenNormal;
        }else if(currentOxygenAmount < 0)
        {
            isDead = true; 
            Die(); 
        }
        totalTime += Time.deltaTime;
        if(totalTime >= 1)
        {
            TakeDamage(2);//����ÿ����ʧ2
            totalTime = 0;
        }

        
    }
    public void TakeDamage(float DamageAmount)
    {
        // Debug.Log("Player took [" + DamageAmount + "] damage");
        currentOxygenAmount -= DamageAmount;
        GameManager.Instance.ProcessPlayerDamage();
    }
    public void AddOxygen(float AddOxygenAmount)
    {
        // Debug.Log("Player gained [" + AddOxygenAmount + "] oxygen");
        currentOxygenAmount += AddOxygenAmount; 
    }
    public void ResetPlayer()
    {
        currentOxygenAmount = originOxygenAmount;
    }

    /*public void ActivateBuff(int buffID) 
    {
        buffs[buffID].Activate();
    }*/
    
    private void Die() {
        isDead = true; 
        GameManager.Instance.ProcessPlayerDeath();
    }
    public float getOxygenAmount()//��gameManager��������ʾѪ����
    {
        return currentOxygenAmount;
    }

    public float getMaxOxygenAmount() {
        return maxOxygenNormal;
    }
}



