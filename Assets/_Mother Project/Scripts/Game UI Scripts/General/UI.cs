using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Xamin;
using static UnityEngine.Rendering.DebugUI;

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
    //public GameObject SituationUI;

    //
    //[SerializeField]
    //GameObject SelectorActionOptions;
    //public CircleSelector _circleSelector;



    [SerializeField]
    RectTransform _roundPanel;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
       
        //_circleSelector = SelectorActionOptions.GetComponent<CircleSelector>();
    }

    public void ResetPanels()
    {
        HideAllPanel();
        //ShowPanel(playerStatSummary);

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
           // PauseGame();
        }
    }


    public void KillMove()
    {
        //killPanel.SetActive(true);
        

       // TempManager.instance.OnGameState(GameStates.MidTurn);

    }

    public void CreateTargetButton(CharacterBaseClasses target)
    {
      /*  Transform targetBtn = Instantiate(targetBTN.transform, targetListpanel.transform);
        targetBtn.GetComponent<TargetButton>().avatarHead.sprite = target.avatarHead;
        targetBtn.GetComponent<TargetButton>().avatarNameTxt.text = target.characterName;
        targetBtn.GetComponent<TargetButton>().target = target;*/


    }

    public void ClearTargetList()
    {
        /*foreach (Transform targetButtons in targetListpanel.transform)
        {
            Destroy(targetButtons.gameObject);
        }*/
    }
    
    // player stat Summary
    // Player Stat Details Panel
    // TimeLine Panel
    // Target List panel
    // Action Parent Panel
    // Action Panel

    public void ShowPanel(Image imageToShow)
    {
        if (imageToShow != null)
        {
            //AudioController.instance.PlaySound();
            imageToShow.GetComponent<CanvasGroup>().alpha = 1;
            imageToShow.GetComponent<CanvasGroup>().interactable = true;
            imageToShow.GetComponent<CanvasGroup>().blocksRaycasts = true;
            
        }
     
    }

    public void HideAllPanel()
    {
     
    }
    public void HidePanel(Image imageToHide)
    {
        if (imageToHide != null)
        {
            imageToHide.GetComponent<CanvasGroup>().alpha = 0;
            imageToHide.GetComponent<CanvasGroup>().interactable = false;
            imageToHide.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }
    public void ShowMovesList(Image whichPanel)
    {
        TempManager.instance.ChangeGameState(GameStates.MidTurn);
        //actionPanel = whichPanel;
        HideAllPanel();
        ShowPanel(whichPanel);
    }

    public void GetPlayerStats(CharacterBaseClasses currentPlayer)
    {
        GetComponent<PlayerStatUI>().GetPlayerStatSummary(currentPlayer);
        GetComponent<PlayerStatUI>().GetPlayerStatDetails(currentPlayer);
    }


    public void SendNotification(string notification)
    {
        actionNotification.AnimateNotification(notification);
    }


    public void ShowFlyingText(string text, Transform parent,Color color)
    {
        GameObject flyingTextGO = Instantiate(flyingTextPrefab, parent);
        //flyingTextGO.GetComponentInChildren<TextMeshPro>().color = color;
        flyingTextGO.GetComponent<FlyingText>().FlyTextUpward(text,color);
    }

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

    public void StartGame()
    {
        startMenu.SetActive(false);
        openingTimeline.Play();

    }
    public void ExitGame()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
        inGameCanvas.SetActive(false);

    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        inGameCanvas.SetActive(true);
    }
    public void CharacterMenu()
    {
        startMenu.SetActive(false);
        characterPanel.SetActive(true);
    }
    public void GoBack()
    {
        characterPanel.SetActive(false);
        startMenu.SetActive(true);
    }

}


