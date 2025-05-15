using UnityEngine;
using UnityEngine.UI;

public class UISkinSelector : MonoBehaviour
{
    [SerializeField] private int skinID;

    [SerializeField] private Image skinIMG;
    [SerializeField] private Image backgroundIMG;
    
    [SerializeField] private CustomizationField customizationField;

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
        UserSession.SetUserCustomizationValue(customizationField, skinID);
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;

        backgroundIMG.color = isSelected ? Color.green : Color.grey;
    }
}
