using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MapManagerST : MonoBehaviour
{
    public static MapManagerST Instance {get; private set; }

    [Header("Song Mapped")]
    [SerializeField] public TextAsset jsonMapHard;
    [SerializeField] public TextAsset jsonMapExpert;
    [SerializeField] public AudioClip levelSong;
    [SerializeField] public string difficulty;

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
        jsonMapHard = jsonMap;
    }

    public void SetJSONMapExpert(TextAsset jsonMap){
        jsonMapExpert = jsonMap;
    }

    public void SetLevelSong(AudioClip levelSong){
        this.levelSong = levelSong;
    }
    
    public void SetDifficulty(string difficulty){
        this.difficulty = difficulty;
    }

    public TextAsset GetJSONMap(){
        switch (difficulty)
            {
                case "Hard":
                    return jsonMapHard;
                case "Expert":
                    return jsonMapExpert;
            }
        return jsonMapHard;
    }
}
