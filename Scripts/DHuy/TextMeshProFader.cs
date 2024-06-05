using TMPro;
using UnityEngine;

public class TextMeshProFader : MonoBehaviour
{
    public TextMeshProUGUI textMesh; // Đối tượng TextMeshPro của bạn
    public float fadeDuration = 2.0f; // Thời gian để hoàn thành một fade in hoặc fade out
    private float currentFadeTime; // Thời gian hiện tại trong quá trình fade
    private bool fadingIn = true; // Kiểm soát xem hiện tại là fade in hay fade out

    void Update()
    {
        if (fadingIn)
        {
            // Fade in
            if (textMesh.color.a < 1.0f)
            {
                float newAlpha = textMesh.color.a + (Time.deltaTime / fadeDuration);
                textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, newAlpha);
            }
            else
            {
                fadingIn = false; // Chuyển sang fade out khi fade in hoàn thành
                currentFadeTime = 0; // Đặt lại bộ đếm thời gian
            }
        }
        else
        {
            // Fade out
            if (textMesh.color.a > 0.0f)
            {
                float newAlpha = textMesh.color.a - (Time.deltaTime / fadeDuration);
                textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, newAlpha);
            }
            else
            {
                fadingIn = true; // Chuyển sang fade in khi fade out hoàn thành
                currentFadeTime = 0; // Đặt lại bộ đếm thời gian
            }
        }
    }
}