using System.Collections;
using UnityEngine;
using TMPro;
using Firebase.Extensions;
using Firebase.Auth;
using Firebase;
using UnityEngine.UI;
using UnityEngine.Networking;
using static System.Net.Mime.MediaTypeNames;

public class EmailPassLogin : MonoBehaviour
{
    #region variables
    [Header("Login")]
    public TMP_InputField LoginEmail;
    public TMP_InputField loginPassword;

    [Header("Extra")]
    public TextMeshProUGUI logTxt;
    public TextMeshProUGUI usernameUI;
    public UnityEngine.UI.Image userProfileImage; // Cambia a RawImage
    public GameObject loginUi, SuccessUi, mask;
    public CanvasGroup logCanvasGroup;
    public GameObject LogInButton;
    public GameObject LogOutButton;
    private AuthResult result;
    private FirebaseAuth auth;
    private Coroutine fadeOutCoroutine;
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
            usernameUI.text = user.DisplayName;
            StartCoroutine(DownloadImage("https://w.wallhaven.cc/full/zy/wallhaven-zyj8gw.jpg"));
        }
        else
        {
            Debug.Log("No user is logged in");
            LogOutButton.SetActive(false);
            LogInButton.SetActive(true);
            usernameUI.text = "";
            mask.SetActive(false);
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
            Debug.LogFormat("User signed in successfully: {0} ({1}) (({2}))",
                result.User.DisplayName, result.User.UserId, result.User.PhotoUrl);

            if (result.User.IsValid())
            {
                showLogMsg("Log in Successful");
                loginUi.SetActive(false);
                SuccessUi.SetActive(true);
                usernameUI.text = result.User.DisplayName;
 
                LogOutButton.SetActive(true);
                LogInButton.SetActive(false);

                StartCoroutine(DownloadImage("https://w.wallhaven.cc/full/zy/wallhaven-zyj8gw.jpg"));
            }

        });
    }

private IEnumerator DownloadImage(string imageUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            userProfileImage.sprite = sprite;
        }
        mask.SetActive(true);
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
        mask.SetActive(false);
    }
    #endregion

    #region extra
    public void showLogMsg(string msg)
    {
        if (fadeOutCoroutine != null)
        {
            StopCoroutine(fadeOutCoroutine);
        }

        logTxt.text = msg;
        logCanvasGroup.alpha = 1;
        fadeOutCoroutine = StartCoroutine(FadeOutLogMsg(2.5f, 1f));
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
        fadeOutCoroutine = null;
    }

    public string GetUserId()
    {
        FirebaseUser user = auth.CurrentUser;
        if (user != null && user.IsValid())
        {
            return user.UserId;
        }
        else
        {
            showLogMsg("No user is currently logged in.");
            return null;
        }
    }
    #endregion

}