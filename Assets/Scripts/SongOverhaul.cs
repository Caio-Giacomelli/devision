using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SongOverhaul : MonoBehaviour
{   
    [SerializeField]
    private GameObject bluePrefab;
    [SerializeField]
    private GameObject redPrefab;

    [Header("Mapping Values")]
    [SerializeField] private float songSpeed;
    [SerializeField] private TextAsset jsonMappedSong;

    private AudioSource audio_source;
    private float previousFrameTime;
    private float lastReportedPlayheadPosition;
    private float songTime;

    private MappingOverhaul mappedSongJSON;

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
            GameObject noteInstantiated;
            
            if (mapping.activatorYPosition == "redY"){
                noteInstantiated = Instantiate(redPrefab, new Vector3(mapping.xPosition, mapping.yPosition, 5), Quaternion.identity);
                
                noteInstantiated.GetComponent<Note>().speed = 0;
                mapping.noteInstantiated = noteInstantiated;

            } else if (mapping.activatorYPosition == "blueY"){
                noteInstantiated = Instantiate(bluePrefab, new Vector3(mapping.xPosition, mapping.yPosition, 5), Quaternion.identity);

                noteInstantiated.GetComponent<Note>().speed = 0;
                mapping.noteInstantiated = noteInstantiated;
            }           
        }
    }
}