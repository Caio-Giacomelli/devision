using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    [SerializeField] private HealthBar healthBar;
    public bool createMode;
    public int score_per_note = 10;
    int multiplier = 1;
    int streak = 0;
    float health;
    public float life_per_second;
    public AudioMixer mixer;

    // Start is called before the first frame update
    void Start()
    {
        health = 1f;
        PlayerPrefs.SetInt("Score", 0);
        PlayerPrefs.SetInt("Mult", 1);
        PlayerPrefs.SetString("PreviousScene", SceneManager.GetActiveScene().name);
        InvokeRepeating("SetHealthBarSize", 0.0f, 0.3f);

        float sliderValue = PlayerPrefs.GetFloat("musicSlider");
        float musicVol = Mathf.Log10(sliderValue) * 20;
        mixer.SetFloat("MusicVol", musicVol);
    }

    // Update is called once per frame
    void Update()
    {
        if (health == 0f && !createMode){
            Lose();
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (!createMode) Destroy(other.gameObject);
        health -= .05f;
        health = Mathf.Clamp(health, 0f, 1f);
        healthBar.SetSize(health);

        ResetStreak(); 
    }

    public void AddStreak(){
        streak++;
        //Debug.Log("### Streak: " + streak);
        //Debug.Log("### Multiplier: " + multiplier);
        if(streak >= 24){
            multiplier = 4;
            health += 0.1f * multiplier;
            health = Mathf.Clamp(health, 0f, 1f);
            healthBar.SetSize(health);
        } 
        else if (streak >= 16){
            multiplier = 3;
            health += 0.1f * multiplier;
            health = Mathf.Clamp(health, 0f, 1f);
            healthBar.SetSize(health);
        }
        else if (streak >= 8){
            multiplier = 2;
            health += 0.1f * multiplier;
            health = Mathf.Clamp(health, 0f, 1f);
            healthBar.SetSize(health);
        } else {
            multiplier = 1;
            health += 0.1f * multiplier;
            health = Mathf.Clamp(health, 0f, 1f);
            healthBar.SetSize(health);
        }
        UpdateGUI();
    }

    public void ResetStreak(){
        streak = 0;
        multiplier = 1;
        health -= .05f;
        health = Mathf.Clamp(health, 0f, 1f);
        healthBar.SetSize(health);
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
}
