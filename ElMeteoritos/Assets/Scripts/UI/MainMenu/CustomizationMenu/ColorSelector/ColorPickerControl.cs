using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ColorPickerControl : MonoBehaviour
{
    public float currentHue, currentSat, currentVal;

    [SerializeField]
    private RawImage hueImage, satValImage, outputImage;

    [SerializeField]
    private Slider hueSlider;

    private Texture2D hueTexture, svTexture, outputTexture;

    [SerializeField]
    private GameObject changeThisColor;

    [SerializeField] private SVImageControl svImageControl;

    public Color currentColor;

    private void Awake()
    {
        svImageControl = GetComponentInChildren<SVImageControl>();

        CreateHueImage();
        CreateSVImage();
        UpdateOutputImage();
    }

    private void CreateHueImage()
    {
        hueTexture = new Texture2D(1, 16)
        {
            wrapMode = TextureWrapMode.Clamp,
            name = "HueTexture"
        };

        for (int i = 0; i < hueTexture.height; i++)
        {
            hueTexture.SetPixel(0, i, Color.HSVToRGB((float)i / hueTexture.height, 1, 1f));
        }
        hueTexture.Apply();
        currentHue = 0;
        hueImage.texture = hueTexture;
    }

    private void CreateSVImage()
    {
        svTexture = new Texture2D(16, 16)
        {
            wrapMode = TextureWrapMode.Clamp,
            name = "SatValTexture"
        };

        for (int y = 0; y < svTexture.height; y++)
        {
            for(int x = 0; x < svTexture.width; x++)
            {
                svTexture.SetPixel(x, y, Color.HSVToRGB(currentHue, (float)x / svTexture.width, (float)y / svTexture.height));
            }
        }

        svTexture.Apply();
        currentSat = 0;
        currentVal = 0;

        satValImage.texture = svTexture;
    }

    private void CreateOutputImage()
    {
        outputTexture = new Texture2D(1, 16)
        {
            wrapMode = TextureWrapMode.Clamp,
            name = "OutputTexture"
        };

        Color currentColor = Color.HSVToRGB(currentHue, currentSat, currentVal);

        for (int i = 0; i < outputTexture.height; i++)
        {
            outputTexture.SetPixel(0, i, currentColor);
        }

        outputTexture.Apply();

        outputImage.texture = outputTexture;
    }

    public void SetChangeThisColor(GameObject newTarget) => changeThisColor = newTarget;

    private void UpdateOutputImage()
    {
        currentColor = Color.HSVToRGB(currentHue, currentSat, currentVal);

        if (changeThisColor.TryGetComponent<ParticleSystem>(out var PScomponent))
        {
            var main = PScomponent.main;
            main.startColor = currentColor;
        }
        else if (changeThisColor.TryGetComponent<SpriteRenderer>(out var SRcomponent))
        {
            SRcomponent.color = currentColor;
        }
        //changeThisColor.GetComponent<Image>().color = currentColor;
    }

    public void SetSV(float S, float V)
    {
        currentSat = S;
        currentVal = V;

        UpdateOutputImage();
    }

    public void UpdateSVImage()
    {
        currentHue = hueSlider.value;

        for(int y = 0; y < svTexture.height; y++)
        {
            for(int x = 0; x < svTexture.width; x++)
            {
                svTexture.SetPixel(x, y, Color.HSVToRGB(currentHue, (float)x / svTexture.width, (float)y / svTexture.height));
            }
        }

        svTexture.Apply();

        UpdateOutputImage();
    }

    public void SetColor(Color color)
    {
        currentColor = color;
        Color.RGBToHSV(color, out currentHue, out currentSat, out currentVal);

        if (svTexture == null)
            CreateSVImage();

        hueSlider.onValueChanged.RemoveAllListeners();

        hueSlider.value = currentHue;
        svImageControl.SetPickerPosition(currentSat, currentVal);

        UpdateSVImage();
        UpdateOutputImage();

        hueSlider.onValueChanged.AddListener(delegate { UpdateSVImage(); });
    }
}
