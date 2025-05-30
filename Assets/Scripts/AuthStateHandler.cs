using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class AuthStateHandler : MonoBehaviour
{
    [SerializeField]
    GameObject _panelAuth;

    [SerializeField]
    GameObject _panelHome;

    [SerializeField]
    GameObject _panelIntro;

    private void Reset()
    {
        _panelAuth = GameObject.Find("PanelAuth");
        _panelHome = GameObject.Find("Panel_Home");
        _panelIntro = GameObject.Find("Panel_Intro");

        _panelHome.SetActive(false);
    }

    
    void Start()
    {
        FirebaseAuth.DefaultInstance.StateChanged += HandleStateChanged;
        _panelIntro.SetActive(true);
    }

    private void HandleStateChanged(object sender, EventArgs e)
    {
        if (FirebaseAuth.DefaultInstance.CurrentUser != null)
        {
            Invoke("SetAuth", 1.5f);
            setOnline();
            
        }
        else
        {
            //_panelIntro.SetActive(true);
            _panelAuth.SetActive(true);
            _panelHome.SetActive(false);
        }
    }

    public void SetAuth()
    {
        //_panelIntro.SetActive(true);
        _panelAuth.SetActive(false);
        _panelHome.SetActive(true);
        Debug.Log(FirebaseAuth.DefaultInstance.CurrentUser.Email);
    }

    private void setOnline()
    {
        var mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        FirebaseDatabase.DefaultInstance.GetReference("users/" + userId + "/username").GetValueAsync().ContinueWithOnMainThread(task =>
         {
             if (task.IsFaulted)
             {
                 // Handle the error...
             }
             else if (task.IsCompleted)
             {
                 DataSnapshot snapshot = task.Result;
                 string username = snapshot.Value.ToString();
                 PlayerPrefs.SetString("username", username);
                 mDatabaseRef.Child("users-online").Child(userId).SetValueAsync(username);

             }
         });

    }
}
