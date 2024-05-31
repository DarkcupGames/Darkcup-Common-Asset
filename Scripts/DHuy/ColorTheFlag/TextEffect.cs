using DarkcupGames;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextEffect : MonoBehaviour
{
    [SerializeField] float distance;
    [SerializeField] new AudioClip audio;
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        AddVelocity();
        SoundManager.Instance.PlayClipOneShot(GameplaySound.Instance.completeStepSound, 0.4f);
        //PlaySoundEffect();
    }
    private void AddVelocity()
    {
        Vector3 velocity = new Vector3(0, transform.position.y + distance, 0) - transform.position;
        rb.velocity = velocity;
        transform.DORotate(new Vector3(0,0,20), 0.7f).OnComplete(() =>
        {
            transform.DOScale(Vector3.zero, 0.45f).SetEase(Ease.InQuad).OnComplete(() =>
            {
                gameObject.SetActive(false);
                rb.velocity = Vector3.zero;
                transform.rotation  = Quaternion.identity;
                transform.localScale = Vector3.one;
                sprite.DOFade(1, 0);
            });
            sprite.DOFade(0, 0.45f).SetEase(Ease.InQuad);
        });
    }

    private void PlaySoundEffect()
    {
        SoundManager.Instance.PlayClipOneShot(audio);
    }
}
