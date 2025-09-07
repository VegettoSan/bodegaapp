using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using UnityEngine;
using UnityEngine.Android;

public class ProductManager : MonoBehaviour
{
    private string productsPath;
    private Dictionary<string, string> products = new Dictionary<string, string>();

    void Awake()
    {

        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
        }

        //productsPath = Path.Combine(Application.persistentDataPath, "productos.csv");
        // Ruta a la memoria compartida
        string rutaBase = "/storage/emulated/0/";

        // Carpeta que quieres crear
        string nombreCarpeta = "Bodega BM";

        // Ruta completa
        string rutaCompleta = Path.Combine(rutaBase, nombreCarpeta);

        // Verificar si existe
        if (!Directory.Exists(rutaCompleta))
        {
            Directory.CreateDirectory(rutaCompleta);
            FindObjectOfType<UIManager>()?.LogMessage("Carpeta creada en: " + rutaCompleta);
        }
        else
        {
            FindObjectOfType<UIManager>()?.LogMessage("La carpeta ya existe en: " + rutaCompleta);
        }
        productsPath = Path.Combine(rutaCompleta + "/", "productos.csv");

        LoadProducts();
    }

    void LoadProducts()
    {
        products.Clear();
        if (!File.Exists(productsPath))
        {
            FindObjectOfType<UIManager>()?.LogMessage("Archivo Base no encontrado en " + productsPath);
            return;
        }

        FindObjectOfType<UIManager>()?.LogMessage("Archivo Base encontrado en " + productsPath);
        //FindObjectOfType<UIManager>()?.LogMessage(File.ReadAllText(productsPath));
        
        foreach (var line in File.ReadAllLines(productsPath))
        {
            var parts = line.Split(';');
            if (parts.Length >= 2)
            {
                string reference = parts[0].Trim();
                string name = parts[1].Trim();
                if (!products.ContainsKey(reference))
                    products.Add(reference, name);
            }
        }
    }

    public string GetProductName(string reference)
    {
        return products.ContainsKey(reference) ? products[reference] : null;
    }

    public void AddProduct(string reference, string name)
    {
        if (!products.ContainsKey(reference))
        {
            products.Add(reference, name);
            SaveProducts();
        }
    }

    void SaveProducts()
    {
        using (StreamWriter writer = new StreamWriter(productsPath, false))
        {
            foreach (var kvp in products)
                writer.WriteLine($"{kvp.Key},{kvp.Value}");
        }
    }
}
