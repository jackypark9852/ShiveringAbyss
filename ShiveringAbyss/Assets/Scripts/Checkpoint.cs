using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    bool isActivated = false; // 是否玩家已经到达重生点
    private void OnTriggerEnter2D(Collider2D other) {
        if(isActivated == true) {return;}
        // Debug.Log("Checkpoint reached！");
        if(other.gameObject.tag == "Player") {
            isActivated = true; 
            GameManager.Instance.ProcessReachedCheckpoint(transform);
        }
    }
}
