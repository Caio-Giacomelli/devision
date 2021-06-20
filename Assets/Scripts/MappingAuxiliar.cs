using UnityEngine;

public class MappingAuxiliar : MonoBehaviour{
    
    [Header ("Managers")]
    public GameObject _musicManager;

    [Header ("Control")]
    [Range(0.1f, 4f)] public float _songSpeed;
    public float _delayMS;

    private AudioSource _audioSource;
    private SongSpawner _songManager;
    private bool _shouldUpdateDelayMs = true;

    void Awake(){
        _audioSource = _musicManager.GetComponent<AudioSource>();
        _songManager = _musicManager.GetComponent<SongSpawner>();     
    }

    void FixedUpdate(){
        if (_shouldUpdateDelayMs) _delayMS = _songManager._mappingSong.fixedDelay;
        _shouldUpdateDelayMs = false;
    }

    void Update(){
        _audioSource.pitch = _songSpeed;
        _songManager._mappingSong.fixedDelay = _delayMS;
    }
}
