using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGeneration : MonoBehaviour
{
    public int dirtLayerHeight = 5;

    [Header("Tile Atlas")]
    public TileAtlas tileAtlas;

    [Header("Generation Settings")]
    public int chunkSize = 16;
    public int worldSize = 100;
    public bool createCaves = true;
    public float surfaceValue = 0.2f;
    public float heightMultiplier = 25f;
    public int heightAddition = 35;

    [Header("Noise Settings")]
    public float caveFreq = 0.09f;
    public float terrainFreq = 0.04f;
    public float seed;
    public Texture2D caveNoiseTexture;

    [Header("Ore Settings")]
    public float coalRarity;
    public float coalSize;
    public float ironRarity, ironSize;
    public float goldRarity, goldSize;
    public float diamondRarity, diamondSize;
    public Texture2D coalSpread;
    public Texture2D ironSpread;
    public Texture2D goldSpread;
    public Texture2D diamondSpread;

    [Header("Other Settings")]
    public int spawnTreeChance = 10;

    private GameObject[] worldChunks;
    private List<Vector2> worldTiles = new List<Vector2>();
    private List<GameObject> worldTileObjects = new List<GameObject>();


    public GameObject myPrefab;


    private void OnValidate()
    {
        if (caveNoiseTexture == null)
        {
            caveNoiseTexture = new Texture2D(worldSize, worldSize);
            coalSpread = new Texture2D(worldSize, worldSize);
            ironSpread = new Texture2D(worldSize, worldSize);
            goldSpread = new Texture2D(worldSize, worldSize);
            diamondSpread = new Texture2D(worldSize, worldSize);

        }

        GenerateNoiseTexture(caveFreq, surfaceValue, caveNoiseTexture);
        GenerateNoiseTexture(coalRarity, coalSize, coalSpread);
        GenerateNoiseTexture(ironRarity, ironSize, ironSpread);
        GenerateNoiseTexture(goldRarity, goldSize, goldSpread);
        GenerateNoiseTexture(diamondRarity, diamondSize, diamondSpread);
    }
    public void Start()
    {
        if (caveNoiseTexture == null)
        {
            caveNoiseTexture = new Texture2D(worldSize, worldSize);
            coalSpread = new Texture2D(worldSize, worldSize);
            ironSpread = new Texture2D(worldSize, worldSize);
            goldSpread = new Texture2D(worldSize, worldSize);
            diamondSpread = new Texture2D(worldSize, worldSize);

        }
        // temporary testing and fix
        seed = 12345;
        //seed = Random.Range(-10000, 10000);
        GenerateNoiseTexture(caveFreq, surfaceValue, caveNoiseTexture);
        GenerateNoiseTexture(coalRarity, coalSize, coalSpread);
        GenerateNoiseTexture(ironRarity, ironSize, ironSpread);
        GenerateNoiseTexture(goldRarity, goldSize, goldSpread);
        GenerateNoiseTexture(diamondRarity, diamondSize, diamondSpread);


        CreateChunks();
        GenerateTerrain();

        // Spawn a player
        //Instantiate(myPrefab, new Vector3(60, 60, 0), Quaternion.identity);
    }


    public void CreateChunks()
    {
        int numChunks = (int)Mathf.Ceil((float)worldSize / chunkSize);
        worldChunks = new GameObject[numChunks];
        for (int i = 0; i < numChunks; i++)
        {
            GameObject newChunck = new GameObject();
            newChunck.name = "Chunk " + i.ToString();
            newChunck.transform.parent = this.transform;
            worldChunks[i] = newChunck;
        }
    }

    public void GenerateTerrain()

    {
        for (int x = 0; x < worldSize; x++)
        {

            float height = Mathf.PerlinNoise((x + seed) * terrainFreq, seed * terrainFreq) * heightMultiplier + heightAddition;

            for (int y = 0; y < height; y++)
            {
                Sprite tileSprite;

                if (y < height - dirtLayerHeight)
                {
                    if (coalSpread.GetPixel(x, y).r > 0.5f)
                        tileSprite = tileAtlas.coal.tileSprite;
                    else if (ironSpread.GetPixel(x, y).r > 0.5f)
                        tileSprite = tileAtlas.iron.tileSprite;
                    else if (goldSpread.GetPixel(x, y).r > 0.5f)
                        tileSprite = tileAtlas.gold.tileSprite;
                    else if (diamondSpread.GetPixel(x, y).r > 0.5f)
                        tileSprite = tileAtlas.diamond.tileSprite;
                    else
                        tileSprite = tileAtlas.stone.tileSprite;
                }
                else if (y < height - 1)
                {
                    tileSprite = tileAtlas.dirt.tileSprite;

                }
                else
                {
                    // Top layer of the terrain
                    tileSprite = tileAtlas.grass.tileSprite;

                }

                if (!createCaves)
                {
                    PlaceTile(tileSprite, x, y, false);
                }
                else
                {
                    if (caveNoiseTexture.GetPixel(x, y).r > 0.5f)
                    {
                        PlaceTile(tileSprite, x, y, false);
                    }
                }

                if (y > height - 1)
                {

                    int t = Random.Range(0, spawnTreeChance);

                    if (t == 1)
                    {
                        // generate a tree
                        if (worldTiles.Contains(new Vector2(x, y)) && !worldTiles.Contains(new Vector2(x + 1, y + 1)) && !worldTiles.Contains(new Vector2(x - 1, y + 1)))
                        {
                            GenerateTree(x, y + 1);
                        }

                    }
                }
            }
        }
    }

    void GenerateTree(int x, int y)
    {
        PlaceTile(tileAtlas.logBot.tileSprite, x, y, true);

        int topOfTree = Random.Range(5, 13);

        // Generate mid of tree
        for (int m = 1; m < topOfTree; m++)
        {
            PlaceTile(tileAtlas.logMid.tileSprite, x, y + m, true);
        }

        // Generate leaves temporary
        PlaceTile(tileAtlas.leaf.tileSprite, x, y + topOfTree, true);

        PlaceTile(tileAtlas.leaf.tileSprite, x, y + topOfTree + 2, true);
        PlaceTile(tileAtlas.leaf.tileSprite, x + 1, y + topOfTree, true);
        PlaceTile(tileAtlas.leaf.tileSprite, x - 1, y + topOfTree, true);

        PlaceTile(tileAtlas.leaf.tileSprite, x, y + topOfTree + 1, true);
        PlaceTile(tileAtlas.leaf.tileSprite, x + 1, y + topOfTree + 1, true);
        PlaceTile(tileAtlas.leaf.tileSprite, x - 1, y + topOfTree + 1, true);





    }

    public void RemoveTile(int x, int y)
    {
        if (worldTiles.Contains(new Vector2Int(x, y)) && x >= 0 && x <= worldSize && y >= 0 && y <= worldSize)
        {
            Destroy(worldTileObjects[worldTiles.IndexOf(new Vector2(x, y))]);
        }


    }

    private void PlaceTile(Sprite tileSprite, int x, int y, bool backgroundElement)
    {
        GameObject newTile = new GameObject();

        float chunkCoord = (Mathf.Round(x / chunkSize) * chunkSize);
        chunkCoord /= chunkSize;
        newTile.transform.parent = worldChunks[(int)chunkCoord].transform;

        newTile.AddComponent<SpriteRenderer>();
        if (!backgroundElement)
        {
            newTile.AddComponent<BoxCollider2D>();
            newTile.GetComponent<BoxCollider2D>().size = Vector2.one;
            newTile.tag = "Ground";
        }
        newTile.GetComponent<SpriteRenderer>().sprite = tileSprite;
        newTile.GetComponent<SpriteRenderer>().sortingOrder = -5;
        newTile.name = tileSprite.name;
        newTile.transform.position = new Vector2(x + 0.5f, y + 0.5f);

        worldTiles.Add(newTile.transform.position - Vector3.one * 0.5f);
        worldTileObjects.Add(newTile);
    }

    public void GenerateNoiseTexture(float frequency, float limit, Texture2D noiseTexture)
    {

        for (int x = 0; x < noiseTexture.width; x++)
        {
            for (int y = 0; y < noiseTexture.height; y++)
            {
                float v = Mathf.PerlinNoise((x + seed) * frequency, (y + seed) * frequency);
                if (v > limit)
                {
                    noiseTexture.SetPixel(x, y, Color.white);
                }
                else
                {
                    noiseTexture.SetPixel(x, y, Color.black);
                }

            }
        }
        noiseTexture.Apply();
    }
}
