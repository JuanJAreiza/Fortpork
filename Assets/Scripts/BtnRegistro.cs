using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BtnRegistro : MonoBehaviour
{
    [SerializeField]
    private Button _registrationButton;

    [SerializeField]
    private TMP_InputField _emailInputField;
    [SerializeField]
    private TMP_InputField _passwordInputField;
    [SerializeField]
    private TMP_InputField _usernameInputField;

    [SerializeField]
    private TextMeshProUGUI _errorText;

    private Coroutine _registrationCoroutine;

    private void Reset()
    {
        _registrationButton = GetComponent<Button>();
    }

    void Start()
    {
        _registrationButton.onClick.AddListener(HandleRegistrationButtonClick);
    }

    private void HandleRegistrationButtonClick()
    {
        string email = _emailInputField.text;
        string password = _passwordInputField.text;
        string username = _usernameInputField.text;

        RegisterUser(email, password, username);
    }


    async void RegisterUser(string email, string password, string username)
    {
        _errorText.text = "";

        // Validaciones de campos
        if (string.IsNullOrEmpty(email))
        {
            _errorText.text = "Por favor ingresa un correo electrónico.";
            _errorText.color = new Color32(180, 0, 69, 255);
            return;
        }

        if (string.IsNullOrEmpty(username))
        {
            _errorText.text = "Por favor ingresa un nombre de usuario.";
            _errorText.color = new Color32(180, 0, 69, 255);
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            _errorText.text = "Por favor ingresa una contraseña.";
            _errorText.color = new Color32(180, 0, 69, 255);
            return;
        }

        // Validaciones adicionales del username
        if (username.Length < 3)
        {
            _errorText.text = "El nombre de usuario debe tener al menos 3 caracteres.";
            _errorText.color = new Color32(180, 0, 69, 255);
            return;
        }

        if (!System.Text.RegularExpressions.Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$"))
        {
            _errorText.text = "El nombre de usuario solo puede contener letras, números y guiones bajos (_).";
            _errorText.color = new Color32(180, 0, 69, 255);
            return;
        }

        var auth = FirebaseAuth.DefaultInstance;

        try
        {
            var result = await auth.CreateUserWithEmailAndPasswordAsync(email, password);

            Debug.LogFormat("Usuario registrado: {0} ({1})",
                result.User.DisplayName, result.User.UserId);

            _emailInputField.text = "";
            _passwordInputField.text = "";
            _usernameInputField.text = "";

            _errorText.text = "¡Registro exitoso!";
            _errorText.color = new Color32(32, 140, 0, 255);

            await System.Threading.Tasks.Task.Yield();
            await System.Threading.Tasks.Task.Delay(2000);

            FirebaseDatabase.DefaultInstance.RootReference.Child("users").Child(result.User.UserId).Child("username").SetValueAsync(username);
        }
        catch (FirebaseException firebaseEx)
        {
            var errorCode = (AuthError)firebaseEx.ErrorCode;
            string message = GetErrorMessage(errorCode);

            _errorText.text = message;
            _errorText.color = new Color32(180, 0, 69, 255);
            Debug.LogError("Firebase error: " + message);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error general en registro: " + ex.Message);
            _errorText.text = "Error desconocido. Intenta nuevamente.";
            _errorText.color = new Color32(180, 0, 69, 255);
        }
    }



    private static string GetErrorMessage(AuthError errorCode)
    {
        switch (errorCode)
        {
            case AuthError.MissingEmail:
                return "Por favor ingresa un correo electrónico.";
            case AuthError.MissingPassword:
                return "Por favor ingresa una contraseña.";
            case AuthError.WeakPassword:
                return "La contraseña es demasiado débil.";
            case AuthError.EmailAlreadyInUse:
                return "Este correo ya está registrado.";
            case AuthError.InvalidEmail:
                return "El correo electrónico no es válido.";
            default:
                return "Error desconocido. Intenta nuevamente.";
        }
    }
}
