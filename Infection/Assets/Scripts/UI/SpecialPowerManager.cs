using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class SpecialPowerManager : MonoBehaviour
{
    [Header("Parameters")] 
    [SerializeField] private float specialCooldown = 15f;
    
    [Header("Events")] 
    public UnityEvent specialPowerReady;
    
    [Header("References")]
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private GameObject countdownGroup;
    [SerializeField] private GameObject readyGroup;

    private bool _paused;
    private float _timeLeft;
    private Coroutine _coroutine;

    private void FixedUpdate()
    {
        countdownText.text = ((int)_timeLeft).ToString();
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
        ResetCooldown();
    }

    public void OnRoundEnding()
    {
        if (_coroutine != null)
            StopCoroutine(_coroutine);
        
        _timeLeft = specialCooldown;
    }

    public void OnSpecialPowerUsed()
    {
        ResetCooldown();
    }

    private void ResetCooldown()
    {
        if (_coroutine != null)
            StopCoroutine(_coroutine);
        
        _timeLeft = specialCooldown;
        
        countdownGroup.SetActive(true);
        readyGroup.SetActive(false);
        
        _coroutine = StartCoroutine(CountdownCoroutine());
    }
    
    private IEnumerator CountdownCoroutine()
    {
        // Make sure we spawn the right amount of NPCs
        while (_timeLeft > 0)
        {
            _timeLeft--;
            yield return new WaitForSecondsRealtime(1);
        }
            
        countdownGroup.SetActive(false);
        readyGroup.SetActive(true);
        
        specialPowerReady ??= new UnityEvent();
        specialPowerReady.Invoke();
    }
}
