using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Game;
using NPCs;
using Player.Enums;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionController : MonoBehaviour
{
    [Header("Cleanup Parameters")]
    [SerializeField] private float cleanupRange = 0.5f;
    
    [Header("NPC Types")]
    [SerializeField] private NPCType[] canBeMasked;
    [SerializeField] private NPCType[] canBeVaccinated;
    
    [Header("Audio")] 
    [SerializeField] private AudioSource noEnemiesInRange;
    [SerializeField] private AudioSource masked;
    [SerializeField] private AudioSource vaccinated;
    [SerializeField] private AudioSource cced;
    [SerializeField] private AudioSource cleaned;
    
    [Header("References")] 
    [SerializeField] private PlayerInputManager input;
    [SerializeField] private InfectionManager infectionManager;
    [SerializeField] private Vector3 playerTransform;
    
    private Dictionary<int, NPCInteractionController> _npcsInRange;

    private void Start()
    {
        input.reference.actions[ActionTypes.Mask].performed += OnMaskPerformed;
        input.reference.actions[ActionTypes.Vaccinate].performed += OnVaccinatePerformed;
        input.reference.actions[ActionTypes.CC].performed += OnCCPerformed;
        input.reference.actions[ActionTypes.Clean].performed += OnCleanPerformed;
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

    private void OnCleanPerformed(InputAction.CallbackContext callbackContext)
    {
        var tileOffset = new Vector3(0f, 0.5f, 0f);
        
        var closestInfectedTiles = infectionManager.GetRails()
            .Where(r => infectionManager.GetRailInfectionStatus(r))
            .Where(r => Vector3.Distance(r, transform.position - playerTransform + tileOffset) < cleanupRange)
            .OrderBy(r => Vector3.Distance(r, transform.position - playerTransform + tileOffset))
            .ToArray();
        
        if (closestInfectedTiles.Length <= 0)
        {
            noEnemiesInRange.Play();
            return;
        }
            
        cleaned.Play();
        
        infectionManager.UpdateRailInfectionStatus(closestInfectedTiles.First(), false);
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
