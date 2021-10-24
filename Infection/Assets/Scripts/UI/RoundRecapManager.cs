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
        [SerializeField] private GameObject player;

        private int _currentCountdown;
        private Coroutine _coroutine;
        
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

            // Disable player between rounds
            player.SetActive(false);
            
            if (success)
            {
                countdownText.text = _currentCountdown.ToString();
                _coroutine = StartCoroutine(CountdownCoroutine());
            }
        }

        public void OnGamePaused(bool paused)
        {
            if (paused)
            {
                if (_coroutine != null)
                    StopCoroutine(_coroutine);
            }
            else
            {
                _coroutine = StartCoroutine(CountdownCoroutine());
            }
        }

        private IEnumerator CountdownCoroutine()
        {
            while (_currentCountdown >= 0)
            {
                yield return new WaitForSecondsRealtime(1);
                countdownText.text = (_currentCountdown--).ToString();
            }
            
            // Re-enable player when round starts
            player.SetActive(true);
            
            roundRecap.SetActive(false);
            roundManager.StartRound();
        }
    }
}