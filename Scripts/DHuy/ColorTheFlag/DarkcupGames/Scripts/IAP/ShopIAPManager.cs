using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Purchasing;
using System;
using UnityEngine.SceneManagement;

public enum IAP_ID { no_ads, vip1, unlock_all_hero, beginer }

namespace DarkcupGames
{
    public class ShopIAPManager : MonoBehaviour
    {
        public static ShopIAPManager Instance;
        public static MyIAPManager iap;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            if (iap == null)
            {
                Init();
            }
        }

        public void Init()
        {
            iap = new MyIAPManager();
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            builder.AddProduct("no_ads", ProductType.NonConsumable);
            UnityPurchasing.Initialize(iap, builder);
        }

        public bool IsInitDone()
        {
            if (iap == null) return false;
            if (iap.initSuccess == false) return false;
            if (iap.prices == null) return false;
            return true;
        }

        public void BuyProduct(string productId, Action onComplete)
        {
            MyIAPManager.currentBuySKU = productId;
            iap.OnPurchaseClicked(productId, onComplete);
        }

        public void OnBuyComlete(string sku)
        {
            if (GameSystem.userdata.boughtItems == null)
            {
                GameSystem.userdata.boughtItems.Add(sku);
                GameSystem.SaveUserDataToLocal();
            }
        }

        public void BuyNoAdsPackage()
        {
            if (IsInitDone() == false)
            {
                return;
            }
            string id = IAP_ID.no_ads.ToString();

            if (GameSystem.userdata.boughtItems == null) GameSystem.userdata.boughtItems = new List<string>();
            bool boughNoAds = GameSystem.userdata.boughtItems.Contains(id);
            if (boughNoAds) return;
            iap.OnPurchaseClicked(id, () =>
            {

                if (GameSystem.userdata.boughtItems == null) GameSystem.userdata.boughtItems = new List<string>();
                if (GameSystem.userdata.boughtItems.Contains(id) == false)
                {
                    GameSystem.userdata.boughtItems.Add(id);
                    GameSystem.SaveUserDataToLocal();
                }
                string currentScene = SceneManager.GetActiveScene().name;
                SceneManager.LoadScene(currentScene);
            });
        }

        public void BuyUnlockAllHeroPackage()
        {
            if (IsInitDone() == false) return;
            if (GameSystem.userdata.boughtItems == null) GameSystem.userdata.boughtItems = new List<string>();
            bool boughAllHero = GameSystem.userdata.boughtItems.Contains(IAP_ID.unlock_all_hero.ToString());
            if (boughAllHero) return;

            string id = IAP_ID.unlock_all_hero.ToString();
            iap.OnPurchaseClicked(id, () =>
            { 
                if (GameSystem.userdata.boughtItems == null) GameSystem.userdata.boughtItems = new List<string>();
                if (GameSystem.userdata.boughtItems.Contains(id) == false)
                {
                    GameSystem.userdata.boughtItems.Add(id);
                    GameSystem.SaveUserDataToLocal();
                }
                LeanTween.delayedCall(2f, () =>
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                });
            });
        }
    }
}