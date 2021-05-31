using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement; // withdraw after improving the way that non-implemented level works

public class SongOverhaul : MonoBehaviour
{   
    [Header("Note Prefabs")]
    [SerializeField] private GameObject bluePrefab;
    [SerializeField] private GameObject redPrefab;
    [SerializeField] private GameObject creatorPrefab;
    [SerializeField] private GameObject winPrefab;

    [Header("Mapping Values")]
    [SerializeField] private float noteSpeed;
    
    [Header("Song Mapped")]
    [SerializeField] private TextAsset jsonMappedSong;
    [SerializeField] public float songTime; 

    private AudioSource audioSource;
    private MappingOverhaul mappedSongJSON;
    
    private float previousFrameTime;
    private float lastReportedPlayheadPosition; 
    private float videoCalibrationDelay;

    void Awake(){
        jsonMappedSong = MapManagerST.Instance.GetJSONMap();
        
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = MapManagerST.Instance.levelSong;

        if (jsonMappedSong == null || audioSource.clip == null){
            Debug.LogWarning("Level not implemented");
            SceneManager.LoadScene("Level Select");
        }
    }
  
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
            Vector3 updatedNotePosition = new Vector3(mapping.xPosition, mapping.yPosition - ((songTime - mapping.strumTime) * noteSpeed) + videoCalibrationDelay, 5);
            if (mapping.noteInstantiated != null){
                mapping.noteInstantiated.transform.position = updatedNotePosition;
            }
        }             
    }

    private void StartSongControlVariables(){
        previousFrameTime = Time.time;
        lastReportedPlayheadPosition = 0f;
        audioSource.Play();
    }

    private void ControlSongVariables(){
        songTime += Time.time - previousFrameTime;
        previousFrameTime = Time.time;
        if(audioSource.time != lastReportedPlayheadPosition) {
            songTime = (songTime + audioSource.time)/2;
            lastReportedPlayheadPosition = audioSource.time;
        }
    }

    private void DeserializeMappedSong(){
        mappedSongJSON = JsonUtility.FromJson<MappingOverhaul>(jsonMappedSong.text);
        noteSpeed = mappedSongJSON.noteSpeed;

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