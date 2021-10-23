using System.Collections.Generic;
using System.Linq;
using Common;
using NPCs;
using Player.Enums;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionController : MonoBehaviour
{
    [Header("NPCTypes")]
    [SerializeField] private NPCType[] canBeMasked;
    [SerializeField] private NPCType[] canBeVaccinated;
    
    [Header("Audio")] 
    [SerializeField] private AudioSource noEnemiesInRange;
    [SerializeField] private AudioSource masked;
    [SerializeField] private AudioSource vaccinated;
    [SerializeField] private AudioSource cced;
    
    [Header("References")] 
    [SerializeField] private PlayerInputController input;
    
    private Dictionary<int, NPCInteractionController> _npcsInRange;

    private void Start()
    {
        input.reference.actions[ActionTypes.Mask].performed += OnMaskPerformed;
        input.reference.actions[ActionTypes.Vaccinate].performed += OnVaccinatePerformed;
        input.reference.actions[ActionTypes.CC].performed += OnCCPerformed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        UpdateInRangeNPC(other, CollisionState.Enter);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        UpdateInRangeNPC(other, CollisionState.Exit);
    }

    private void OnMaskPerformed(InputAction.CallbackContext callbackContext)
    {
        if (!ValidateInRangeNPC())
            return;

        List<NPCInteractionController> validNPCs = new List<NPCInteractionController>();
        validNPCs.AddRange(_npcsInRange.Values.Where(npc => npc != null && canBeMasked.Contains(npc.GetTypeNPC())));
        
        if(validNPCs.Count <= 0)
        {
            noEnemiesInRange.Play();
            return;
        }

        foreach (var npc in validNPCs)
        {
            npc.Masked();
        }
        
        masked.Play();
    }

    private void OnVaccinatePerformed(InputAction.CallbackContext callbackContext)
    {
        if (!ValidateInRangeNPC())
            return;
        
        List<NPCInteractionController> validNPCs = new List<NPCInteractionController>();
        validNPCs.AddRange(_npcsInRange.Values.Where(npc => npc != null && canBeVaccinated.Contains(npc.GetTypeNPC())));
        
        if(validNPCs.Count <= 0)
        {
            noEnemiesInRange.Play();
            return;
        }

        foreach (var npc in validNPCs)
        {
            npc.Vaccinated();
        }
        
        vaccinated.Play();
    }

    private void OnCCPerformed(InputAction.CallbackContext callbackContext)
    {
        if (!ValidateInRangeNPC())
            return;
        
        // Only unmasked & susceptible NPCs can be masked
        List<NPCInteractionController> validNPCs = new List<NPCInteractionController>();
        validNPCs.AddRange(_npcsInRange.Values.Where(npc => npc != null && npc.IsWaiting()));
        
        if(validNPCs.Count <= 1)
        {
            noEnemiesInRange.Play();
            return;
        }

        foreach (var npc in validNPCs)
        {
            npc.CCed();
        }
        
        cced.Play();
    }

    private void UpdateInRangeNPC(Collider2D other, CollisionState state)
    {
        if (!other.gameObject.CompareTag(Tags.NPC))
            return;
        
        _npcsInRange ??= new Dictionary<int, NPCInteractionController>();
        
        if (state == CollisionState.Enter)
            _npcsInRange.Add(other.gameObject.GetInstanceID(), other.gameObject.GetComponentInParent<NPCInteractionController>());
        else
            _npcsInRange.Remove(other.gameObject.GetInstanceID());
    }

    private bool ValidateInRangeNPC()
    {
        _npcsInRange ??= new Dictionary<int, NPCInteractionController>();
        
        var inRange = _npcsInRange.Count > 0;

        if (!inRange)
            noEnemiesInRange.Play();
        
        return inRange;
    }
}
