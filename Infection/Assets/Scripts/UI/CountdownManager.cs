using System;
using System.Collections;
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
    public UnityEvent dangerTimeReached;
    
    [Header("References")]
    [SerializeField] private Slider slider;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI textObject;
    [SerializeField] private RoundManager roundManager;

    private bool _paused;
    private float _timeLeft;
    private Coroutine _coroutine;

    private void FixedUpdate()
    {
        slider.value = Mathf.Lerp(slider.value, _timeLeft / roundManager.roundTotalTime, animationTime * Time.fixedDeltaTime);

        textObject.text = String.Format(timeRemainingText, _timeLeft);
        
        image.color = _timeLeft < roundManager.roundDangerTime ? dangerColor : healthyColor;
    }

    public void OnGamePaused(bool isPaused)
    {
        _paused = isPaused;
        if (_paused)
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);
        }
        else
        {
            _coroutine = StartCoroutine(CountdownCoroutine());
        }
    }

    public void OnRoundStarting()
    {
        if (_coroutine != null)
            StopCoroutine(_coroutine);
        
        _timeLeft = roundManager.roundTotalTime;
        
        _coroutine = StartCoroutine(CountdownCoroutine());
    }

    public void OnRoundEnding()
    {
        if (_coroutine != null)
            StopCoroutine(_coroutine);
        
        _timeLeft = 0f;
    }
    
    private IEnumerator CountdownCoroutine()
    {
        // Make sure we spawn the right amount of NPCs
        while (_timeLeft > 0)
        {
            _timeLeft--;
            
            if (Math.Abs(_timeLeft - roundManager.roundDangerTime) < float.Epsilon)
            {
                dangerTimeReached ??= new UnityEvent();
                dangerTimeReached.Invoke();
            }
            
            yield return new WaitForSecondsRealtime(1);
        }
            
        countdownReached ??= new UnityEvent();
        countdownReached.Invoke();
    }
}
