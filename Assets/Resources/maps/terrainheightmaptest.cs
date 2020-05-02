using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class terrainheightmaptest : MonoBehaviour
{
//https://answers.unity.com/questions/1084016/how-to-use-a-script-to-import-terrain-raw.html?_ga=2.251483398.1278193886.1588249436-1692705686.1588249436
 void LoadTerrain(string aFileName, TerrainData aTerrain)
 {
     int h = aTerrain.heightmapResolution;
     int w = aTerrain.heightmapResolution;
     float[,] data = new float[h, w];
     using (var file = System.IO.File.OpenRead(aFileName))
     using (var reader = new System.IO.BinaryReader(file))
     {
         for (int y = 0; y < h; y++)
         {
             for (int x = 0; x < w; x++)
             {
                 //float v = (float)reader.ReadUInt16() / 0xFFFF;
                 float v = (float)reader.ReadByte() / 0xFF;
                 data[y, x] = v;
             }
         }
     }
     aTerrain.SetHeights(0, 0, data);
    }

    // Start is called before the first frame update
    void Start()
    {
        TerrainData terrainData = (TerrainData)Resources.Load("maps/testmap");
        LoadTerrain("/home/mha/savage2/sav2/opt/Savage2/game/maps/ashrock/heightmap", terrainData);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
