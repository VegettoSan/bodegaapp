using UnityEngine;
using TMPro;
using System.Collections.Generic;

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
            //FindObjectOfType<UIManager>()?.LogMessage("Limpiando Lista");
        }

        // crear nuevos
        foreach (var r in registros)
        {
            GameObject item = Instantiate(itemPrefab, contentParent);
            TextMeshProUGUI texto = item.GetComponentInChildren<TextMeshProUGUI>();

            texto.text = $"{r.Linea} | {r.Referencia} | {r.Nombre} | {r.Cantidad} | {r.Almacen} | {r.UltimaFecha}";

            //FindObjectOfType<UIManager>()?.LogMessage("Creando Item : " + r.ToString());
        }
    }
}
