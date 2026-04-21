using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextFadeInOut : MonoBehaviour
{
    public static TextFadeInOut instance;
    public TextMeshProUGUI textMeshPro;
    public float fadeInDuration = 1.0f;
    public float fadeOutDuration = 1.0f;

    private Queue<string> textQueue = new Queue<string>();
    private bool isFading = false;
    private List<string> displayedLines = new List<string>();

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    void Start()
    {
        // Start with fully transparent text
        textMeshPro.alpha = 0f;
    }

    public void AddTextToQueue(Turn turn)
    {
        string newText = turn.Player.characterName + " used " + turn.Command.GetActionName();
        textQueue.Enqueue(newText);

        if (!isFading && textQueue.Count > 0)
        {
            DisplayNextText();
        }
    }

    private void DisplayNextText()
    {
        if (textQueue.Count > 0)
        {
            string nextText = textQueue.Dequeue();
            StartCoroutine(FadeInText(nextText));
        }
    }

    IEnumerator FadeInText(string newText)
    {
        isFading = true;

        displayedLines.Add(newText);
        textMeshPro.text = string.Join("\n", displayedLines);

        float fadeInTime = 0f;
        while (fadeInTime < fadeInDuration)
        {
            textMeshPro.alpha = Mathf.Lerp(0f, 1f, fadeInTime / fadeInDuration);
            fadeInTime += Time.deltaTime;
            yield return null;
        }

        textMeshPro.alpha = 1f;
        isFading = false;

        if (textQueue.Count > 0)
            DisplayNextText();
    }

    public void ClearText()
    {
        if (!isFading)
            StartCoroutine(FadeOutAndClearText());
    }

    IEnumerator FadeOutAndClearText()
    {
        isFading = true;

        // Fade out
        float fadeOutTime = 0f;
        while (fadeOutTime < fadeOutDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, fadeOutTime / fadeOutDuration);
            textMeshPro.alpha = alpha;
            fadeOutTime += Time.deltaTime;
            yield return null;
        }

        textMeshPro.alpha = 0f;

        textMeshPro.text = "";
        displayedLines.Clear();

        isFading = false;
    }

    public void RemoveLatestEntry()
    {
        if (!isFading && displayedLines.Count > 0)
        {
            displayedLines.RemoveAt(displayedLines.Count - 1);

            if (displayedLines.Count > 0)
            {
                StartCoroutine(FadeOutAndUpdateText(string.Join("\n", displayedLines)));
            }
            else
            {
                ClearText();
            }
        }
    }

    IEnumerator FadeOutAndUpdateText(string newText)
    {
        isFading = true;

        // Fade out
        float fadeOutTime = 0f;
        while (fadeOutTime < fadeOutDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, fadeOutTime / fadeOutDuration);
            textMeshPro.alpha = alpha;
            fadeOutTime += Time.deltaTime;
            yield return null;
        }

        // Update text
        textMeshPro.text = newText;

        // Fade in
        float fadeInTime = 0f;
        while (fadeInTime < fadeInDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, fadeInTime / fadeInDuration);
            textMeshPro.alpha = alpha;
            fadeInTime += Time.deltaTime;
            yield return null;
        }

        textMeshPro.alpha = 1f;

        isFading = false;
    }
}
