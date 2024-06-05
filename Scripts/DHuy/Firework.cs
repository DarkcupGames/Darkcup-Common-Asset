using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Firework : MonoBehaviour
{
    private void OnEnable()
    {
        LeanTween.delayedCall(3f, () =>
        {
            try
            {
                if (gameObject) gameObject.SetActive(false);
            } catch
            {

            }
            
        });
    }
}