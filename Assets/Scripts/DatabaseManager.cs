using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine.UI;

public class DatabaseManager : MonoBehaviour
{

    [Header("Highscore")]
    public Text highScoreText;

    private int scoreValue;
    private string levelName;
    private DatabaseReference DBreference;
    private FirebaseUser User;

    void Awake(){
        HandleSaveHighscore();  
    }

    private void HandleSaveHighscore(){
        levelName = PlayerPrefs.GetString("PreviousScene");
        scoreValue =  PlayerPrefs.GetInt("Score");
        DBreference = ServerManager.Instance.DBreference;
        User = ServerManager.Instance.User;

        StartCoroutine(ConditionToSaveHighscore());
    }

    private IEnumerator ConditionToSaveHighscore(){
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("highscores").Child(levelName).GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null){
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        } 
        else if (DBTask.Result.Value == null){
            StartCoroutine(SaveHighscore());
        } else {
            DataSnapshot snapshot = DBTask.Result;
            Debug.LogWarning(message: $"snapshot {snapshot.Value}");

            long previousHighscore = (long) snapshot.Value;
            if (scoreValue > previousHighscore){
                highScoreText.text = scoreValue.ToString();
                StartCoroutine(SaveHighscore());
            } else {
                highScoreText.text = previousHighscore.ToString();
            }
        }      
    }

    private IEnumerator SaveHighscore(){
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("highscores").Child(levelName).SetValueAsync(scoreValue);
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null){
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        } else {
            /* Successfully saved */
        }
    }
}
