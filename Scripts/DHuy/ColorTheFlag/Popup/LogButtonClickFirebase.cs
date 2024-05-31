using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LogButtonClickFirebase : MonoBehaviour
{
    public string buttonName;
    private Button button;
    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            FirebaseManager.analytics.LogButtonClick(SceneManager.GetActiveScene().name, buttonName);
        });
    }
}