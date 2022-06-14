using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using VOrb.CubesWar;
using GameAnalyticsSDK;
public class IAPCore : MonoBehaviour, IStoreListener
{

    private static IStoreController m_StoreController;
    private static IExtensionProvider m_StoreExtensionProvider;

    public static string noads = "noads";



    public class Receipt
    {

        public string Store;
        public string TransactionID;
        public string Payload;

        public Receipt()
        {
            Store = TransactionID = Payload = "";
        }

        public Receipt(string store, string transactionID, string payload)
        {
            Store = store;
            TransactionID = transactionID;
            Payload = payload;
        }
    }

    public class PayloadAndroid
    {
        public string json;
        public string signature;

        public PayloadAndroid()
        {
            json = signature = "";
        }

        public PayloadAndroid(string _json, string _signature)
        {
            json = _json;
            signature = _signature;
        }
    }


    void Start()
    {
        if (m_StoreController == null) //если еще не инициализаровали систему Unity Purchasing, тогда инициализируем
        {
            InitializePurchasing();
        }
    }

    public void InitializePurchasing()
    {
        if (IsInitialized()) //если уже подключены к системе - выходим из функции
        {
            return;
        }

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        //ѕрописываем свои товары дл€ добавлени€ в билдер
        builder.AddProduct(noads, ProductType.NonConsumable);

        UnityPurchasing.Initialize(this, builder);
    }

    public void Buy_noads()
    {
        BuyProductID(noads);
    }


    void BuyProductID(string productId)
    {
        if (IsInitialized()) //если покупка инициализирована 
        {
            Product product = m_StoreController.products.WithID(productId); //находим продукт покупки 

            if (product != null && product.availableToPurchase) //если продукт найдет и готов дл€ продажи
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                m_StoreController.InitiatePurchase(product); //покупаем
            }
            else
            {
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        else
        {
            Debug.Log("BuyProductID FAIL. Not initialized.");
        }
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args) //контроль покупок
    {
        var window = UIWindowsManager.GetWindow<StartWindow>();
        if (String.Equals(args.purchasedProduct.definition.id, noads, StringComparison.Ordinal)) //тут замен€ем наш ID
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));

            //действи€ при покупке
            GameService.Instance.NoAds = 1;
            GameStorageOperator.PutToDevice(GameStorageOperator.PlayerParamNames.Noads, 1);
            window.NoAdsShow = false;
            try
            {
                var product = m_StoreController.products.WithID(noads);
                string receipt = product.receipt;
                string currency = product.metadata.isoCurrencyCode;
                int amount = decimal.ToInt32(product.metadata.localizedPrice * 100);
                #if UNITY_ANDROID
                    Receipt receiptAndroid = JsonUtility.FromJson<Receipt>(receipt);
                    PayloadAndroid receiptPayload = JsonUtility.FromJson<PayloadAndroid>(receiptAndroid.Payload);
                    GameAnalytics.NewBusinessEventGooglePlay(currency, amount, "OnceBuyNoAdds", noads, "StartPage", receiptPayload.json, receiptPayload.signature);
                #endif
            }
            catch (Exception)
            {
            }
            

        }
        else
        {
            Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
        }

        window.Dimmed = false;
        return PurchaseProcessingResult.Complete;
    }

    //public void RestorePurchases() //¬осстановление покупок (только дл€ Apple). ” гугл это автоматический процесс.
    //{
    //    if (!IsInitialized())
    //    {
    //        Debug.Log("RestorePurchases FAIL. Not initialized.");
    //        return;
    //    }

    //    if (Application.platform == RuntimePlatform.IPhonePlayer ||
    //        Application.platform == RuntimePlatform.OSXPlayer) //если запущенно на эпл устройстве
    //    {
    //        Debug.Log("RestorePurchases started ...");

    //        var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();

    //        apple.RestoreTransactions((result) =>
    //        {
    //            Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
    //        });
    //    }
    //    else
    //    {
    //        Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
    //    }
    //}



    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("OnInitialized: PASS");
        m_StoreController = controller;
        m_StoreExtensionProvider = extensions;
    }

    private bool IsInitialized()
    {
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
        var window = UIWindowsManager.GetWindow<StartWindow>();
        if (failureReason == PurchaseFailureReason.DuplicateTransaction)
        {
            GameService.Instance.NoAds = 1;
            GameStorageOperator.PutToDevice(GameStorageOperator.PlayerParamNames.Noads, 1);
            window.NoAdsShow = false;
        }
        window.Dimmed = false;
    }
}
