using UnityEngine;
using UnityEngine.Purchasing;
using System;
using System.Collections;
using System.Collections.Generic;
//using GoogleMobileAds.Common;

namespace DarkcupGames
{
    public class MyIAPManager : IStoreListener
    {
        public bool initSuccess = false;
        public static string currentBuySKU;
        public Action onProcessSuccess;
        public Dictionary<string, string> prices;
        private IStoreController controller;
        private IExtensionProvider extensions;

        //public void Init()
        //{
        //    var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        //    builder.AddProduct("100_gold_coins", ProductType.Consumable, new IDs
        //    {
        //        {"100_gold_coins_google", GooglePlay.Name},
        //        {"100_gold_coins_mac", MacAppStore.Name}
        //    });
        //    UnityPurchasing.Initialize(this, builder);
        //}

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            this.controller = controller;
            this.extensions = extensions;
            initSuccess = true;
            Debug.Log("Init IAP finished, congratulation!!");
            var products = controller.products.all;
            Debug.Log($"Load all iap data, total {products.Length} items!!");
            prices = new Dictionary<string, string>();
            for (int i = 0; i < products.Length; i++)
            {
                Debug.Log($"Add item with id = {products[i].definition.id}, localizeString = {products[i].metadata.localizedPriceString}");
                prices.Add(products[i].definition.id, products[i].metadata.localizedPriceString);
            }
            Debug.Log($"After add, prices count = {prices.Count}");
            var texts = GameObject.FindObjectsOfType<TextPricingIAP>();
            for (int i = 0; i < texts.Length; i++)
            {
                texts[i].UpdateDisplay();
            }
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.Log("Init IAP failed: " + error);
            initSuccess = false;
            //if (popupConfirm)
            //    popupConfirm.ShowOK("Init Failed", error.ToString());
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
        {
            Debug.Log("Buy product complete, congratulation!!");
            Debug.Log(e);

            if (onProcessSuccess != null)
            {
                MainThreadManager.Instance.ExecuteInUpdate(() => {
                    onProcessSuccess();
                });
            }
            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
        {
            Debug.LogError("Purchase failed at product " + i + " for reason: " + p);
        }

        public void ShowAllProduct()
        {
            foreach (var product in controller.products.all)
            {
                Debug.Log(product.metadata.localizedTitle);
                Debug.Log(product.metadata.localizedDescription);
                Debug.Log(product.metadata.localizedPriceString);
            }
        }

        public void OnPurchaseClicked(string productId, Action onSuccess)
        {
            if (!initSuccess)
            {
                Debug.LogError("Init not finished with product id = " + productId);
                return;
            }
            if (this.controller == null)
            {
                Debug.LogError("Controller is null at product id = " + productId);
                return;
            }
            //Debug.Log("Processing product, id = " + productId);
            this.onProcessSuccess = onSuccess;
            this.controller.InitiatePurchase(productId);
            currentBuySKU = productId;
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            
        }
    }
}