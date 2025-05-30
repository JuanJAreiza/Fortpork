using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DesplegarPanelRegistro : MonoBehaviour
{
    [Header("Panel principal")]
    public RectTransform panelRegistro;

    [Header("Texto 'Registro'")]
    public RectTransform textoRegistroTransform;
    public TextMeshProUGUI textoRegistro;

    [Header("Bot�n de volver")]
    public GameObject botonBack;

    [Header("Posiciones y tama�os")]
    public Vector2 posicionOculta = new Vector2(0, -300); // Ajusta seg�n tu layout
    public Vector2 posicionMostrada = new Vector2(0, 0);

    public Vector2 textoPosInicial = new Vector2(0, 20);
    public Vector2 textoPosFinal = new Vector2(0, 200);

    public int tama�oInicial = 54;
    public int tama�oFinal = 64;

    public float duracion = 0.5f;

    private bool desplegado = false;
    private Coroutine animacionActual;

    public void AlternarPanel()
    {
        if (animacionActual != null) StopCoroutine(animacionActual);
        animacionActual = StartCoroutine(AnimarPanel(!desplegado));
        desplegado = !desplegado;
    }

    IEnumerator AnimarPanel(bool mostrar)
    {
        float tiempo = 0f;

        Vector2 inicioPosPanel = panelRegistro.anchoredPosition;
        Vector2 destinoPosPanel = mostrar ? posicionMostrada : posicionOculta;

        Vector2 inicioPosTexto = textoRegistroTransform.anchoredPosition;
        Vector2 destinoPosTexto = mostrar ? textoPosFinal : textoPosInicial;

        float tama�oInicio = mostrar ? tama�oInicial : tama�oFinal;
        float tama�oDestino = mostrar ? tama�oFinal : tama�oInicial;

        if (mostrar) botonBack.SetActive(false); // Oculta antes de empezar

        while (tiempo < duracion)
        {
            float t = tiempo / duracion;

            panelRegistro.anchoredPosition = Vector2.Lerp(inicioPosPanel, destinoPosPanel, t);
            textoRegistroTransform.anchoredPosition = Vector2.Lerp(inicioPosTexto, destinoPosTexto, t);
            textoRegistro.fontSize = Mathf.Lerp(tama�oInicio, tama�oDestino, t);

            tiempo += Time.deltaTime;
            yield return null;
        }

        panelRegistro.anchoredPosition = destinoPosPanel;
        textoRegistroTransform.anchoredPosition = destinoPosTexto;
        textoRegistro.fontSize = tama�oDestino;

        botonBack.SetActive(mostrar);
    }
}
