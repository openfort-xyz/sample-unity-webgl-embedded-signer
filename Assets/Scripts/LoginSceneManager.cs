using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Openfort;
using Openfort.Model;
using Openfort.Recovery;
public class LoginSceneManager : MonoBehaviour
{
    // Reference to our Authentication service
    private OpenfortSDK Openfort = new OpenfortSDK("pk_live_c7c28825-d406-5f6d-8a7f-0395e3796416");

    [Header("Login")]
    public GameObject loginPanel;
    public InputField email;
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
    /// Login Button means they've selected to submit a email (email) / password combo
    /// Note: in this flow if no account is found, it will ask them to register.
    /// </summary>

    // public async void OnGoogleClicked()
    // {
    //     var getGoogleLink = await Openfort.GetGoogleSigninUrl();
    //     Debug.Log(getGoogleLink);
    //     Application.OpenURL(getGoogleLink);

    // }

    public async void OnLogoutClicked()
    {
        // Openfort.Logout();
        loginPanel.SetActive(true);
        loggedinPanel.SetActive(false);
    }

    public async void OnLoginClicked()
    {
        if (string.IsNullOrEmpty(email.text) || string.IsNullOrEmpty(password.text))
        {
            statusTextLabel.text = "Please provide a correct email/password";
            return;
        }
        statusTextLabel.text = $"Logging In As {email.text} ...";

        try
        {
            string token = await Openfort.LoginWithEmailPassword(email.text, password.text);
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

    }

    /// <summary>
    /// No account was found, and they have selected to register a email (email) / password combo.
    /// </summary>
    public async void OnRegisterButtonClicked()
    {
        if (password.text != confirmPassword.text)
        {
            statusTextLabel.text = "Passwords do not Match.";
            return;
        }

        statusTextLabel.text = $"Registering User {email.text} ...";
        string token = await Openfort.SignUpWithEmailPassword(email.text, password.text);
        try
        {
            Openfort.ConfigureEmbeddedSigner(80001);
        }
        catch (MissingRecoveryMethod)
        {
            await Openfort.ConfigureEmbeddedRecovery(new PasswordRecovery("secret"));
        }
        statusTextLabel.text = $"Logged In As {email.text}";

        Debug.Log(token);
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


    // private async void CheckToken()
    // {
    //     Openfort.Model.AuthResponse token = await Openfort.GetTokenAfterGoogleSignin();
    //     Debug.Log(token);
    //     statusTextLabel.text = $"Logged In With Google";
    //     playerLabel.text = $"Player: {token.PlayerId}";
    //     CancelInvoke();
    //     loginPanel.SetActive(false);
    //     registerPanel.SetActive(false);
    //     loggedinPanel.SetActive(true);
    // }


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

    #endregion
}