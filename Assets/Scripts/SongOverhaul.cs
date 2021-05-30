using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SongOverhaul : MonoBehaviour
{   
    [Header("Note Prefabs")]
    [SerializeField] private GameObject bluePrefab;
    [SerializeField] private GameObject redPrefab;
    [SerializeField] private GameObject creatorPrefab;
    [SerializeField] private GameObject winPrefab;

    [Header("Mapping Values")]
    [SerializeField] private float songSpeed;
    
    [Header("Song Mapped")]
    [SerializeField] private TextAsset jsonMappedSong;

    private AudioSource audio_source;
    private MappingOverhaul mappedSongJSON;
    
    private float previousFrameTime;
    private float lastReportedPlayheadPosition; 
    private float videoCalibrationDelay;
    
    public float songTime; 
    void Start(){     
        videoCalibrationDelay = PlayerPrefs.GetFloat("VideoDelay");

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
            Vector3 updatedNotePosition = new Vector3(mapping.xPosition, mapping.yPosition - ((songTime - mapping.strumTime) * songSpeed) + videoCalibrationDelay, 5);
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
                case "l":
                    mapping.xPosition = MappingOverhaul.ActivatorPositions.leftX;
                    break;
                case "r":
                    mapping.xPosition = MappingOverhaul.ActivatorPositions.rightX;
                    break;
                case "mr":
                    mapping.xPosition = MappingOverhaul.ActivatorPositions.middleRightX;
                    break;
                case "ml":
                    mapping.xPosition = MappingOverhaul.ActivatorPositions.middleLeftX;
                    break;
            }

            switch (mapping.activatorYPosition)
            {
                case "rY":
                    mapping.yPosition = MappingOverhaul.ActivatorPositions.redY;
                    break;
                case "bY":
                    mapping.yPosition = MappingOverhaul.ActivatorPositions.blueY;
                    break;
                case "cY":
                    mapping.yPosition = MappingOverhaul.ActivatorPositions.creatorY;
                    break;
                case "wY":
                    mapping.yPosition = MappingOverhaul.ActivatorPositions.creatorY;
                    break;
            }
        }
    }

    private void InstantiateMappedNotes(){
        foreach (MappingOverhaul.MappingUnit mapping in mappedSongJSON.mappedSong){           
            if (mapping.activatorYPosition == "rY"){
                InstantiatePrefab(mapping, redPrefab);
            } else if (mapping.activatorYPosition == "bY"){
                InstantiatePrefab(mapping, bluePrefab);
            }  else if (mapping.activatorYPosition == "cY"){
                InstantiatePrefab(mapping, creatorPrefab);
            }  else if (mapping.activatorYPosition == "wY"){
                InstantiatePrefab(mapping, winPrefab);
            }         
        }
    }

    private void InstantiatePrefab(MappingOverhaul.MappingUnit unit, GameObject notePrefab){
        GameObject noteInstantiated = Instantiate(notePrefab, new Vector3(unit.xPosition, 100, 5), Quaternion.identity);      
        noteInstantiated.GetComponent<Note>().speed = 0;
        noteInstantiated.GetComponent<Note>().strumTime = unit.strumTime;

        unit.noteInstantiated = noteInstantiated;
    }
}