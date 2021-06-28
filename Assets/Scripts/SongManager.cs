using UnityEngine;

public class SongManager : MonoBehaviour {
    
    private AudioSource _audioSource;
    private float _songTime;
    private float _previousFrameTime;
    private float _lastReportedPlayheadPosition; 

    void Awake(){
        _audioSource = GetComponent<AudioSource>();
    }
 
    void Update(){
        ControlSongVariables();
    }

    public void StartSongControlVariables(){
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
    
    public float getCurrentSongTime(){
        return _songTime;
    }
}
