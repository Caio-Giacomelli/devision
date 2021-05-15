using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    public void LoadScene(string scene){
        Time.timeScale = 1;
        SceneManager.LoadScene(scene);
    }

    public void LoadPreviousScene(){
        Time.timeScale = 1;
        string sceneName = PlayerPrefs.GetString("PreviousScene");
        SceneManager.LoadScene(sceneName);
    } 

    public void Quit(){
        Application.Quit();
    }

    public void Pause(GameObject canvas){
        Time.timeScale = 0;

        AudioSource[] audios = FindObjectsOfType<AudioSource>();       
        foreach(AudioSource a in audios){
            a.Pause(); 
        }

        canvas.SetActive(true);
    }
}