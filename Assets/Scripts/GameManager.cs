using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private HealthBar healthBar;

    [SerializeField]
    private GameObject countdown_animation;

    [SerializeField]
    private int score_per_note = 10;

    [SerializeField]
    private float life_per_second = 0.3f;

    [SerializeField]
    public float note_speed = 4.5f;

    [SerializeField]
    public AudioMixer mixer;

    public bool createMode;

    private int multiplier = 1;
    private int streak = 0;
    private float health;

    void Start(){
        health = 1f;

        InvokeRepeating("SetHealthBarSize", 0.0f, life_per_second);
        HandlePlayerPref();
        HandleMusicVolume();
    }

    void Update(){
        if (health == 0f && !createMode){
            Lose();
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (!createMode) Destroy(other.gameObject);
        LoseHealth();
        ResetStreak(); 
    }

    public void AddStreak(){
        streak++;

        if(streak >= 24){
            multiplier = 4;        
        } 
        else if (streak >= 16){
            multiplier = 3;
        }
        else if (streak >= 8){
            multiplier = 2;
        } else {
            multiplier = 1;
        }

        AddHealth();
        UpdateGUI();
    }

    public void ResetStreak(){
        streak = 0;
        multiplier = 1;
        LoseHealth();
        UpdateGUI();
    }

    void UpdateGUI(){
        PlayerPrefs.SetInt("Mult", multiplier);
    }

    public int GetScore(){
        return score_per_note * multiplier;
    }

    private void SetHealthBarSize(){
        health -= .01f;
        health = Mathf.Clamp(health, 0f, 1f);
        healthBar.SetSize(health);
    }

    public void Win(){
        SceneManager.LoadScene("Win Screen");
    }

    public void Lose(){
        SceneManager.LoadScene("Lose Screen");
    }

    private void AddHealth(){
        health += 0.1f * multiplier;
        health = Mathf.Clamp(health, 0f, 1f);
        healthBar.SetSize(health);
    }

    private void LoseHealth(){
        health -= .05f;
        health = Mathf.Clamp(health, 0f, 1f);
        healthBar.SetSize(health);
    }

    private void HandlePlayerPref(){
        PlayerPrefs.SetInt("Score", 0);
        PlayerPrefs.SetInt("Mult", 1);
        PlayerPrefs.SetString("PreviousScene", SceneManager.GetActiveScene().name);
    }

    private void HandleMusicVolume(){
        float slider_value = PlayerPrefs.GetFloat("musicSlider");
        float musicVol = Mathf.Log10(slider_value) * 20;
        mixer.SetFloat("MusicVol", musicVol);
    }

    public void Resume(GameObject canvas){
        canvas.SetActive(false);
        countdown_animation.SetActive(true);
        StartCoroutine("ResumeWithDelay");
    }

    IEnumerator ResumeWithDelay(){
        float pause_time = Time.realtimeSinceStartup + 3.2f;
        while (Time.realtimeSinceStartup < pause_time){
            yield return 0;
        }

        countdown_animation.SetActive(false);
              
        Time.timeScale = 1;

        AudioSource[] audios = FindObjectsOfType<AudioSource>();       
        foreach(AudioSource a in audios){
            a.Play(); 
        }
    }
}
