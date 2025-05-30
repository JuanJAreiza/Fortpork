using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;
using TMPro;
using System.Collections.Generic;

public class SolicitudItem : MonoBehaviour
{
    public TMP_Text usernameText;
    public TMP_Text estadoText; // Solo para solicitudes enviadas
    public Button btnAceptar;   // Solo para solicitudes recibidas
    public Button btnRechazar;

    private string senderId;
    private string currentUserId;

    void Awake()
    {
        currentUserId = FirebaseAuth.DefaultInstance.CurrentUser?.UserId;
    }

    // Para solicitudes recibidas
    public void SetupSolicitudRecibida(string _senderId, string username)
    {
        senderId = _senderId;
        usernameText.text = username;

        if (btnAceptar != null)
            btnAceptar.onClick.AddListener(Aceptar);

        if (btnRechazar != null)
            btnRechazar.onClick.AddListener(Rechazar);

        if (estadoText != null)
            estadoText.gameObject.SetActive(false);
    }

    // Para solicitudes enviadas
    public void SetupSolicitudEnviada(string username, string estado)
    {
        usernameText.text = username;

        if (estadoText != null)
        {
            estadoText.gameObject.SetActive(true);
            estadoText.text = estado;

            estadoText.color = estado == "Aceptada" ? Color.green :
                               estado == "Rechazada" ? Color.red :
                               Color.black;
        }

        if (btnAceptar != null) btnAceptar.gameObject.SetActive(false);
        if (btnRechazar != null) btnRechazar.gameObject.SetActive(false);
    }

    void Aceptar()
    {
        var db = FirebaseDatabase.DefaultInstance.RootReference;

        // Agregar a ambos como amigos
        var updates = new Dictionary<string, object>
        {
            [$"users/{currentUserId}/amigos/{senderId}"] = true,
            [$"users/{senderId}/amigos/{currentUserId}"] = true,

            // Marcar ambas solicitudes como aceptadas
            [$"users/{currentUserId}/solicitudesRecibidas/{senderId}"] = "Aceptada",
            [$"users/{senderId}/solicitudesEnviadas/{currentUserId}"] = "Aceptada"
        };

        db.UpdateChildrenAsync(updates).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                Debug.Log("Solicitud aceptada y usuarios añadidos como amigos.");
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("Error al aceptar la solicitud: " + task.Exception);
            }
        });
    }


    void Rechazar()
    {
        var db = FirebaseDatabase.DefaultInstance.RootReference;

        var updates = new Dictionary<string, object>
        {
            [$"users/{currentUserId}/solicitudesRecibidas/{senderId}"] = "Rechazada",
            [$"users/{senderId}/solicitudesEnviadas/{currentUserId}"] = "Rechazada"
        };

        db.UpdateChildrenAsync(updates).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                Debug.Log("Solicitud rechazada.");
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("Error al rechazar la solicitud: " + task.Exception);
            }
        });
    }

}
