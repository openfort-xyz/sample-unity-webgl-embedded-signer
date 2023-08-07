using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Proyecto26;
using UnityEngine;
using Openfort;

/// <summary>
/// Handles calls to the Google provider for authentication
/// </summary>
public static class GoogleAuthenticator
{
    private const string
        ClientId = "365961619025-leb6c7q7cv182dop066lm56girln23tf.apps.googleusercontent.com";

    private const string
        ClientSecret = "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";

    private static string RedirectUri = "http://localhost:3000/saveAuthToken"; // TODO: Change this to your function endpoint you can find in https://console.firebase.google.com/u/0/project/[PROJECT_ID]/functions
    private static string GetAuthTokenEndpoint = "https://localhost:3000/getAuthToken"; // TODO: Change this to your function endpoint you can find in https://console.firebase.google.com/u/0/project/[PROJECT_ID]/functions

    /// <summary>
    /// Opens a webpage that prompts the user to sign in and copy the auth code 
    /// </summary>
    /// 

    static async void CheckToken()
    {
        OpenfortAuth authClient = new OpenfortAuth("pk_test_a3ea62f7-52b4-4a08-85be-1b2780302134");

        var token = await authClient.GetTokenAfterGoogleSignin();
        Debug.Log(token);
    }

    public async static void SignInWithGoogle()
    {
        OpenfortAuth authClient = new OpenfortAuth("pk_test_a3ea62f7-52b4-4a08-85be-1b2780302134");

        var getGoogleLink = await authClient.GetGoogleSigninUrl();
        Debug.Log(getGoogleLink);

        InvokeRepeating("CheckToken", 2f, 1f);



        // var guid = Guid.NewGuid().ToString();
        // Application.OpenURL(
        //     $"https://accounts.google.com/o/oauth2/v2/auth?client_id={ClientId}&redirect_uri={RedirectUri}&response_type=code&scope=email&state={guid}");

        // WaitForCode(guid);
    }

    private static void WaitForCode(string guid)
    {
        RestClient.Request(new RequestHelper
        {
            Method = "GET",
            Uri = GetAuthTokenEndpoint,
            Params = new Dictionary<string, string>
            {
                {"state", guid}
            }
        }).Then(async response =>
            {
                var success = response.Text != "";

                if (success)
                {
                    ExchangeAuthCodeWithIdToken(response.Text,
                        idToken =>
                        {
                            // Here it should request the API to see if the auth was successful and get the access_token
                        });
                }
                else
                {
                    await Task.Delay(3000);
                    WaitForCode(guid);
                }
            }).Catch(Debug.Log);
    }

    /// <summary>
    /// Exchanges the Auth Code with the user's Id Token
    /// </summary>
    /// <param name="code"> Auth Code </param>
    /// <param name="callback"> What to do after this is successfully executed </param>
    private static void ExchangeAuthCodeWithIdToken(string code, Action<string> callback)
    {
        RestClient.Request(new RequestHelper
        {
            Method = "POST",
            Uri = "https://oauth2.googleapis.com/token",
            Params = new Dictionary<string, string>
            {
                {"code", code},
                {"client_id", ClientId},
                {"client_secret", ClientSecret},
                {"redirect_uri", RedirectUri},
                {"grant_type", "authorization_code"}
            }
        }).Then(
            response =>
            {
                var data =
                    StringSerializationAPI.Deserialize(typeof(GoogleIdTokenResponse), response.Text) as
                        GoogleIdTokenResponse;
                callback(data.id_token);
            }).Catch(Debug.Log);
    }
}