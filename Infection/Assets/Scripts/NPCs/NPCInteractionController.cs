using Player.Enums;
using UnityEngine;
using UnityEngine.Events;

namespace NPCs
{
    public class NPCInteractionController : MonoBehaviour
    {
        [Header("Events")]
        public UnityEvent npcScared;
        [HideInInspector] [Tooltip("Parent GameObject, Previous Type, New Type")]
        public UnityEvent<GameObject, NPCType, NPCType> npcTypeUpdated;

        private ScoreManager _scoreManager;
        
        private NPCType _npcType;
        private bool _waiting;

        private void Start()
        {
            _scoreManager = FindObjectOfType<ScoreManager>();

            if (_scoreManager == null)
                throw new UnityException("No Score Manager was found");
        }

        public NPCType GetTypeNPC()
        {
            return _npcType;
        }
        public bool IsWaiting()
        {
            return _waiting;
        }
        
        // Only use on initialization, this method will not invoke the type updated event
        public void SetTypeNPC(NPCType type)
        {
            _npcType = type;
        }
        
        // Only use on initialization, this method will not invoke the type updated event
        public void SetWaiting(bool waiting)
        {
            _waiting = waiting;
        }

        private void UpdateTypeNPC(NPCType type)
        {
            npcTypeUpdated.Invoke(gameObject, _npcType, type);
        }

        public void Masked()
        {
            _scoreManager.UpdateScore(1);
            UpdateTypeNPC(_npcType == NPCType.Infected ? NPCType.MaskInfected : NPCType.Mask);
        }

        public void Vaccinated()
        {
            _scoreManager.UpdateScore(1);
            UpdateTypeNPC(NPCType.Vaccinated);
        }

        public void CCed()
        {
            _scoreManager.UpdateScore(1);
            npcScared.Invoke();
        }
    }
}