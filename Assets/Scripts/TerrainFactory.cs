using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts {

    class TerrainFactory {

        public static void GenerateHeights(Terrain terrain, float tileSize, float ruggedness) {

            float[,] heights = new float[terrain.terrainData.heightmapWidth, terrain.terrainData.heightmapHeight];

            int offset = UnityEngine.Random.Range(0, 1000);

            for (int i = 0; i < terrain.terrainData.heightmapWidth; i++) {
                for (int k = 0; k < terrain.terrainData.heightmapHeight; k++) {
                    heights[i, k] = 
                        Mathf.PerlinNoise(
                            (offset + i / (float)terrain.terrainData.heightmapWidth)  * tileSize, 
                            (offset + k / (float)terrain.terrainData.heightmapHeight) * tileSize) / 10.0f;
                    heights[i, k] *= ruggedness;
                }
            }

            terrain.terrainData.SetHeights(0, 0, heights);
        }

    }
}
