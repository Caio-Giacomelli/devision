using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameManager : MonoBehaviour{

    [Header("_health Configuration")]
    [SerializeField] private HealthBar _healthBar;
    [SerializeField] private float _repeatRateHealthDegen = 0.3f;
    [SerializeField] private float _amountHealthDegen = 0.01f;
    [SerializeField] private float _missNoteHealthDegen = 0.05f;
    [SerializeField] private float _hitNoteHealthRegen = 0.1f;

    [Header("Score Configuration")]
    [SerializeField] private int _scorePerNote = 10;
    [SerializeField] private int _scorePerLongPressedNote = 1;
    [SerializeField] private int[] _streakThresholds = new int[3];

    [Header("UI Configuration")]
    [SerializeField] private GameObject _countdownAnimation;
    [SerializeField] private Toggle _godToggle;

    [Header("Gameplay Configuration")]
    [SerializeField] public bool _godMode;
    [SerializeField] public bool _isPaused;
    
    [Header("Audio Configuration")]
    [SerializeField] private AudioMixer _mixer;

    private float _health = 1f;
    private int _multiplier = 1;
    private int _streak = 0;

    void Start(){       

        StartGodModeVariables();    
        StartPlayerPrefs();
        HandleMusicVolume();

        InvokeRepeating("HandleHealthBarDegen", 0.0f, _repeatRateHealthDegen);
    }

    void Update(){
        if (_health == 0f){
            Lose();
        }
    }

    void OnTriggerEnter2D(Collider2D other){
        if (other.tag != "Long Blue Note") Destroy(other.gameObject);
        LoseHealth();
        ResetStreak(); 
    }

    public void AddStreak(){
        _streak++;

        if(_streak >= _streakThresholds[2]){
            _multiplier = 4;        
        } 
        else if (_streak >= _streakThresholds[1]){
            _multiplier = 3;
        }
        else if (_streak >= _streakThresholds[0]){
            _multiplier = 2;
        }

        AddHealth();
        UpdateGUI();
    }

    public void ResetStreak(){
        _streak = 0;
        _multiplier = 1;
        LoseHealth();
        UpdateGUI();
    }

    void UpdateGUI(){
        PlayerPrefs.SetInt("Mult", _multiplier);
    }

    public int GetScore(){
        return _scorePerNote * _multiplier;
    }

    public int GetLongNoteScore(){
        return (int) (_scorePerLongPressedNote * _multiplier);
    }

    private void HandleHealthBarDegen(){
        _health -= _amountHealthDegen;
        _health = Mathf.Clamp(_health, 0f, 1f);
        _healthBar.SetSize(_health);
    }

    public void Win(){
        if (SceneManager.GetActiveScene().name == "Mapping" ) SceneManager.LoadScene("Mapping");
        else SceneManager.LoadScene("Win Screen");
    }

    public void Lose(){
        SceneManager.LoadScene("Lose Screen");
    }

    private void AddHealth(){
        _health += _hitNoteHealthRegen;
        _health = Mathf.Clamp(_health, 0f, 1f);
        _healthBar.SetSize(_health);
    }

    private void LoseHealth(){
        _health -= _missNoteHealthDegen;
        _health = Mathf.Clamp(_health, 0f, 1f);
        _healthBar.SetSize(_health);
    }

    private void StartPlayerPrefs(){
        PlayerPrefs.SetInt("Score", 0);
        PlayerPrefs.SetInt("Mult", 1);
        PlayerPrefs.SetString("PreviousScene", SceneManager.GetActiveScene().name);
    }

    private void HandleMusicVolume(){
        float sliderValue = PlayerPrefs.GetFloat("musicSlider");
        float musicVol = Mathf.Log10(sliderValue) * 20;
        _mixer.SetFloat("MusicVol", musicVol);
    }

    public void Resume(GameObject canvas){
        canvas.SetActive(false);
        _countdownAnimation.SetActive(true);
        StartCoroutine("ResumeWithDelay");
    }

    IEnumerator ResumeWithDelay(){
        float pauseTime = Time.realtimeSinceStartup + 2f;
        while (Time.realtimeSinceStartup < pauseTime){
            yield return 0;
        }

        _countdownAnimation.SetActive(false);
              
        Time.timeScale = 1;

        AudioSource[] audios = FindObjectsOfType<AudioSource>();       
        foreach(AudioSource a in audios){
            if (a.playOnAwake || a.panStereo == 0.02f) a.Play(); 
        }
        _isPaused = false;
    }

    private void StartGodModeVariables(){
        _godToggle.isOn = PlayerPrefs.GetInt("_godMode") == 1 ? true : false;
        _godMode = PlayerPrefs.GetInt("_godMode") == 1 ? true : false;
    }

    public void setGodMode(bool isGod){
        _godMode = isGod;
        PlayerPrefs.SetInt("_godMode", isGod ? 1 : 0);    
    }
}
