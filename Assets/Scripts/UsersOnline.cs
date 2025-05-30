using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UsersOnline : MonoBehaviour
{
    [Header("Componentes de Notificación")]
    [SerializeField] private RectTransform notificationBox;
    [SerializeField] private TMP_Text notificationText;

    [Header("Configuración de Animación")]
    [SerializeField] private float slideDuration = 0.5f;
    [SerializeField] private float visibleDuration = 2f;
    [SerializeField] private Vector2 offScreenPos = new Vector2(-800, 0);
    [SerializeField] private Vector2 onScreenPos = new Vector2(0, 0);

    private string _currentUserId;
    private HashSet<string> initialUserIds = new HashSet<string>();
    private bool hasInitialized = false;

    private Queue<string> notificationQueue = new Queue<string>();
    private bool isShowingNotification = false;

    void Start()
    {
        _currentUserId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        var reference = FirebaseDatabase.DefaultInstance.GetReference("users-online");

        // Obtener usuarios que ya estaban online al iniciar sesión
        reference.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && task.Result != null)
            {
                foreach (var child in task.Result.Children)
                {
                    initialUserIds.Add(child.Key);
                }
                hasInitialized = true;
            }
        });

        reference.ChildAdded += HandleChildAdded;
        reference.ChildRemoved += HandleChildRemoved;
    }

    private void HandleChildAdded(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null || !hasInitialized) return;

        string userId = args.Snapshot.Key;
        if (userId == _currentUserId || initialUserIds.Contains(userId)) return;

        FirebaseDatabase.DefaultInstance
            .GetReference("users/" + userId + "/username")
            .GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted && !task.IsFaulted)
                {
                    string username = task.Result.Value?.ToString();
                    if (!string.IsNullOrEmpty(username))
                        EnqueueNotification($"{username} se conectó");
                }
            });
    }

    private void HandleChildRemoved(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null || !hasInitialized) return;

        string userId = args.Snapshot.Key;
        if (userId == _currentUserId) return;

        FirebaseDatabase.DefaultInstance
            .GetReference("users/" + userId + "/username")
            .GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted && !task.IsFaulted)
                {
                    string username = task.Result.Value?.ToString();
                    if (!string.IsNullOrEmpty(username))
                        EnqueueNotification($"{username} se desconectó");
                }
            });
    }

    private void EnqueueNotification(string message)
    {
        notificationQueue.Enqueue(message);
        if (!isShowingNotification)
        {
            StartCoroutine(ProcessNotifications());
        }
    }

    private IEnumerator ProcessNotifications()
    {
        isShowingNotification = true;

        while (notificationQueue.Count > 0)
        {
            string message = notificationQueue.Dequeue();
            notificationText.text = message;

            notificationBox.anchoredPosition = offScreenPos;
            yield return StartCoroutine(Slide(notificationBox, offScreenPos, onScreenPos, slideDuration));

            yield return new WaitForSeconds(visibleDuration);

            yield return StartCoroutine(Slide(notificationBox, onScreenPos, offScreenPos, slideDuration));
        }

        isShowingNotification = false;
    }

    private IEnumerator Slide(RectTransform target, Vector2 from, Vector2 to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            target.anchoredPosition = Vector2.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        target.anchoredPosition = to;
    }

    private void OnApplicationQuit()
    {
        if (FirebaseAuth.DefaultInstance.CurrentUser == null) return;

        var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        FirebaseDatabase.DefaultInstance.GetReference("users-online").Child(userId).SetValueAsync(null);
    }
}
