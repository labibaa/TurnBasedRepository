using UnityEngine;
using TMPro;

public class BlinkText : MonoBehaviour
{
    private TextMeshProUGUI textMeshPro;
    public float blinkSpeed = 1.0f; // Speed of blinking
    
    private bool isBlinking = true;
    private float alphaValue = 1.0f;
    private bool fadingOut = true;

    void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();

    }

    void Update()
    {
        if (isBlinking)
        {
            Blink();
        }
    }

    void Blink()
    {
        if (fadingOut)
        {
            alphaValue -= Time.deltaTime * blinkSpeed;
            if (alphaValue <= 0)
            {
                alphaValue = 0;
                fadingOut = false;
            }
        }
        else
        {
            alphaValue += Time.deltaTime * blinkSpeed;
            if (alphaValue >= 1)
            {
                alphaValue = 1;
                fadingOut = true;
            }
        }

        Color color = textMeshPro.color;
        color.a = alphaValue;
        textMeshPro.color = color;
    }
}
