using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Electric : MonoBehaviour
{
    public GameObject electricPrefab;
    void Start()
    {
        Tilemap tilemap = GetComponent<Tilemap>();
        foreach (var pos in tilemap.cellBounds.allPositionsWithin) {   
            Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
            Vector3 place = tilemap.GetCellCenterWorld(localPlace);
            if (tilemap.HasTile(localPlace)) {
                Instantiate(electricPrefab, place, Quaternion.identity);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
