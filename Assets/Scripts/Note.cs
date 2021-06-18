using UnityEngine;
using System.Collections;

public class Note : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField]
    public float speed;

    public float strumTime;

    void Awake(){
        rb = GetComponent<Rigidbody2D>();
        GetComponent<Activator>();
    }
    
    void Start(){
        rb.velocity = new Vector2(0, -speed);
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.GetComponent<ActivatorOverhaul>() != null){
            Queue notes = other.gameObject.GetComponent<ActivatorOverhaul>().notes;
            if(notes.Count > 0){
                notes.Dequeue();
            }
        }
    }
}
