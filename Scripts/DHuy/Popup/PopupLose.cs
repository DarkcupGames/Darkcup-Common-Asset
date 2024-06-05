using System.Collections;
using UnityEngine;
using DG.Tweening;
using DarkcupGames;
using UnityEngine.SceneManagement;

public class PopupLose : MonoBehaviour
{
    public const float SCALE_TIME = 0.5f;

    [SerializeField] private Transform body;

    private void OnEnable()
    {
        StartCoroutine(IEEffect());
        SoundManager.Instance.PlayClipOneShot(GameplaySound.Instance.loseSound);
        GameSceneManager.Instance.HideNormalCanvas();
        FirebaseManager.analytics.LogUIAppear(SceneManager.GetActiveScene().name, "popup_lose");
    }

    IEnumerator IEEffect()
    {
        body.localScale = Vector2.zero;
        body.DOScale(1f, SCALE_TIME).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(SCALE_TIME);
        yield return new WaitForSeconds(1f);
        body.DOScale(0f, SCALE_TIME).SetEase(Ease.InBack);
        yield return new WaitForSeconds(SCALE_TIME);
        gameObject.SetActive(false);
        GameManager.Instance.RestartLevel();
    }
}
