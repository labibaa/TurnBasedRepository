using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class EndTurnGlow : MonoBehaviour
{
    public float hoverScale = 1.2f;
    public float hoverOffset = 20f;
    public Color startColor = Color.white;
    public Color endColor = Color.red;
    public float duration = 1.0f;

    private Image buttonImage;
    public Vector3 minScale = new Vector3(1.0f, 1.0f, 1.0f);
    public Vector3 maxScale = new Vector3(1.2f, 1.2f, 1.2f);
    private Color initialColor;

    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        buttonImage = GetComponent<Image>();
        
        if (buttonImage != null)
        {
            initialColor = buttonImage.color;

        }
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        float t = Mathf.PingPong(timer / duration, 1.0f);
        if (HandleTurnNew.instance.turnsToBePerformed.Count > 0)
        {
            
            buttonImage.color = Color.Lerp(startColor, endColor, t);
            transform.localScale = Vector3.Lerp(minScale, maxScale, t);


        }
        else
        {
            buttonImage.color = initialColor;
        }
    }
}
