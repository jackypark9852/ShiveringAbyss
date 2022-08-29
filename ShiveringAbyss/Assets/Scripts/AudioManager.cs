using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource MainMenuBGM;
    [SerializeField] AudioSource InGameBGM;
    [SerializeField] AudioSource EndScreenBGM;
    public static AudioManager Instance; 
    private void Awake() {
        if(Instance == null) {
            Instance = this; 
            DontDestroyOnLoad(gameObject); 
        }
        else {
            Destroy(gameObject); 
        }
    }

    public void SetMainMenuMusic() {
        MainMenuBGM.Play(); 
        InGameBGM.Stop();
        EndScreenBGM.Stop(); 
    }
    public void SetInGameMusic() {
        MainMenuBGM.Stop(); 
        InGameBGM.Play();
        EndScreenBGM.Stop(); 
    }
    public void SetEndScreenMusic() {
        MainMenuBGM.Stop(); 
        InGameBGM.Stop();
        EndScreenBGM.Play(); 
    }

}
