using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement; // withdraw after improving the way that non-implemented level works

public class SongSpawner : MonoBehaviour
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
    [HideInInspector] public Mapping mappedSongJSON;
    
    private float previousFrameTime;
    private float lastReportedPlayheadPosition; 
    private float videoCalibrationDelay;

    void Awake(){
        audioSource = GetComponent<AudioSource>();
        
        if (jsonMappedSong == null){
            jsonMappedSong = MapManagerST.Instance.GetJSONMap();
            audioSource.clip = MapManagerST.Instance.levelSong;
        }
        
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
        foreach (Mapping.MappingUnit mapping in mappedSongJSON.mappedSong)
        {
            Vector3 updatedNotePosition = new Vector3(mapping.xPosition, mapping.yPosition - ((songTime - (mapping.strumTime + mappedSongJSON.offset)) * noteSpeed) + videoCalibrationDelay + mappedSongJSON.fixedDelay, 5);
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
        mappedSongJSON = JsonUtility.FromJson<Mapping>(jsonMappedSong.text);
        noteSpeed = mappedSongJSON.noteSpeed;

        foreach (Mapping.MappingUnit mapping in mappedSongJSON.mappedSong)
        {
            switch (mapping.activatorXPosition)
            {
                case "l":
                    mapping.xPosition = Mapping.ActivatorPositions.leftX;
                    break;
                case "r":
                    mapping.xPosition = Mapping.ActivatorPositions.rightX;
                    break;
                case "mr":
                    mapping.xPosition = Mapping.ActivatorPositions.middleRightX;
                    break;
                case "ml":
                    mapping.xPosition = Mapping.ActivatorPositions.middleLeftX;
                    break;
            }

            switch (mapping.activatorYPosition)
            {
                case "rY":
                    mapping.yPosition = Mapping.ActivatorPositions.redY;
                    break;
                case "bY":
                    mapping.yPosition = Mapping.ActivatorPositions.blueY;
                    break;
                case "cY":
                    mapping.yPosition = Mapping.ActivatorPositions.creatorY;
                    break;
                case "wY":
                    mapping.yPosition = Mapping.ActivatorPositions.creatorY;
                    break;
            }
        }
    }

    private void InstantiateMappedNotes(){
        foreach (Mapping.MappingUnit mapping in mappedSongJSON.mappedSong){           
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

    private void InstantiatePrefab(Mapping.MappingUnit unit, GameObject notePrefab){
        GameObject noteInstantiated = Instantiate(notePrefab, new Vector3(unit.xPosition, 100, 5), Quaternion.identity);      
        noteInstantiated.GetComponent<Note>().speed = 0;
        noteInstantiated.GetComponent<Note>().strumTime = unit.strumTime;

        unit.noteInstantiated = noteInstantiated;
    }
}