using Firebase;
using Firebase.Auth;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ForgotPasswordManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField _emailInputField;
    [SerializeField] private Button _resetPasswordButton;
    [SerializeField] private TextMeshProUGUI _statusText;

    private FirebaseAuth _auth;

    private void Start()
    {
        _auth = FirebaseAuth.DefaultInstance;
        _resetPasswordButton.onClick.AddListener(HandlePasswordReset);
    }

    private void HandlePasswordReset()
    {
        string email = _emailInputField.text.Trim();

        if (string.IsNullOrEmpty(email))
        {
            _statusText.text = "Por favor, ingresa tu correo electrónico.";
            _statusText.color = Color.red;
            return;
        }

        StartCoroutine(SendPasswordResetEmail(email));
    }

    private IEnumerator SendPasswordResetEmail(string email)
    {
        _statusText.text = "Enviando correo...";
        _statusText.color = Color.yellow;

        var resetTask = _auth.SendPasswordResetEmailAsync(email);
        yield return new WaitUntil(() => resetTask.IsCompleted);

        if (resetTask.IsCanceled)
        {
            _statusText.text = "Operación cancelada.";
            _statusText.color = Color.red;
            Debug.LogError("SendPasswordResetEmailAsync fue cancelado.");
        }
        else if (resetTask.IsFaulted)
        {
            FirebaseException firebaseEx = resetTask.Exception.Flatten().InnerExceptions[0] as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
            string errorMessage = GetErrorMessage(errorCode);

            _statusText.text = errorMessage;
            _statusText.color = Color.red;
            Debug.LogError("Error al enviar el correo: " + errorMessage);
        }
        else
        {
            _statusText.text = "¡Correo enviado! Revisa tu bandeja de entrada.";
            _statusText.color = Color.green;
            Debug.Log("Correo de restablecimiento enviado a: " + email);
        }
    }

    private string GetErrorMessage(AuthError errorCode)
    {
        switch (errorCode)
        {
            case AuthError.InvalidEmail:
                return "Correo electrónico no válido.";
            case AuthError.UserNotFound:
                return "No existe una cuenta con este correo.";
            case AuthError.NetworkRequestFailed:
                return "Error de conexión. Revisa tu internet.";
            default:
                return "Error desconocido. Intenta de nuevo más tarde.";
        }
    }
}