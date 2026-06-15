using System;
using UnityEngine;

namespace CleanWave.Core
{
    /// <summary>
    /// Local coin state for MVP systems. Other systems can call AddCoins/TrySpendCoins
    /// without depending on a specific HUD implementation.
    /// </summary>
    public class CoinWallet : MonoBehaviour
    {
        public static CoinWallet Instance { get; private set; }

        [SerializeField] private int startingCoins;

        private int coins;

        public int Coins => coins;

        public event Action<int> CoinsChanged;

        public static CoinWallet EnsureInstance()
        {
            if (Instance != null)
                return Instance;

            CoinWallet found = FindFirstObjectByType<CoinWallet>();
            if (found != null)
            {
                Instance = found;
                return Instance;
            }

            GameObject walletObject = new GameObject("CoinWallet");
            return walletObject.AddComponent<CoinWallet>();
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            coins = Mathf.Max(0, startingCoins);
        }

        private void OnEnable()
        {
            if (Instance == null)
                Instance = this;
        }

        private void Start()
        {
            CoinsChanged?.Invoke(coins);
        }

        public void AddCoins(int amount)
        {
            if (amount <= 0)
                return;

            coins += amount;
            CoinsChanged?.Invoke(coins);
        }

        public bool TrySpendCoins(int amount)
        {
            if (amount <= 0)
                return true;

            if (coins < amount)
                return false;

            coins -= amount;
            CoinsChanged?.Invoke(coins);
            return true;
        }

        public void SetCoinsForTest(int amount)
        {
            coins = Mathf.Max(0, amount);
            CoinsChanged?.Invoke(coins);
        }
    }
}
