using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour 
{
    public static GameManager Instance;

    [Header("Level Indexes")]
    [SerializeField] int currentLevelIdx;
    [SerializeField] int lastLevelIdx = 3; 
    [SerializeField] int endScreenIndex = 4; 
    [SerializeField] HealthBarBehavior healthBar; 

    [Header("玩家重生点")]
    [SerializeField] float playerFallDamage; // 玩家掉落地图时受的伤害

    Transform currentCheckpoint; // 目前玩家到达的重生点 

    void Awake() {
        if(Instance == null) {
            Instance = this; 
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        ResetSessionData();
        UpdateHUD();
    }

    void Update(){}

    private void UpdateHUD(){}

    public void ProcessLevelComplete() {
        ++currentLevelIdx;
        loadNextLevel();
    }
    
    private void loadNextLevel(){
        // 如果卡编号在有效范围内，载入关卡
        if(currentLevelIdx <= lastLevelIdx){
            SceneManager.LoadSceneAsync(currentLevelIdx); 
        } 
        // 关卡编号大于最后一关，判定玩家成功通关,重置GameManager
        else {
            AudioManager.Instance.SetEndScreenMusic();
            SceneManager.LoadSceneAsync(endScreenIndex);
            RestartGameSession(); 
        }
    }
    public void ProcessPlayerDamage() {
        healthBar.setHealth(PlayerManager.Instance.getOxygenAmount(), PlayerManager.Instance.getMaxOxygenAmount());
    }
    public void ProcessPlayerDeath(){
        AudioManager.Instance.SetMainMenuMusic();
        SceneManager.LoadScene(0); // 跳转至主界面
        RestartGameSession(); 
    }
    public void ProcessReachedCheckpoint(Transform checkpoint) { // 玩家到达重生点
        currentCheckpoint = checkpoint; // 更新玩家重生点
        Debug.Log("Checkpoint processed: " + currentCheckpoint.position);
    }

    public void ProcessPlayerFall() {
        PlayerMovement playerMovement =  FindObjectOfType<PlayerMovement>(); // 在场景中寻找玩家控制器
        playerMovement.transform.position = currentCheckpoint.position; // 将玩家传送到当前重生点
        PlayerManager.Instance.TakeDamage(playerFallDamage); // 玩家受跌落伤害
    }
    public void RestartGameSession(){ // 完全重置游戏状态，游戏再开始时重新生成新的GameManager
        Instance = null; 
        Destroy(gameObject);
    }
    private void ResetSessionData(){}

    public int getCurrentLevel() {
        return currentLevelIdx; 
    }
}
