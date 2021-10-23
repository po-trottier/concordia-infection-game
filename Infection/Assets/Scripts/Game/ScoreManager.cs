using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int _score;
    private int _lives = 3;
    
    public int GetScore()
    {
        return _score;
    }

    public void UpdateScore(int delta)
    {
        _score += delta;
        _score = Math.Max(_score, 0);
    }
    
    public int GetLives()
    {
        return _lives;
    }

    public void UpdateLives(int delta)
    {
        _lives += delta;
        _lives = Math.Max(_lives, 0);
    }
}
