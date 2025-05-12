using DG.Tweening;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ActionNotification : MonoBehaviour
{
  
    public RectTransform notification;
    public TextMeshProUGUI ActionNotificationText;

    public float slideDuration = 1f;
    public float waitDuration = 1f;

 
    public void AnimateNotification(string actionName)
    {
        // Calculate the target position for the slide-in animation
        Vector2 targetPosition = new Vector2(0f, notification.anchoredPosition.y);

        // Slide in from outside the screen on the right

        ActionNotificationText.text = actionName;
        ActionNotificationText.ForceMeshUpdate(true);
        //ActionNotification.
        Debug.Log(actionName);
        notification.anchoredPosition = new Vector2(Screen.width, notification.anchoredPosition.y);
        notification.DOAnchorPosX(targetPosition.x, slideDuration)
            .SetEase(Ease.OutQuint)
            .OnComplete(() => WaitAndSlideOut());

    }

    private void WaitAndSlideOut()
    {
        // Wait for the specified duration
        DOVirtual.DelayedCall(waitDuration, () =>
        {
            // Calculate the target position for the slide-out animation
            Vector2 targetPosition = new Vector2(Screen.width+300f, notification.anchoredPosition.y);

            // Slide out to the right, outside the screen
            notification.DOAnchorPosX(targetPosition.x, slideDuration)
                .SetEase(Ease.InQuint);
        });
    }
}


