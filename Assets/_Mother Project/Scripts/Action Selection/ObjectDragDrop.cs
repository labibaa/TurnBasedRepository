using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectDragDrop : MonoBehaviour, IPointerDownHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    CanvasGroup canvasGroup;
    Vector2 startPosition;
    public Transform originalParent { get; private set; }
    public int originalSiblingIndex { get; private set; }
    Transform mainPanel;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        startPosition = rectTransform.anchoredPosition ;
        originalParent = transform.parent;
        originalSiblingIndex = transform.GetSiblingIndex();
        mainPanel = ActionSpawner.Instance.mainPanel;

    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.SetParent(mainPanel.transform, true);
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (transform.parent == mainPanel.transform)
        {
            // Return to grid parent
            transform.SetParent(originalParent, false);
            transform.SetSiblingIndex(originalSiblingIndex);

            // Reset position (or drop somewhere new if you want)
            rectTransform.anchoredPosition = startPosition;
        }

        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
       
    }
}
