using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI : MonoBehaviour
{

    public static UI instance;

   [SerializeField] ActionNotification actionNotification;

    public GameObject flyingTextPrefab;

    public PlayableDirector openingTimeline;

    public GameObject startMenu;
    public GameObject pauseMenu;
    public GameObject winMenu;
    public GameObject inGameCanvas;
    public GameObject characterPanel;

    [Header("Menu SFX")]
    public AudioClip startGameSfx;
    public AudioClip pauseSfx;
    public AudioClip resumeSfx;
    public AudioClip menuNavigateSfx;
    //public GameObject SituationUI;

    [SerializeField]
    RectTransform _roundPanel;

    // ==========================================
    // LIFECYCLE
    // ==========================================

    // Sets up the singleton instance so other systems can dispatch UI events through UI.instance.
    private void Awake()
    {
        instance = this;
    }

    // ==========================================
    // PANEL VISIBILITY
    // ==========================================

    // Resets panel state by delegating to HideAllPanel.
    public void ResetPanels()
    {
        HideAllPanel();
    }

    // Makes a panel fully visible and interactable by flipping its CanvasGroup values on.
    public void ShowPanel(Image imageToShow)
    {
        if (imageToShow != null)
        {
            CanvasGroup cg = imageToShow.GetComponent<CanvasGroup>();
            cg.alpha = 1;
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }
    }

    // Placeholder hook for hiding every panel in one call.
    public void HideAllPanel()
    {
    }

    // Hides a single panel by zeroing its CanvasGroup alpha and disabling interactions and raycasts.
    public void HidePanel(Image imageToHide)
    {
        if (imageToHide != null)
        {
            CanvasGroup cg = imageToHide.GetComponent<CanvasGroup>();
            cg.alpha = 0;
            cg.interactable = false;
            cg.blocksRaycasts = false;
        }
    }

    // Switches to the MidTurn state, hides all panels, and shows the provided moves list panel.
    public void ShowMovesList(Image whichPanel)
    {
        TempManager.instance.ChangeGameState(GameStates.MidTurn);
        //actionPanel = whichPanel;
        HideAllPanel();
        ShowPanel(whichPanel);
    }

    // ==========================================
    // STAT DISPLAY
    // ==========================================

    // Forwards the current player's data to PlayerStatUI for both summary HUD and detail panel updates.
    public void GetPlayerStats(CharacterBaseClasses currentPlayer)
    {
        PlayerStatUI statUI = GetComponent<PlayerStatUI>();
        statUI.GetPlayerStatSummary(currentPlayer);
        statUI.GetPlayerStatDetails(currentPlayer);
    }

    // ==========================================
    // NOTIFICATIONS & FLYING TEXT
    // ==========================================

    // Sends a notification string through the ActionNotification banner (also triggers its SFX).
    public void SendNotification(string notification)
    {
        actionNotification.AnimateNotification(notification);
    }

    // Spawns a flying text prefab that floats upward from the given parent transform.
    public void ShowFlyingText(string text, Transform parent,Color color)
    {
        GameObject flyingTextGO = Instantiate(flyingTextPrefab, parent);
        //flyingTextGO.GetComponentInChildren<TextMeshPro>().color = color;
        flyingTextGO.GetComponent<FlyingText>().FlyTextUpward(text,color);
    }

    // ==========================================
    // ROUND PANEL ANIMATION
    // ==========================================

    // Slides the round panel in from the right, holds briefly, then slides it out to the left.
    public async UniTask AnimatePanelAsync()
    {
        Vector2 originalPosition = _roundPanel.anchoredPosition;
        // Move the panel from the right to the middle
        var moveTween = _roundPanel.DOMoveX(Screen.width/2, 1.0f).SetEase(Ease.InCubic);

        // Wait for the moveTween to complete
        await moveTween.AsyncWaitForCompletion();

        // Wait for 1 second
        await UniTask.Delay(500);

        // Move the panel back to the left
        moveTween= _roundPanel.DOMoveX(-Screen.width, 1.0f).SetEase(Ease.OutExpo);

        // Wait for the second moveTween to complete
        await moveTween.AsyncWaitForCompletion();
        _roundPanel.anchoredPosition = originalPosition;



    }

    // ==========================================
    // MENU ACTIONS (each plays a menu SFX)
    // ==========================================

    // Plays the start-game SFX, hides the start menu, and kicks off the opening cinematic timeline.
    public void StartGame()
    {
        PlayMenuSfx(startGameSfx);
        startMenu.SetActive(false);
        openingTimeline.Play();

    }

    // Quits the application.
    public void ExitGame()
    {
        Application.Quit();
    }

    // Reloads the currently active scene.
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Plays the pause SFX, freezes time, shows the pause menu, and hides the in-game canvas.
    public void PauseGame()
    {
        PlayMenuSfx(pauseSfx);
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
        inGameCanvas.SetActive(false);

    }

    // Plays the resume SFX, un-freezes time, hides the pause menu, and restores the in-game canvas.
    public void ResumeGame()
    {
        PlayMenuSfx(resumeSfx);
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        inGameCanvas.SetActive(true);
    }

    // Plays the menu-navigate SFX and switches from the start menu to the character selection panel.
    public void CharacterMenu()
    {
        PlayMenuSfx(menuNavigateSfx);
        startMenu.SetActive(false);
        characterPanel.SetActive(true);
    }

    // Plays the menu-navigate SFX and returns from the character panel back to the start menu.
    public void GoBack()
    {
        PlayMenuSfx(menuNavigateSfx);
        characterPanel.SetActive(false);
        startMenu.SetActive(true);
    }

    // ==========================================
    // AUDIO HELPER
    // ==========================================

    // Null-safe wrapper for firing a single menu SFX through the SoundManager.
    private void PlayMenuSfx(AudioClip clip)
    {
        if (SoundManager.Instance == null || clip == null) return;
        SoundManager.Instance.PlaySound(clip);
    }

}


