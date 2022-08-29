using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageExit : MonoBehaviour
{
    bool isCompleted = false;

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player" && !isCompleted) {
            isCompleted = true;
            GameManager.Instance.ProcessLevelComplete(); 
        }
    }
}
