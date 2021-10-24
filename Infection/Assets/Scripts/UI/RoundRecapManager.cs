using System.Collections;
using Game;
using TMPro;
using UnityEngine;

namespace UI
{
    public class RoundRecapManager : MonoBehaviour
    {
        [Header("Text Parameters")]
        [SerializeField] private string successText = "You stopped the spread!";
        [SerializeField] private string failureText = "You were unable to stop the spread...";
        [SerializeField] private int countdown = 3;
        
        [Header("Colors")]
        [SerializeField] private Color successColor = Color.green;
        [SerializeField] private Color failureColor = Color.red;
        
        [Header("Text Field References")]
        [SerializeField] private GameObject betterLuckNextTimeGroup;
        [SerializeField] private GameObject countdownGroup;
        [SerializeField] private TextMeshProUGUI countdownText;
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private TextMeshProUGUI scoreText;
        
        [Header("Menu References")]
        [SerializeField] private GameObject roundRecap;
        
        [Header("Other References")]
        [SerializeField] private RoundManager roundManager;
        [SerializeField] private ScoreManager scoreManager;

        private int _currentCountdown;
        
        private void Start()
        {
            roundRecap.SetActive(false);
        }

        public void OnRoundEnding(bool success)
        {
            _currentCountdown = countdown;
            
            statusText.color = success ? successColor : failureColor;
            statusText.text = success ? successText : failureText;
            
            scoreText.text = scoreManager.GetScore().ToString("D4");
            
            roundRecap.SetActive(true);
            
            countdownGroup.SetActive(success);
            betterLuckNextTimeGroup.SetActive(!success);
            
            if (success)
            {
                countdownText.text = _currentCountdown.ToString();
                StartCoroutine(CountdownCoroutine());
            }
        }

        private IEnumerator CountdownCoroutine()
        {
            while (_currentCountdown >= 0)
            {
                yield return new WaitForSecondsRealtime(1);
                countdownText.text = (_currentCountdown--).ToString();
            }
            
            roundRecap.SetActive(false);
            roundManager.StartRound();
        }
    }
}