using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public ProductManager productManager;
    public MovementManager movementManager;
    public CSVManager csvManager;
    public UIListaCSV listaCsv;

    [Header("UI Inputs")]
    public TMP_InputField referenceInput;
    public TMP_InputField nameInput;
    public TMP_InputField quantityInput;
    public TMP_Dropdown almacenDropdown;
    public TMP_InputField exportNameInput;
    public TMP_Text debugText;

    private void Update()
    {
        if (string.IsNullOrEmpty(referenceInput.text))
        {
            nameInput.text = string.Empty;
        }
    }
    public void OnSaveMovement()
    {
        string reference = referenceInput.text.Trim();
        string name = productManager.GetProductName(reference);
        if (string.IsNullOrEmpty(name))
        {
            name = nameInput.text.Trim();
            productManager.AddProduct(reference, name);
        }

        int cantidad = int.Parse(quantityInput.text.Trim());
        string almacen = almacenDropdown.options[almacenDropdown.value].text;

        movementManager.AddMovement(reference, name, cantidad, almacen);

        LogMessage($"Guardado: {reference} - {name} - {cantidad} - {almacen}");
    }

    public void OnExport()
    {
        string exportName = exportNameInput.text.Trim();
        if (!string.IsNullOrEmpty(exportName))
            movementManager.ExportMovements(exportName);
    }

    public void UpdateName(string result)
    {
        var name = FindObjectOfType<ProductManager>()?.GetProductName(result);
        if (name != null)
        {
            nameInput.text = name;
            LogMessage("Nombre de producto: " + name);
        }
        else if (result == null || string.IsNullOrEmpty(referenceInput.text))
        {
            nameInput.text = "";
            LogMessage("Nombre de no encontrado producto");
        }
    }

    public void UpdateList()
    {
        //FindObjectOfType<UIManager>()?.LogMessage(File.ReadAllText(movementManager.movementsPath));
        csvManager.CargarCSV(File.ReadAllLines(movementManager.movementsPath));
        listaCsv.MostrarRegistros(FindObjectOfType<CSVManager>()?.registros);
    }

    public void LogMessage(string message)
    {
        Debug.Log(message); // También lo manda a la consola de Unity
        if (debugText != null)
        {
            debugText.text += "\n" + message;  // Lo escribe en pantalla
        }
    }
}
