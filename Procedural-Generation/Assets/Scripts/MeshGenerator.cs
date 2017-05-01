using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator{

	public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve _heightCurve, int levelOfDetail)
    {
        AnimationCurve heightCurve = new AnimationCurve(_heightCurve.keys);

        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        float topLeftX = (width - 1) / -2f;
        float TopLeftZ = (height - 1) / 2f;

        int meshSimplify = (levelOfDetail == 0) ? 1: levelOfDetail * 2;
        int verticeisPerLine = (width - 1) / meshSimplify + 1;

        MeshData meshData = new MeshData(verticeisPerLine, verticeisPerLine);
        int vertexIndex = 0;

        for (int y = 0; y < height; y+= meshSimplify)
        {
            for (int x = 0; x < height; x+= meshSimplify)
            {
                meshData.verticies[vertexIndex] = new Vector3(topLeftX + x, heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier, TopLeftZ - y);
                meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

                if (x < width - 1 && y < height - 1)
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + verticeisPerLine + 1, vertexIndex + verticeisPerLine);
                    meshData.AddTriangle(vertexIndex + verticeisPerLine + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }
        return meshData;
    } 
}

public class MeshData
{
    public Vector3[] verticies;
    public int[] triangles;
    public Vector2[] uvs;
    int trianglesIndex;

    public MeshData(int meshWidth, int meshHeight)
    {
        verticies = new Vector3[meshWidth * meshHeight];
        uvs = new Vector2[meshWidth * meshHeight];
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
    }

    public void AddTriangle(int a, int b, int c)
    {
        triangles[trianglesIndex] = a;
        triangles[trianglesIndex+1] = b;
        triangles[trianglesIndex+2] = c;

        trianglesIndex += 3;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = verticies;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals ();
        return mesh;
    }
}
