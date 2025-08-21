using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ActionSlot : MonoBehaviour, IDropHandler
{

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            RectTransform dragged = eventData.pointerDrag.GetComponent<RectTransform>();
            // If slot already has a child, send it back to its grid
            if (transform.childCount > 0)
            {
                Transform existingChild = transform.GetChild(0);

                ObjectDragDrop existingScript = existingChild.GetComponent<ObjectDragDrop>();
                if (existingScript != null)
                {
                    // Re-parent existing child back to its grid parent
                    existingChild.SetParent(existingScript.originalParent);
                    existingChild.SetSiblingIndex(existingScript.originalSiblingIndex);
                    (existingChild as RectTransform).anchoredPosition = Vector2.zero;
                }
            }

            // Put inside slot
            dragged.SetParent(transform);
            dragged.anchoredPosition = Vector2.zero;
        }
    }
}
