using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridHover : MonoBehaviour 

{
    Renderer gridRenderer;
    [SerializeField]
    Color gridColorOnHover;
    [SerializeField]
    Color gridColorExitHover;
    [SerializeField]
    public LayerMask layerMask;
   
    [SerializeField]
    LineRenderer _lineRenderer;
    Ray ray;
    RaycastHit hit;
    private Renderer lastHoveredRenderer;
    private SpriteRenderer lastHoveredSpriteRenderer;
    Camera _main;
    public GameObject coloredobject;
    #region Raycast
    [SerializeField]
    public LayerMask targetLayerMask;
    [SerializeField]
    public LayerMask nonCharacterTargetLayerMask;
    RaycastHit hitTarget;
    Ray rayTarget;
    #endregion

    private void Start()
    {
        _main = Camera.main;

        

        _lineRenderer.positionCount = 2;
    }
    public void RestoreColor()
    {
        if(coloredobject)
        {
            coloredobject.GetComponent<Renderer>().material.color = gridColorExitHover;
        }
       
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
          //  RestoreColor();
        }
     
        if (TempManager.instance.currentState == GameStates.TargetSelectionTurn)
        {
            TargetRaycast();
        }
        
        if (GridMovement.instance.inPathSelection)
        {
            MouseHoverFunction();
           // RenderLineActive();
        }
        else
        {
           // RenderLineDeactive();
        }
    }


    void TargetRaycast() //target selection while grid is on using mouse hover
    {
        
        if (Input.GetMouseButtonDown(0) && TempManager.instance.currentState == GameStates.TargetSelectionTurn)  // Change the input event as needed
        {
            
            rayTarget = _main.ScreenPointToRay(Input.mousePosition);
         
            if (Physics.Raycast(rayTarget, out hitTarget, Mathf.Infinity, targetLayerMask))
            {
                hitTarget.collider.GetComponent<TemporaryStats>().SelectTarget();
            }
            if (Physics.Raycast(rayTarget, out hitTarget, Mathf.Infinity, nonCharacterTargetLayerMask)) //object interaction
            {
                hitTarget.collider.GetComponent<INonCharacterTarget>().CommandInfo(TurnManager.instance.players[TurnManager.instance.currentPlayerIndex].GetComponent<CharacterBaseClasses>(),DAOScriptableObject.instance.GetImprovedActionData(StringData.directory, "Move"));
                hitTarget.collider.GetComponent<INonCharacterTarget>().CommandCreator();
            }
        }
    }


    void MouseHoverFunction()
    {
        ray = _main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            Renderer newHoveredRenderer = hit.collider.GetComponent<Renderer>();
            //SpriteRenderer newSpriteRenderer = hit.collider.GetComponent<SpriteRenderer>();
            if (newHoveredRenderer != null)
            {
                
                // Mouse is hovering over an object with the specified layer mask
                if (lastHoveredRenderer != null)
                {
                    // Restore the color of the last hovered object


                    lastHoveredRenderer.material.color = gridColorExitHover;
                    //lastHoveredSpriteRenderer.color = gridColorExitHover;
                }

                // Store the color of the new hovered object
               lastHoveredRenderer = newHoveredRenderer;
               //lastHoveredSpriteRenderer= newSpriteRenderer;
               gridColorExitHover = newHoveredRenderer.material.color;
               

                // Change the color of the new hovered object
               newHoveredRenderer.material.color = gridColorOnHover;
              // newSpriteRenderer.color = gridColorOnHover;
            }
            coloredobject = hit.collider.gameObject;
        }
        else
        {
            // Mouse is not over any object with the specified layer mask
            if (lastHoveredRenderer != null)
            {
                //Debug.Log("D");
                // Restore the color of the last hovered object
                lastHoveredRenderer.material.color = gridColorExitHover;
                lastHoveredRenderer = null;
            }
        }
    }
    
    void RenderLineActive()
    {
        Vector3 gridLineStat = TempManager.instance.attacker.GetComponent<TemporaryStats>().currentPlayerGridPosition;
        _lineRenderer.SetPosition(0,new Vector3(gridLineStat.x, gridLineStat.y, gridLineStat.z));
        Vector3 mouseScreenPos = Input.mousePosition;

        // Convert the mouse position from screen to world coordinates
        Vector3 mouseWorldPos = _main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 10f));

        // Update the Line Renderer position to follow the mouse
        if (hit.collider?.gameObject != null && GridMovement.instance.InAdjacentMatrix(TempManager.instance.attacker.GetComponent<TemporaryStats>().currentPlayerGridPosition, hit.collider.gameObject.transform.position, TempManager.instance.attacker.GetComponent<TemporaryStats>().CurrentDex))
        {
            
                _lineRenderer.SetPosition(1, new Vector3(hit.collider.transform.position.x, hit.collider.transform.position.y, hit.collider.transform.position.z));
            
          
        }
        
    }

    void RenderLineDeactive()
    {
        
        _lineRenderer.SetPosition(0,Vector3.zero);
        _lineRenderer.SetPosition(1, Vector3.zero);
    }

}
