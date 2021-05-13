using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    private Transform bar;
    
    private void Start(){
        bar = transform.Find("Bar");
    }

    public void SetSize(float size_normalized){
        bar.localScale = new Vector3(size_normalized, 1f);
    }
}
