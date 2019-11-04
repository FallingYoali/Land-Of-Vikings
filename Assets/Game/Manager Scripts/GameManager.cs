using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TerrainGenerator terrainsToGenerate;
    public int numOfTerrains;
    // Start is called before the first frame update
    void Start()
    {
        terrainsToGenerate.SetTerrain(numOfTerrains, this.transform);
    }
}
