using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour{
    [SerializeField] private string _nameScore;
    private Text _scoreText;

    void Awake(){
        _scoreText = GetComponent<Text>();
    }

    void Update(){
        _scoreText.text = PlayerPrefs.GetInt(_nameScore)+"";
    }
}
