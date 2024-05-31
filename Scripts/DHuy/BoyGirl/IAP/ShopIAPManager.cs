using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Purchasing;
using System;
using UnityEngine.SceneManagement;

public enum IAP_ID { no_ads, vip1, unlock_all_hero, beginer }

namespace DarkcupGames {
    public class ShopIAPManager : MonoBehaviour {
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

        public void BuyProduct(string productId, Action onComplete) {
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
            //Debug.Log($"Clicking buy ads");
            //Debug.Log($"iap.prices is null = {iap.prices == null}");
            //Debug.Log($"iap.prices.count = {iap.prices.Count}");
            if (IsInitDone() == false)
            {
                //Debug.Log($"Init done == false, iap = {iap}, iap.initSuccess = {iap.initSuccess}, iap.prices = {iap.prices}");
                return;
            }
            string id = IAP_ID.no_ads.ToString();

            if (GameSystem.userdata.boughtItems == null) GameSystem.userdata.boughtItems = new List<string>();
            bool boughNoAds = GameSystem.userdata.boughtItems.Contains(id);
            if (boughNoAds) return;
            iap.OnPurchaseClicked(id, () =>
            {
                int DIAMOND_AMOUNT = 0;
                int GOLD_AMOUNT = 0;

                if (GameSystem.userdata.boughtItems == null) GameSystem.userdata.boughtItems = new List<string>();
                if (GameSystem.userdata.boughtItems.Contains(id) == false)
                {
                    GameSystem.userdata.boughtItems.Add(id);
                    GameSystem.userdata.diamond += DIAMOND_AMOUNT;
                    GameSystem.userdata.gold += GOLD_AMOUNT;
                    GameSystem.SaveUserDataToLocal();
                }
                string sceneName = SceneManager.GetActiveScene().name;
                if (sceneName == Constants.SCENE_HOME)
                {
                    Home.Instance.UpdateDisplay();
                } else if (sceneName == Constants.SCENE_GAMEPLAY)
                {
                    Gameplay.Instance.UpdateDisplay();
                }
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
                int DIAMOND_AMOUNT = 0;
                int GOLD_AMOUNT = 0;

                if (GameSystem.userdata.boughtItems == null) GameSystem.userdata.boughtItems = new List<string>();
                if (GameSystem.userdata.boughtItems.Contains(id) == false)
                {
                    GameSystem.userdata.boughtItems.Add(id);
                    GameSystem.userdata.diamond += DIAMOND_AMOUNT;
                    GameSystem.userdata.gold += GOLD_AMOUNT;
                    GameSystem.SaveUserDataToLocal();
                }
                //var allHeroes = new List<string>();
                //allHeroes.AddRange(DataManager.Instance.allyNames);
                //allHeroes.Remove("E18");
                //for (int i = 0; i < allHeroes.Count; i++)
                //{
                //    if (GameSystem.userdata.unlockedHeros.Contains(allHeroes[i]) == false)
                //    {
                //        GameSystem.userdata.unlockedHeros.Add(allHeroes[i]);
                //    }
                //}
                //GameSystem.SaveUserDataToLocal();
                //List<ResourceData> list = new List<ResourceData>();
                //list.Add(new ResourceData()
                //{
                //    sprite = sprVipPack[0],
                //    amount = 1
                //});
                //list.Add(new ResourceData()
                //{
                //    sprite = sprVipPack[1],
                //    amount = 1
                //});
                //list.Add(new ResourceData()
                //{
                //    sprite = sprVipPack[2],
                //    amount = DIAMOND_AMOUNT
                //});
                //list.Add(new ResourceData()
                //{
                //    sprite = sprVipPack[3],
                //    amount = GOLD_AMOUNT
                //});
                //ResourcesGain.Instance.DisplayResources(list);
                //shop.UpdateDisplay();
                LeanTween.delayedCall(2f, () =>
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                });
            });
        }
    }
}