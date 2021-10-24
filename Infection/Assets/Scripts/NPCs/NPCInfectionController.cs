using System.Collections.Generic;
using System.Linq;
using Player.Enums;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class NPCInfectionController : MonoBehaviour
{
    [Header("Infection Rate Parameters")]
    [SerializeField] private float infectionRate = 0.15f;
    
    [Header("Contamination Rate Parameters")]
    [SerializeField] private float maskContaminationRate = 0.2f;
    [SerializeField] private float noMaskContaminationRate = 0.4f;
    [SerializeField] private float susceptibleContaminationRate = 0.6f;
    [SerializeField] private float vaccinatedContaminationRate = 0.05f;

    [Header("Other Parameters")]
    [SerializeField] private NPCType[] infectedTypes;
    [SerializeField] private float infectionRange;
    
    [HideInInspector] [Tooltip("Parent GameObject, Previous Type, New Type")]
    public UnityEvent<GameObject, NPCType, NPCType> npcTypeUpdated;
    
    [Header("Tilemap References")]
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase tile;
    
    private NPCType _npcType;
    private Dictionary<Vector3, TileBase> _rails;
    private Vector3 _currentTilePosition;

    private bool _isInfected => infectedTypes.Contains(_npcType);
    
    private void Start()
    {
        FindRailsInTilemap();
    }

    private void FixedUpdate()
    {
        var closestTiles = _rails.Keys
            .Where(r => Vector3.Distance(r, transform.position) < infectionRange)
            .OrderBy(r => Vector3.Distance(r, transform.position))
            .ToArray();
        
        // If no tile is close or it's the same as last update nothing to do
        if (!closestTiles.Any() || _currentTilePosition == closestTiles.First()) 
            return;

        _currentTilePosition = closestTiles.First();

        if (_isInfected)
            AttemptInfection(_rails[_currentTilePosition]);
        else
            AttemptContamination();
    }

    public void SetTypeNPC(NPCType type)
    {
        _npcType = type;
    }

    private void AttemptInfection(TileBase tile)
    {
        if (Random.value > infectionRate)
            return;
        
        Debug.Log($"Infect {tile.name}");
    }

    private void AttemptContamination()
    {
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
        
        npcTypeUpdated.Invoke(gameObject, _npcType, _npcType == NPCType.Mask ? NPCType.MaskInfected : NPCType.Infected);
    }
    
    private void FindRailsInTilemap()
    {
        _rails = new Dictionary<Vector3, TileBase>();

        for (var x = tilemap.cellBounds.xMin; x < tilemap.cellBounds.xMax; x++)
        {
            for (var y = tilemap.cellBounds.yMin; y < tilemap.cellBounds.yMax; y++)
            {
                var coordinates = new Vector3Int(x, y, (int)tilemap.transform.position.z);
                    
                // If there is no tile or the tile is not in the list of tiles we are looking for at the current coordinates continue
                var foundTile = tilemap.GetTile(coordinates);
                if (!tilemap.HasTile(coordinates) || tile.name != foundTile.name) 
                    continue;
                    
                _rails.Add(tilemap.GetCellCenterWorld(coordinates), foundTile);
            }
        }
    }
}
