using System.Collections;
using UnityEngine;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine.UI;

public class DatabaseManager : MonoBehaviour{

    [Header("Highscore")]
    public Text _highscoreText;

    private DatabaseReference _DBReference;
    private FirebaseUser _user;
    private int _scoreValue;
    private string _levelName;

    void Awake(){
        HandleSaveHighscore();  
    }

    private void HandleSaveHighscore(){
        _levelName = PlayerPrefs.GetString("NextScene");
        _scoreValue =  PlayerPrefs.GetInt("Score");
        _DBReference = ServerManagerST.Instance.DBreference;
        _user = ServerManagerST.Instance.User;

        StartCoroutine(ConditionToSaveHighscore());
    }

    private IEnumerator ConditionToSaveHighscore(){
        var DBTask = _DBReference.Child("users").Child(_user.UserId).Child("highscores").Child(_levelName).GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null){
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        } 
        else if (DBTask.Result.Value == null){
            _highscoreText.text = _scoreValue.ToString();
            StartCoroutine(SaveHighscore());
        } else {
            DataSnapshot snapshot = DBTask.Result;

            long previousHighscore = (long) snapshot.Value;
            if (_scoreValue > previousHighscore){
                _highscoreText.text = _scoreValue.ToString();
                StartCoroutine(SaveHighscore());
            } else {
                _highscoreText.text = previousHighscore.ToString();
            }
        }      
    }

    private IEnumerator SaveHighscore(){
        var DBTask = _DBReference.Child("users").Child(_user.UserId).Child("highscores").Child(_levelName).SetValueAsync(_scoreValue);
        
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null){
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        } else {
            Debug.Log(message: $"Highscore successfully saved");
        }
    }
}
