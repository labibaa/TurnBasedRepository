using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    public float interactRadius ;
    public Collider[] collidersBuffer = new Collider[10]; // Adjust the buffer size as needed   

    [SerializeField] InteractionPromptUI promptUI;
    void Update()
    {
        //add the interactor to main characters and the itemclass(interactable objects)
        //to the GO that are interactacble and change their layer to Interactable

        var nearestInteractableGameObject = GetNearInteractableObject();

        if(nearestInteractableGameObject == null )
        {
            promptUI.closePopUp();
            return;
        }
        else
        {
            promptUI.SetInteractionPopUp();
        }
        if(Input.GetKeyDown( KeyCode.I ) )
        {
            
            if( nearestInteractableGameObject != null )
            {
                nearestInteractableGameObject.Interact();

               
            }

        }
     
    }
   
    private IInteractable GetNearInteractableObject()
    { 
        //GameObject nearInteractableObject = null;
        Vector3 playerPosition = transform.position;

         // Get the number of colliders in the interactable layer within the specified radius
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, interactRadius, collidersBuffer, LayerMask.GetMask("Interactable"));



        float shortestDistance = float.MaxValue;
        IInteractable closestInteractable = null;

        for (int i = 0; i < numColliders; i++)
        {
            IInteractable interactable = collidersBuffer[i].GetComponent<IInteractable>();
            if (interactable != null)
            {
                float distance = Vector3.Distance(transform.position, collidersBuffer[i].transform.position);

                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    closestInteractable = interactable;
                }
            }
        }


        return closestInteractable;

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }






}
