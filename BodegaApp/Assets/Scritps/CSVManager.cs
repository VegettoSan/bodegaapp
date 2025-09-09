using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RegistroCSV
{
    public string Linea;
    public string Referencia;
    public string Nombre;
    public int Cantidad;
    public string Almacen;
    public string UltimaFecha;
}

public class CSVManager : MonoBehaviour
{
    public List<RegistroCSV> registros = new List<RegistroCSV>();
    public List<Producto> productos = new List<Producto>();

    public void CargarCSV(string[] lineas)
    {
        registros.Clear();
        //FindObjectOfType<UIManager>()?.LogMessage("Limpiando Registros");

        for (int i = 1; i < lineas.Length; i++) // desde 1 para saltar cabecera
        {
            string[] valores = lineas[i].Split(';');
            if (valores.Length >= 5)
            {
                RegistroCSV r = new RegistroCSV()
                {
                    Linea = i.ToString(),
                    Referencia = valores[0],
                    Nombre = valores[1],
                    Cantidad = int.Parse(valores[2]),
                    Almacen = valores[3],
                    UltimaFecha = valores[4]
                };
                registros.Add(r);
                //FindObjectOfType<UIManager>()?.LogMessage(r.ToString());
            }
        }
    }
    public void CargarCSVBaseData(string[] lineas)
    {
        productos.Clear();
        //FindObjectOfType<UIManager>()?.LogMessage("Limpiando Registros");

        for (int i = 1; i < lineas.Length; i++) // desde 1 para saltar cabecera
        {
            string[] valores = lineas[i].Split(';');
            if (valores.Length >= 3)
            {
                Producto p = new Producto()
                {
                    referencia = i.ToString(),
                    nombre = valores[0],
                    precioVenta = valores[1],
                    codigoInterno = valores[2]
                };
                productos.Add(p);
                //FindObjectOfType<UIManager>()?.LogMessage(r.ToString());
            }
        }
    }
    public void RecalcularLineas()
    {
        for (int i = 0; i < registros.Count; i++)
        {
            registros[i].Linea = (i + 1).ToString(); // 1-based
        }
    }
}