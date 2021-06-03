using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapUtils : MonoBehaviour
{
    [Header ("Managers")]
    public GameObject musicManager;

    [Header ("Control")]
    [Range(0.5f, 4f)]
    public float songSpeed;
    public float delayMS;

    private AudioSource audioSource;
    private SongOverhaul songManager;
    private bool shouldFixedUpdate = true;

    void Start(){
        audioSource = musicManager.GetComponent<AudioSource>();
        songManager = musicManager.GetComponent<SongOverhaul>();
       
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
