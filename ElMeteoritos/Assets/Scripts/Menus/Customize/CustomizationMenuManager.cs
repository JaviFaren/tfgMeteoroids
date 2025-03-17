using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomizationMenuManager : MonoBehaviour
{
    [Header("Menus")]
    public GameObject shipColor;
    public ColorPickerControl colorPicker;

    [Header("Botones")]
    public Button saveShipColorBTN;

    [Header("Textos")]
    public TextMeshProUGUI customizationCategoryTMP;

    [Header("Imagenes")]
    public Image playerVisualizer;
}
