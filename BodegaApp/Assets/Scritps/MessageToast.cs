using UnityEngine;
using TMPro;
using System.Collections;

public class MessageToast : MonoBehaviour
{
    public static MessageToast Instance { get; private set; }

    [Header("Config")]
    public TMP_Text messageText;   // Texto en UI
    public float showTime = 3f;    // Tiempo visible antes del fade
    public float fadeSpeed = 2f;   // Velocidad del desvanecimiento

    private Coroutine currentRoutine;

    void Awake()
    {
        // Configurar singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void ShowMessage(string msg)
    {
        if (messageText == null) return;

        // Pone el texto
        messageText.text = msg;

        // Si ya hay una rutina corriendo, reinicia
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowAndFade());
    }

    private IEnumerator ShowAndFade()
    {
        // Asegurar visibilidad total
        Color c = messageText.color;
        c.a = 1f;
        messageText.color = c;

        // Esperar tiempo fijo
        yield return new WaitForSeconds(showTime);

        // Fade out
        while (messageText.color.a > 0f)
        {
            c.a -= fadeSpeed * Time.deltaTime;
            messageText.color = c;
            yield return null;
        }

        currentRoutine = null;
    }
}
