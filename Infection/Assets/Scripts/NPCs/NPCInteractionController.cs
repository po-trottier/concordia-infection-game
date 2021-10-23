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

        private NPCType _npcType;
        private bool _waiting;

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
            UpdateTypeNPC(NPCType.Mask);
        }

        public void Vaccinated()
        {
            UpdateTypeNPC(NPCType.Vaccinated);
        }

        public void CCed()
        {
            npcScared.Invoke();
        }
    }
}