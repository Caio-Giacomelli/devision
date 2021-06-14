using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class ActivatorOverhaul : MonoBehaviour
{
    [SerializeField]
    private KeyCode key;
    
    [SerializeField]
    private GameObject instantiate_note;

    private bool active = false;  
    private Color old_color;
    private SpriteRenderer sr;
    private AudioSource audio_source;
     private GameObject gm, mm;
    private Queue notes = new Queue();

    GameObject currentGodNote = null;
    
    void Awake(){
        sr = GetComponent<SpriteRenderer>();
        
        audio_source = gameObject.GetComponentInParent(typeof(AudioSource)) as AudioSource;
    }
    
    void Start(){
        gm = GameObject.Find("GameManager");
        mm = GameObject.Find("MusicManager");
        old_color = sr.color;
    }

    void Update(){
        if (Time.timeScale != 0 && !gm.GetComponent<GameManager>().godMode){     
            HandleKeyInput();
            HandleTouchInput();           
        }
    }

    void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.tag == "Win Note" && gameObject.tag == "Red Activator"){
            gm.GetComponent<GameManager>().Win();
        }       
        
        if ((gameObject.tag == "Blue Activator" && other.gameObject.tag == "Blue Note") || 
            (gameObject.tag == "Red Activator" && other.gameObject.tag == "Red Note"))
        {
            active = true;
            notes.Enqueue(other.gameObject);      
        }
    }

    void OnTriggerExit2D(Collider2D other){
        active = false;
        if (notes.Count > 0) notes.Dequeue();     
    }

    // Handle God Mode
    void OnTriggerStay2D(Collider2D other){
        if (gm.GetComponent<GameManager>().godMode)  HandleGodMode();
    }

    private bool CheckHasTouchInput(){
        if (Input.touchCount > 0){

            for (int i = 0; i < Input.touchCount; i++){
                //The reason why I have to make the position convert to World View is because the position is measured in pixels. I need the value in units.
                Vector3 touch_unit_position = Camera.main.ScreenToWorldPoint(Input.GetTouch(i).position);

                //Convert to Vector2 for 2D reasons
                Vector2 touch_unit_position_2d = new Vector2(touch_unit_position.x, touch_unit_position.y);

                //We now raycast with this information. If we have hit something we can process it.
                //TODO: Study Raycast constructor
                RaycastHit2D hitInformation = Physics2D.Raycast(touch_unit_position_2d, Camera.main.transform.forward);              
                Debug.Log(message: $"Input.GetTouch(i).phase {Input.GetTouch(i).phase}");
                Debug.Log(message: $"hitInformation.collider {hitInformation.collider}");
                if (Input.GetTouch(i).phase == TouchPhase.Began && hitInformation.collider != null){
                    //We should have hit something with a 2D Physics collider!
                    GameObject touchedObject = hitInformation.transform.gameObject;
                    if (touchedObject.transform.name == gameObject.name){
                        return true;
                    }
                } 
            }
        }
        return false;
    }

    private void HandleKeyInput(){
        if (Input.GetKeyDown(key)){
            StartCoroutine(HandlePressedActivator());
        }

        if (Input.GetKeyDown(key) && active){       
            HandleSuccessNote();  
        }
        else if (Input.GetKeyDown(key) && !active){       
            gm.GetComponent<GameManager>().ResetStreak();
        }
    }

    private void HandleTouchInput(){
        if (CheckHasTouchInput()){
            Debug.Log(message: $"CheckHasTouchInput = true e active = {active}");
            StartCoroutine(HandlePressedActivator());
            audio_source.Play();
            if (notes.Count > 0){
                HandleSuccessNote();
            } else {
                gm.GetComponent<GameManager>().ResetStreak(); 
            }
        }
    }

    private void HandleSuccessNote(){
        Destroy((GameObject) notes.Dequeue());
        gm.GetComponent<GameManager>().AddStreak();
        AddScore();
        active = false;
    }

    private void HandleGodMode(){
        if (notes.Count > 0 ) currentGodNote = (GameObject) notes.Dequeue();
        if (currentGodNote != null && (currentGodNote.transform.position.y - gameObject.transform.position.y) < 0.001){
            StartCoroutine(HandlePressedActivator());
            Destroy(currentGodNote);
        }
    }

    private void AddScore(){
        PlayerPrefs.SetInt("Score", PlayerPrefs.GetInt("Score") + gm.GetComponent<GameManager>().GetScore());
    }

    IEnumerator HandlePressedActivator(){     
        sr.color = new Color(0, 0, 0);
        yield return new WaitForSeconds(0.05f);
        sr.color = old_color;
    }
}
