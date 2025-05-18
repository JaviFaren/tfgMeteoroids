using UnityEngine;
using UnityEngine.UI;

public class UISkinSelector : MonoBehaviour
{
    public int skinID;

    [SerializeField] private Image skinIMG;
    [SerializeField] private Image backgroundIMG;

    public CustomizationField customizationField;

    [SerializeField] private bool isSelected;

    private UICustomizationMenuManager customizationManager;

    public void Setup(int skinID, Sprite skinSprite, CustomizationField customizationField, UICustomizationMenuManager manager)
    {
        this.skinID = skinID;
        this.skinIMG.sprite = skinSprite;
        this.customizationField = customizationField;
        this.customizationManager = manager;

        SetSelected(false);
    }

    public void OnClick()
    {
        customizationManager.OnSkinSelected(this);
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;

        backgroundIMG.color = isSelected ? Color.green : Color.grey;
    }
}
