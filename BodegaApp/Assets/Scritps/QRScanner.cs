using UnityEngine;
using UnityEngine.UI;
using ZXing;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Android;

public class QRScanner : MonoBehaviour
{
    [Header("UI Elements")]
    public RawImage cameraPreview;       // RawImage para mostrar la cámara
    public AspectRatioFitter aspectRatioFitter;
    public TMP_InputField referenceInput;    // Campo de texto donde va la referencia
    public TMP_InputField nameInput;    // Campo de texto donde va el nombre
    public Button toggleCameraButton;    // Botón para activar/desactivar cámara
    public TMP_Text toggleButtonText;        // Texto del botón (ej: "Activar cámara")

    private WebCamTexture camTexture;
    private bool isScanning = false;
    private IBarcodeReader reader;

    void Start()
    {
        reader = new BarcodeReader();

        if (toggleCameraButton != null)
            toggleCameraButton.onClick.AddListener(ToggleCamera);

        UpdateButtonText();
    }

    void ToggleCamera()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }

        if (isScanning)
        {
            StopCamera();
        }
        else
        {
            StartCamera();
        }

        UpdateButtonText();
    }

    void StartCamera()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length > 0)
        {
            camTexture = new WebCamTexture(devices[0].name);
            cameraPreview.texture = camTexture;
            camTexture.Play();

            isScanning = true;
            StartCoroutine(ScanContinuously());
        }
        else
        {
            Debug.LogError("No se encontró ninguna cámara en el dispositivo.");
            FindObjectOfType<UIManager>()?.LogMessage("No se encontró ninguna cámara en el dispositivo.");
        }
    }

    void StopCamera()
    {
        if (camTexture != null && camTexture.isPlaying)
        {
            camTexture.Stop();
        }

        isScanning = false;
    }

    IEnumerator ScanContinuously()
    {
        while (isScanning)
        {
            try
            {
                if (camTexture != null && camTexture.isPlaying)
                {
                    var result = reader.Decode(camTexture.GetPixels32(),
                                               camTexture.width,
                                               camTexture.height);

                    if (result != null)
                    {
                        // Poner el texto del QR en el campo editable
                        referenceInput.text = result.Text;
                        FindObjectOfType<UIManager>()?.LogMessage("Código detectado: " + result.Text);
                        referenceInput.onEndEdit.Invoke(referenceInput.text);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("Error al decodificar: " + ex.Message);
                FindObjectOfType<UIManager>()?.LogMessage("Error al decodificar: " + ex.Message);
            }

            yield return null; // esperar al siguiente frame
        }
    }

    void UpdateButtonText()
    {
        if (toggleButtonText != null)
        {
            toggleButtonText.text = isScanning ? "Desactivar cámara" : "Activar cámara";
        }
    }
}
