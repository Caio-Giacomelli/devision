using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    public void LoadScene(string scene){
        SceneManager.LoadScene(scene);
    }

    public void LoadPreviousScene(){
        string sceneName = PlayerPrefs.GetString("PreviousScene");
        SceneManager.LoadScene(sceneName);
    } 

    public void Quit(){
        Application.Quit();
    }

    public void PauseUnpause(){
        Sprite play_sprite = Resources.Load<Sprite>("Sprites/Play");
        Sprite pause_sprite = Resources.Load<Sprite>("Sprites/Pause");
        
        if (Time.timeScale == 0){
            Time.timeScale = 1;

            AudioSource[] audios = FindObjectsOfType<AudioSource>();
            
            foreach(AudioSource a in audios){
                a.Play(); 
            }
            
            Button mybutton = GameObject.Find("Pause Button").GetComponent<Button>();
            mybutton.image.sprite = pause_sprite;
        }
        else {
            Time.timeScale = 0;

            AudioSource[] audios = FindObjectsOfType<AudioSource>();
            
            foreach(AudioSource a in audios){
                a.Pause(); 
            }
            Button mybutton = GameObject.Find("Pause Button").GetComponent<Button>();
            mybutton.image.sprite = play_sprite;
        }     
    }
}
