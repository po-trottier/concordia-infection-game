using System;
using System.Collections;
using System.Collections.Generic;
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
        [Range(0f, 2f)]
        [SerializeField] private float targetPositionRandomness = 1f;
        
        [Header("NPC Parameters")]
        [Tooltip("The chances to spawn an infected NPC. Total of all the rates should be 1")]
        [Range(0, 1)] [SerializeField] private float infectedRate = 1f / 6f;
        [Tooltip("The chances to spawn a masked NPC. Total of all the rates should be 1")]
        [Range(0, 1)] [SerializeField] private float maskRate = 1f / 6f;
        [Tooltip("The chances to spawn a infected masked NPC. Total of all the rates should be 1")]
        [Range(0, 1)] [SerializeField] private float maskInfectedRate = 1f / 6f;
        [Tooltip("The chances to spawn an unmasked NPC. Total of all the rates should be 1")]
        [Range(0, 1)] [SerializeField] private float noMaskRate = 1f / 6f;
        [Tooltip("The chances to spawn a susceptible NPC. Total of all the rates should be 1")]
        [Range(0, 1)] [SerializeField] private float susceptibleRate = 1f / 6f;
        [Tooltip("The chances to spawn a vaccinated NPC. Total of all the rates should be 1")]
        [Range(0, 1)] [SerializeField] private float vaccinatedRate = 1f / 6f;
        
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
        [SerializeField] private GameObject maskInfectedPrefab;
        [SerializeField] private GameObject noMaskPrefab;
        [SerializeField] private GameObject susceptiblePrefab;
        [SerializeField] private GameObject vaccinatedPrefab;

        [HideInInspector] public uint infectedCount;
        [HideInInspector] public uint maskCount;
        [HideInInspector] public uint maskInfectedCount;
        [HideInInspector] public uint noMaskCount;
        [HideInInspector] public uint susceptibleCount;
        [HideInInspector] public uint vaccinatedCount;
        
        private Vector3[] _doorPositions;
        private Vector3[] _shopPositions;
        
        private uint _spawnedCountNPC;
        private uint _currentCountNPC;
        private float _currentSpeedtNPC;
        private float _spawnDelay;
        
        private Coroutine _coroutine;

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

            _coroutine = StartCoroutine(SpawnCoroutine());
        }

        public void OnRoundEnding()
        {
            StopCoroutine(_coroutine);
            CleanNPCs();
        }

        public void OnNPCDestroyed(NPCType type)
        {
            UpdateCountNPC(type, -1);
        }

        public void OnNPCTypeChanged(GameObject npc, NPCType previous, NPCType future)
        {
            var path = npc.GetComponent<NPCPathController>();
            
            if (path == null)
                return;
            
            SpawnNPC(future, npc.transform.position, path.GetTargetPosition(), path.GetExitPosition());
            DestroyImmediate(npc);
            
            UpdateCountNPC(previous, -1);
            UpdateCountNPC(future, 1);
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
            var npcType = GetRandomTypeNPC();

            int doorIndex = (int)Math.Round((double)Random.Range(0, _doorPositions.Length));
            int exitIndex = (int)Math.Round((double)Random.Range(0, _doorPositions.Length));
            int shopIndex = (int)Math.Round((double)Random.Range(0, _shopPositions.Length));

            float randomOffset = Random.value * targetPositionRandomness;
            Vector3 positionOffset = new Vector3(randomOffset, 0f, 0f);
            
            SpawnNPC(npcType, _doorPositions[doorIndex], _shopPositions[shopIndex] + positionOffset, _doorPositions[exitIndex]);
            UpdateCountNPC(npcType, 1);

            _spawnedCountNPC++;
        }

        private void SpawnNPC(NPCType npcType, Vector3 spawnPosition, Vector3 targetPosition, Vector3 exitPosition)
        {
            GameObject prefab = null;
            switch (npcType)
            {
                case NPCType.Infected: 
                    prefab = infectedPrefab;
                    break;
                case NPCType.Mask: 
                    prefab = maskPrefab;
                    break;
                case NPCType.MaskInfected: 
                    prefab = maskInfectedPrefab;
                    break;
                case NPCType.NoMask: 
                    prefab = noMaskPrefab;
                    break;
                case NPCType.Susceptible: 
                    prefab = susceptiblePrefab;
                    break;
                case NPCType.Vaccinated: 
                    prefab = vaccinatedPrefab;
                    break;
            }

            if (prefab == null)
                throw new UnityException("Invalid NPC Type Selected. Make sure the sum of the spawn rates is equal to 1.");
            
            var npc = Instantiate(prefab);
            npc.transform.position = spawnPosition;
            npc.layer = LayerMask.NameToLayer(Layers.NPC);

            var infection = npc.GetComponent<NPCInfectionController>();
            infection.SetTypeNPC(npcType);

            var interactions = npc.GetComponent<NPCInteractionController>();
            interactions.SetTypeNPC(npcType);
            
            interactions.npcTypeUpdated.AddListener(OnNPCTypeChanged);
            
            var path = npc.GetComponent<NPCPathController>();
            path.SetTargetPosition(targetPosition);
            path.SetExitPosition(exitPosition);
            path.SetSpeed(_currentSpeedtNPC);
            path.SetTypeNPC(npcType);
            
            path.npcDestroyed.AddListener(OnNPCDestroyed);
        }
        
        private void CleanNPCs()
        {
            var npcs = GameObject.FindGameObjectsWithTag(Tags.NPC);
            foreach (var npc in npcs)
            {
                Destroy(npc);
            }

            infectedCount = 0;
            maskCount = 0;
            maskInfectedCount = 0;
            noMaskCount = 0;
            susceptibleCount = 0;
            vaccinatedCount = 0;
        }

        private NPCType GetRandomTypeNPC()
        {
            float randomSeed = Random.value;

            if (randomSeed < infectedRate)
                return NPCType.Infected;

            if (randomSeed < infectedRate + maskRate)
                return NPCType.Mask;

            if (randomSeed < infectedRate + maskRate + maskInfectedRate)
                return NPCType.MaskInfected;

            if (randomSeed < infectedRate + maskRate + maskInfectedRate + noMaskRate)
                return NPCType.NoMask;

            if (randomSeed < infectedRate + maskRate + maskInfectedRate + noMaskRate + susceptibleRate)
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

        private void UpdateCountNPC(NPCType type, int delta)
        {
            switch (type)
            {
                case NPCType.Infected: 
                    infectedCount = (uint)Math.Max(infectedCount + delta, 0);
                    break;
                case NPCType.Mask: 
                    maskCount = (uint)Math.Max(maskCount + delta, 0);
                    break;
                case NPCType.MaskInfected: 
                    maskInfectedCount = (uint)Math.Max(maskInfectedCount + delta, 0);
                    break;
                case NPCType.NoMask: 
                    noMaskCount = (uint)Math.Max(noMaskCount + delta, 0);
                    break;
                case NPCType.Susceptible: 
                    susceptibleCount = (uint)Math.Max(susceptibleCount + delta, 0);
                    break;
                case NPCType.Vaccinated: 
                    vaccinatedCount = (uint)Math.Max(vaccinatedCount + delta, 0);
                    break;
            }  
        }
        
        private void CalculateSpawnDelay(float roundTotalTime, float roundSafeTime)
        {
            _spawnDelay = Mathf.Floor((roundTotalTime - roundSafeTime) / _currentCountNPC);
        }
    }
}