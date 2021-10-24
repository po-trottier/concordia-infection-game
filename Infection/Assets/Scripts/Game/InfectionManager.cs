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
        
        private Dictionary<Vector3, bool> _rails;

        private void Start()
        {
            FindRailsInTilemap();
        }
        
        public Vector3[] GetRails()
        {
            return _rails.Keys.ToArray();
        }
        
        public bool GetRailInfectionStatus(Vector3 railPosition)
        {
            return _rails[railPosition];
        }

        public void UpdateRailInfectionStatus(Vector3 railPosition, bool infected)
        {
            _rails[railPosition] = infected;
            
            // By default it's set to "Lock Colour".
            tilemap.SetTileFlags(tilemap.WorldToCell(railPosition), TileFlags.None);
            // Set the colour.
            tilemap.SetColor(tilemap.WorldToCell(railPosition), infected ? infectedColor : Color.white);
            
            if (infected)
                return;
            
            // If the player decontaminated a surface give them 2 points
            scoreManager.UpdateScore(2);
        }
        
        private void FindRailsInTilemap()
        {
            _rails = new Dictionary<Vector3, bool>();

            for (var x = tilemap.cellBounds.xMin; x < tilemap.cellBounds.xMax; x++)
            {
                for (var y = tilemap.cellBounds.yMin; y < tilemap.cellBounds.yMax; y++)
                {
                    var coordinates = new Vector3Int(x, y, (int)tilemap.transform.position.z);
                    
                    // If there is no tile or the tile is not in the list of tiles we are looking for at the current coordinates continue
                    var foundTile = tilemap.GetTile<Tile>(coordinates);
                    if (!tilemap.HasTile(coordinates) || tile.name != foundTile.name) 
                        continue;
                    
                    _rails.Add(tilemap.GetCellCenterWorld(coordinates), false);
                }
            }
        }
    }
}