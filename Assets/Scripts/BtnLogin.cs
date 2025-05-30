using Firebase;
using Firebase.Auth;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BtnLogin : MonoBehaviour
{
    [SerializeField]
    private Button _loginButton;

    [SerializeField]
    private TMP_InputField _emailInputField;

    [SerializeField]
    private TMP_InputField _passwordInputFiel;

    [SerializeField]
    private TextMeshProUGUI _errorText;

    private void Reset()
    {
        _loginButton = GetComponent<Button>();
        string email = GameObject.Find("if_Email").GetComponent<TMP_InputField>().text;
        string password = GameObject.Find("if_Password").GetComponent<TMP_InputField>().text;
    }

    void Start()
    {
        _loginButton.onClick.AddListener(RegisterUser);
    }

    async void RegisterUser()
    {
        _errorText.text = "";

        var auth = FirebaseAuth.DefaultInstance;
        string email = _emailInputField.text;
        string password = _passwordInputFiel.text;

        try
        {
            var result = await auth.SignInWithEmailAndPasswordAsync(email, password);

            Debug.LogFormat("User signed in successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);

            _emailInputField.text = "";
            _passwordInputFiel.text = "";

            _errorText.text = "¡A Jugar!";
            _errorText.color = new Color32(32, 140, 0, 255);
            await System.Threading.Tasks.Task.Yield();
            await System.Threading.Tasks.Task.Delay(2000);
            _errorText.text = "";
        }
        catch (FirebaseException firebaseEx)
        {
            _errorText.color = new Color32(180, 0, 69, 255);
            var errorCode = (AuthError)firebaseEx.ErrorCode;
            string message = GetErrorMessage(errorCode);
            Debug.LogError("Firebase error: " + message);
            _errorText.text = message;
        }
        catch (Exception ex)
        {
            Debug.LogError("General login error: " + ex.Message);
            _errorText.color = new Color32(180, 0, 69, 255);
            _errorText.text = "Error al iniciar sesión.";
        }
    }

    private void ShowError(string message)
    {
        Debug.LogError(message);
        _errorText.text = message;
    }

    private static string GetErrorMessage(AuthError errorCode)
    {
        switch (errorCode)
        {
            case AuthError.AccountExistsWithDifferentCredentials:
                return "Ya existe una cuenta con credenciales diferentes.";
            case AuthError.MissingPassword:
                return "Hace falta el password.";
            case AuthError.WeakPassword:
                return "El password es débil.";
            case AuthError.WrongPassword:
                return "El password es incorrecto.";
            case AuthError.EmailAlreadyInUse:
                return "Ya existe una cuenta con ese correo electrónico.";
            case AuthError.InvalidEmail:
                return "Correo electrónico inválido.";
            case AuthError.MissingEmail:
                return "Hace falta el correo electrónico.";
            case AuthError.UserNotFound:
                return "No se encontró ninguna cuenta con ese correo.";
            case AuthError.UserDisabled:
                return "Esta cuenta ha sido deshabilitada.";
            default:
                return "Ocurrió un error desconocido.";
        }
    }

    void Update()
    {

    }
}
