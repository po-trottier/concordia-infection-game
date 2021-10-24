using System.Linq;
using Game;
using Player.Enums;
using UnityEngine;
using UnityEngine.Events;

public class NPCInfectionController : MonoBehaviour
{
    [Header("Infection Rate Parameters")]
    [Range(0f, 1f)] [SerializeField] private float noMaskInfectionRate = 0.2f;
    [Range(0f, 1f)] [SerializeField] private float maskInfectionRate = 0.05f;
    
    [Header("Contamination Rate Parameters")]
    [Range(0f, 1f)] [SerializeField] private float maskContaminationRate = 0.4f;
    [Range(0f, 1f)] [SerializeField] private float noMaskContaminationRate = 0.5f;
    [Range(0f, 1f)] [SerializeField] private float susceptibleContaminationRate = 0.9f;
    [Range(0f, 1f)] [SerializeField] private float vaccinatedContaminationRate = 0.1f;

    [Header("Other Parameters")]
    [SerializeField] private NPCType[] infectedTypes;
    [SerializeField] private float infectionRange = 0.5f;

    [Header("Audio")] 
    [SerializeField] private AudioSource infectionAudio;
    [SerializeField] private AudioSource contaminationAudio;
    
    [HideInInspector] [Tooltip("Parent GameObject, Previous Type, New Type")]
    public UnityEvent<GameObject, NPCType, NPCType> npcTypeUpdated;
    
    private NPCType _npcType;
    private Vector3 _currentTilePosition;
    private InfectionManager _infectionManager;

    private bool _isInfected => infectedTypes.Contains(_npcType);
    
    private void Start()
    {
        _infectionManager ??= FindObjectOfType<InfectionManager>();

        if (_infectionManager == null)
            throw new UnityException("No InfectionManager was found");
    }

    private void FixedUpdate()
    {
        // Get the closest infected tiles
        var closestTiles = _infectionManager.GetRails()
            .Where(r => Vector3.Distance(r, transform.position) < infectionRange)
            .OrderBy(r => Vector3.Distance(r, transform.position))
            .ToArray();
        
        // If no infected tile is close or it's the same as last update nothing to do
        if (!closestTiles.Any() || _currentTilePosition == closestTiles.First()) 
            return;
        
        _currentTilePosition = closestTiles.First();

        if (_isInfected)
            AttemptInfection(_currentTilePosition);
        else
            AttemptContamination(closestTiles);
    }

    public void SetTypeNPC(NPCType type)
    {
        _npcType = type;
    }

    private void AttemptInfection(Vector3 railPosition)
    {
        var infectionRate = 0f;
        
        switch (_npcType)
        {
            case NPCType.Infected:
                infectionRate = noMaskInfectionRate;
                break;
            case NPCType.MaskInfected:
                infectionRate = maskInfectionRate;
                break;
        }
        
        if (Random.value > infectionRate)
            return;
        
        infectionAudio.Play();
        
        _infectionManager.UpdateRailInfectionStatus(railPosition, true);
    }

    private void AttemptContamination(Vector3[] closestTiles)
    {
        var closestInfectedTiles = closestTiles.Where(r => _infectionManager.GetRailInfectionStatus(r));
        
        if (!closestInfectedTiles.Any())
            return;

        float contaminationRate = 0f;

        switch (_npcType)
        {
            case NPCType.Mask:
                contaminationRate = maskContaminationRate;
                break;
            case NPCType.NoMask:
                contaminationRate = noMaskContaminationRate;
                break;
            case NPCType.Susceptible:
                contaminationRate = susceptibleContaminationRate;
                break;
            case NPCType.Vaccinated:
                contaminationRate = vaccinatedContaminationRate;
                break;
        }
        
        if (Random.value > contaminationRate)
            return;
        
        AudioSource.PlayClipAtPoint(contaminationAudio.clip, transform.position);
        npcTypeUpdated.Invoke(gameObject, _npcType, _npcType == NPCType.Mask ? NPCType.MaskInfected : NPCType.Infected);
    }
}
