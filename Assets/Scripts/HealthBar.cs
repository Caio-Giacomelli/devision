using UnityEngine;

public class HealthBar : MonoBehaviour{
    private Transform _barTransform;
    
    private void Awake(){
        _barTransform = transform.Find("Bar");
    }

    public void SetSize(float sizeNormalized){
        _barTransform.localScale = new Vector3(sizeNormalized, 1f);
    }
}
