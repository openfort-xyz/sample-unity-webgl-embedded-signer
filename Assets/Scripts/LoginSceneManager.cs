using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Openfort;

public class LoginSceneManager : MonoBehaviour
{
    // Reference to our Authentication service
    OpenfortAuth authClient = new OpenfortAuth("YOUR_PUBLISHABLE_KEY");

    [Header("Login")]
    public GameObject loginPanel;
    public InputField username;
    public InputField password;

    [Header("Register")]
    public GameObject registerPanel;
    public InputField confirmPassword;

    [Header("LoggedIn")]
    public GameObject loggedinPanel;
    public Text playerLabel;

    [Header("General")]
    public Text statusTextLabel;

    #region UNITY_LIFECYCLE


    private void Start()
    {
        // Hide all our panels until we know what UI to display
        registerPanel.SetActive(false);
        loggedinPanel.SetActive(false);
        loginPanel.SetActive(true);

    }
    #endregion

    #region PUBLIC_BUTTON_METHODS
    /// <summary>
    /// Login Button means they've selected to submit a username (email) / password combo
    /// Note: in this flow if no account is found, it will ask them to register.
    /// </summary>

    public async void OnGoogleClicked()
    {
        var getGoogleLink = await authClient.GetGoogleSigninUrl();
        Debug.Log(getGoogleLink);
        Application.OpenURL(getGoogleLink);

        InvokeRepeating("CheckToken", 2f, 1f);
    }

    public async void OnLogoutClicked()
    {
        authClient.Logout();
        loginPanel.SetActive(true);
        loggedinPanel.SetActive(false);
    }

    public async void OnLoginClicked()
    {
        if (string.IsNullOrEmpty(username.text) || string.IsNullOrEmpty(password.text))
        {
            statusTextLabel.text = "Please provide a correct username/password";
            return;
        }
        statusTextLabel.text = $"Logging In As {username.text} ...";

        try
        {
            Openfort.Model.AuthResponse loginResponse = await authClient.Login(username.text, password.text);
            loginPanel.SetActive(false);
            statusTextLabel.text = $"Logged In As {username.text}";
            playerLabel.text = $"Player: {loginResponse.PlayerId}";

            loggedinPanel.SetActive(true);
        }
        catch (System.Exception)
        {
            loginPanel.SetActive(false);
            registerPanel.SetActive(true);
        }

    }

    /// <summary>
    /// No account was found, and they have selected to register a username (email) / password combo.
    /// </summary>
    public async void OnRegisterButtonClicked()
    {
        if (password.text != confirmPassword.text)
        {
            statusTextLabel.text = "Passwords do not Match.";
            return;
        }

        statusTextLabel.text = $"Registering User {username.text} ...";
        var signupResponse = await authClient.Signup(username.text, password.text, username.text);
        Debug.Log(signupResponse);
        registerPanel.SetActive(false);
        loggedinPanel.SetActive(true);
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
        Openfort.Model.AuthResponse token = await authClient.GetTokenAfterGoogleSignin();
        Debug.Log(token);
        statusTextLabel.text = $"Logged In As {username.text}";
        playerLabel.text = $"Player: {token.PlayerId}";
        CancelInvoke();
        loginPanel.SetActive(false);
        registerPanel.SetActive(false);
        loggedinPanel.SetActive(true);
    }


    private void ResetFormsAndStatusLabel()
    {
        // Reset all forms
        username.text = string.Empty;
        password.text = string.Empty;
        confirmPassword.text = string.Empty;
        // Reset logged in player label
        playerLabel.text = string.Empty;
        // Reset status text
        statusTextLabel.text = string.Empty;
    }

    #endregion
}