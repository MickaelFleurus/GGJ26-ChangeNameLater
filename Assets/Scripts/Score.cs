using UnityEngine;
using System;

public class Score
{
    private static Score instance;
    public static Score Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new Score();
            }
            return instance;
        }
    }

    private float mStartTime;
    private float mEndTime;
    private int mScore;

    private Score()
    {
        // Private constructor to prevent instantiation
        GameEvents.OnInGame += StartGame;
        GameEvents.OnGameWon += EndGame;
        GameEvents.OnLootCollectedWithData += IncreaseScore;
    }

    private void StartGame()
    {
        mScore = 0;
        mStartTime = Time.time;
    }

    private void EndGame()
    {
        mEndTime = Time.time;
    }

    private void IncreaseScore(int value, LootType lootType)
    {
        mScore += value;
    }

    public string GetScore() { return mScore.ToString(); }

    public int GetScoreFloat() { return mScore; }

    public string GetDurationAsString()
    {
        float duration = mEndTime - mStartTime;
        int minutes = (int)(duration / 60);
        int seconds = (int)(duration % 60);
        return $"{minutes:00}:{seconds:00}";
    }
}
