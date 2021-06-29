using System.Collections;
using UnityEngine;

public class Activator : MonoBehaviour{
    
    [Header("Key Testing Configuration")]
    [SerializeField] private KeyCode _key;

    public Queue _activeNotes = new Queue();
    public Queue _longActiveNotes = new Queue();
    
    private SpriteRenderer _spriteRenderer;
    private AudioSource _audioSource;
    private GameManager _gameManager;
    private GameObject _currentGodNoteOnStay = null;
    private Color _baseActivatorColor;  
    
    void Awake(){
        _spriteRenderer = GetComponent<SpriteRenderer>();   
        _audioSource = gameObject.GetComponentInParent(typeof(AudioSource)) as AudioSource;
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    
    void Start(){
        _baseActivatorColor = _spriteRenderer.color;
    }

    void Update(){      
        if (ShouldActivatorPause()) return;   
        
        HandleKeyInput();
        HandleTouchInput();           
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

    void OnTriggerStay2D(Collider2D other){
        if (_gameManager._godMode) HandleGodMode();
    }

    private bool CheckHasTouchInput(){
        if (Input.touchCount == 0) return false;

        for (int i = 0; i < Input.touchCount; i++){
            Vector3 touch_unit_position = Camera.main.ScreenToWorldPoint(Input.GetTouch(i).position);
            Vector2 touch_unit_position_2d = new Vector2(touch_unit_position.x, touch_unit_position.y);
            RaycastHit2D hitInformation = Physics2D.Raycast(touch_unit_position_2d, Camera.main.transform.forward);              

            if (Input.GetTouch(i).phase == TouchPhase.Began && hitInformation.collider != null){
                GameObject touchedObject = hitInformation.transform.gameObject;

                if (touchedObject.transform.name == gameObject.name){
                    return true;
                }
            } 
        }
        return false;
    }

    private void HandleKeyInput(){
        if (Input.GetKeyDown(_key)){
            StartCoroutine(HandlePressedActivator());
        }

        if (Input.GetKeyDown(_key)){       
            if (hasNoteOnQueue()) HandleSuccessNote();  
        }
        else if (Input.GetKeyDown(_key)){       
            _gameManager.ResetStreak();
        }
    }

    private void HandleTouchInput(){
        if (CheckHasTouchInput()){
            StartCoroutine(HandlePressedActivator());
            _audioSource.Play();
            
            if (hasNoteOnQueue()){
                HandleSuccessNote();
            } 
            else if (hasNoteOnLongQueue()){
                HandleLongSuccessNote();
            }
            else {
                _gameManager.ResetStreak(); 
            }
        }
    }

    private void HandleLongSuccessNote(){       
        GameObject noteToDestroy = (GameObject) _longActiveNotes.Dequeue();
        NoteLong currentActiveLongNote = noteToDestroy.GetComponent<NoteLong>();
        currentActiveLongNote.RemoveNote();


        //_gameManager.AddStreak();
        //AddScore();
    }

    private void HandleSuccessNote(){       
        GameObject noteToDestroy = (GameObject) _activeNotes.Dequeue();
        Destroy(noteToDestroy);

        _gameManager.AddStreak();
        AddScore();
    }

    private void HandleGodMode(){
        if (hasNoteOnQueue()) _currentGodNoteOnStay = (GameObject) _activeNotes.Dequeue();
        if (_currentGodNoteOnStay != null && (_currentGodNoteOnStay.transform.position.y - gameObject.transform.position.y) < 0.001){
            StartCoroutine(HandlePressedActivator());
            Destroy(_currentGodNoteOnStay);

            _gameManager.AddStreak();
            AddScore();
        }
    }

    private void AddScore(){
        PlayerPrefs.SetInt("Score", PlayerPrefs.GetInt("Score") + _gameManager.GetScore());
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