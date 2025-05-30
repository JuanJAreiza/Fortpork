using System.Collections;
using UnityEngine;

public class StartScreenManager : MonoBehaviour
{
    [SerializeField] private GameObject _panelAuth;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float fadeDuration = 1f;

    private static bool introMostrada = false;
    private bool hasTouched = false;

    void Start()
    {
        if (introMostrada)
        {
            gameObject.SetActive(false);
            _panelAuth.SetActive(true);
        }
    }

    void Update()
    {
        if (!hasTouched && Input.GetMouseButtonDown(0))
        {
            hasTouched = true;
            introMostrada = true;
            Debug.Log("Touched");
            StartCoroutine(FadeOutAndSwitch());
        }
    }

    IEnumerator FadeOutAndSwitch()
    {
        float elapsedTime = 0f;
        float startAlpha = _canvasGroup.alpha;

        while (elapsedTime < fadeDuration)
        {
            float t = elapsedTime / fadeDuration;
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }
}
