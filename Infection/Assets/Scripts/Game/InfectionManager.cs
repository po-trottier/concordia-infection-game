using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game
{
    public class InfectionManager : MonoBehaviour
    {
        [Header("Infection Parameters")]
        [SerializeField] private Color infectedColor = Color.red;
     
        [Header("Tilemap References")]
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private Tile tile;

        [Header("Other References")] 
        [SerializeField] private ScoreManager scoreManager;
        
        private Dictionary<Vector3, Tile> _rails;
        private Dictionary<Vector3, bool> _railsInfectionStatus;

        private void Start()
        {
            FindRailsInTilemap();
            InitializeRailInfectionStatus();
        }
        
        public Dictionary<Vector3, Tile> GetRails()
        {
            return _rails;
        }
        
        public bool GetRailInfectionStatus(Vector3 railPosition)
        {
            return _railsInfectionStatus[railPosition];
        }
        
        public void InitializeRailInfectionStatus()
        {
            _railsInfectionStatus = new Dictionary<Vector3, bool>();
            _railsInfectionStatus.AddRange(_rails.Select(r => new KeyValuePair<Vector3, bool>(r.Key, false)));
        }

        public void UpdateRailInfectionStatus(Vector3 railPosition, bool infected)
        {
            _railsInfectionStatus[railPosition] = infected;
            
            _rails[railPosition].color = infected ? infectedColor : Color.white;
            
            if (infected)
                return;
            else 
                Debug.Log("Infected Rail at: " + railPosition);
            
            // If the player decontaminated a surface give them 2 points
            scoreManager.UpdateScore(2);
        }
        
        private void FindRailsInTilemap()
        {
            _rails = new Dictionary<Vector3, Tile>();

            for (var x = tilemap.cellBounds.xMin; x < tilemap.cellBounds.xMax; x++)
            {
                for (var y = tilemap.cellBounds.yMin; y < tilemap.cellBounds.yMax; y++)
                {
                    var coordinates = new Vector3Int(x, y, (int)tilemap.transform.position.z);
                    
                    // If there is no tile or the tile is not in the list of tiles we are looking for at the current coordinates continue
                    var foundTile = tilemap.GetTile<Tile>(coordinates);
                    if (!tilemap.HasTile(coordinates) || tile.name != foundTile.name) 
                        continue;
                    
                    _rails.Add(tilemap.GetCellCenterWorld(coordinates), foundTile);
                }
            }
        }
    }
}