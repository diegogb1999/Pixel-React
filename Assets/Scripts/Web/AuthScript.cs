using System.Collections;
using UnityEngine;
using TMPro;
using Firebase.Extensions;
using Firebase.Auth;
using Firebase;
using UnityEngine.UI;

public class EmailPassLogin : MonoBehaviour
{
    #region variables
    [Header("Login")]
    public TMP_InputField LoginEmail;
    public TMP_InputField loginPassword;

    [Header("Extra")]
    public TextMeshProUGUI logTxt;
    public TextMeshProUGUI usernameUI;
    public GameObject loginUi, SuccessUi;
    public CanvasGroup logCanvasGroup;
    public GameObject LogInButton;
    public GameObject LogOutButton;
    private AuthResult result;
    private FirebaseAuth auth;
    #endregion

    public void Start()
    {
        auth = FirebaseAuth.DefaultInstance;

        FirebaseUser user = auth.CurrentUser;
        if (user != null && user.IsValid())
        {
            Debug.Log("User is logged in: " + user.UserId);
            LogOutButton.SetActive(true);
            LogInButton.SetActive(false);
            usernameUI.text = user.DisplayName + "///" + user.UserId;
        }
        else
        {
            Debug.Log("No user is logged in");
            LogOutButton.SetActive(false);
            LogInButton.SetActive(true);
            usernameUI.text = "";
        }
    }

    #region Login
    public void Login()
    {
        string email = LoginEmail.text;
        string password = loginPassword.text;

        Credential credential =
        EmailAuthProvider.GetCredential(email, password);
        auth.SignInAndRetrieveDataWithCredentialAsync(credential).ContinueWithOnMainThread(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInAndRetrieveDataWithCredentialAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                showLogMsg("Username or password incorrect");
                Debug.LogError("SignInAndRetrieveDataWithCredentialAsync encountered an error: " + task.Exception);
                return;
            }
            result = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);

            if (result.User.IsValid())
            {
                showLogMsg("Log in Successful");
                loginUi.SetActive(false);
                SuccessUi.SetActive(true);
                usernameUI.text = result.User.DisplayName + "///" + result.User.UserId;
                LogOutButton.SetActive(true);
                LogInButton.SetActive(false);
            }

        });
    }

    public void showLoginUI()
    {
        loginUi.SetActive(true);
    }

    public void Logout()
    {
        auth.SignOut();
        Debug.Log("User has signed out");
        showLogMsg("Logged out successfully");
        LogOutButton.SetActive(false);
        LogInButton.SetActive(true);
        usernameUI.text = "";
    }
    #endregion

    #region extra
    void showLogMsg(string msg)
    {
        logTxt.text = msg;
        logCanvasGroup.alpha = 1;
        StartCoroutine(FadeOutLogMsg(5f, 2f));
    }

    IEnumerator FadeOutLogMsg(float delay, float duration)
    {
        yield return new WaitForSeconds(delay);

        float startAlpha = logCanvasGroup.alpha;
        float rate = 1.0f / duration;
        float progress = 0.0f;

        while (progress < 1.0f)
        {
            logCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0, progress);
            progress += rate * Time.deltaTime;
            yield return null;
        }

        logCanvasGroup.alpha = 0;
    }
    #endregion

}