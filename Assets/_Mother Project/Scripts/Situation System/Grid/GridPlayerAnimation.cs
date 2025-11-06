using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class GridPlayerAnimation : MonoBehaviour
{

    Animator animator;
    private int animIDSpeed;
    private int animIDMotionSpeed;
    CharacterController controller;

    //Audio

    public AudioClip[] FootStepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        AssignAnimationID();
    }

    private void AssignAnimationID()
    {
        animIDSpeed = Animator.StringToHash("Speed");
        animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }
    // Controls player animation in a grid-based movement system.
    public void SetMoveAnimation(float animValue,int animMotionValue)
    {
        animator.SetFloat(animIDSpeed,animValue);
        animator.SetFloat(animIDMotionSpeed,animMotionValue);
    }
    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (FootStepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootStepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootStepAudioClips[index], transform.TransformPoint(controller.center), FootstepAudioVolume);
            }
        }
    }
}
