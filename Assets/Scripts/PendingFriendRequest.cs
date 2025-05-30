/*using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PendingFriendRequest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        var reference = mDatabaseRef.Child("users").Child(userId).Child("solicitudesRecibidas");
        reference.ChildAdded += HandleChildAdded;
        reference.ChildChanged += HandleChildChanged;
        //reference.ChildRemoved += HandleChildRemoved;
    }

    private async void HandleChildAdded(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        DataSnapshot snapshot = args.Snapshot;

        Debug.Log(snapshot.Key + ": Solicitud pendiente");
    }

    private async void HandleChildChanged(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        DataSnapshot snapshot = args.Snapshot;

        string friendId = snapshot.Key;
        int estado = (int)snapshot.Value;

        string friendUsername = (await FirebaseDatabase.DefaultInstance
            .GetReference("users/" + friendId + "/username").GetValueAsync()).Value.ToString();

        if (estado == 1)
        {
            Debug.Log(friendId + " ha aceptado tu solicitud");

            eliminarSolicitud(snapshot.Key);

            var mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
            var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

            mDatabaseRef.Child("users").Child(userId).Child("friends").SetValueAsync(friendId);
        }
        if (estado == 2)
        {
            Debug.Log(friendId + " ha rechazado tu solicitud");
            eliminarSolicitud(snapshot.Key);
            
        }
    }
    /*
    private void HandleChildRemoved(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        DataSnapshot snapshot = args.Snapshot;
        Debug.Log(snapshot.Value + " se ha desconectado");
    }
    */
/*
    private void eliminarSolicitud(string requestUserId)
    {
        var mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        mDatabaseRef.Child("users")
                                 .Child(userId)
                                 .Child("solicitudesEnviadas")
                                 .Child(requestUserId)
                                 .SetValueAsync(null);
    }

}
*/