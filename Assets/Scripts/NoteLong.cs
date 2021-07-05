using System.Collections;
using UnityEngine;

public class NoteLong : MonoBehaviour {
    public float _strumTime;
    public float _endTime;
    public int teste = 0;

    private void OnTriggerExit2D(Collider2D other){
        Activator activator = other.gameObject.GetComponent<Activator>();      
        if (activator == null) return;

        Queue notes = activator._longActiveNotes;
        if(notes.Count > 0){
            notes.Dequeue();
        }
    }

    public void RemoveNote(){
        this.gameObject.transform.GetChild(1).gameObject.SetActive(false);
        teste = 1;
    }

    public void setBarScale(float currentTime, float offset, float chartSpeed, float calibDelay, float fixedDelay, float activatorYposition){

        Transform noteTransform = this.gameObject.transform;        

        float yBase = activatorYposition - ((currentTime - (currentTime + offset)) * chartSpeed) + calibDelay + fixedDelay;
        float yCeiling = activatorYposition - ((currentTime - (_endTime + offset)) * chartSpeed) + calibDelay + fixedDelay;

        float yLongBarLocalScale = (yCeiling - yBase);
        Debug.Log(message: $"yLongBarLocalScale: {yLongBarLocalScale}");

        if(yLongBarLocalScale >= 0){
            noteTransform.GetChild(0).localScale = new Vector3(0.2f, yLongBarLocalScale, 1f);
            Debug.Log(message: $"Scale: {noteTransform.GetChild(0).localScale}");
        }
 
    }
}
