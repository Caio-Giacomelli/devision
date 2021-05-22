using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SongOverhaul : MonoBehaviour
{

    [SerializeField]
    private GameObject testNote;
    
    [SerializeField]
    private GameObject testActivator;

    [Header("Mapping Values")]
    [SerializeField] private float songSpeed;

    private AudioSource audio_source;
    private float previousFrameTime;
    private float lastReportedPlayheadPosition;
    private float songTime;

    void Start(){        
        audio_source = GetComponent<AudioSource>();

        previousFrameTime = Time.time;
        lastReportedPlayheadPosition = 0f;
        audio_source.Play();
    }

    void Update(){

        songTime += Time.time - previousFrameTime;
        previousFrameTime = Time.time;
        if(audio_source.time != lastReportedPlayheadPosition) {
            songTime = (songTime + audio_source.time)/2;
            lastReportedPlayheadPosition = audio_source.time;
        }

        RenderNoteFallingDownScreen();
    }

    private void RenderNoteFallingDownScreen(){
        Debug.LogWarning(message: $"songTime  in time {songTime}");
        
        Vector3 newValue = new Vector3(testActivator.transform.position.x, testActivator.transform.position.y - (songTime - 8.697f) * songSpeed, 5);
        testNote.transform.position = newValue;

        Debug.LogError(message: $"newValue  in time {newValue}");
    }
}
