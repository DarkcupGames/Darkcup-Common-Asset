using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonSetting : MonoBehaviour
{
    public SettingKey key;
    public Sprite sprOn;
    public Sprite sprOff;
    public Image img;

    private void Awake()
    {
        img = GetComponent<Image>();
    }
    private void Start()
    {
        UpdateDislpay();
    }

    public void UpdateDislpay()
    {
        Debug.LogWarning(GameSystem.userdata.dicSetting[key]);
        if (GameSystem.userdata.dicSetting[key] == true)
        {
            img.sprite = sprOn;
        }
        else
        {
            img.sprite = sprOff;
        }
    }

    public void OnCLick()
    {
        GameSystem.userdata.dicSetting[key] = !GameSystem.userdata.dicSetting[key];
        GameSystem.SaveUserDataToLocal();
        BackgroundMusic.Instance.CheckSetting();
        UpdateDislpay();
        FirebaseManager.analytics.LogButtonClick(SceneManager.GetActiveScene().name, "button_setting_" + key);
    }
}
