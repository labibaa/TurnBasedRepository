using UnityEngine;
using System.Collections;

public class HoverEffectController : MonoBehaviour
{
    public GameObject virtualCamera;
    public HoverEffectCharacter[] characters;
    

   // private float timeToCameraGoBackDefault = 3f;

    public bool isReady = false; // To track if the script is ready to handle mouse events

    private void Start()
    {
        StartCoroutine(DelayMouseEnterActivation(0.5f)); // Start the delay coroutine
    }

    private IEnumerator DelayMouseEnterActivation(float delay)
    {
        yield return new WaitForSeconds(delay);
        isReady = true; // Enable mouse events after the delay
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            DisableHoverEffect();
        }
    }


    public void EnableHoverEffect(HoverEffectCharacter characterToFocus)
    {
        //isReady = false;
        foreach (HoverEffectCharacter character in characters)
        {
            character.gameObject.SetActive(false);
        }
        characterToFocus.gameObject.SetActive(true);
        characterToFocus.CharacterSelected();
        Invoke("NotReady", 1.5f);
    }

    public void DisableHoverEffect()
    {
       
        foreach (HoverEffectCharacter character in characters)
        {
            character.virtualCamera.SetActive(false);
            character.gameObject.SetActive(true);
            character.canvasToEnable.gameObject.SetActive(false);
            character.ResetAnimation();
        }
        
       
        Invoke("GetReady", 1f);
    }

    public void GetReady()
    {
        isReady = true;
    }

    public void NotReady()
    {
        isReady = false;
    }
}
