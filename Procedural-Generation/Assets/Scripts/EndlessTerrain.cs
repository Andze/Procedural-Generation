using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EndlessTerrain : MonoBehaviour
{
    const float viewerMoveThresholdForChunkUpdate = 25f;
    const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;

    public LODInfo[] detailLevels;
    public static float maxViewDst;

    public Transform viewer;
    public Material mapMaterial;
	public GameObject Tree;
    public static Vector2 viewerPosition;
    Vector2 viewerPositionOld;
    static MapGenerator mapGenerator;
    int chunkSize;
    int chunksVisibleInViewDst;

    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    static List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

    void Start()
    {
        mapGenerator = FindObjectOfType<MapGenerator>();

        maxViewDst = detailLevels[detailLevels.Length - 1].visibleDstThreshold;
        chunkSize = MapGenerator.mapChunkSize - 1;
        chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / chunkSize);

        UpdateVisibleChunks();
    }

    void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);

        if ((viewerPositionOld - viewerPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate)
        {
            viewerPositionOld = viewerPosition;
            UpdateVisibleChunks();
        }
    }

    void UpdateVisibleChunks()
    {

        for (int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++)
        {
            terrainChunksVisibleLastUpdate[i].SetVisible(false);
        }
        terrainChunksVisibleLastUpdate.Clear();

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

        for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++)
        {
            for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                {
                    terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();                 
                }
                else
                {
                    terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, detailLevels, transform, mapMaterial, false));
                }

            }
        }
    }

    public class TerrainChunk
    {
        GameObject meshObject;
        Vector2 position;
        Bounds bounds;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;

        LODInfo[] detailLevels;
        LODMesh[] lodMeshes;

        MapData mapData;
        bool mapDataReceived;
        bool hasLoaded;
        int previousLODIndex = -1;

        public TerrainChunk(Vector2 coord, int size, LODInfo[] detailLevels, Transform parent, Material material, bool Loaded)
        {
            this.detailLevels = detailLevels;

            position = coord * size;
            bounds = new Bounds(position, Vector2.one * size);
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);

            meshObject = new GameObject("Terrain Chunk");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshRenderer.material = material;
            meshObject.transform.position = positionV3;
            meshObject.transform.parent = parent;
            SetVisible(false);
            hasLoaded = Loaded;
            lodMeshes = new LODMesh[detailLevels.Length];
            for (int i = 0; i < detailLevels.Length; i++)
            {
				lodMeshes[i] = new LODMesh(detailLevels[i].lod, UpdateTerrainChunk);
            }

            mapGenerator.RequestMapData(position, OnMapDataReceived);
        }

        void OnMapDataReceived(MapData mapData)
        {
            this.mapData = mapData;
            mapDataReceived = true;

            Texture2D texture = TextureGenerator.TextureFromColourMap(mapData.colourMap, MapGenerator.mapChunkSize, MapGenerator.mapChunkSize);
            meshRenderer.material.mainTexture = texture;

            UpdateTerrainChunk();
        }
			
        public void UpdateTerrainChunk()
        {
            if (mapDataReceived)
            {
                float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
                bool visible = viewerDstFromNearestEdge <= maxViewDst;

                if (visible)
                {
                    int lodIndex = 0;

                    for (int i = 0; i < detailLevels.Length - 1; i++)
                    {
                        if (viewerDstFromNearestEdge > detailLevels[i].visibleDstThreshold)
                        {

                            lodIndex = i + 1;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (lodIndex != previousLODIndex)
                    {
                        LODMesh lodMesh = lodMeshes[lodIndex];
                        if (lodMesh.hasMesh)
                        {
                            previousLODIndex = lodIndex;
                            meshFilter.mesh = lodMesh.mesh;

                            if (hasLoaded == false)
                            {
                                //Objects to spawn
                                GameObject Pine_Tree = GameObject.Find("Pine_Tree");
                                GameObject Grass = GameObject.Find("Grass");
                                GameObject Palm_Tree = GameObject.Find("Palm_Tree");
                                GameObject Pine_TreeSnow = GameObject.Find("Pine_TreeSnow");
                                GameObject Rock = GameObject.Find("Rock");
                                GameObject Beach_Tree = GameObject.Find("Beach_Tree");
                                GameObject SeaWeed = GameObject.Find("SeaWeed");
                                GameObject Shell = GameObject.Find("Shell");
                                GameObject Bush = GameObject.Find("Bush");
                                GameObject Bush_Snow = GameObject.Find("Bush_Snow");
                                GameObject Dead_Tree = GameObject.Find("Dead_Tree");
                                GameObject Wall = GameObject.Find("Wall");
                                GameObject Keep = GameObject.Find("Keep");
                                GameObject FarmHouse = GameObject.Find("FarmHouse1");
                                GameObject House_1 = GameObject.Find("House_1");
                                GameObject House_2 = GameObject.Find("House_2");
                                GameObject House_3 = GameObject.Find("House_3");
                                GameObject House_4 = GameObject.Find("House_4");
                                GameObject Keep_Snow = GameObject.Find("Keep_Snow");
                                GameObject Cactus = GameObject.Find("Cactus");
                                GameObject Church = GameObject.Find("Church");
                                GameObject Dock = GameObject.Find("Dock");
                                GameObject Empty = new GameObject();

                                GameObject ObjectToSpawn = new GameObject();
                                ObjectToSpawn.transform.SetParent(meshObject.transform);

                                //holder for all objects
                                GameObject SpawnedObjects = new GameObject(); SpawnedObjects.name = "SpawnedObjects";

                                //set it to mesh chunk
                                SpawnedObjects.transform.SetParent(meshObject.transform);
                                SpawnedObjects.transform.position = meshObject.transform.position;

                                //loop over everything
                                for (int x = 0; x < 241; x += Random.Range(4,8))
                                {
                                    for (int y = 0; y < 241; y += Random.Range(5, 9))
                                    {
                                        if (mapData.heightMap[x, y] >= 0.0f && mapData.heightMap[x, y] <= 0.1f)
                                        {
                                            //seaweed
                                            GameObject Tree = Instantiate(SeaWeed, new Vector3(meshObject.transform.localPosition.x + x + Random.Range(-3, 3), -mapGenerator.meshHeightCurve.Evaluate(mapData.heightMap[x, y]) * 20.0f, meshObject.transform.localPosition.z + y), (Quaternion.Euler(90, 0, 0)));
                                            Tree.transform.Rotate(0, 0, Random.Range(0,360));
                                            float RandomScale = SeaWeed.transform.localScale.x + Random.Range(-0.1f, 0.1f);
                                            Tree.transform.localScale = new Vector3(RandomScale, RandomScale, RandomScale);
                                            Tree.transform.SetParent(SpawnedObjects.transform);
                                        }
                                        if (mapData.heightMap[x, y] >= 0.2f && mapData.heightMap[x, y] <= 0.25f) {

                                            //DOCKS DOCKS BABY
                                            float randomSeed = Random.Range(0, 101);
                                            if (randomSeed > 0 && randomSeed < 90) ObjectToSpawn = Empty;
                                            if (randomSeed > 90 && randomSeed < 100) ObjectToSpawn = Dock;

                                            GameObject Tree = Instantiate(ObjectToSpawn, new Vector3(meshObject.transform.localPosition.x + x + Random.Range(-3, 3), -mapGenerator.meshHeightCurve.Evaluate(mapData.heightMap[x, y]) * 20.0f, meshObject.transform.localPosition.z + y), (Quaternion.Euler(90, 0, 0)));
                                              float RandomScale = ObjectToSpawn.transform.localScale.x + Random.Range(0.1f, 0.25f);
                                            Tree.transform.localScale = new Vector3(RandomScale, RandomScale, RandomScale);
                                            Tree.transform.Rotate(0, 0, Random.Range(0, 360));
                                            Tree.transform.SetParent(SpawnedObjects.transform);
                                        }

                                        if (mapData.heightMap[x, y] >= 0.1f && mapData.heightMap[x, y] <= 0.35f)
                                        {                                          
                                            GameObject Tree = Instantiate(Shell, new Vector3(meshObject.transform.localPosition.x + x + Random.Range(-3,3), -mapGenerator.meshHeightCurve.Evaluate(mapData.heightMap[x, y]) * 20.0f, meshObject.transform.localPosition.z + y), (Quaternion.Euler(0, 0, 0)));
                                            Tree.transform.Rotate(0, Random.Range(0, 360), 0);
                                            Tree.transform.SetParent(SpawnedObjects.transform);
                                        }
                                        if (mapData.heightMap[x, y] >= 0.25f && mapData.heightMap[x, y] <= 0.35f)
                                        {                                           
                                            float randomSeed = Random.Range(0, 101);
                                            if (randomSeed > 0 && randomSeed < 95) ObjectToSpawn = Palm_Tree;
                                            if (randomSeed > 95 && randomSeed < 100) ObjectToSpawn = Cactus;
                                            //Palm Trees
                                            GameObject Tree = Instantiate(ObjectToSpawn, new Vector3(meshObject.transform.localPosition.x + x, -mapGenerator.meshHeightCurve.Evaluate(mapData.heightMap[x, y]) * 20.0f, meshObject.transform.localPosition.z + y), (Quaternion.Euler(90, 0, 0)));
                                            float RandomScale = ObjectToSpawn.transform.localScale.x + Random.Range(-0.05f, 0.1f);
                                            Tree.transform.localScale = new Vector3(RandomScale, RandomScale, RandomScale);
                                            Tree.transform.Rotate(0, 0, Random.Range(0, 360));
                                            Tree.transform.SetParent(SpawnedObjects.transform);
                                            
                                        }
                                        if (mapData.heightMap[x, y] >= 0.35f && mapData.heightMap[x, y] <= 0.5f)
                                        {
                                            //beach and Bush grass farms
                                            float randomSeed = Random.Range(0,101);
                                            if (randomSeed > 0 && randomSeed < 30) ObjectToSpawn = Beach_Tree;
                                            if(randomSeed > 30 && randomSeed < 60) ObjectToSpawn = Bush;
                                            if (randomSeed > 60 && randomSeed < 90) ObjectToSpawn = Grass;
                                            if (randomSeed > 90) { ObjectToSpawn = FarmHouse; }
                                            //grass 1
                                            GameObject Tree = Instantiate(ObjectToSpawn, new Vector3(meshObject.transform.localPosition.x + x, -mapGenerator.meshHeightCurve.Evaluate(mapData.heightMap[x, y]) * 20.0f, meshObject.transform.localPosition.z + y), (Quaternion.Euler(90, 0, 0)));
                                            Tree.transform.Rotate(0, 0, Random.Range(0, 360));
                                            if (ObjectToSpawn == FarmHouse) Tree.transform.Rotate(90, 0,0);
                                            float RandomScale = ObjectToSpawn.transform.localScale.x + Random.Range(-0.05f, 0.1f);
                                            Tree.transform.localScale = new Vector3(RandomScale, RandomScale, RandomScale);
                                            Tree.transform.SetParent(SpawnedObjects.transform);
                                        }
                                        if (mapData.heightMap[x, y] >= 0.5f && mapData.heightMap[x, y] <= 0.6f)
                                        {
                                            //Grass 2    Pine and Rock    
                                            float randomSeed = Random.Range(0, 101);
                                            if (randomSeed > 0 && randomSeed < 70) ObjectToSpawn = Pine_Tree;
                                            if (randomSeed > 70 && randomSeed < 90) ObjectToSpawn = Beach_Tree;
                                            if (randomSeed > 90 && randomSeed < 100) ObjectToSpawn = Dead_Tree;
                                           
                                            GameObject Tree = Instantiate(ObjectToSpawn, new Vector3(meshObject.transform.localPosition.x + x, -mapGenerator.meshHeightCurve.Evaluate(mapData.heightMap[x, y]) * 20.0f, meshObject.transform.localPosition.z + y), (Quaternion.Euler(90, 0, 0))); Tree.transform.Rotate(0, 0, Random.Range(0, 360));
                                            float RandomScale = ObjectToSpawn.transform.localScale.x + Random.Range(-0.05f, 0.1f);
                                            Tree.transform.localScale = new Vector3(RandomScale, RandomScale, RandomScale);
                                            Tree.transform.SetParent(SpawnedObjects.transform);                                    
                                        }
                                        // CITY GENERATION--------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                                        if (mapData.heightMap[x, y] >= 0.65f && mapData.heightMap[x, y] <= 0.85f)
                                        {
                                            //Rock   
                                            float randomSeed = Random.Range(0, 101);
                                            if (randomSeed > 80 && randomSeed < 100) ObjectToSpawn = Church;
                                            if (randomSeed > 0 && randomSeed < 40) ObjectToSpawn = House_3;
                                            if (randomSeed > 40 && randomSeed < 80) ObjectToSpawn = House_4;


                                            GameObject Tree = Instantiate(ObjectToSpawn, new Vector3(meshObject.transform.localPosition.x + x, -mapGenerator.meshHeightCurve.Evaluate(mapData.heightMap[x, y]) * 20.0f, meshObject.transform.localPosition.z + y), (Quaternion.Euler(90, 0, 0)));
                                            Tree.transform.Rotate(0, 0, Random.Range(0, 360));
                                            float RandomScale = ObjectToSpawn.transform.localScale.x + Random.Range(-0.05f, 0.1f);
                                            Tree.transform.localScale = new Vector3(RandomScale, RandomScale, RandomScale);
                                            Tree.transform.SetParent(SpawnedObjects.transform);
                                        }
                                        if (mapData.heightMap[x, y] >= 0.85f && mapData.heightMap[x, y] <= 0.95f)
                                        {
                                            //Rock 2     
                                            float randomSeed = Random.Range(0, 101);
                                            if (randomSeed > 0 && randomSeed < 20) ObjectToSpawn = Keep;
                                            if (randomSeed > 20 && randomSeed < 60) ObjectToSpawn = House_1;
                                            if (randomSeed > 60 && randomSeed < 100) ObjectToSpawn = House_2;

                                            GameObject Tree = Instantiate(ObjectToSpawn, new Vector3(meshObject.transform.localPosition.x + x, -mapGenerator.meshHeightCurve.Evaluate(mapData.heightMap[x, y]) * 20.0f, meshObject.transform.localPosition.z + y), (Quaternion.Euler(90, 0, 0)));
                                            Tree.transform.Rotate(0, 0, Random.Range(0, 360));
                                            float RandomScale = ObjectToSpawn.transform.localScale.x + Random.Range(-0.05f, 0.1f);
                                            Tree.transform.localScale = new Vector3(RandomScale, RandomScale, RandomScale);
                                            Tree.transform.SetParent(SpawnedObjects.transform);
                                        }
                                        if (mapData.heightMap[x, y] > 0.95f)
                                        {
                                            //Rock 2     
                                            float randomSeed = Random.Range(0, 101);
                                            if (randomSeed > 0 && randomSeed < 65) ObjectToSpawn = Keep_Snow;
                                            if (randomSeed > 60 && randomSeed < 80) ObjectToSpawn = Pine_TreeSnow;
                                            if (randomSeed > 80 && randomSeed < 100) ObjectToSpawn = Bush_Snow;

                                            //Snow      
                                            GameObject Tree = Instantiate(ObjectToSpawn, new Vector3(meshObject.transform.localPosition.x + x, -mapGenerator.meshHeightCurve.Evaluate(mapData.heightMap[x, y]) * 20.0f, meshObject.transform.localPosition.z + y), (Quaternion.Euler(90, 0, 0)));
                                            Tree.transform.Rotate(0, 0, Random.Range(0, 360));
                                            float RandomScale = ObjectToSpawn.transform.localScale.x + Random.Range(-0.05f, 0.1f);
                                            Tree.transform.localScale = new Vector3(RandomScale, RandomScale, RandomScale);
                                            Tree.transform.SetParent(SpawnedObjects.transform);
                                        }

                                    }
                                }

                                for (int x = 0; x < 241; x ++ )
                                {
                                    for (int y = 0; y < 241; y ++ )
                                    {
                                        if (mapData.heightMap[x, y] >= 0.625f && mapData.heightMap[x, y] <= 0.64f)
                                        {

                                            GameObject Tree = Instantiate(Wall, new Vector3(meshObject.transform.localPosition.x + x, -mapGenerator.meshHeightCurve.Evaluate(mapData.heightMap[x, y]) * 20.0f, meshObject.transform.localPosition.z + y), (Quaternion.Euler(90, 0, 0)));
                                            Tree.transform.SetParent(SpawnedObjects.transform);                                           
                                        }
                                    }
                                }

                                SpawnedObjects.transform.Rotate(new Vector3(180.0f, 0, 0));
                                SpawnedObjects.transform.localPosition = new Vector3(SpawnedObjects.transform.localPosition.x - 120.5f, 0, SpawnedObjects.transform.localPosition.z + 120.5f);
                                hasLoaded = true;
                            }

                        }
                        else if (!lodMesh.hasRequestedMesh)
                        {
                            lodMesh.RequestMesh(mapData);
                        }
                    }
                    terrainChunksVisibleLastUpdate.Add(this);
                }
				
                SetVisible(visible);
            }
        }

        public void SetVisible(bool visible)
        {
            meshObject.SetActive(visible);
        }

        public bool IsVisible()
        {
            return meshObject.activeSelf;
        }

    }

    public class LODMesh
    {
        public Mesh mesh;
		public Vector3[] Verts;
        public bool hasRequestedMesh;
        public bool hasMesh;
        int lod;
        System.Action updateCallback;

        public LODMesh(int lod, System.Action updateCallback)
        {
            this.lod = lod;
            this.updateCallback = updateCallback;
        }

        public void OnMeshDataReceived(MeshData meshData)
        {
            mesh = meshData.CreateMesh();
			hasMesh = true;

            updateCallback();
        }

        public void RequestMesh(MapData mapData)
        {
            hasRequestedMesh = true;
            mapGenerator.RequestMeshData(mapData, lod, OnMeshDataReceived);
        }

    }

    [System.Serializable]
    public struct LODInfo
    {
        public int lod;
        public float visibleDstThreshold;
    }

}