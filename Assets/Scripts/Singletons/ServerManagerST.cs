using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System.Threading.Tasks;

public class ServerManagerST : MonoBehaviour
{

    public static ServerManagerST Instance {get; private set; }

    public bool has_connected;
    public FirebaseAuth auth;
    public FirebaseUser User;
    public DatabaseReference DBreference;

    void Awake() {
        if (Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(handleFirebaseConnection);
        } else {
            Destroy(gameObject);
        }

    }

    private void handleFirebaseConnection(Task<DependencyStatus> task) {
        var dependencyStatus = task.Result;
        if (dependencyStatus == Firebase.DependencyStatus.Available) {
            has_connected = true;
            auth = FirebaseAuth.DefaultInstance;
            DBreference = FirebaseDatabase.DefaultInstance.RootReference;
        }
        else
        {
            Debug.LogError(System.String.Format(
            "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
        }
    }
}
