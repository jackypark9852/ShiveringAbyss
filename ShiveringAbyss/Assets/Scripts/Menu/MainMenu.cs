    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    int firstLevelBuildIndex = 1; 
    private void Start() {
        AudioManager.Instance.SetMainMenuMusic(); // ???????
    }
    public void PlayGame() {
        print("PlayGame£¡£¡£¡£¡");
        SceneManager.LoadSceneAsync(firstLevelBuildIndex); 
        AudioManager.Instance.SetInGameMusic(); // ??????????
    }

    public void QuitGame(){
        print("QuitGame£¡£¡£¡£¡");
        Application.Quit();
    }
}
