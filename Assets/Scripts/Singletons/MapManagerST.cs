using UnityEngine;

public class MapManagerST : MonoBehaviour{
    public static MapManagerST Instance {get; private set;}

    [Header("Song Charts")]
    [SerializeField] public TextAsset _jsonMapHard;
    [SerializeField] public TextAsset _jsonMapExpert;

    [Header("Audio Clip")]
    [SerializeField] public AudioClip _levelSong;
    
    [Header("Song Settings")]
    [SerializeField] public string _difficulty;

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        } else if (Instance != this) {
            Destroy(Instance.gameObject);
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }  

    public void SetJSONMapHard(TextAsset jsonMap){
        _jsonMapHard = jsonMap;
    }

    public void SetJSONMapExpert(TextAsset jsonMap){
        _jsonMapExpert = jsonMap;
    }

    public void SetLevelSong(AudioClip _levelSong){
        this._levelSong = _levelSong;
    }
    
    public void SetDifficulty(string _difficulty){
        this._difficulty = _difficulty;
    }

    public TextAsset GetJSONMap(){
        switch (_difficulty){
                case "Hard":
                    return _jsonMapHard;
                case "Expert":
                    return _jsonMapExpert;
            }
        return _jsonMapHard;
    }
}
