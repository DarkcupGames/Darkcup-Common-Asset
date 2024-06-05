using UnityEngine;
using UnityEngine.Purchasing;
using System;
using System.Collections;
using System.Collections.Generic;

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

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            this.controller = controller;
            this.extensions = extensions;
            initSuccess = true;
            var products = controller.products.all;
            prices = new Dictionary<string, string>();
            for (int i = 0; i < products.Length; i++)
            {
                prices.Add(products[i].definition.id, products[i].metadata.localizedPriceString);
            }
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
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
        {
            if (onProcessSuccess != null)
            {
                MainThreadManager.Instance.ExecuteInUpdate(() =>
                {
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
            this.onProcessSuccess = onSuccess;
            this.controller.InitiatePurchase(productId);
            currentBuySKU = productId;
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {

        }
    }
}