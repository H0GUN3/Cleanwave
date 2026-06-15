using System;
using CleanWave.Core;
using UnityEngine;

public class CleanWaveRunState : MonoBehaviour
{
    public static CleanWaveRunState Instance { get; private set; }

    public event Action<int, int> ScoreChanged;

    int score = CleanWaveGameConstants.StartingGameScore;

    public int Score => score;
    public int Coins => CoinWallet.EnsureInstance().Coins;

    public static CleanWaveRunState EnsureInstance()
    {
        if (Instance != null)
            return Instance;

        CleanWaveRunState found = FindFirstObjectByType<CleanWaveRunState>();
        if (found != null)
        {
            Instance = found;
            return Instance;
        }

        GameObject stateObject = new GameObject("CleanWave_RunState");
        return stateObject.AddComponent<CleanWaveRunState>();
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        score = CleanWaveGameConstants.StartingGameScore;
    }

    public void RecordCorrectSort(TrashType trashType)
    {
        CoinWallet.EnsureInstance().AddCoins(CleanWaveGameConstants.CorrectSortCoinReward);
        ScoreChanged?.Invoke(Score, Coins);
    }

    public void ApplyWrongSortPenalty()
    {
        score = Mathf.Max(0, score + CleanWaveGameConstants.WrongSortScorePenalty);
        ScoreChanged?.Invoke(Score, Coins);
    }

}
