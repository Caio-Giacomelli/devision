using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

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

    public void LoadPlayableScene(string difficulty){
        string scene_name = PlayerPrefs.GetString("NextScene") + " " + difficulty;
        SceneManager.LoadScene(scene_name);
    }

    public void SelectNextScene(string level_name){
        PlayerPrefs.SetString("NextScene", level_name);
    } 

    public void Quit(){
        Application.Quit();
    }

    public void Pause(GameObject canvas){
        GameObject gm = GameObject.Find("GameManager");
        if (!gm.GetComponent<GameManager>().is_paused){            
            Time.timeScale = 0;

            AudioSource[] audios = FindObjectsOfType<AudioSource>();       
            foreach(AudioSource a in audios){
                a.Pause(); 
            }

            canvas.SetActive(true);
            gm.GetComponent<GameManager>().is_paused = true;
        }     
    }
}