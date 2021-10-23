using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ScoreTextManager : MonoBehaviour
    {
        [Header("Sprites")]
        [SerializeField] private Sprite fullHeart;
        [SerializeField] private Sprite emptyHeart;
        
        [Header("Text Field References")]
        [SerializeField] private TextMeshProUGUI scoreText;
        
        [Header("Heart References")]
        [SerializeField] private Image heart1;
        [SerializeField] private Image heart2;
        [SerializeField] private Image heart3;

        [Header("Other References")] 
        [SerializeField] private ScoreManager scoreManager;

        private int _livesLocal;
        
        private void FixedUpdate()
        {
            scoreText.text = scoreManager.GetScore().ToString("D4");

            if (_livesLocal == scoreManager.GetLives())
                return;

            _livesLocal = scoreManager.GetLives();

            heart3.sprite = _livesLocal >= 3 ? fullHeart : emptyHeart;
            heart2.sprite = _livesLocal >= 2 ? fullHeart : emptyHeart;
            heart1.sprite = _livesLocal >= 1 ? fullHeart : emptyHeart;
        }
    }
}