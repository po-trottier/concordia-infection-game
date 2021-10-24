using Common;
using Player;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public class RoundManager : MonoBehaviour
    {
        [Header("Parameters")]
        [Tooltip("Total time allowed in the round in seconds")]
        public float roundTotalTime = 60f;
        [Tooltip("The time at which the progress bar will turn red in seconds")]
        public float roundDangerTime = 10f;
        [Tooltip("The time at which NPCs stop spawning to let the player clear out the map")]
        public float roundStopSpawnTime = 20f;
        
        [Header("Physics Parameters")]
        public Vector3 gravity = Physics.gravity;

        [Header("Events")]
        [SerializeField] private UnityEvent<RoundModel> roundStarting;
        [SerializeField] private UnityEvent<bool> roundEnded;

        [Header("Audio")] 
        [SerializeField] private AudioSource roundSuccess;
        [SerializeField] private AudioSource roundFailure;
        [SerializeField] private AudioSource dangerClock;
        
        [Header("Music")] 
        [SerializeField] private AudioSource music;
        
        [Header("References")] 
        [SerializeField] private PlayerMovementController movementController;

        [HideInInspector]
        public uint round;
        
        private void Start()
        {
            StartRound();
        }
        
        public void OnDangerTimeReached()
        {
            dangerClock.Play();
        }
        
        public void OnCountdownReached()
        {
            EndRound(true);
        }

        public void EndRound(bool success)
        {
            dangerClock.Stop();
            music.Stop();

            var audioSource = success ? roundSuccess : roundFailure;
            audioSource.Play();
            
            roundEnded ??= new UnityEvent<bool>();
            roundEnded.Invoke(success);
        }
        
        public void StartRound()
        {
            music.Play();

            Physics.gravity = gravity;
            Time.timeScale = 1f;
            movementController.desiredPlayerSpeed = movementController.playerSpeed;
            
            roundStarting ??= new UnityEvent<RoundModel>();
            roundStarting.Invoke(new RoundModel(++round, roundTotalTime, roundStopSpawnTime));
        }
    }
}