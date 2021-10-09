using System;
using System.Timers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CountdownManager : MonoBehaviour
{
    [Header("Timer Parameters")]
    [SerializeField] private float countdownTime = 60f;
    [SerializeField] private float dangerTime = 10f;
    
    [Header("Color Parameters")]
    [SerializeField] private Color healthyColor = Color.green;
    [SerializeField] private Color dangerColor = Color.red;
    
    [Header("Countdown Parameters")]
    [SerializeField] private float animationTime = 0.75f;
    [SerializeField] private string timeRemainingText = "{0} seconds left";
    
    [Header("References")]
    [SerializeField] private Slider slider;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI textObject;

    private Timer _timer;
    private float _timeLeft;

    public void UpdateTimerState(bool isPaused)
    {
        if (isPaused)
            _timer.Stop();
        else 
            _timer.Start();
    }

    private void Start()
    {
        _timeLeft = countdownTime;
        
        _timer = new Timer(1000);
        _timer.AutoReset = true;
        _timer.Elapsed += (sender, args) =>  _timeLeft--;;
        
        _timer.Start();
    }

    private void OnDestroy()
    {
        _timer.Dispose();
    }

    private void FixedUpdate()
    {
        slider.value = Mathf.Lerp(slider.value, _timeLeft / countdownTime, animationTime * Time.fixedDeltaTime);

        textObject.text = String.Format(timeRemainingText, _timeLeft);
        
        if (_timeLeft < dangerTime)
        {
            image.color = dangerColor;
        }
        else
        {
            image.color = healthyColor;
        }
    }
}
