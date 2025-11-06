using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Cinemachine;

public class FlyingText : MonoBehaviour
{
    public TMP_Text popupText;
    public float popupDuration = 0.01f;
    public float popupHeight = 1f;

    private CinemachineVirtualCameraBase activeGridCam;
    CinemachineBrain brain;

    private void Start()
    {
         brain = Camera.main.GetComponent<CinemachineBrain>();
        SelectActiveGridCam();
    }

    private void Update()
    {
        SelectActiveGridCam();
        transform.LookAt(transform.position + activeGridCam.transform.forward);
    }

    private void SelectActiveGridCam()
    {
       
        if (brain != null)
        {
            CinemachineVirtualCameraBase activeCam = (CinemachineVirtualCameraBase)brain.ActiveVirtualCamera;
            activeGridCam = activeCam;
        }
      
    }

    public void FlyTextUpward(string text, Color color)
    {
        popupText.text = text;

        popupText.transform.localScale = Vector3.zero; // Start with zero scale
        popupText.color = color;

        float scaleDuration = 0.2f; // Adjust as needed for the desired speed

        Sequence popSequence = DOTween.Sequence();

        // Start small
        popupText.transform.localScale = Vector3.zero;

        // Scale up to bigger
        popSequence.Append(popupText.transform.DOScale(Vector3.one * 1.0f, scaleDuration).SetEase(Ease.OutExpo));

        // Wait for a short duration
        popSequence.AppendInterval(0.2f);

        // Scale down back to smaller
        popSequence.Append(popupText.transform.DOScale(Vector3.zero, scaleDuration).SetEase(Ease.InExpo));

        // Complete action
        popSequence.OnComplete(() =>
        {
            Destroy(gameObject); // Destroy the object after the animation completes
        });

        // Play the sequence
        popSequence.Play();
    }
}
