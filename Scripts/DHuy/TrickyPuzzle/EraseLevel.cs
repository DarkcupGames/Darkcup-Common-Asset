using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Spine.Unity;
using DarkcupGames;

public class EraseLevel : LevelManager, IDragHandler, IEndDragHandler
{
    public PaintToSpriteMaskController draw;
    public Camera mainCam;
    //public SkeletonAnimation skeletonAnimation;

    private void Start() {
        EraserShowPosition eraserShowPosition = draw.GetComponent<EraserShowPosition>();
        if (eraserShowPosition == null)
        {
            eraserShowPosition = draw.gameObject.AddComponent<EraserShowPosition>();
        }

        StartCoroutine(IELevel1());
    }


    public void OnDrag(PointerEventData eventData)
    {
        draw.isDrawing = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        draw.isDrawing = false;
        
    }
    public IEnumerator IELevel1() {
        while (true)
        {
            //animAfter.AnimationName = "normal";
            //yield return new WaitUntil(() => {
            //    return draw.isDrawing == false;
            //});

            //yield return new WaitUntil(() => {
            //    return draw.isDrawing == true;
            //});

            //yield return new WaitUntil(() => {
            //    return draw.isDrawing == false;
            //});


            yield return new WaitUntil(() => {
                return Input.GetMouseButtonUp(0);
            });


            if (draw.IsDrawFinished())
            {
                draw.GetComponent<EraserShowPosition>().eraser.gameObject.SetActive(false);
                Win();
                break;
            }
            else
            {
                draw.ClearDraw();
                Gameplay.Instance.Virate();
            }
        }

        yield return new WaitForSeconds(2f);

    }

    public override void Win() {
        draw.isDrawing = false;
        draw.gameObject.SetActive(false);

        EraserShowPosition erase = draw.GetComponent<EraserShowPosition>();
        if (erase != null) {
            erase.eraser.SetActive(false);
        }
        Gameplay.Instance.Win(this);

        //skeletonAnimation.AnimationName = "win";
        //skeletonAnimation.maskInteraction = SpriteMaskInteraction.None;
    }

    public override Vector3 GetGuidePosition() {
        return draw.transform.position;
    }

    
}
