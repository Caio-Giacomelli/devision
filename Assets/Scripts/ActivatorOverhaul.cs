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
    private GameObject note, gm;
    
    void Awake(){
        sr = GetComponent<SpriteRenderer>();
        
        audio_source = gameObject.GetComponentInParent(typeof(AudioSource)) as AudioSource;
    }
    
    void Start(){
        gm = GameObject.Find("GameManager");
        old_color = sr.color;
    }

    void Update(){
        if (Time.timeScale != 0){
            if (gm.GetComponent<GameManager>().createMode){
                HandleCreateMode();
            }
            else{        
                HandleKeyInput();
                HandleTouchInput();           
            }
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
            note = other.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D other){
        active = false;     
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
            StartCoroutine(HandlePressedActivator());
            audio_source.Play();
            if (active){
                HandleSuccessNote();
            } else {
                gm.GetComponent<GameManager>().ResetStreak(); 
            }
        }
    }

    private void HandleSuccessNote(){
        Destroy(note);
        gm.GetComponent<GameManager>().AddStreak();
        AddScore();
        active = false;
    }

    private void HandleCreateMode(){
        instantiate_note.GetComponent<Note>().speed = gm.GetComponent<GameManager>().note_speed;
        
        if (Input.GetKeyDown(key)){
            Instantiate(instantiate_note, transform.position, Quaternion.identity);
        }
        if (CheckHasTouchInput()){
            Instantiate(instantiate_note, new Vector3(transform.position.x, transform.position.y, 5), Quaternion.identity);
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
