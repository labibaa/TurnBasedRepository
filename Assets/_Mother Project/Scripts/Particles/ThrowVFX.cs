using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.Events;
using System;
using DG.Tweening;
using System.Net;

public class ThrowVFX : MonoBehaviour
{
    private Transform target;
    private GameObject targetAnimator;
    public float ThrowSpeed = 12.0f;
    public static event Action hitTrigger;
    public ParticleSystem particlePrefab;
    // public Vector3 controlPointOffset = new Vector3(0, 5, 0);
    public String hurtAnimation;
    bool Intarget;

    void Update()
    {
        if(target != null && !Intarget)
        {
            // Calculate the direction to the target
            Vector3 targetDirection = target.position - transform.position;

            // Calculate the rotation needed to look at the target
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            // Only rotate around the Y axis
            targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);

            // Smoothly rotate towards the target rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 20.0f * Time.deltaTime);

            transform.position = Vector3.MoveTowards(transform.position, target.position, ThrowSpeed * Time.deltaTime);
           
           
            if (Vector3.Distance(target.position, transform.position) <= 0.0001f)
            {
                if (gameObject.GetComponent<VisualEffect>())
                {
                    VisualEffect visualEffect = GetComponent<VisualEffect>();
                    visualEffect.SendEvent("hit");
                    visualEffect.SendEvent("stop");
                    StartCoroutine(DestroyTargetVFX(visualEffect));
                }
               
                //target.gameObject.GetComponent<Animator>().Play("Damage1");
                //AnimationPlay("Damage1", targetAnimator


                if (particlePrefab != null)
                {
                    ParticleSystem particle = Instantiate(particlePrefab,transform.position , particlePrefab.transform.rotation,transform);
                    particle.Play();
                }
               
                Intarget = true;
                hitTrigger?.Invoke();
                if(targetAnimator!= null )
                {
                    if(hurtAnimation != null)
                    {
                        targetAnimator.GetComponent<Animator>().Play(hurtAnimation);
                    }
                    else
                    {
                        targetAnimator.GetComponent<Animator>().Play("Damage1"); //change the hard coded animation 
                    }
                   
                }
               
            }
        }
       
      
    }

    public IEnumerator DestroyTargetVFX(VisualEffect desEffect)
    {
        yield return new WaitForSeconds(1f);
        Destroy(desEffect.gameObject);
        //  Destroy(hiteffect.gameObject);
        Debug.Log("destroyt");
    }
    public void SetTargetVFX(Transform newTarget)
    {
        target = newTarget;
    }
    public void SetTargetAnimator(GameObject newtargetAnimator)
    {
        targetAnimator = newtargetAnimator;
    }

    //public void tr()
    //{

    //    Debug.Log("fgfg");
    //    Vector3 startPos = transform.position;
    //    Vector3 midPoint = startPos + (target.position - startPos) / 2f + Vector3.up * 5f;

    //    transform.DOPath(new Vector3[] { startPos, midPoint, target.position }, 2f, PathType.CatmullRom)
    //        .SetEase(Ease.OutQuad); // You can adjust the ease type here

    //}
}
