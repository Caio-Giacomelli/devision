using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Volume : MonoBehaviour{
     
    [SerializeField] private AudioMixer _mixer;
    [SerializeField] private Slider _slider;

    void Start(){
        _slider.value = PlayerPrefs.GetFloat("musicSlider");
    }

    public void SetVolume(float sliderValue){
        float musicVol = Mathf.Log10(sliderValue) * 20;
        
        PlayerPrefs.SetFloat("musicSlider", sliderValue);
        _mixer.SetFloat("MusicVol", musicVol);
    }    
}
