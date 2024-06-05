using System.Collections;
using UnityEngine;
using DG.Tweening;
using DarkcupGames;
using UnityEngine.SceneManagement;

public class PopupWin : MonoBehaviour
{
    public const float SCALE_TIME = 0.5f;
    [SerializeField] private MapCamera _cameraMovement;
    [SerializeField] private Transform greenTick;

    private void OnEnable()
    {
        GameManager.Instance.StartCoroutine(IEEffect());
        SoundManager.Instance.PlayClipOneShot(GameplaySound.Instance.winSound);
        GameSceneManager.Instance.HideNormalCanvas();
        FirebaseManager.analytics.LogUIAppear(SceneManager.GetActiveScene().name, "popup_win");
    }

    IEnumerator IEEffect()
    {
        greenTick.localScale = Vector2.zero;
        greenTick.DOScale(1f, SCALE_TIME).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(SCALE_TIME);
        yield return new WaitForSeconds(1f);
        greenTick.DOScale(0f, SCALE_TIME).SetEase(Ease.InBack);
        yield return new WaitForSeconds(SCALE_TIME);
        gameObject.SetActive(false);
        CameraManager.Instance.ZoomOut();
        GameManager.Instance.popupCongratulation.gameObject.SetActive(true);
    }
}