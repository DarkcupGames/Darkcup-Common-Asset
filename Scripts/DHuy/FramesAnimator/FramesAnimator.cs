using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Random = UnityEngine.Random;

public enum EndOfAnimation { DoNothing, SetActiveFalse, Destroy, SetActiveFalseParent, DestroyParent, DisableAnimation }
[System.Serializable]
public class FramesData {
    public string animationName;
    public Sprite[] sprites;
    public bool looping = true;
    public EndOfAnimation endOfAnimation;
}

[RequireComponent(typeof(SpriteRenderer))]
public class FramesAnimator : MonoBehaviour
{
    [SerializeField]
    public List<FramesData> animationDatas;

    public float RATE = 0.2f;
    public bool looping = true;

    Sprite[] spritesheet = new Sprite[0];

    private SpriteRenderer spriteRenderer;
    private int frameshow = 0;
    private float count;
    float delay = 0f;

    
    EndOfAnimation endOfAnimation = EndOfAnimation.DoNothing;
    EndOfAnimation currentEndOfAnim;
    Action doneAction = null;

    public SpriteRenderer SpriteRenderer => spriteRenderer;
    void Start()
    {
        count = Random.Range(0f, RATE); //make animation pause for random time
        spriteRenderer = GetComponent<SpriteRenderer>();
        frameshow = 0;
        delay = Random.Range(0f, RATE);
    }

    void Update()
    {
        delay -= Time.deltaTime;
        if (delay > 0) return;

        if (spritesheet.Length == 0) return;

        if (spritesheet == null) return;

        count += Time.deltaTime;
        
        if (count > RATE) {
            count = 0;
            DrawNextFrame();
        }
    }

    private void OnEnable()
    {
        count = 0;
        frameshow = 0;
        currentEndOfAnim = endOfAnimation;
        looping = true;
    }

    public virtual void DrawNextFrame()
    {
        frameshow++;

        if (frameshow >= spritesheet.Length)
        {
            bool canContinueAnim = CheckContinueAnimation();
            if (canContinueAnim == false) return;
            frameshow = 0;
        }

        spriteRenderer.sprite = spritesheet[frameshow];
    }

    public bool CheckContinueAnimation()
    {
        if (looping)
        {
            return true;
        }

        if (doneAction != null)
        {
            doneAction?.Invoke();
            return false;
        }

        if (currentEndOfAnim == EndOfAnimation.DoNothing) return false;
        if (currentEndOfAnim == EndOfAnimation.SetActiveFalse) gameObject.SetActive(false);
        if (currentEndOfAnim == EndOfAnimation.Destroy) Destroy(gameObject);
        if (currentEndOfAnim == EndOfAnimation.SetActiveFalseParent) transform.parent.gameObject.SetActive(false);
        if (currentEndOfAnim == EndOfAnimation.DestroyParent) Destroy(transform.parent.gameObject);
        if (currentEndOfAnim == EndOfAnimation.DisableAnimation) this.enabled = false;

        return true;
    }

    public void SetRandomFrame() {
        frameshow = Random.Range(0, spritesheet.Length);
    }

    public void SetAnimation(string animationName, Action doneAction = null) {
        FramesData data = animationDatas.Find(x => x.animationName == animationName);
        if (data == null) {
            Debug.LogError("Could not found " + animationName + " in game object " + gameObject.name + " anim will not run!");
            return;
        }

        looping = data.looping;
        if(doneAction != null)
        {
            this.doneAction = doneAction;
        }       
        this.endOfAnimation = data.endOfAnimation;

        if (data != null) {
            spritesheet = data.sprites;
        }
        //Sprite[] sprites = 
        //if (spineAnimation != null) {
        //    spineAnimation.AnimationName = animationName;
        //    if (doneAction != null) {
        //        var myAnimation = spineAnimation.Skeleton.Data.FindAnimation(animationName);
        //        float duration = myAnimation.Duration - 0.1f;
        //        StartCoroutine(IEDelayCall(duration, doneAction));
        //    }
        //}
        //if (animator != null) {
        //    animator.Play(animationName);
        //}
    }
}
