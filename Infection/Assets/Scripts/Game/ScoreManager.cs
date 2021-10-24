using System;
using Game;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Header("Parameters")] 
    [SerializeField] private int lives = 3;

    [Header("Audio")] 
    [SerializeField] private AudioSource lifeLost;
    
    [Header("References")] 
    [SerializeField] private RoundManager roundManager;
    
    private int _score;
    
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
        return lives;
    }

    public void UpdateLives(int delta)
    {
        lives += delta;
        lives = Math.Max(lives, 0);
        
        // If we lost a life
        if (delta < 0)
            lifeLost.Play();
        
        if (lives == 0)
            roundManager.EndRound(false);
    }
}
