using System;
using System.Collections.Generic;
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
    public TMP_Dropdown fileDropdown;
    public TMP_Text debugText;

    [Header("Busqueda Productos")]
    public TMP_InputField searchInput;
    public UIListaCSV listaProductos;

    void Start()
    {
        LoadAvailableFiles();
    }

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

        MessageToast.Instance.ShowMessage($"Guardado: {reference} - {name} - {cantidad} - {almacen}");
        LoadAvailableFiles();
    }

    public void LoadAvailableFiles()
    {
        fileDropdown.ClearOptions();
        List<string> options = new List<string>();

        string basePath = "/storage/emulated/0/Bodega BM/";
        string[] folders = { "Salidas", "Exportados", "Salidas Bodega" };

        foreach (var folder in folders)
        {
            string dir = Path.Combine(basePath, folder);
            if (Directory.Exists(dir))
            {
                foreach (var file in Directory.GetFiles(dir, "*.csv"))
                {
                    // ⛔ ignorar backups
                    if (file.Contains("-bak-"))
                        continue;

                    string fileName = Path.GetFileNameWithoutExtension(file); // MiArchivo-2025-09-09
                    string displayName = fileName;

                    // si tiene formato con fecha al final
                    int idx = fileName.LastIndexOf('-');
                    if (idx > 0)
                    {
                        string maybeDate = fileName.Substring(idx + 1); // 2025-09-09
                        if (DateTime.TryParse(maybeDate, out DateTime fecha))
                        {
                            string baseName = fileName.Substring(0, idx); // MiArchivo
                            string diaMes = fecha.ToString("dd-MM");       // 09-09
                            displayName = baseName + "-" + diaMes;
                        }
                    }

                    options.Add(folder + "/" + displayName);
                }
            }
        }

        fileDropdown.AddOptions(options);
        fileDropdown.onValueChanged.AddListener(OnFileSelected);
    }

    void OnFileSelected(int index)
    {
        string option = fileDropdown.options[index].text;
        string basePath = "/storage/emulated/0/Bodega BM/";

        // reconstruimos la ruta real (con año incluido)
        string folder = option.Split('/')[0];
        string displayName = option.Split('/')[1];

        // buscar el archivo real en la carpeta correspondiente
        string dir = Path.Combine(basePath, folder);
        foreach (var file in Directory.GetFiles(dir, "*.csv"))
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            if (fileName.Contains(displayName)) // coincide con el nombre mostrado
            {
                movementManager.SetSelectedFile(file);

                // mostrar registros en la UI
                csvManager.CargarCSV(File.ReadAllLines(file));
                listaCsv.MostrarRegistros(csvManager.registros);
                break;
            }
        }
    }

    public void OnExport()
    {
        string exportName = exportNameInput.text.Trim();
        movementManager.SaveToExportOrSelected(exportName);
        LoadAvailableFiles();
    }

    public void UpdateName(string result)
    {
        var name = FindObjectOfType<ProductManager>()?.GetProductName(result);
        if (name != null)
        {
            nameInput.text = name;
            MessageToast.Instance.ShowMessage("Nombre de producto: " + name);
        }
        else if (result == null || string.IsNullOrEmpty(referenceInput.text))
        {
            nameInput.text = "";
            MessageToast.Instance.ShowMessage("Nombre de no encontrado producto");
        }
    }

    public void UpdateList()
    {
        //FindObjectOfType<UIManager>()?.LogMessage(File.ReadAllText(movementManager.movementsPath));
        csvManager.CargarCSV(File.ReadAllLines(movementManager.movementsPath));
        listaCsv.MostrarRegistros(FindObjectOfType<CSVManager>()?.registros);
    }
    public void UpdateListBaceData()
    {
        //FindObjectOfType<UIManager>()?.LogMessage(File.ReadAllText(movementManager.movementsPath));
        csvManager.CargarCSVBaseData(File.ReadAllLines(movementManager.movementsPath));
        listaProductos.MostrarProductos(FindObjectOfType<CSVManager>()?.productos);
    }

    public void OnSearchProduct()
    {
        string query = searchInput.text.Trim();
        if (string.IsNullOrEmpty(query))
        {
            MessageToast.Instance.ShowMessage("Escribe algo para buscar.");
            return;
        }

        // ✅ ahora devuelve List<Producto>
        var resultados = productManager.Buscar(query);

        if (resultados.Count > 0)
        {
            listaProductos.MostrarProductos(resultados); // ✅ recibe List<Producto>
            MessageToast.Instance.ShowMessage(resultados.Count + " productos encontrados.");
        }
        else
        {
            MessageToast.Instance.ShowMessage("No se encontraron coincidencias.");
            listaProductos.MostrarProductos(new List<Producto>()); // ✅ lista vacía de Producto
        }
    }

    // ✅ corregido: ahora recibe List<Producto>, no tuplas
    void MostrarResultadosProductos(List<Producto> productos)
    {
        listaProductos.MostrarProductos(productos);
    }
}
