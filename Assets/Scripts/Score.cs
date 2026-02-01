using UnityEngine;
using System;

public class Score : MonoBehaviour
{

    private static Score instance;
    public static Score Instance => instance;


    private float mStartTime;
    private float mEndTime;

    private int mScore;
    void Awake()
    {
        instance = this;
        GameEvents.OnInGame += StartGame;
        GameEvents.OnGameWon += EndGame;
        GameEvents.OnLootCollectedWithData += IncreaseScore;
    }

    void StartGame()
    {
        mScore = 0;
        mStartTime = Time.time;
    }

    void EndGame()
    {
        mEndTime = Time.time;
    }

    void IncreaseScore(int value, LootType lootType)
    {
        mScore += value;
    }

    public string GetScore() { return mScore.ToString(); }

    public string GetDurationAsString()
    {
        float duration = mEndTime - mStartTime;
        int minutes = (int)(duration / 60);
        int seconds = (int)(duration % 60);
        return $"{minutes:00}:{seconds:00}";
    }
}
