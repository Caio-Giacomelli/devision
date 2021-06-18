using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AuthManager : MonoBehaviour{
    
    [Header("Firebase Settings")]
    public FirebaseUser _user;

    [Header("Login")]
    public InputField _emailLogin;
    public InputField _passwordLogin;
    public Text _warningLoginText;
    public Text _confirmLoginText;

    [Header("Register")]
    public InputField _usernameRegister;
    public InputField _emailRegister;
    public InputField _passwordRegister;
    public InputField _passwordVerifyRegister;
    public Text _warningRegisterText;
    public Text _confirmRegisterText;

    [Header("Game Settings")]
    public string _sceneAfterAuth;

    public void LoginButton(){
        StartCoroutine(Login(_emailLogin.text, _passwordLogin.text));
    }

    public void RegisterButton(){
        StartCoroutine(Register(_emailRegister.text, _passwordRegister.text, _usernameRegister.text, _passwordVerifyRegister.text));
        
    }

    public void ClearLoginFields(){
        _emailLogin.text = "";
        _passwordLogin.text = "";
    }

    public void ClearRegisterFields(){
        _usernameRegister.text = "";
        _emailRegister.text = "";
        _passwordRegister.text = "";
        _passwordVerifyRegister.text = "";
    }

    private IEnumerator Login(string _email, string _password){
        FirebaseAuth auth = ServerManagerST.Instance.auth;
    
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);

        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null){
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
            string message = "Login Failed!";        
            switch (errorCode){
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
            }
            _warningLoginText.text = message;
        }
        else{
            _user = LoginTask.Result;
            ServerManagerST.Instance.User = _user;
            Debug.LogFormat("User signed in successfully: {0} ({1})", _user.DisplayName, _user.Email);
            _warningLoginText.text = "";
            _confirmLoginText.text = _user.DisplayName + " Has Logged In";

            yield return new WaitForSeconds(2);

            SceneManager.LoadScene(_sceneAfterAuth);
        }
    }

    private IEnumerator Register(string _email, string _password, string _username, string _passwordVerify){
        if (_username == ""){
            _warningRegisterText.text = "Missing Username";
        }
        else if(_password != _passwordVerify){
            _warningRegisterText.text = "Password Does Not Match!";
        }
        else {
            FirebaseAuth auth = ServerManagerST.Instance.auth;
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);

            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null){
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                string message = "Register Failed!";
                switch (errorCode){
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;
                    case AuthError.WeakPassword:
                        message = "Weak Password";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email Already In Use";
                        break;
                }
                _warningRegisterText.text = message;
            }
            else{
                _user = RegisterTask.Result;
                if (_user != null){
                    UserProfile profile = new UserProfile{DisplayName = _username};

                    var ProfileTask = _user.UpdateUserProfileAsync(profile);
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null){
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError) firebaseEx.ErrorCode;
                        _warningRegisterText.text = "Username Set Failed!";
                    }
                    else{
                        _warningRegisterText.text = "";
                        _confirmRegisterText.text = _username + " Has Registered Successfully";
                    }
                }
            }
        }
    }
}
