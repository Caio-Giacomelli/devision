using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Volume : MonoBehaviour
{
    [SerializeField]
    private AudioMixer mixer;

    [SerializeField]
    private Slider slider;

    void Start(){
        slider.value = PlayerPrefs.GetFloat("musicSlider");
    }

    public void SetVolume(float sliderValue){
        float musicVol = Mathf.Log10(sliderValue) * 20;
        
        PlayerPrefs.SetFloat("musicSlider", sliderValue);

        mixer.SetFloat("MusicVol", musicVol);
    }    
}
