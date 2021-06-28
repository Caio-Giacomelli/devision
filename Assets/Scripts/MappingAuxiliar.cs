using UnityEngine;

public class MappingAuxiliar : MonoBehaviour{
    
    [Header ("Managers")]
    public GameObject _songManagerObject;
    public GameObject _noteSpawnerObject;

    [Header ("Control")]
    [Range(0.1f, 4f)] public float _songSpeed;
    public float _delayMS;

    private AudioSource _audioSource;
    private NoteSpawner _noteSpawner;
    private bool _shouldUpdateDelayMs = true;

    void Awake(){
        _audioSource = _songManagerObject.GetComponent<AudioSource>();
        _noteSpawner = _noteSpawnerObject.GetComponent<NoteSpawner>();     
    }

    void FixedUpdate(){
        if (_shouldUpdateDelayMs) _delayMS = _noteSpawner._mappingSong.fixedDelay;
        _shouldUpdateDelayMs = false;
    }

    void Update(){
        _audioSource.pitch = _songSpeed;
        _noteSpawner._mappingSong.fixedDelay = _delayMS;
    }
}
