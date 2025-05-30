using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using TMPro;
using UnityEngine;

public class LabelUsername : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _label;

    private void Reset()
    {
        _label = GetComponent<TMP_Text>();
    }

    void Start()
    {
        FirebaseAuth.DefaultInstance.StateChanged += HandleStateChanged;

        if (FirebaseAuth.DefaultInstance.CurrentUser != null)
        {
            SetUsername();
        }
    }

    private void HandleStateChanged(object sender, EventArgs e)
    {
        if (FirebaseAuth.DefaultInstance.CurrentUser != null)
        {
            SetUsername();
        }
    }

    private void SetUsername()
    {
        var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        FirebaseDatabase.DefaultInstance
            .GetReference("users/" + userId + "/username")
            .GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Error al obtener el nombre de usuario.");
                }
                else if (task.IsCompleted && task.Result.Exists)
                {
                    DataSnapshot snapshot = task.Result;
                    _label.text = snapshot.Value.ToString();
                }
            });
    }

    private void OnDestroy()
    {
        FirebaseAuth.DefaultInstance.StateChanged -= HandleStateChanged;
    }
}
