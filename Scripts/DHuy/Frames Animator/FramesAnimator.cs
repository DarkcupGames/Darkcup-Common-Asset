using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Random = UnityEngine.Random;


public enum EndOfAnimation { DoNothing, SetActiveFalse, Destroy, SetActiveFalseParent, DestroyParent, DisableAnimation, DoAction }
[System.Serializable]
public class FramesData {
    public string animationName;
    public List<Sprite> sprites;
    public EndOfAnimation endOfAnimation;
}

public class FramesAnimator : MonoBehaviour
{
    public bool debug;
    [SerializeField]
    public List<FramesData> animationDatas;
    public string defaultAnim;
    public float RATE = 0.2f;

    List<Sprite> spritesheet = new List<Sprite>(0);

    private FramesData framesData;
    private int frameshow = 0;
    private float count;
    float delay = 0f;

    EndOfAnimation endOfAnimation = EndOfAnimation.DoNothing;
    EndOfAnimation currentEndOfAnim;
    Action doneAction = null;
    string currentAnimationName;
    public string CurrentAnimationName => currentAnimationName;
    public virtual void Start()
    {
        count = Random.Range(0f, RATE); //make animation pause for random time
        frameshow = 0;
        delay = Random.Range(0f, RATE);
        if (defaultAnim != "")
        {
            SetAnimation(defaultAnim, doneAction);
        }
    }

    void Update()
    {
        delay -= Time.deltaTime;
        if (delay > 0) return;
        if (spritesheet.Count == 0) return;
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
        frameshow = -1;
        DrawNextFrame();
        currentEndOfAnim = endOfAnimation;
    }

    public virtual void DrawNextFrame()
    {
        frameshow++;
        if (frameshow >= spritesheet.Count)
        {
            bool canContinueAnim = CheckContinueAnimation();
            if (canContinueAnim == false) return;
            frameshow = 0;
        }
        if (debug)
            Debug.Log($"draw next frame for frameshow ={frameshow}, doneAction = {doneAction}");
        ApplySprite(spritesheet[frameshow]);
    }

    public virtual void ApplySprite(Sprite sprite) {
        //spriteRenderer.sprite = sprite;
        //apply here
    }

    public bool CheckContinueAnimation()
    {
        if (debug) {
            Debug.Log($"End of animation, doneAction={doneAction}");
        }
        if (doneAction != null) {
            doneAction.Invoke();
            return false;
        }
        if (endOfAnimation == EndOfAnimation.SetActiveFalse)
        {
            gameObject.SetActive(false);
            return false;
        }
        if (endOfAnimation == EndOfAnimation.Destroy)
        {
            Destroy(gameObject);
            return false;
        }
        if (endOfAnimation == EndOfAnimation.SetActiveFalseParent)
        {
            transform.parent.gameObject.SetActive(false);
            return false;
        }
        if (endOfAnimation == EndOfAnimation.DestroyParent)
        {
            Destroy(transform.parent.gameObject);
            return false;
        }
        if (endOfAnimation == EndOfAnimation.DisableAnimation)
        {
            this.enabled = false;
            return false;
        }
        return true;
    }

    public void SetRandomFrame() {
        frameshow = Random.Range(0, spritesheet.Count);
    }

    public void SetAnimation(string animationName, Action doneAction) {
        currentAnimationName = animationName;
        string callingName = (new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().Name;
        if (debug)
        Debug.Log($"set animation is calling from {callingName}");

        FramesData data = animationDatas.Find(x => x.animationName == animationName);
        if (data == null) {
            Debug.LogError("Not found animation: " + animationName);
        }
        framesData = data;
        if (debug)
            Debug.Log("set done action");
        this.doneAction = doneAction;
        this.endOfAnimation = data.endOfAnimation;

        if (data != null) {
            if (debug)
                Debug.Log("set spritesheet to data spritesheet");
            spritesheet = data.sprites;
            frameshow = 0;
        }
        if (debug)
            Debug.Log($"end of set animation, doneAction ={this.doneAction}");
    }

    private void OnDisable() {
        if (debug) {
            Debug.Log($"disable game object, set doneAction = null");
        }
        doneAction = null;
        frameshow = 0;
        count = 0;
    }
}