using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour{
    public void LoadScene(string scene){
        Time.timeScale = 1;
        SceneManager.LoadScene(scene);
    }

    public void LoadPlayableScene(){
        Time.timeScale = 1;
        SceneManager.LoadScene("Gameplay");
    }

    public void SelectNextScene(string level_name){
        PlayerPrefs.SetString("NextScene", level_name);
    } 

    public void Pause(GameObject canvas){
        GameObject gm = GameObject.Find("GameManager");
        if (!gm.GetComponent<GameManager>()._isPaused){            
            Time.timeScale = 0;

            AudioSource[] audios = FindObjectsOfType<AudioSource>();       
            foreach(AudioSource a in audios){
                a.Pause(); 
            }

            canvas.SetActive(true);
            gm.GetComponent<GameManager>()._isPaused = true;
        }     
    }

    public void Quit(){
        Application.Quit();
    }
}