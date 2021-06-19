using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System.Threading.Tasks;

public class ServerManagerST : MonoBehaviour{

    public static ServerManagerST Instance {get; private set;}
    
    public FirebaseAuth _auth;
    public FirebaseUser _user;
    public DatabaseReference _dBReference;
    public bool _hasConnected;

    void Awake() {
        if (Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    void Start(){
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(handleFirebaseConnection);
    }

    private void handleFirebaseConnection(Task<DependencyStatus> task) {
        var dependencyStatus = task.Result;
        if (dependencyStatus == Firebase.DependencyStatus.Available) {
            _hasConnected = true;
            _auth = FirebaseAuth.DefaultInstance;
            _dBReference = FirebaseDatabase.DefaultInstance.RootReference;
        }
        else
        {
            Debug.LogError(System.String.Format(
            "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
        }
    }
}
