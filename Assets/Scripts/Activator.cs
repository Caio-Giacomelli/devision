using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activator : MonoBehaviour
{
    
    public KeyCode key;
    public bool createMode;
    public GameObject instantiate_note;

    bool active = false;  
    Color old;
    SpriteRenderer sr;
    GameObject note, gm;
    


    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("GameManager");
        old = sr.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (createMode){

            if (Input.GetKeyDown(key)){
                Instantiate(instantiate_note, transform.position, Quaternion.identity);
            }

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
                            Instantiate(instantiate_note, new Vector3(transform.position.x, transform.position.y, 5), Quaternion.identity);
                        }
                    } 
                }
            }
        }


        else{        
            if (Input.GetKeyDown(key)){
                StartCoroutine(Pressed());
            }
            
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
                            StartCoroutine(Pressed());
                            if (active){
                                Destroy(note);
                                gm.GetComponent<GameManager>().AddStreak();
                                AddScore();
                                active = false;
                            } else {
                                gm.GetComponent<GameManager>().ResetStreak(); 
                            }
                        }
                    } 
                }
            }

            if (Input.GetKeyDown(key) && active)
            {       
                Destroy(note);
                gm.GetComponent<GameManager>().AddStreak(); 
                AddScore();
                active = false;   
            }
            if (Input.GetKeyDown(key) && !active)
            {       
                gm.GetComponent<GameManager>().ResetStreak();
            }                   
        }
   
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "Win Note" && gameObject.tag == "Red Activator"){
            gm.GetComponent<GameManager>().Win();
        }
        
        
        if ((gameObject.tag == "Blue Activator" && col.gameObject.tag == "Blue Note") || (gameObject.tag == "Red Activator" && col.gameObject.tag == "Red Note"))
        {
            active = true;
            note = col.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        active = false;     
    }

    void AddScore()
    {
        PlayerPrefs.SetInt("Score", PlayerPrefs.GetInt("Score") + gm.GetComponent<GameManager>().GetScore());
    }

    IEnumerator Pressed()
    {     
        sr.color = new Color(0, 0, 0);
        yield return new WaitForSeconds(0.05f);
        sr.color = old;

    }
}
