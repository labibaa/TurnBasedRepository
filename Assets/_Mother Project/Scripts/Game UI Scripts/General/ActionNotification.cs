using DG.Tweening;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ActionNotification : MonoBehaviour
{
    public static ActionNotification instance;
    
    public RectTransform notification;
    public TextMeshProUGUI ActionNotificationText;

    public float slideDuration = 1f;
    public float waitDuration = 1f;

    public AudioClip notificationSfx;

    // ==========================================
    // LIFECYCLE
    // ==========================================

    // Sets up the singleton instance for other systems to send notifications through.
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // ==========================================
    // NOTIFICATION ANIMATION
    // ==========================================

    // Sets the text, plays the notification SFX, and slides the banner in from the right edge.
    public void AnimateNotification(string actionName)
    {
        // Calculate the target position for the slide-in animation
        Vector2 targetPosition = new Vector2(0f, notification.anchoredPosition.y);

        // Slide in from outside the screen on the right

        ActionNotificationText.text = actionName;
        ActionNotificationText.ForceMeshUpdate(true);

        if (SoundManager.Instance != null && notificationSfx != null)
        {
            SoundManager.Instance.PlaySound(notificationSfx);
        }

        notification.anchoredPosition = new Vector2(Screen.width, notification.anchoredPosition.y);
        notification.DOAnchorPosX(targetPosition.x, slideDuration)
            .SetEase(Ease.OutQuint)
            .OnComplete(() => WaitAndSlideOut());

    }

    // Waits for the configured duration, then slides the banner back out past the right edge of the screen.
    private void WaitAndSlideOut()
    {
        // Wait for the specified duration
        DOVirtual.DelayedCall(waitDuration, () =>
        {
            // Calculate the target position for the slide-out animation
            Vector2 targetPosition = new Vector2(Screen.width + 300f, notification.anchoredPosition.y);

            // Slide out to the right, outside the screen
            notification.DOAnchorPosX(targetPosition.x, slideDuration)
                .SetEase(Ease.InQuint);
        });
    }
}


