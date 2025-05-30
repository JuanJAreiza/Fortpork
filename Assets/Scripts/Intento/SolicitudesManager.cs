using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using Firebase.Auth;
using TMPro;
using Firebase.Extensions;

public class SolicitudesManager : MonoBehaviour
{
    public GameObject prefabSolicitudRecibida;
    public GameObject prefabSolicitudEnviada;
    public Transform contentRecibidas;
    public Transform contentEnviadas;

    private string currentUserId;
    private DatabaseReference recibidasRef;
    private DatabaseReference enviadasRef;

    void Start()
    {
        currentUserId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        SetupListeners();
    }

    void SetupListeners()
    {
        recibidasRef = FirebaseDatabase.DefaultInstance.GetReference("users").Child(currentUserId).Child("solicitudesRecibidas");
        enviadasRef = FirebaseDatabase.DefaultInstance.GetReference("users").Child(currentUserId).Child("solicitudesEnviadas");

        recibidasRef.ValueChanged += OnSolicitudesRecibidasChanged;
        enviadasRef.ValueChanged += OnSolicitudesEnviadasChanged;
    }

    void OnSolicitudesRecibidasChanged(object sender, ValueChangedEventArgs args)
    {
        foreach (Transform child in contentRecibidas)
        {
            Destroy(child.gameObject);
        }

        if (args.Snapshot.Exists)
        {
            foreach (DataSnapshot solicitud in args.Snapshot.Children)
            {
                string senderId = solicitud.Key;

                FirebaseDatabase.DefaultInstance.GetReference("users").Child(senderId).Child("username").GetValueAsync().ContinueWithOnMainThread(userTask =>
                {
                    if (userTask.IsCompleted && userTask.Result.Exists)
                    {
                        string username = userTask.Result.Value.ToString();
                        GameObject entry = Instantiate(prefabSolicitudRecibida, contentRecibidas);
                        entry.GetComponent<SolicitudItem>().SetupSolicitudRecibida(senderId, username);
                    }
                });
            }
        }
    }

    void OnSolicitudesEnviadasChanged(object sender, ValueChangedEventArgs args)
    {
        foreach (Transform child in contentEnviadas)
        {
            Destroy(child.gameObject);
        }

        if (args.Snapshot.Exists)
        {
            foreach (DataSnapshot solicitud in args.Snapshot.Children)
            {
                string receptorId = solicitud.Key;
                string estado = solicitud.Value.ToString();

                FirebaseDatabase.DefaultInstance.GetReference("users").Child(receptorId).Child("username").GetValueAsync().ContinueWithOnMainThread(userTask =>
                {
                    if (userTask.IsCompleted && userTask.Result.Exists)
                    {
                        string username = userTask.Result.Value.ToString();
                        GameObject entry = Instantiate(prefabSolicitudEnviada, contentEnviadas);
                        entry.GetComponent<SolicitudItem>().SetupSolicitudEnviada(username, estado);
                    }
                });
            }
        }
    }

    void OnDestroy()
    {
        if (recibidasRef != null)
            recibidasRef.ValueChanged -= OnSolicitudesRecibidasChanged;

        if (enviadasRef != null)
            enviadasRef.ValueChanged -= OnSolicitudesEnviadasChanged;
    }
}
