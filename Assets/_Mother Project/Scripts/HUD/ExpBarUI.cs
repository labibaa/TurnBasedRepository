using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ExpBarUI : MonoBehaviour
{
    public Image expBar;
    public CharacterBaseClasses myCharacter;

    [Header("EXP Bar Fill")]
    [SerializeField] private float expFillDuration = 0.8f;
    [SerializeField] private Ease expFillEase = Ease.OutQuad;

    private Tween _expFillTween;

    public void FillExpBar()
    {
        FillExpBar(myCharacter);
    }

    public void FillExpBar(CharacterBaseClasses character)
    {
        if (expBar == null || character == null) return;
        TemporaryStats ts = character.GetComponent<TemporaryStats>();
        if (ts == null) return;

        int maxExp = character.MaxExperiencePoint;
        if (maxExp <= 0) return;

        float targetFill = Mathf.Clamp01((float)ts.CurrentExp / maxExp);

        if (_expFillTween != null && _expFillTween.IsActive()) _expFillTween.Kill();

        expBar.fillAmount = 0f;
        _expFillTween = expBar.DOFillAmount(targetFill, expFillDuration).SetEase(expFillEase);
    }

    private void OnDisable()
    {
        if (_expFillTween != null && _expFillTween.IsActive()) _expFillTween.Kill();
    }
}
