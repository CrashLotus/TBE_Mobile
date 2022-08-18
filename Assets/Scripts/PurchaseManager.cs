using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Purchasing;
using Unity.Services.Core;

public class PurchaseManager : MonoBehaviour, IStoreListener
{
    static PurchaseManager s_theManager;
    static IStoreController m_StoreController;          // The Unity Purchasing system.
    static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.
    IAppleExtensions m_AppleExtensions;
    IGooglePlayStoreExtensions m_GoogleExtensions;

    ConfigurationBuilder builder;

    public static PurchaseManager Get()
    {
        if (null == s_theManager)
        {   // just add this as a component to the existing GameManager object
            GameManager gm = GameManager.Get();
            s_theManager = gm.gameObject.AddComponent<PurchaseManager>();
            s_theManager.Initialize();
        }
        return s_theManager;
    }

    public bool BuyProductID(string productId)
    {
        if (IsInitialized())
        {
            Product product = m_StoreController.products.WithID(productId);
            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product:" + product.definition.id.ToString()));
                m_StoreController.InitiatePurchase(product);
                return true;
            }
            Debug.LogError("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            return false;
        }
        Debug.LogError("BuyProductID FAIL. Not initialized.");
        return false;
    }

    private bool IsInitialized()
    {
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    async void Initialize()
    {
        Debug.Log("Start Initialize()");
        try
        {
            await UnityServices.InitializeAsync();
            Debug.Log("InitializePurchasing()");
            InitializePurchasing();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        Debug.Log("Finish Initialize()");
    }

    void InitializePurchasing()
    {
        if (IsInitialized())
        {
            return;
        }

        builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        builder.AddProduct("5_time_crystals", ProductType.Consumable);
        builder.AddProduct("10_time_crystals", ProductType.Consumable);
        builder.AddProduct("50_time_crystals", ProductType.Consumable);
        builder.AddProduct("100_time_crystals", ProductType.Consumable);

        builder.Configure<IGooglePlayConfiguration>().SetObfuscatedAccountId("test1");

        builder.Configure<IGooglePlayConfiguration>().SetDeferredPurchaseListener(OnDeferredPurchase);

        Debug.Log("Starting Initialized...");
        UnityPurchasing.Initialize(this, builder);
    }

    void OnDeferredPurchase(Product product)
    {
        Debug.Log($"Purchase of {product.definition.id} is deferred");
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("OnInitialized: PASS");

        m_StoreController = controller;
        m_StoreExtensionProvider = extensions;
        m_AppleExtensions = extensions.GetExtension<IAppleExtensions>();
        m_GoogleExtensions = extensions.GetExtension<IGooglePlayStoreExtensions>();

        Dictionary<string, string> dict = m_AppleExtensions.GetIntroductoryPriceDictionary();

        foreach (Product item in controller.products.all)
        {
            if (item.receipt != null)
            {
                string intro_json = (dict == null || !dict.ContainsKey(item.definition.storeSpecificId)) ? null : dict[item.definition.storeSpecificId];

                if (item.definition.type == ProductType.Subscription)
                {
                    SubscriptionManager p = new SubscriptionManager(item, null);
                    SubscriptionInfo info = p.getSubscriptionInfo();
                    Debug.Log("SubInfo: " + info.getProductId().ToString());
                    Debug.Log("getExpireDate: " + info.getExpireDate().ToString());
                    Debug.Log("isSubscribed: " + info.isSubscribed().ToString());
                }
            }
        }
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError("OnInitializeFailed InitializationFailureReason:" + error);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        Debug.Log("Receipt:" + args.purchasedProduct.receipt.ToString());

        string productId = args.purchasedProduct.definition.id;
        if (productId == "5_time_crystals")
        {
            SaveData data = SaveData.Get();
            data.AddTimeCrystals(5);
        }
        else if (productId == "10_time_crystals")
        {
            SaveData data = SaveData.Get();
            data.AddTimeCrystals(10);
        }
        else if (productId == "50_time_crystals")
        {
            SaveData data = SaveData.Get();
            data.AddTimeCrystals(50);
        }
        else if (productId == "100_time_crystals")
        {
            SaveData data = SaveData.Get();
            data.AddTimeCrystals(100);
        }

        Debug.Log(string.Format("ProcessPurchase: Complete. Product:" + args.purchasedProduct.definition.id + " - " + args.purchasedProduct.transactionID.ToString()));
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.LogError(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }
}
