using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common;
using Player.Enums;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace NPCs
{
    public class NPCSpawner : MonoBehaviour
    {
        [Header("Parameters")]
        [SerializeField] private uint initialCountNPC = 20;
        [SerializeField] private float roundIncreaseMultiplierNPC = 1.25f;
        [SerializeField] private float initialSpeedNPC = 0.5f;
        [SerializeField] private float roundIncreaseMultiplierSpeed = 1.1f;
        
        [Header("NPC Parameters")]
        [Tooltip("The chances to spawn an infected NPC. Total of all the rates should be 1")]
        [Range(0, 1)] [SerializeField] private float infectedRate = 0.2f;
        [Tooltip("The chances to spawn a masked NPC. Total of all the rates should be 1")]
        [Range(0, 1)] [SerializeField] private float maskRate = 0.2f;
        [Tooltip("The chances to spawn an unmasked NPC. Total of all the rates should be 1")]
        [Range(0, 1)] [SerializeField] private float noMaskRate = 0.2f;
        [Tooltip("The chances to spawn a susceptible NPC. Total of all the rates should be 1")]
        [Range(0, 1)] [SerializeField] private float susceptibleRate = 0.2f;
        [Tooltip("The chances to spawn a vaccinated NPC. Total of all the rates should be 1")]
        [Range(0, 1)] [SerializeField] private float vaccinatedRate = 0.2f;
        
        [Header("Tilemap References")]
        [SerializeField] private Tilemap doorsTilemap;
        [SerializeField] private Tilemap shopsTilemap;
        
        [Header("Tile References")] 
        [Tooltip("Select the bottom tile as to spawn the NPC at the right height")]
        [SerializeField] private TileBase[] doorTiles;
        [Tooltip("Select the bottom left tile as we will add offsets for each NPC present at the shop")]
        [SerializeField] private TileBase[] shopTiles;
        
        [Header("NPC References")]
        [SerializeField] private GameObject infectedPrefab;
        [SerializeField] private GameObject maskPrefab;
        [SerializeField] private GameObject noMaskPrefab;
        [SerializeField] private GameObject susceptiblePrefab;
        [SerializeField] private GameObject vaccinatedPrefab;

        [HideInInspector] public uint infectedCount;
        [HideInInspector] public uint maskCount;
        [HideInInspector] public uint noMaskCount;
        [HideInInspector] public uint susceptibleCount;
        [HideInInspector] public uint vaccinatedCount;
        
        private Vector3[] _doorPositions;
        private Vector3[] _shopPositions;
        
        private uint _spawnedCountNPC;
        private uint _currentCountNPC;
        private float _currentSpeedtNPC;
        private float _spawnDelay;

        public void OnRoundStarting(RoundModel round)
        {
            // Initialize values during round 1
            if (round.RoundCount == 1)
            {
                _currentCountNPC = initialCountNPC;
                _currentSpeedtNPC = initialSpeedNPC;
            
                CleanNPCs();
                _doorPositions = FindInTilemap(doorsTilemap, doorTiles, true);
                _shopPositions = FindInTilemap(shopsTilemap, shopTiles);
                
                if (_doorPositions.Length == 0 || _shopPositions.Length == 0)
                    throw new UnityException("No door or shops were found.");
            }
            else
            {
                // Increment values for all subsequent rounds
                _currentCountNPC = (uint)(_currentCountNPC * roundIncreaseMultiplierNPC);
                _currentSpeedtNPC *= roundIncreaseMultiplierSpeed;
            }

            _spawnedCountNPC = 0;

            CalculateSpawnDelay(round.TotalTime, round.SafeTime);

            StartCoroutine(SpawnCoroutine());
        }

        public void OnRoundEnding()
        {
            StopCoroutine(SpawnCoroutine());
            CleanNPCs();
        }

        public void OnNPCDestroyed(NPCType type)
        {
            switch (type)
            {
                case NPCType.Infected: 
                    infectedCount--;
                    break;
                case NPCType.Mask: 
                    maskCount--;
                    break;
                case NPCType.NoMask: 
                    noMaskCount--;
                    break;
                case NPCType.Susceptible: 
                    susceptibleCount--;
                    break;
                case NPCType.Vaccinated: 
                    vaccinatedCount--;
                    break;
            }   
        }
        
        private IEnumerator SpawnCoroutine()
        {
            // Make sure we spawn the right amount of NPCs
            while (_spawnedCountNPC <= _currentCountNPC)
            {
                SpawnNPC();
                yield return new WaitForSeconds(_spawnDelay);
            }
        }

        private void SpawnNPC()
        {
            GameObject prefab = null;
            var npcType = GetRandomTypeNPC();
            switch (npcType)
            {
                case NPCType.Infected: 
                    prefab = infectedPrefab;
                    infectedCount++;
                    break;
                case NPCType.Mask: 
                    prefab = maskPrefab;
                    maskCount++;
                    break;
                case NPCType.NoMask: 
                    prefab = noMaskPrefab;
                    noMaskCount++;
                    break;
                case NPCType.Susceptible: 
                    prefab = susceptiblePrefab;
                    susceptibleCount++;
                    break;
                case NPCType.Vaccinated: 
                    prefab = vaccinatedPrefab;
                    vaccinatedCount++;
                    break;
            }

            if (prefab == null)
                throw new UnityException("Invalid NPC Type Selected. Make sure the sum of the spawn rates is equal to 1.");

            int doorIndex = (int)Math.Round((double)Random.Range(0, _doorPositions.Length));
            int exitIndex = (int)Math.Round((double)Random.Range(0, _doorPositions.Length));
            int shopIndex = (int)Math.Round((double)Random.Range(0, _shopPositions.Length));
            
            var npc = Instantiate(prefab);
            npc.transform.position = _doorPositions[doorIndex];

            var controller = npc.GetComponent<NPCController>();
            controller.SetTargetPosition(_shopPositions[shopIndex]);
            controller.SetExitPosition(_doorPositions[exitIndex]);
            controller.SetSpeed(_currentSpeedtNPC);
            controller.SetType(npcType);
            
            controller.npcDestroyed.AddListener(OnNPCDestroyed);

            _spawnedCountNPC++;
        }
        
        private static void CleanNPCs()
        {
            var npcs = GameObject.FindGameObjectsWithTag(Tags.NPC);
            foreach (var npc in npcs)
            {
                Destroy(npc);
            }
        }

        private NPCType GetRandomTypeNPC()
        {
            float randomSeed = Random.value;

            if (randomSeed < infectedRate)
                return NPCType.Infected;

            if (randomSeed < infectedRate + maskRate)
                return NPCType.Mask;

            if (randomSeed < infectedRate + maskRate + noMaskRate)
                return NPCType.NoMask;

            if (randomSeed < infectedRate + maskRate + noMaskRate + susceptibleRate)
                return NPCType.Susceptible;

            if (randomSeed >= 1 - vaccinatedRate)
                return NPCType.Vaccinated;

            return NPCType.None;
        }

        private Vector3[] FindInTilemap(Tilemap tilemap, TileBase[] tiles, bool offsetPosition = false)
        {
            var foundPositions = new List<Vector3>();
            var tilesList = new List<TileBase>(tiles);

            for (var x = tilemap.cellBounds.xMin; x < tilemap.cellBounds.xMax; x++)
            {
                for (var y = tilemap.cellBounds.yMin; y < tilemap.cellBounds.yMax; y++)
                {
                    var coordinates = new Vector3Int(x, y, (int)tilemap.transform.position.z);
                    
                    // If there is no tile or the tile is not in the list of tiles we are looking for at the current coordinates continue
                    if (!tilemap.HasTile(coordinates) || !tilesList.Find(t => t.name == tilemap.GetTile(coordinates).name)) 
                        continue;
                    
                    // Shift by 1 cell in the right direction
                    var shift =  new Vector3Int(coordinates.x > 0 ? 1 : -1, 0, 0);
                    
                    foundPositions.Add(tilemap.GetCellCenterWorld(coordinates + (offsetPosition ? shift : Vector3Int.zero)));
                }
            }
            
            return foundPositions.ToArray();
        }
        
        private void CalculateSpawnDelay(float roundTotalTime, float roundSafeTime)
        {
            _spawnDelay = Mathf.Floor((roundTotalTime - roundSafeTime) / _currentCountNPC);
        }
    }
}