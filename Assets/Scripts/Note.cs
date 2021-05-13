using UnityEngine;

public class Note : MonoBehaviour
{
    Rigidbody2D rb;

    [SerializeField]
    private float speed;

    void Awake(){
        rb = GetComponent<Rigidbody2D>();
    }
    
    void Start(){
        rb.velocity = new Vector2(0, -speed);
    }
}
