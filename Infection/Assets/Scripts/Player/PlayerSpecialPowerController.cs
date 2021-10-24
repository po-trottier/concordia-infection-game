using System.Collections;
using Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerSpecialPowerController : MonoBehaviour
    {
        [Header("Parameters")] 
        [Range(1f, 10f)] [SerializeField] private float specialDuration = 5f;
        [Range(0.1f, 1f)] [SerializeField] private float timeModulationMultiplier = 0.25f;
        
        [Header("Events")] 
        public UnityEvent specialPowerUsed;
        
        [Header("Audio")] 
        [SerializeField] private AudioSource noEnemiesInRange;
        [SerializeField] private AudioSource slowDown;
        [SerializeField] private AudioSource faster;
        
        [Header("References")] 
        [SerializeField] private PlayerInputManager input;
        [SerializeField] private PlayerMovementController movementController;

        private bool _ready;
        private bool _inProgress;
        private float _countdown;

        private Coroutine _coroutine;

        private void Start()
        {
            input.reference.actions[ActionTypes.Special].performed += OnSpecialPerformed;

            _ready = false;
        }

        public void OnSpecialPowerReady()
        {
            _countdown = specialDuration;
            _ready = true;
        }

        public void OnGamePaused(bool isPaused)
        {
            if (isPaused)
            {
                if (_coroutine != null)
                    StopCoroutine(_coroutine);
            }
            else if (_inProgress)
            {
                Time.timeScale = timeModulationMultiplier;
                _coroutine = StartCoroutine(CountdownCoroutine());
            }
        }

        public void OnRoundEnding()
        {
            _inProgress = false;
            _ready = false;
        }

        private void OnSpecialPerformed(InputAction.CallbackContext _)
        {
            if (!_ready)
            {
                noEnemiesInRange.Play();
                return;
            }

            slowDown.Play();
            
            _inProgress = true;
            
            movementController.desiredPlayerSpeed /= timeModulationMultiplier;
            Physics.gravity = new Vector3(0f, Mathf.Pow(Physics.gravity.y, 1 / timeModulationMultiplier), 0f);
            Time.timeScale = timeModulationMultiplier;

            _coroutine = StartCoroutine(CountdownCoroutine());
        }

        private IEnumerator CountdownCoroutine()
        {
            while (_countdown >= 0)
            {
                yield return new WaitForSecondsRealtime(1);
                _countdown--;
            }
            
            faster.Play();
            
            movementController.desiredPlayerSpeed *= timeModulationMultiplier;
            Physics.gravity = new Vector3(0f, Mathf.Pow(Physics.gravity.y, timeModulationMultiplier), 0f);
            Time.timeScale = 1f;

            _inProgress = false;
            _ready = false;
            
            specialPowerUsed ??= new UnityEvent();
            specialPowerUsed.Invoke();
        }
    }
}