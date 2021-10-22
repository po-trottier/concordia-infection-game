using System;
using System.Collections;
using System.Timers;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CountdownManager : MonoBehaviour
{
    [Header("Color Parameters")]
    [SerializeField] private Color healthyColor = Color.green;
    [SerializeField] private Color dangerColor = Color.red;
    
    [Header("Countdown Parameters")]
    [SerializeField] private float animationTime = 0.75f;
    [SerializeField] private string timeRemainingText = "{0} seconds left";

    [Header("Events")] 
    public UnityEvent countdownReached;
    
    [Header("References")]
    [SerializeField] private Slider slider;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI textObject;
    [SerializeField] private RoundManager roundManager;

    private float _timeLeft;

    public void OnGamePaused(bool isPaused)
    {
        if (isPaused)
            StopCoroutine(CountdownCoroutine());
        else
            StartCoroutine(CountdownCoroutine());
    }

    public void OnRoundStarting()
    {
        _timeLeft = roundManager.roundTotalTime;
        
        StartCoroutine(CountdownCoroutine());
    }

    private void FixedUpdate()
    {
        slider.value = Mathf.Lerp(slider.value, _timeLeft / roundManager.roundTotalTime, animationTime * Time.fixedDeltaTime);

        textObject.text = String.Format(timeRemainingText, _timeLeft);
        
        image.color = _timeLeft < roundManager.roundDangerTime ? dangerColor : healthyColor;
    }
    
    private IEnumerator CountdownCoroutine()
    {
        // Make sure we spawn the right amount of NPCs
        while (_timeLeft > 0)
        {
            _timeLeft--;
            yield return new WaitForSecondsRealtime(1);
        }
            
        countdownReached ??= new UnityEvent();
        countdownReached.Invoke();
    }
}
