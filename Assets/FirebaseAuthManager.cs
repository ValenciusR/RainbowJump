using Firebase;
using Firebase.Auth;
using RainbowJump.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using System.Threading.Tasks;
using Google;
using UnityEngine.UI;
using Firebase.Extensions;

public class FirebaseAuthManager : MonoBehaviour
{

    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser user;

    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;

    public TMP_InputField nameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField confpasswordRegisterField;


    public SettingsButton settingsButton;
    [SerializeField]
    public Manager gameManager;


    public string webClientId = "198202739770-1kqbgabahq29u514aqb7ni7a8shefqcs.apps.googleusercontent.com";

    private GoogleSignInConfiguration configuration;

    void Awake()
    {
        configuration = new GoogleSignInConfiguration
        {
            WebClientId = webClientId,
            RequestIdToken = true
        };
    }

    private void Start()
    {

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("error");
            }
        });
    }

    void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;

        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    void AuthStateChanged(object sender, EventArgs e)
    {
        if(auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;

            if (!signedIn && user != null) 
            {
                Debug.Log("Signed Out" + user.UserId);
            }

            user = auth.CurrentUser;

            if(signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
            }
        }
    }

    public void Login()
    {
        StartCoroutine(LoginAsync(emailLoginField.text, passwordLoginField.text));
    }

    public void LoadData()
    {
        gameManager.DestroyChildLB();
        gameManager.ReloadAllDataMyLeaderboard();
    }

    private IEnumerator LoginAsync(string email, string pass)
    {
        var loginTask = auth.SignInWithEmailAndPasswordAsync(email, pass);

        yield return new WaitUntil(() => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
            Debug.LogError(loginTask.Exception);

            FirebaseException firebaseException = loginTask.Exception.GetBaseException() as FirebaseException;
            AuthError authError = (AuthError)firebaseException.ErrorCode;

            string failedMsg = "Login Failed because ";
            switch (authError)
            {
                case AuthError.InvalidEmail:
                    failedMsg += "Email is invalid";
                    break;
                case AuthError.WrongPassword :
                    failedMsg += "Password is wrong";
                    break;
                case AuthError.MissingEmail:
                    failedMsg += "Email is missing";
                    break;
                case AuthError.MissingPassword:
                    failedMsg += "password is missing";
                    break;
            }

            Debug.Log(failedMsg);
        }
        else
        {
            FirebaseUser user = loginTask.Result.User;  

            Debug.LogFormat("{0} successfully logged In", user.DisplayName);

            settingsButton.profilePanel.SetActive(true);
            settingsButton.loginPanel.SetActive(false);
            settingsButton.nameText.text = settingsButton.firebaseAuthManager.user.DisplayName;
            LoadData();
        }


    }

    public void Register()
    {
        StartCoroutine(RegisterAsync(nameRegisterField.text, emailRegisterField.text, passwordRegisterField.text, confpasswordRegisterField.text));
    }

    private IEnumerator RegisterAsync(string name, string email, string password, string confpassword)
    {
        if (name == "")
        {
            Debug.LogError("Username is empty");
        }else if(email == "")
        {
            Debug.LogError("Email is empty");
        }else if(password != confpassword)
        {
            Debug.LogError("Password does not match");
        }
        else
        {
            var registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);

            yield return new WaitUntil(() => registerTask.IsCompleted);

            if(registerTask.Exception != null) { 
                Debug.LogError(registerTask.Exception);

                FirebaseException firebaseException = registerTask.Exception.GetBaseException() as FirebaseException;
                AuthError authError = (AuthError)firebaseException.ErrorCode;

                string failedMsg = "Registration failed because ";
                switch(authError) {
                    case AuthError.InvalidEmail:
                        failedMsg += "Email is invalid";
                        break;
                    case AuthError.WrongPassword:
                        failedMsg += "Wrong password";
                        break;
                    case AuthError.MissingEmail:
                        failedMsg += "Email is missing";
                        break;
                    case AuthError.MissingPassword:
                        failedMsg += "Password is missing";
                        break;
                    default:
                        failedMsg = "Registration Failed";
                        break;
                }

                Debug.Log(failedMsg);   

            }
            else
            {
                user = registerTask.Result.User;

                UserProfile userProfile = new UserProfile { DisplayName = name };

                var updateProfileTask = user.UpdateUserProfileAsync(userProfile);

                yield return new WaitUntil(() => updateProfileTask.IsCompleted);

                if(updateProfileTask.Exception != null) {
                    user.DeleteAsync();

                    Debug.LogError(updateProfileTask.Exception);

                    FirebaseException firebaseException = updateProfileTask.Exception.GetBaseException() as FirebaseException;
                    AuthError authError = (AuthError)firebaseException.ErrorCode;

                    string failedMsg = "Profile update failed! because ";
                    switch (authError)
                    {
                        case AuthError.InvalidEmail:
                            failedMsg += "Email is invalid";
                            break;
                        case AuthError.WrongPassword:
                            failedMsg += "Wrong password";
                            break;
                        case AuthError.MissingEmail:
                            failedMsg += "Email is missing";
                            break;
                        case AuthError.MissingPassword:
                            failedMsg += "Password is missing";
                            break;
                        default:
                            failedMsg = "Registration Failed";
                            break;
                    }
                    Debug.Log(failedMsg);
                }
                else
                {
                    Debug.Log("Registrarion completed, welcome " + user.DisplayName);
                    settingsButton.profilePanel.SetActive(true);
                    settingsButton.loginPanel.SetActive(false);
                    settingsButton.nameText.text = settingsButton.firebaseAuthManager.user.DisplayName;
                }
            }
        }
    }

    public void Logout()
    {
        if(auth != null && user != null)
        {
            auth.SignOut();
        }
    }

    public string GetUserID()
    {
        if (auth != null && user != null)
        {
            string id = user.UserId;
            return id;
        }
        else
        {
            string name = "guest";
            return name;
        }
    }


    public void OnSignIn()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(
          OnAuthenticationFinished);
    }

    public void OnSignOut()
    {
        GoogleSignIn.DefaultInstance.SignOut();
    }

    public void OnDisconnect()
    {
        GoogleSignIn.DefaultInstance.Disconnect();
    }

    internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            using (IEnumerator<System.Exception> enumerator =
                    task.Exception.InnerExceptions.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    GoogleSignIn.SignInException error =
                            (GoogleSignIn.SignInException)enumerator.Current;
                    Debug.LogError("Got Error: " + error.Status + " " + error.Message);
                }
                else
                {
                    Debug.LogError("Got Unexpected Exception?!?" + task.Exception);
                }
            }
        }
        else if (task.IsCanceled)
        {
            Debug.LogError("Canceled");
        }
        else
        {
            Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(task.Result.IdToken, null);

            auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Signin With Credential Async was cancelled.");
                    return;
                }
                if (task.IsCanceled)
                {
                    Debug.LogError("Signin With Credential Async encountered error " + task.Exception);
                    return;
                }
                user = auth.CurrentUser;

            });
        }
    }

    public void OnSignInSilently()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        Debug.LogError("Calling SignIn Silently");

        GoogleSignIn.DefaultInstance.SignInSilently()
              .ContinueWith(OnAuthenticationFinished);
    }


    public void OnGamesSignIn()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = true;
        GoogleSignIn.Configuration.RequestIdToken = false;

        Debug.LogError("Calling Games SignIn");

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(
          OnAuthenticationFinished);
    }
}


