using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class MovementManager : MonoBehaviour
{
    public string movementsPath;
    private Dictionary<string, MovementRecord> records = new Dictionary<string, MovementRecord>();

    [Serializable]
    public class MovementRecord
    {
        public string reference;
        public string name;
        public string almacen;
        public int cantidad;
        public string lastDate;
    }

    void Awake()
    {
        string rutaBase = "/storage/emulated/0/Bodega BM/";
        string nombreCarpeta = "Salidas";
        string rutaCompleta = Path.Combine(rutaBase, nombreCarpeta);

        if (!Directory.Exists(rutaCompleta))
            Directory.CreateDirectory(rutaCompleta);

        movementsPath = Path.Combine(rutaCompleta, $"salidas {DateTime.Now.ToString("yyyy-MM-dd")}.csv");
        Debug.Log("MovementsPath inicializado en: " + movementsPath);

        if (!File.Exists(movementsPath))
        {
            File.WriteAllText(movementsPath, "Referencia,Nombre,Cantidad,Almacen,UltimaFecha\n");
            Debug.Log("Archivo creado en: " + movementsPath);
        }

        //movementsPath = Path.Combine(rutaCompleta + "/", "salidas.csv");

        LoadMovements();
    }

    void LoadMovements()
    {
        records.Clear();
        if (!File.Exists(movementsPath)) return;

        var lineas = File.ReadAllLines(movementsPath);

        for (int i = 1; i < lineas.Length; i++) // empieza en 1 para saltar encabezado
        {
            var parts = lineas[i].Split(';');
            if (parts.Length >= 5)
            {
                string key = parts[0] + "_" + parts[3];
                records[key] = new MovementRecord
                {
                    reference = parts[0],
                    name = parts[1],
                    cantidad = int.Parse(parts[2]),
                    almacen = parts[3],
                    lastDate = parts[4]
                };
            }
        }
    }

    public void AddMovement(string reference, string name, int cantidad, string almacen)
    {
        string key = reference + "_" + almacen;
        if (records.ContainsKey(key))
        {
            records[key].cantidad += cantidad;
            records[key].lastDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        else
        {
            records[key] = new MovementRecord
            {
                reference = reference,
                name = name,
                cantidad = cantidad,
                almacen = almacen,
                lastDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
        }
        SaveMovements();
    }

    void SaveMovements()
    {
        using (StreamWriter writer = new StreamWriter(movementsPath, false))
        {
            // Escribir encabezado
            writer.WriteLine("Referencia;Nombre;Cantidad;Almacen;UltimaFecha");

            foreach (var rec in records.Values)
                writer.WriteLine($"{rec.reference};{rec.name};{rec.cantidad};{rec.almacen};{rec.lastDate}");
        }
    }

    public List<MovementRecord> GetRecords()
    {
        return new List<MovementRecord>(records.Values);
    }

    // Exportar con nombre personalizado
    public void ExportMovements(string exportFileName)
    {
        if (string.IsNullOrEmpty(movementsPath))
        {
            FindObjectOfType<UIManager>()?.LogMessage("ERROR: movementsPath está vacío, no se puede exportar.");
            return;
        }

        string rutaBase = "/storage/emulated/0/Bodega BM/";
        string nombreCarpeta = "Exportadas";
        string rutaCompleta = Path.Combine(rutaBase, nombreCarpeta);

        if (!Directory.Exists(rutaCompleta))
            Directory.CreateDirectory(rutaCompleta);

        string exportPath = Path.Combine(rutaCompleta, exportFileName + DateTime.Now.ToString("yyyy-MM-dd") + ".csv");

        try
        {
            File.Copy(movementsPath, exportPath, true);
            FindObjectOfType<UIManager>()?.LogMessage("Archivo exportado en: " + exportPath);
        }
        catch (System.Exception ex)
        {
            FindObjectOfType<UIManager>()?.LogMessage("Error al exportar: " + ex.Message);
        }
    }
}
