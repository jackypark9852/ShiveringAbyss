using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class EndMenu : MonoBehaviour
{
    [SerializeField] int mainMenuBuildIdx;
    public void ReturnToMainMenu() {
        SceneManager.LoadScene(mainMenuBuildIdx); 
    }

    public void QuitGame() {
        Application.Quit();
    }
}
