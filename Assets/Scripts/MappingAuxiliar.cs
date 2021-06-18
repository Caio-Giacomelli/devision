using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MappingAuxiliar : MonoBehaviour
{
    [Header ("Managers")]
    public GameObject musicManager;

    [Header ("Control")]
    [Range(0.1f, 4f)]
    public float songSpeed;
    public float delayMS;

    private AudioSource audioSource;
    private SongSpawner songManager;
    private bool shouldFixedUpdate = true;

    void Start(){
        audioSource = musicManager.GetComponent<AudioSource>();
        songManager = musicManager.GetComponent<SongSpawner>();
       
    }

    void FixedUpdate(){
        if (shouldFixedUpdate) delayMS = songManager.mappedSongJSON.fixedDelay;
        shouldFixedUpdate = false;
    }

    void Update(){
        audioSource.pitch = songSpeed;
        songManager.mappedSongJSON.fixedDelay = delayMS;
    }
}
