using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BF : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadScene(string scene){
        SceneManager.LoadScene(scene);
    } 

    public void Quit(){
        Application.Quit();
    }

    public void PauseUnpause(){
        Sprite playSprite = Resources.Load<Sprite>("Sprites/Play");
        Sprite pauseSprite = Resources.Load<Sprite>("Sprites/Pause");
        
        if (Time.timeScale == 0){
            Time.timeScale = 1;

            AudioSource[] audios = FindObjectsOfType<AudioSource>();
            
            foreach(AudioSource a in audios){
                a.Play(); 
            }
            

            Button mybutton = GameObject.Find("Pause Button").GetComponent<Button>();
            mybutton.image.sprite = pauseSprite;
        }
        else {
            Time.timeScale = 0;

            AudioSource[] audios = FindObjectsOfType<AudioSource>();
            
            foreach(AudioSource a in audios){
                a.Pause(); 
            }
            Button mybutton = GameObject.Find("Pause Button").GetComponent<Button>();
            mybutton.image.sprite = playSprite;
        }     
    }
}
