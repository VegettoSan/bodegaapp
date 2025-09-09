using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;

[System.Serializable]
public class Producto
{
    public string referencia;
    public string nombre;
    public string precioVenta;
    public string codigoInterno;
}

public class ProductManager : MonoBehaviour
{
    private string productsPath;
    private Dictionary<string, string> products = new Dictionary<string, string>();
    private List<Producto> productos = new List<Producto>();

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
            //MessageToast.Instance.ShowMessage("Carpeta creada en: " + rutaCompleta);
        }
        else
        {
            //MessageToast.Instance.ShowMessage("La carpeta ya existe en: " + rutaCompleta);
        }
        productsPath = Path.Combine(rutaCompleta + "/", "data.csv");

        LoadProducts();
    }

    void LoadProducts()
    {
        productos.Clear();
        if (!File.Exists(productsPath))
        {
            //MessageToast.Instance.ShowMessage("Archivo Base no encontrado en " + productsPath);
            Invoke("LoadProducts",3f);
            return;
        }

        //MessageToast.Instance.ShowMessage("Archivo Base encontrado en " + productsPath);

        var lineas = File.ReadAllLines(productsPath);
        for (int i = 1; i < lineas.Length; i++) // saltar encabezado
        {
            var parts = lineas[i].Split(';');
            if (parts.Length >= 4)
            {
                productos.Add(new Producto
                {
                    referencia = parts[0].Trim(),
                    nombre = parts[1].Trim(),
                    precioVenta = parts[2].Trim(),
                    codigoInterno = parts[3].Trim()
                });
            }
        }
        foreach (var line in lineas)
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

    // 🔍 Buscar por referencia o nombre
    public List<Producto> Buscar(string query)
    {
        query = query.ToLower();
        var results = new List<Producto>();
        foreach (var p in productos)
        {
            if (p.referencia.ToLower().Contains(query) ||
                p.nombre.ToLower().Contains(query))
            {
                results.Add(p);
            }
        }
        return results;
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
                writer.WriteLine($"{kvp.Key};{kvp.Value}");
        }
    }

    public List<(string referencia, string nombre)> BuscarPorReferencia(string refText)
    {
        var results = new List<(string, string)>();
        foreach (var kvp in products)
        {
            if (kvp.Key.Contains(refText, System.StringComparison.OrdinalIgnoreCase))
                results.Add((kvp.Key, kvp.Value));
        }
        return results;
    }

    public List<(string referencia, string nombre)> BuscarPorNombre(string nameText)
    {
        var results = new List<(string, string)>();
        foreach (var kvp in products)
        {
            if (kvp.Value.Contains(nameText, System.StringComparison.OrdinalIgnoreCase))
                results.Add((kvp.Key, kvp.Value));
        }
        return results;
    }
}