using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;
using TMPro;
using System.Collections.Generic;

public class UserListManager : MonoBehaviour
{
    public GameObject userEntryPrefab;
    public Transform contentUsuarios;

    [Header("Iconos")]
    public Sprite iconSolicitudEnviada;
    public Sprite iconEnviarSolicitud;

    private string currentUserId;
    private Dictionary<string, GameObject> userEntries = new Dictionary<string, GameObject>();

    void Start()
    {
        currentUserId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        FirebaseDatabase.DefaultInstance.GetReference("users").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && task.Result.Exists)
            {
                foreach (Transform child in contentUsuarios)
                    Destroy(child.gameObject);

                userEntries.Clear();

                foreach (DataSnapshot user in task.Result.Children)
                {
                    string uid = user.Key;
                    if (uid == currentUserId) continue;

                    string username = user.Child("username").Value?.ToString() ?? "Sin nombre";

                    GameObject entry = Instantiate(userEntryPrefab, contentUsuarios);
                    entry.transform.Find("UsernameText").GetComponent<TMP_Text>().text = username;

                    Button btn = entry.transform.Find("btnEnviarSolicitud").GetComponent<Button>();
                    Image icon = btn.transform.Find("Icon").GetComponent<Image>();
                    TMP_Text estadoAmigoText = entry.transform.Find("EstadoAmigoText").GetComponent<TMP_Text>();
                    estadoAmigoText.gameObject.SetActive(false);

                    btn.onClick.AddListener(() => EnviarSolicitud(uid, btn, icon));
                    icon.sprite = iconEnviarSolicitud;

                    Image estadoCircle = entry.transform.Find("EstadoCircle").GetComponent<Image>();
                    estadoCircle.color = Color.red;

                    userEntries[uid] = entry;

                    // Verificar si ya son amigos
                    FirebaseDatabase.DefaultInstance
                        .GetReference("users").Child(currentUserId).Child("amigos").Child(uid)
                        .GetValueAsync().ContinueWithOnMainThread(amigosTask =>
                        {
                            if (amigosTask.IsCompleted && amigosTask.Result.Exists)
                            {
                                btn.interactable = false;
                                estadoAmigoText.gameObject.SetActive(true);
                                estadoAmigoText.text = "Amigo";
                            }
                        });
                }

                // Escuchar en tiempo real los usuarios online
                FirebaseDatabase.DefaultInstance.GetReference("users-online")
                    .ValueChanged += OnUsersOnlineChanged;
            }
        });
    }

    void EnviarSolicitud(string receptorId, Button btn, Image icon)
    {
        string emisorId = currentUserId;
        DatabaseReference db = FirebaseDatabase.DefaultInstance.RootReference;

        var updates = new Dictionary<string, object>
        {
            [$"users/{receptorId}/solicitudesRecibidas/{emisorId}"] = "Pendiente",
            [$"users/{emisorId}/solicitudesEnviadas/{receptorId}"] = "Pendiente"
        };

        db.UpdateChildrenAsync(updates).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                btn.interactable = false;
                icon.sprite = iconSolicitudEnviada;
                icon.color = Color.green;
            }
            else
            {
                Debug.LogWarning("Error al enviar solicitud: " + task.Exception);
            }
        });
    }

    void OnUsersOnlineChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null) return;

        HashSet<string> onlineUserIds = new HashSet<string>();
        foreach (DataSnapshot user in args.Snapshot.Children)
            onlineUserIds.Add(user.Key);

        foreach (var kvp in userEntries)
        {
            string uid = kvp.Key;
            GameObject entry = kvp.Value;
            Image estadoCircle = entry.transform.Find("EstadoCircle").GetComponent<Image>();
            estadoCircle.color = onlineUserIds.Contains(uid) ? Color.green : Color.red;
        }
    }

    void OnDestroy()
    {
        FirebaseDatabase.DefaultInstance.GetReference("users-online")
            .ValueChanged -= OnUsersOnlineChanged;
    }
}
