using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class TileData : ScriptableObject
{
    public List<TileBase> tiles;
    public bool plowable;

    public void Interact()
    {
        Debug.Log("interact a mammt");
    }

}
