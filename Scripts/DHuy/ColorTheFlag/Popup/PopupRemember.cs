using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class PopupRemember : MonoBehaviour
{
    public const float REMEMBER_TIME = 1.5f;

    [SerializeField] private TextMeshProUGUI txtCountryName;
    [SerializeField] private Image imgFlag;
    [SerializeField] private Transform startPos;
    [SerializeField] private Transform center;
    [SerializeField] private Transform endPos;
    [SerializeField] private Transform body;

    public void OnEnable()
    {
        StartCoroutine(IEEffect());
    }

    IEnumerator IEEffect()
    {
        body.localScale = Vector2.zero;
        body.transform.position = startPos.position;
        body.DOScale(1f, 1f);
        body.DOMove(center.position, 1f);
        yield return new WaitForSeconds(1f);
        yield return new WaitForSeconds(REMEMBER_TIME);
        body.DOMove(endPos.position, 1f);
        body.DOScale(Vector2.zero, 1f);
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }

    public void SetData(CountryData data)
    {
        imgFlag.sprite = data.flag;
    }
}