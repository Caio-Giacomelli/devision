using System.Collections;
using UnityEngine;

public class NoteLong : MonoBehaviour {
    public float _strumTime;
    public float _endTime;
    public bool _shouldDecreaseNoteBar = false;

    private void OnTriggerExit2D(Collider2D other){
        Activator activator = other.gameObject.GetComponent<Activator>();      
        if (activator == null) return;

        Queue notes = activator._longActiveNotes;
        if(notes.Count > 0){
            notes.Dequeue();
        }
        Destroy(this.gameObject);
    }

    public void RemoveNote(float yActivatorPosition){
        Vector3 currentNotePosition = this.gameObject.transform.position;
        GameObject childObject = this.gameObject.transform.GetChild(1).gameObject;
        childObject.SetActive(false);

        GameObject barObject = this.gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;
        barObject.GetComponent<SpriteRenderer>().color = new Color(255, 116, 32);


        this.gameObject.transform.position = new Vector3(currentNotePosition.x, yActivatorPosition, currentNotePosition.z);
        _shouldDecreaseNoteBar = true;
    }

    public void setBarScale(float currentTime, float offset, float chartSpeed, float calibDelay, float fixedDelay, float activatorYposition){

        if (this == null || this.gameObject == null) return; 

        Transform noteTransform = this.gameObject.transform;        

        float yBase = activatorYposition - ((currentTime - (currentTime + offset)) * chartSpeed) + calibDelay + fixedDelay;
        float yCeiling = activatorYposition - ((currentTime - (_endTime + offset)) * chartSpeed) + calibDelay + fixedDelay;

        float yLongBarLocalScale = (yCeiling - yBase);

        if(yLongBarLocalScale >= 0){
            noteTransform.GetChild(0).localScale = new Vector3(0.2f, yLongBarLocalScale, 1f);
        } else {
            Destroy(this.gameObject);
        }
    }
}
