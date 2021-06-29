using System.Collections;
using UnityEngine;

public class NoteLong : MonoBehaviour {
    public float _strumTime;
    public float _endTime;

    private void OnTriggerExit2D(Collider2D other){
        Activator activator = other.gameObject.GetComponent<Activator>();      
        if (activator == null) return;

        Queue notes = activator._activeNotes;
        if(notes.Count > 0){
            notes.Dequeue();
        }
    }
}
