using UnityEngine;
using System.Collections;

public class HoverEffectCharacter : MonoBehaviour
{
   
    public Canvas canvasToEnable;
    public Animator animator;
    public string animationName;
    public string defaultAnimationName;


    public GameObject virtualCamera;


    [SerializeField] private HoverEffectController hoverController;



    private void OnMouseEnter()
    {
        if (hoverController.isReady)
        {
            Debug.Log("Mouse Entered");
            hoverController.EnableHoverEffect(this);
        }
    }

    private void OnMouseExit()
    {
        if (!hoverController.isReady)
        {
            Debug.Log("Mouse Exited");
            hoverController.DisableHoverEffect();
            ResetAnimation();
        }

    }

    public void CharacterSelected()
    {
        virtualCamera.SetActive(true);
        canvasToEnable.gameObject.SetActive(true);
        animator.Play(animationName);
    }


    public void ResetAnimation()
    {
        animator.Play(defaultAnimationName);
        animator.Update(0); // Force the animator to update the animation state
    }
}
