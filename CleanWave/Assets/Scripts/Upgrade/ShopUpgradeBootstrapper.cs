using CleanWave.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ShopUpgradeBootstrapper
{
    const float RuntimeTriggerRadius = 1.5f;
    static bool sceneLoadedSubscribed;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AttachShopInteractions()
    {
        SubscribeSceneLoaded();

        if (!GameplaySceneUtility.IsGameplayScene())
            return;

        CleanWave.Core.CoinWallet.EnsureInstance();
        SortingFeedbackPopup.EnsureInstance();

        ConfigureShop("shop_200", 200, 1);
        ConfigureShop("shop_400", 400, 2);
    }

    static void SubscribeSceneLoaded()
    {
        if (sceneLoadedSubscribed)
            return;

        SceneManager.sceneLoaded += HandleSceneLoaded;
        sceneLoadedSubscribed = true;
    }

    static void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AttachShopInteractions();
    }

    static void ConfigureShop(string objectName, int coinCost, int targetLevel)
    {
        GameObject shop = GameObject.Find(objectName);
        if (shop == null)
            return;

        Collider2D shopCollider = shop.GetComponent<Collider2D>();
        if (shopCollider == null)
        {
            CircleCollider2D trigger = shop.AddComponent<CircleCollider2D>();
            trigger.radius = RuntimeTriggerRadius;
            shopCollider = trigger;
        }

        shopCollider.isTrigger = true;

        ShopUpgradeInteraction interaction = shop.GetComponent<ShopUpgradeInteraction>();
        if (interaction == null)
            interaction = shop.AddComponent<ShopUpgradeInteraction>();

        interaction.Configure(coinCost, targetLevel);
    }
}
