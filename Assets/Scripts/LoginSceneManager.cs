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
    OpenfortAuth authClient = new OpenfortAuth("pk_test_9b35edfd-f40b-527f-a5ff-7ed129cfe454", "http://localhost:3000");

    [Header("Login")]
    public GameObject loginPanel;
    public InputField username;
    public InputField password;

    [Header("Register")]
    public GameObject registerPanel;
    public InputField confirmPassword;

    [Header("General")]
    public Text statusTextLabel;

    #region UNITY_LIFECYCLE


    private void Start()
    {
        // Hide all our panels until we know what UI to display
        registerPanel.SetActive(false);
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

    public async void OnLoginClicked()
    {
        if (string.IsNullOrEmpty(username.text) || string.IsNullOrEmpty(password.text))
        {
            statusTextLabel.text = "Please provide a correct username/password";
            return;
        }
        statusTextLabel.text = $"Logging In As {username.text} ...";

        loginPanel.SetActive(false);
        OpenfortAuth authClient = new OpenfortAuth("pk_test_9b35edfd-f40b-527f-a5ff-7ed129cfe454", "http://localhost:3000");

        var loginResponse = await authClient.Login(username.text, password.text);
        Debug.Log(loginResponse);
        // _authService.Email = username.text;
        // _authService.Password = password.text;
        // _authService.Authenticate(_currentAuthType);
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

        registerPanel.SetActive(false);
        statusTextLabel.text = $"Registering User {username.text} ...";
        OpenfortAuth authClient = new OpenfortAuth("pk_test_9b35edfd-f40b-527f-a5ff-7ed129cfe454", "http://localhost:3000");

        var signupResponse = await authClient.Signup(username.text, password.text, username.text);
        Debug.Log(signupResponse);
        // _authService.Email = username.text;
        // _authService.Password = password.text;
        // _authService.Authenticate(_currentAuthType);
    }

    /// <summary>
    /// They have opted to cancel the Registration process.
    /// Possibly they typed the email address incorrectly.
    /// </summary>
    public void OnCancelRegisterButtonClicked()
    {
        ResetFormsAndStatusLabel();

        // Show panels
        registerPanel.SetActive(false);
        loginPanel.SetActive(true);
    }

    public void OnBackToLoginClicked()
    {
        // We don't use it for the moment, can bring problems.

        ResetFormsAndStatusLabel();

        loginPanel.SetActive(true);



        // Logout from PlayFab
        //TODO
    }

    private async void CheckToken()
    {
        var token = await authClient.GetTokenAfterGoogleSignin();
        Debug.Log(token);
        CancelInvoke();
    }


    private void ResetFormsAndStatusLabel()
    {
        // Reset all forms
        username.text = string.Empty;
        password.text = string.Empty;
        confirmPassword.text = string.Empty;

        // Reset status text
        statusTextLabel.text = string.Empty;
    }

    #endregion
}