using UnityEngine;
using UnityEngine.SceneManagement;

public class SongSpawner : MonoBehaviour
{   
    [Header("Note Prefabs")]
    [SerializeField] private GameObject _bluePrefab;
    [SerializeField] private GameObject _redPrefab;
    [SerializeField] private GameObject _creatorPrefab;
    [SerializeField] private GameObject _winPrefab;
    [SerializeField] private GameObject _longNoteBar;

    [Header("Chart Speed")]
    [SerializeField] private float _chartSpeed;
  
    [HideInInspector] public Mapping _mappingSong;
    private TextAsset _jsonChartAsset;
    private AudioSource _audioSource;
    private float _songTime;
    private float _previousFrameTime;
    private float _lastReportedPlayheadPosition; 
    private float _videoCalibrationDelay;

    void Awake(){
        _audioSource = GetComponent<AudioSource>();
    }
  
    void Start(){     
        _videoCalibrationDelay = PlayerPrefs.GetFloat("VideoDelay");

        if (_jsonChartAsset == null){
            _jsonChartAsset = MapManagerST.Instance.GetJSONMap();
            _audioSource.clip = MapManagerST.Instance._levelSong;
        }
        
        if (_jsonChartAsset == null || _audioSource.clip == null){
            Debug.LogWarning("Level not implemented");
            SceneManager.LoadScene("Level Select");
        }

        DeserializeMappedSong();
        InstantiateMappedNotes();
        StartSongControlVariables();
    }

    void Update(){
        ControlSongVariables();
        RenderNoteFallingDownScreen();
    }

    private void RenderNoteFallingDownScreen(){
        foreach (Mapping.MappingUnit unit in _mappingSong.mappedSong){
            Vector3 updatedNotePosition = new Vector3(unit.xPosition, unit.yPosition - ((_songTime - (unit.strumTime + _mappingSong.offset)) * _chartSpeed) + _videoCalibrationDelay + _mappingSong.fixedDelay, 5);
            if (unit.noteInstantiated != null){
                unit.noteInstantiated.transform.position = updatedNotePosition; 
            }
            if (unit.longBarInstantiated != null){
                float yPositionLongBar = (unit.strumTime + unit.endContinuous) / 2;
                Debug.Log(message: $"endContinuous {unit.endContinuous}");
                Debug.Log(message: $"unit.strumTime {unit.strumTime}");
                Debug.Log(message: $"yPositionLongBar {yPositionLongBar}");

                Vector3 updatedLongBarInstantiated = new Vector3(unit.xPosition, unit.yPosition - ((_songTime - (yPositionLongBar + _mappingSong.offset)) * _chartSpeed) + _videoCalibrationDelay + _mappingSong.fixedDelay, 5);
                unit.longBarInstantiated.transform.position = updatedNotePosition; 
            }
        }             
    }

    private void StartSongControlVariables(){
        _previousFrameTime = Time.time;
        _lastReportedPlayheadPosition = 0f;
        _audioSource.Play();
    }

    private void ControlSongVariables(){
        _songTime += Time.time - _previousFrameTime;
        _previousFrameTime = Time.time;
        
        if(_audioSource.time != _lastReportedPlayheadPosition){
            _songTime = (_songTime + _audioSource.time)/2;
            _lastReportedPlayheadPosition = _audioSource.time;
        }
    }

    private void DeserializeMappedSong(){
        _mappingSong = JsonUtility.FromJson<Mapping>(_jsonChartAsset.text);
        _chartSpeed = _mappingSong.noteSpeed;

        foreach (Mapping.MappingUnit mapping in _mappingSong.mappedSong){
            switch (mapping.activatorXPosition){
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
        foreach (Mapping.MappingUnit mapping in _mappingSong.mappedSong){           
            if (mapping.endContinuous > 0){
                InstantiateLongNotePrefab(mapping, _bluePrefab);
            }
            if (mapping.activatorYPosition == "rY"){
                InstantiatePrefab(mapping, _redPrefab);
            } else if (mapping.activatorYPosition == "bY"){
                InstantiatePrefab(mapping, _bluePrefab);
            }  else if (mapping.activatorYPosition == "cY"){
                InstantiatePrefab(mapping, _creatorPrefab);
            }  else if (mapping.activatorYPosition == "wY"){
                InstantiatePrefab(mapping, _winPrefab);
            }
        }
    }

    private void InstantiatePrefab(Mapping.MappingUnit unit, GameObject notePrefab){
        GameObject noteInstantiated = Instantiate(notePrefab, new Vector3(unit.xPosition, 100, 5), Quaternion.identity);
        noteInstantiated.GetComponent<Note>()._strumTime = unit.strumTime;

        unit.noteInstantiated = noteInstantiated;
    }

    private void InstantiateLongNotePrefab(Mapping.MappingUnit unit, GameObject notePrefab){
        InstantiatePrefab(unit, notePrefab);
        InstantiateLongBar(unit);
    }

    private void InstantiateLongBar(Mapping.MappingUnit unit){
        GameObject longBar = Instantiate(_longNoteBar, new Vector3(unit.xPosition, 100, 5), Quaternion.identity);
        longBar.transform.localScale = new Vector3(0.2f, ((float) unit.endContinuous) - unit.strumTime, 1f);

        unit.longBarInstantiated = longBar;
    }
}