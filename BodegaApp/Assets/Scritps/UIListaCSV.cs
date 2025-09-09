using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using System;

public class UIListaCSV : MonoBehaviour
{
    public GameObject itemPrefab;      // Prefab con 1 TMP_Text
    public Transform contentParent;    // El Content del ScrollView

    public void MostrarRegistros(List<RegistroCSV> registros)
    {
        // limpiar los anteriores
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < registros.Count; i++)
        {
            var r = registros[i];
            GameObject item = Instantiate(itemPrefab, contentParent);
            TextMeshProUGUI texto = item.GetComponentInChildren<TextMeshProUGUI>();

            texto.text = $"{r.Linea} | {r.Referencia} | {r.Nombre} | {r.Cantidad} | {r.Almacen} | {r.UltimaFecha}";

            // buscar el botón y asignar acción
            Button deleteBtn = item.GetComponentInChildren<Button>();
            if (deleteBtn != null)
            {
                int index = i; // capturar índice
                deleteBtn.onClick.AddListener(() =>
                {
                    EliminarRegistro(index);
                });
            }
        }
    }

    void EliminarRegistro(int index)
    {
        var csv = FindObjectOfType<CSVManager>();
        var mm = FindObjectOfType<MovementManager>();

        if (index >= 0 && index < csv.registros.Count)
        {
            // Guardar la fila eliminada en backup
            var eliminado = csv.registros[index];
            GuardarBackup(mm.selectedFilePath, eliminado);

            // Quitar de la lista
            csv.registros.RemoveAt(index);

            // Recalcular líneas
            csv.RecalcularLineas();

            // Reescribir archivo actualizado
            using (StreamWriter writer = new StreamWriter(mm.selectedFilePath, false))
            {
                writer.WriteLine("Referencia;Nombre;Cantidad;Almacen;UltimaFecha");
                foreach (var rec in csv.registros)
                {
                    writer.WriteLine($"{rec.Referencia};{rec.Nombre};{rec.Cantidad};{rec.Almacen};{rec.UltimaFecha}");
                }
            }

            // refrescar UI
            MostrarRegistros(csv.registros);
            MessageToast.Instance.ShowMessage("Fila eliminada y enviada a backup.");
        }
    }

    void GuardarBackup(string originalPath, RegistroCSV eliminado)
    {
        // Carpeta Backups
        string basePath = "/storage/emulated/0/Bodega BM/";
        string backupDir = Path.Combine(basePath, "Backups");
        if (!Directory.Exists(backupDir))
            Directory.CreateDirectory(backupDir);

        // Nombre del archivo backup basado en el original
        string name = Path.GetFileNameWithoutExtension(originalPath); // Inventario-2025-09-09
        string backupName = name + "-bak.csv";
        string backupPath = Path.Combine(backupDir, backupName);

        bool nuevoArchivo = !File.Exists(backupPath);

        using (StreamWriter writer = new StreamWriter(backupPath, true))
        {
            if (nuevoArchivo)
            {
                writer.WriteLine("Linea;Referencia;Nombre;Cantidad;Almacen;UltimaFecha");
            }
            writer.WriteLine($"{eliminado.Linea};{eliminado.Referencia};{eliminado.Nombre};{eliminado.Cantidad};{eliminado.Almacen};{eliminado.UltimaFecha}");
        }
    }

    public void MostrarProductos(List<Producto> productos)
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        foreach (var p in productos)
        {
            GameObject item = Instantiate(itemPrefab, contentParent);
            TextMeshProUGUI texto = item.GetComponentInChildren<TextMeshProUGUI>();
            // buscar el botón y asignar acción
            Button deleteBtn = item.GetComponentInChildren<Button>();
            if (deleteBtn != null)
            {
                Destroy(deleteBtn.gameObject);
            }

            texto.text = $"{p.referencia} | {p.nombre} | {p.precioVenta} | {p.codigoInterno}";
        }
    }

}
