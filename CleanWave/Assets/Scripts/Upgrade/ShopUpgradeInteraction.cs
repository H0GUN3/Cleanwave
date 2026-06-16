using CleanWave.Core;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class ShopUpgradeInteraction : MonoBehaviour
{
    const string UpgradeClipPath = "Audio/Game/upgrade_success";

    [SerializeField] int coinCost = 200;
    [SerializeField] int targetLevel = 1;
    [SerializeField] float fallbackInteractionRange = 1.5f;
    [SerializeField] bool logInteraction = true;
    [SerializeField, Range(0f, 1f)] float upgradeVolume = 1f;

    PlayerUpgrade playerInRange;
    PlayerUpgrade cachedPlayer;
    AudioClip upgradeClip;

    public int CoinCost => coinCost;
    public int TargetLevel => targetLevel;

    public void Configure(int cost, int level)
    {
        coinCost = Mathf.Max(0, cost);
        targetLevel = Mathf.Max(0, level);
    }

    void Awake()
    {
        Collider2D shopCollider = GetComponent<Collider2D>();
        if (shopCollider != null)
            shopCollider.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerUpgrade player = other.GetComponentInParent<PlayerUpgrade>();
        if (player != null)
            playerInRange = player;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (playerInRange != null)
            return;

        PlayerUpgrade player = other.GetComponentInParent<PlayerUpgrade>();
        if (player != null)
            playerInRange = player;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        PlayerUpgrade player = other.GetComponentInParent<PlayerUpgrade>();
        if (player != null && player == playerInRange)
            playerInRange = null;
    }

    void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null || !keyboard.fKey.wasPressedThisFrame)
            return;

        PlayerUpgrade player = GetInteractablePlayer();
        if (player == null)
            return;

        playerInRange = player;
        TryPurchaseUpgrade(player);
    }

    public bool TryPurchaseUpgrade(PlayerUpgrade player)
    {
        if (player == null)
            return false;

        if (targetLevel >= player.StageCount)
        {
            SortingFeedbackPopup.ShowMessage("업그레이드 단계가 없습니다.");
            return false;
        }

        if (player.CurrentLevel >= targetLevel)
        {
            SortingFeedbackPopup.ShowMessage("이미 업그레이드 되었습니다.");
            return false;
        }

        CoinWallet wallet = CoinWallet.EnsureInstance();
        if (!wallet.TrySpendCoins(coinCost))
        {
            SortingFeedbackPopup.ShowMessage("코인이 부족합니다.");
            return false;
        }

        player.SetEvolutionLevel(targetLevel);
        SortingFeedbackPopup.ShowMessage("구매 성공!");
        PlayUpgradeSound();

        if (logInteraction)
            Debug.Log($"[ShopUpgradeInteraction] {name} spent {coinCost} coins and upgraded player to level {targetLevel}.", this);

        return true;
    }

    void PlayUpgradeSound()
    {
        if (upgradeClip == null)
            upgradeClip = Resources.Load<AudioClip>(UpgradeClipPath);

        if (upgradeClip != null && upgradeClip.loadState == AudioDataLoadState.Unloaded)
            upgradeClip.LoadAudioData();

        if (upgradeClip != null)
            AudioSource.PlayClipAtPoint(upgradeClip, transform.position, upgradeVolume);
    }

    PlayerUpgrade GetInteractablePlayer()
    {
        if (playerInRange != null)
            return playerInRange;

        if (cachedPlayer == null)
            cachedPlayer = FindFirstObjectByType<PlayerUpgrade>();

        if (cachedPlayer == null)
            return null;

        float distance = Vector2.Distance(transform.position, cachedPlayer.transform.position);
        return distance <= fallbackInteractionRange ? cachedPlayer : null;
    }
}
