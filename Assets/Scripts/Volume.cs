using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Volume : MonoBehaviour
{
    public AudioMixer mixer;
    public Slider slider;

    void Start()
    {
        slider.value = PlayerPrefs.GetFloat("musicSlider");
    }

    public void setVolume(float sliderValue){
        float musicVol = Mathf.Log10(sliderValue) * 20;
        
        PlayerPrefs.SetFloat("musicSlider", sliderValue);

        mixer.SetFloat("MusicVol", musicVol);
    }    
}
