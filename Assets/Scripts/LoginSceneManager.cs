using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using Openfort;
using Openfort.Model;
using Openfort.Recovery;
using Newtonsoft.Json;
public class LoginSceneManager : MonoBehaviour
{
    // Reference to our Authentication service
    private string AccessToken;
    private string key;
    private OpenfortSDK Openfort = new OpenfortSDK("pk_test_505bc088-905e-5a43-b60b-4c37ed1f887a");
    [Header("Loading")]
    public GameObject loadingPanel;

    [Header("Login")]
    public GameObject loginPanel;
    public InputField email;
    public InputField password;
    public Button signinButton;
    public Button googleButton;

    [Header("Register")]
    public GameObject registerPanel;
    public InputField confirmPassword;
    public Button registerButton;

    [Header("LoggedIn")]
    public GameObject loggedinPanel;
    public GameObject openLinkButton;
    public Text playerLabel;
    public Button mintButton;
    public Button logoutButton;
    private string transactionHash;

    [Header("General")]
    public Text statusTextLabel;

    #region UNITY_LIFECYCLE


    private void Start()
    {
        // Hide all our panels until we know what UI to display
        registerPanel.SetActive(false);
        loggedinPanel.SetActive(false);
        openLinkButton.SetActive(false);
        loadingPanel.SetActive(false);
        loginPanel.SetActive(true);

    }
    #endregion

    #region PUBLIC_BUTTON_METHODS
    /// <summary>
    /// Login Button means they've selected to submit a email (email) / password combo
    /// Note: in this flow if no account is found, it will ask them to register.
    /// </summary>

    public async void OnGoogleClicked()
    {
        googleButton.interactable = false;
        loadingPanel.SetActive(true);
        OAuthInitResponse initAuthResponse = await Openfort.InitOAuth(OAuthProvider.Google);
        key = initAuthResponse.Key;
        Application.OpenURL(initAuthResponse.Url);
        InvokeRepeating("CheckToken", 2f, 1f);

    }

    public void LogoutClicked()
    {
        loadingPanel.SetActive(true);
        // Call the async method without awaiting it.
        // Handle the task's completion including any exceptions.
        var logoutTask = OnLogoutClicked();
        HandleTask(logoutTask);
    }

    private async void HandleTask(Task task)
    {
        try
        {
            // Await the task, allowing any exceptions to propagate.
            await task;
        }
        catch (Exception ex)
        {
            // Log or handle the exception as appropriate.
            Debug.LogError($"Error during logout: {ex.Message}");
        }
    }

    // This remains your async Task method which should not be directly used with UI event listeners.
    private async Task OnLogoutClicked()
    {
        logoutButton.interactable = false;
        await Openfort.Logout();
        loginPanel.SetActive(true);
        loggedinPanel.SetActive(false);
        logoutButton.interactable = true;
        loadingPanel.SetActive(false);
    }

    public async void OnLoginClicked()
    {
        loadingPanel.SetActive(true);
        signinButton.interactable = false;
        if (string.IsNullOrEmpty(email.text) || string.IsNullOrEmpty(password.text))
        {
            statusTextLabel.text = "Please provide a correct email/password";
            return;
        }
        statusTextLabel.text = $"Logging In As {email.text} ...";

        try
        {
            AccessToken = await Openfort.LoginWithEmailPassword(email.text, password.text);
            try
            {
                Openfort.ConfigureEmbeddedSigner(80001);
            }
            catch (MissingRecoveryMethod)
            {
                await Openfort.ConfigureEmbeddedRecovery(new PasswordRecovery("secret"));
            }
            loginPanel.SetActive(false);
            statusTextLabel.text = $"Logged In As {email.text}";

            loggedinPanel.SetActive(true);
        }
        catch (System.Exception)
        {
            loginPanel.SetActive(false);
            registerPanel.SetActive(true);
        }
        signinButton.interactable = true;
        loadingPanel.SetActive(false);
    }

    /// <summary>
    /// No account was found, and they have selected to register a email (email) / password combo.
    /// </summary>
    public async void OnRegisterButtonClicked()
    {
        loadingPanel.SetActive(true);
        registerButton.interactable = false;
        if (password.text != confirmPassword.text)
        {
            statusTextLabel.text = "Passwords do not Match.";
            return;
        }

        statusTextLabel.text = $"Registering User {email.text} ...";
        AccessToken = await Openfort.SignUpWithEmailPassword(email.text, password.text);
        try
        {
            Openfort.ConfigureEmbeddedSigner(80001);
        }
        catch (MissingRecoveryMethod)
        {
            await Openfort.ConfigureEmbeddedRecovery(new PasswordRecovery("secret"));
        }
        statusTextLabel.text = $"Logged In As {email.text}";

        registerPanel.SetActive(false);
        loggedinPanel.SetActive(true);
        registerButton.interactable = true;
        loadingPanel.SetActive(false);
    }

    /// <summary>
    /// They have opted to cancel the Registration process.
    /// Possibly they typed the email address incorrectly.
    /// </summary>
    public void OnCancelRegisterButtonClicked()
    {
        ResetFormsAndStatusLabel();

        registerPanel.SetActive(false);
        loginPanel.SetActive(true);
    }

    public void OnBackToLoginClicked()
    {
        ResetFormsAndStatusLabel();
        loginPanel.SetActive(true);
    }


    private async void CheckToken()
    {
        AccessToken = await Openfort.AuthenticateWithOAuth(OAuthProvider.Google, key);
        try
        {
            Openfort.ConfigureEmbeddedSigner(80001);
        }
        catch (MissingRecoveryMethod)
        {
            await Openfort.ConfigureEmbeddedRecovery(new PasswordRecovery("secret"));
        }
        statusTextLabel.text = $"Logged In With Google";
        playerLabel.text = $"Player logged in";
        CancelInvoke();
        loginPanel.SetActive(false);
        registerPanel.SetActive(false);
        loggedinPanel.SetActive(true);
        loadingPanel.SetActive(false);
        googleButton.interactable = false;
    }


    private void ResetFormsAndStatusLabel()
    {
        // Reset all forms
        email.text = string.Empty;
        password.text = string.Empty;
        confirmPassword.text = string.Empty;
        // Reset logged in player label
        playerLabel.text = string.Empty;
        // Reset status text
        statusTextLabel.text = string.Empty;
    }
    public class RootObject
    {
        public TransactionIntentResponse Data { get; set; }
    }
    public async void OnMintClicked()
    {
        loadingPanel.SetActive(true);
        mintButton.interactable = false;
        var webRequest = UnityWebRequest.Post("http://192.168.0.20:4000/mint", "");
        webRequest.SetRequestHeader("Authorization", "Bearer " + AccessToken);
        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.SetRequestHeader("Accept", "application/json");
        await SendWebRequestAsync(webRequest);

        Debug.Log("Mint request sent");
        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Mint Failed: " + webRequest.error);
            return;
        }


        var responseText = webRequest.downloadHandler.text;
        Debug.Log("Mint Response: " + responseText);
        var responseJson = JsonConvert.DeserializeObject<RootObject>(responseText);
        var id = responseJson.Data.Id;
        if (responseJson.Data.NextAction == null)
        {
            Debug.Log("No Next Action");
            return;
        }

        var nextAction = responseJson.Data.NextAction.Payload.UserOpHash;

        Debug.Log("Next Action: " + nextAction);
        var intentResponse = await Openfort.SendSignatureTransactionIntentRequest(id, nextAction);
        transactionHash = intentResponse.Response.TransactionHash;
        openLinkButton.SetActive(true);
        mintButton.interactable = true;
        loadingPanel.SetActive(false);
    }
    public void OpenLink()
    {
        Application.OpenURL("https://mumbai.polygonscan.com/tx/" + transactionHash);
    }
    private Task SendWebRequestAsync(UnityWebRequest webRequest)
    {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        webRequest.SendWebRequest().completed += _ =>
        {
            switch (webRequest.result)
            {
                case UnityWebRequest.Result.Success:
                    tcs.SetResult(true);
                    break;
                default:
                    tcs.SetException(new Exception(webRequest.error));
                    break;
            }
        };
        return tcs.Task;
    }

    #endregion
}