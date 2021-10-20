using Common;
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
        
        [Header("Events")]
        public UnityEvent<RoundModel> roundStarting;
        public UnityEvent roundEnded;

        [HideInInspector]
        public uint round;

        private void Start()
        {
            roundStarting ??= new UnityEvent<RoundModel>();
            StartRound();
        }

        public void StartRound()
        {
            roundStarting.Invoke(new RoundModel(++round, roundTotalTime, roundStopSpawnTime));
        }
    }
}