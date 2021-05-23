using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SongOverhaul : MonoBehaviour
{   
    [Header("Note Prefabs")]
    [SerializeField]
    private GameObject bluePrefab;
    [SerializeField]
    private GameObject redPrefab;

    [Header("Mapping Values")]
    [SerializeField] 
    private float songSpeed;
    
    [Header("Song Mapped")]
    [SerializeField] 
    private TextAsset jsonMappedSong;

    private AudioSource audio_source;
    private MappingOverhaul mappedSongJSON;
    
    private float previousFrameTime;
    private float lastReportedPlayheadPosition;
    public float songTime;   

    void Start(){        
        DeserializeMappedSong();
        InstantiateMappedNotes();
        StartSongControlVariables();
    }

    void Update(){
        ControlSongVariables();
        RenderNoteFallingDownScreen();
    }

    private void RenderNoteFallingDownScreen(){
        foreach (MappingOverhaul.MappingUnit mapping in mappedSongJSON.mappedSong)
        {
            Vector3 updatedNotePosition = new Vector3(mapping.xPosition, mapping.yPosition - (songTime - mapping.strumTime) * songSpeed, 5);
            if (mapping.noteInstantiated != null){
                mapping.noteInstantiated.transform.position = updatedNotePosition;
            }
        }             
    }

    private void StartSongControlVariables(){
        audio_source = GetComponent<AudioSource>();
        previousFrameTime = Time.time;
        lastReportedPlayheadPosition = 0f;
        audio_source.Play();
    }

    private void ControlSongVariables(){
        songTime += Time.time - previousFrameTime;
        previousFrameTime = Time.time;
        if(audio_source.time != lastReportedPlayheadPosition) {
            songTime = (songTime + audio_source.time)/2;
            lastReportedPlayheadPosition = audio_source.time;
        }
    }

    private void DeserializeMappedSong(){
        mappedSongJSON = JsonUtility.FromJson<MappingOverhaul>(jsonMappedSong.text);

        foreach (MappingOverhaul.MappingUnit mapping in mappedSongJSON.mappedSong)
        {
            switch (mapping.activatorXPosition)
            {
                case "leftX":
                    mapping.xPosition = MappingOverhaul.ActivatorPositions.leftX;
                    break;
                case "rightX":
                    mapping.xPosition = MappingOverhaul.ActivatorPositions.rightX;
                    break;
                case "middleRightX":
                    mapping.xPosition = MappingOverhaul.ActivatorPositions.middleRightX;
                    break;
                case "middleLeftX":
                    mapping.xPosition = MappingOverhaul.ActivatorPositions.middleLeftX;
                    break;
            }

            switch (mapping.activatorYPosition)
            {
                case "redY":
                    mapping.yPosition = MappingOverhaul.ActivatorPositions.redY;
                    break;
                case "blueY":
                    mapping.yPosition = MappingOverhaul.ActivatorPositions.blueY;
                    break;
            }
        }
    }

    private void InstantiateMappedNotes(){
        foreach (MappingOverhaul.MappingUnit mapping in mappedSongJSON.mappedSong){           
            if (mapping.activatorYPosition == "redY"){
                InstantiatePrefab(mapping, redPrefab);
            } else if (mapping.activatorYPosition == "blueY"){
                InstantiatePrefab(mapping, bluePrefab);
            }           
        }
    }

    private void InstantiatePrefab(MappingOverhaul.MappingUnit unit, GameObject notePrefab){
        GameObject noteInstantiated = Instantiate(notePrefab, new Vector3(unit.xPosition, 100, 5), Quaternion.identity);      
        noteInstantiated.GetComponent<Note>().speed = 0;

        unit.noteInstantiated = noteInstantiated;
    }
}