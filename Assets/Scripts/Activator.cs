using System.Collections;
using UnityEngine;

public class Activator : MonoBehaviour{
    
    [Header("Key Testing Configuration")]
    [SerializeField] private KeyCode _key;

    public Queue _activeNotes = new Queue();
    public Queue _longActiveNotes = new Queue();
    
    private SpriteRenderer _spriteRenderer;
    private ParticleSystem _particleSystem;
    private GameManager _gameManager;
    private AudioSource _audioSource;
    private GameObject _currentGodNoteOnStay = null;
    private GameObject _currentGodNoteLongOnStay = null;
    private GameObject _currentActiveLongNoteGameObject;
    private TouchPhase _currentTouchPhase;
    private NoteLong _currentActiveLongNoteComponent;
    private Color _baseActivatorColor;

    void Awake(){
        _spriteRenderer = GetComponent<SpriteRenderer>();   
        _particleSystem = GetComponent<ParticleSystem>();  
        _audioSource = gameObject.GetComponentInParent(typeof(AudioSource)) as AudioSource;
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    
    void Start(){
        _baseActivatorColor = _spriteRenderer.color;
    }

    void Update(){      
        if (_gameManager._godMode) HandleGodMode();  
        if (ShouldActivatorPause()) return; 

        _currentTouchPhase = CheckHasTouchInput();

        HandleTouchInput();
        HandleLongNoteContinuousInput(); 

        HandleActivatorEffects();     
    }


    private void HandleActivatorEffects(){
        switch (_currentTouchPhase)
        {
            case TouchPhase.Began:
                _spriteRenderer.color = new Color(0, 0, 0);
                break;
            case TouchPhase.Ended:
                _spriteRenderer.color = _baseActivatorColor;
                break;
            case TouchPhase.Canceled:
                _spriteRenderer.color = _baseActivatorColor;
                break;
        }
    }
    

    void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.tag == "Win Note" && gameObject.tag == "Red Activator"){
            _gameManager.Win();
        }       
        
        if ((gameObject.tag == "Blue Activator" && other.gameObject.tag == "Blue Note") || 
            (gameObject.tag == "Red Activator" && other.gameObject.tag == "Red Note"))
        {
            _activeNotes.Enqueue(other.gameObject);      
        }

        if ((gameObject.tag == "Blue Activator" && other.gameObject.tag == "Long Blue Note") || 
            (gameObject.tag == "Red Activator" && other.gameObject.tag == "Long Red Note"))
        {
            _longActiveNotes.Enqueue(other.gameObject);      
        }
    }

    void OnTriggerExit2D(Collider2D other){
        if (_gameManager._godMode) _spriteRenderer.color = _baseActivatorColor;
    }

    private TouchPhase CheckHasTouchInput(){
        if (Input.touchCount == 0) return TouchPhase.Canceled;

        for (int i = 0; i < Input.touchCount; i++){
            Vector3 touch_unit_position = Camera.main.ScreenToWorldPoint(Input.GetTouch(i).position);
            Vector2 touch_unit_position_2d = new Vector2(touch_unit_position.x, touch_unit_position.y);
            RaycastHit2D hitInformation = Physics2D.Raycast(touch_unit_position_2d, Camera.main.transform.forward);              

            if (hitInformation.collider != null){
                GameObject touchedObject = hitInformation.transform.gameObject;

                if (touchedObject.transform.name == gameObject.name){
                    return Input.GetTouch(i).phase;
                }
            } 
        }
        return TouchPhase.Canceled;
    }

    private void HandleLongNoteContinuousInput(){
        if (_currentActiveLongNoteComponent == null || !_currentActiveLongNoteComponent._shouldDecreaseNoteBar) return;
        
        if (_currentTouchPhase == TouchPhase.Stationary){
            _spriteRenderer.color = new Color(0, 0, 0);
            AddLongNoteScore();
        } else if (_currentTouchPhase == TouchPhase.Ended){
            _spriteRenderer.color = _baseActivatorColor;
            Destroy(_currentActiveLongNoteGameObject);
        }
    }

    private void HandleTouchInput(){
        if (_currentTouchPhase == TouchPhase.Began){
            _audioSource.Play();
            
            if (hasNoteOnQueue()){
                HandleSuccessNote();
            } 
            else if (hasNoteOnLongQueue()){
                HandleLongSuccessNote();
            }
            else {
                Debug.Log("Reset Streak");
                _gameManager.ResetStreak(); 
            }
        }
    }

    private void HandleLongSuccessNote(){      
        GameObject noteToDestroy = (GameObject) _longActiveNotes.Dequeue();      
        
        NoteLong currentActiveLongNote = noteToDestroy.GetComponent<NoteLong>();
        currentActiveLongNote.RemoveNote(this.gameObject.transform.position.y);
        
        _currentActiveLongNoteComponent = currentActiveLongNote;
        _currentActiveLongNoteGameObject = noteToDestroy;
    }

    private void HandleSuccessNote(){       
        GameObject noteToDestroy = (GameObject) _activeNotes.Dequeue();
        _particleSystem.Play();

        Destroy(noteToDestroy);
        
        _gameManager.AddStreak();
        AddScore();
    }

    private void HandleGodMode(){
        if (hasNoteOnQueue()) _currentGodNoteOnStay = (GameObject) _activeNotes.Dequeue();
        if (hasNoteOnLongQueue()) _currentGodNoteLongOnStay = (GameObject) _longActiveNotes.Dequeue();

        if (_currentGodNoteOnStay != null && (_currentGodNoteOnStay.transform.position.y - gameObject.transform.position.y) < 0.001){
            Destroy(_currentGodNoteOnStay);
            
            StartCoroutine(HandlePressedActivator());
            
            _gameManager.AddStreak();
            AddScore();
        }

        if (_currentGodNoteLongOnStay != null && (_currentGodNoteLongOnStay.transform.position.y - gameObject.transform.position.y) < 0.12){
            
            NoteLong currentActiveLongNote = _currentGodNoteLongOnStay.GetComponent<NoteLong>();
            currentActiveLongNote.RemoveNote(this.gameObject.transform.position.y);
            
            _currentActiveLongNoteComponent = currentActiveLongNote;
            _currentActiveLongNoteGameObject = _currentGodNoteLongOnStay;
            _spriteRenderer.color = new Color(0, 0, 0);
            AddLongNoteScore();
        }
    }

    private void AddScore(){
        PlayerPrefs.SetInt("Score", PlayerPrefs.GetInt("Score") + _gameManager.GetScore());
    }

    private void AddLongNoteScore(){
        PlayerPrefs.SetInt("Score", PlayerPrefs.GetInt("Score") + _gameManager.GetLongNoteScore());
    }

    private bool ShouldActivatorPause(){
        return Time.timeScale == 0 || _gameManager._godMode ? true : false;
    }

    private bool hasNoteOnQueue(){
        return _activeNotes.Count > 0 ? true : false;
    }

    private bool hasNoteOnLongQueue(){
        return _longActiveNotes.Count > 0 ? true : false;
    }

    IEnumerator HandlePressedActivator(){     
        _spriteRenderer.color = new Color(0, 0, 0);
        yield return new WaitForSeconds(0.05f);
        _spriteRenderer.color = _baseActivatorColor;
    }
}