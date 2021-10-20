using System;
using System.Collections;
using System.Linq;
using Common;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NPCs
{
    public class NPCSpawner : MonoBehaviour
    {
        [Header("Parameters")]
        [SerializeField] private uint initialCountNPC = 20;
        [SerializeField] private float roundIncreaseMultiplierNPC = 1.25f;
        [SerializeField] private float initialSpeedNPC = 0.5f;
        [SerializeField] private float roundIncreaseSpeed = 0.05f;
        
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
        
        [Header("NPC References")]
        [SerializeField] private GameObject infectedPrefab;
        [SerializeField] private GameObject maskPrefab;
        [SerializeField] private GameObject noMaskPrefab;
        [SerializeField] private GameObject susceptiblePrefab;
        [SerializeField] private GameObject vaccinatedPrefab;

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
                FindDoors();
                FindShops();
                
                if (_doorPositions.Length == 0 || _shopPositions.Length == 0)
                    throw new UnityException("No door or shops were found.");
            }
            else
            {
                // Increment values for all subsequent rounds
                _currentCountNPC = (uint)(_currentCountNPC * roundIncreaseMultiplierNPC);
                _currentSpeedtNPC += roundIncreaseSpeed;
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
            var prefab = GetRandomPrefabNPC();

            int doorIndex = (int)Math.Round((double)Random.Range(0, _doorPositions.Length));
            int exitIndex = (int)Math.Round((double)Random.Range(0, _doorPositions.Length));
            int shopIndex = (int)Math.Round((double)Random.Range(0, _shopPositions.Length));
            
            var npc = Instantiate(prefab);
            npc.transform.position = _doorPositions[doorIndex];

            var controller = npc.GetComponent<NPCController>();
            controller.SetTargetPosition(_shopPositions[shopIndex]);
            controller.SetExitPosition(_doorPositions[exitIndex]);
            controller.SetSpeed(_currentSpeedtNPC);

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

        private GameObject GetRandomPrefabNPC()
        {
            float randomSeed = Random.value;

            if (randomSeed < infectedRate)
            {
                return infectedPrefab;
            }

            if (randomSeed < infectedRate + maskRate)
            {
                return maskPrefab;
            }

            if (randomSeed < infectedRate + maskRate + noMaskRate)
            {
                return noMaskPrefab;
            }

            if (randomSeed < infectedRate + maskRate + noMaskRate + susceptibleRate)
            {
                return susceptiblePrefab;
            }

            if (randomSeed >= 1 - vaccinatedRate) {
                return vaccinatedPrefab;
            }

            return null;
        }
        
        private void FindDoors()
        {
            _doorPositions = GameObject.FindGameObjectsWithTag(Tags.Door).Select(go => go.transform.position).ToArray();
        }
        
        private void FindShops()
        {
            _shopPositions = GameObject.FindGameObjectsWithTag(Tags.Shop).Select(go => go.transform.position).ToArray();
        }
        
        private void CalculateSpawnDelay(float roundTotalTime, float roundSafeTime)
        {
            _spawnDelay = Mathf.Floor((roundTotalTime - roundSafeTime) / _currentCountNPC);
        }
    }
}