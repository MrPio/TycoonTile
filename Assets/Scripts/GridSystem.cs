using System;
using System.Collections;
using System.Collections.Generic;
using Classes;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class GridSystem : MonoBehaviour
{
    private enum GridType
    {
        GridGrass,
        GridHole,
        GridMain
    }

    [SerializeField] private int rows = 5;
    [SerializeField] private int cols = 8;
    [SerializeField] [Range(0, 1)] private float tileSize = 1;
    [SerializeField] Tile.TileType tileType;
    [SerializeField] List<Object> tileList = new();
    [SerializeField] List<Object> tileBackgroundList = new();
    [SerializeField] private bool random;
    [SerializeField] private bool chessTiles;
    [SerializeField] private bool shade;
    [SerializeField] [Range(0.3f, 0.3f)] private float shadeOpacity = 0.4f;
    [SerializeField] [Range(-0.3f, 0.3f)] private float shadeOffsetX = 0.1f;
    [SerializeField] [Range(-0.3f, 0.3f)] private float shadeOffsetY = 0.1f;
    [SerializeField] private bool clickable;
    [SerializeField] private GridType gridType;
    public GameObject[][] Grid;
    public GameObject[][] BackgroundGrid;
    private static readonly HashSet<GridType> Spawned = new();
    private bool _treeSpawned;

    public Vector2 bottomLeftPoint { get; private set; }
    public Vector2 size { get; private set; }

    void Start()
    {
        Grid = new GameObject[rows + 2][];
        for (var i = 0; i < Grid.Length; i++)
            Grid[i] = new GameObject[cols + 2];

        BackgroundGrid = new GameObject[rows + 2][];
        for (var i = 0; i < BackgroundGrid.Length; i++)
            BackgroundGrid[i] = new GameObject[cols + 2];
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        for (var row = 1; row < rows+1; row++)
        {
            for (var col = 1; col < cols+1; col++)
            {
                //Random Generation
                if (random && Random.Range(0f, 1f) > GetSpawnProbability(col, row))
                    continue;

                //Tile Background
                if (tileBackgroundList.Count > 0)
                {
                    for (int i = -1; i < 2; ++i)
                    for (int j = -1; j < 2; ++j)
                        if (BackgroundGrid[row + j][col + i] == null)
                        {
                            var backgroundTileGo1 =
                                (GameObject) Instantiate(tileBackgroundList[Random.Range(0, tileBackgroundList.Count)],
                                    transform);
                            backgroundTileGo1.transform.position = new Vector2(col + i, row + j);
                            backgroundTileGo1.transform.localScale = new Vector2(1, 1);
                            backgroundTileGo1.GetComponent<SpriteRenderer>().sortingLayerName = "Hole";
                            BackgroundGrid[row + j][col + i] = backgroundTileGo1;
                        }

                    var backgroundTileGo =
                        (GameObject) Instantiate(tileBackgroundList[Random.Range(0, tileBackgroundList.Count)],
                            transform);
                    backgroundTileGo.transform.position = new Vector2(col, row);
                    backgroundTileGo.transform.localScale = new Vector2(1, 1);
                    backgroundTileGo.GetComponent<SpriteRenderer>().sortingLayerName = "Hole";
                    BackgroundGrid[row][col] = backgroundTileGo;
                }

                //Tile Shadow
                if (shade)
                {
                    var shadeGo = (GameObject) Instantiate(Resources.Load("tile_shade"), transform);
                    shadeGo.transform.position = new Vector2(col, row) + new Vector2(shadeOffsetX, shadeOffsetY);
                    shadeGo.transform.localScale = new Vector2(tileSize, tileSize);
                    shadeGo.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, shadeOpacity);
                    shadeGo.GetComponent<SpriteRenderer>().sortingLayerName = "Grid";
                }

                //Chess Tile or random type
                int typeNum;
                if (chessTiles)
                    typeNum = (row + col) % 2;
                else
                    typeNum = Random.Range(0, TileResources.TilesDictionary[tileType].Length);

                //Position and scale
                GameObject tile;
                if (clickable)
                    tile = (GameObject) Instantiate(Resources.Load("tile"), transform);
                else
                    tile = (GameObject) Instantiate(tileList[typeNum], transform);
                if (clickable)
                    tile.GetComponent<SpriteRenderer>().color =
                        TileResources.HexToColor(TileResources.TilesDictionary[tileType][typeNum].color);
                tile.transform.position = new Vector2(col, row);
                tile.transform.localScale = new Vector2(tileSize, tileSize);

                //Tile Clickable, add script
                if (clickable)
                {
                    tile.GetComponent<SpriteRenderer>().sortingLayerName = "Grid";
                    var tileResources = tile.AddComponent<TileResources>();
                    tile.AddComponent<AudioSource>();
                    tileResources.SetOffsetY(shadeOffsetY);
                    tileResources.row = row;
                    tileResources.col = col;
                    tileResources.type = typeNum;
                    tileResources.tile = TileResources.TilesDictionary[Tile.TileType.Wood][typeNum];
                }

                //Populate array
                Grid[row][col] = tile;
            }
        }

        float gridW = cols+2;
        float gridH = rows+2;
        size = new Vector2(gridW, gridH);
        bottomLeftPoint = new Vector2(-gridW / 2 + 1f / 2, -gridH / 2 + 1f / 2);
        transform.position = bottomLeftPoint;

        var boxCollider2D = gameObject.AddComponent<BoxCollider2D>();
        boxCollider2D.size = new Vector2(gridW, gridH);
        boxCollider2D.offset = -bottomLeftPoint;
        boxCollider2D.enabled = false;

        Spawned.Add(gridType);

        if (!_treeSpawned && Spawned.Contains(GridType.GridGrass) && Spawned.Contains(GridType.GridMain))
        {
            _treeSpawned = true;
            GameObject.Find("GridGrass").GetComponent<GrassScript>().SpawnTree();
            GameObject.Find("GridGrass").GetComponent<GrassScript>().SpawnFlowers();
        }
    }

    public List<GameObject> GetFlow(GameObject tile)
    {
        var check = new bool[rows][];
        for (var i = 0; i < rows; i++)
            check[i] = new bool[cols];

        var result = new List<GameObject>();
        var toCheck = new List<GameObject> {tile};

        while (toCheck.Count != 0)
        {
            var toCheckScript = toCheck[0].GetComponent<TileResources>();
            foreach (var t in GetAdjacent(toCheckScript.row, toCheckScript.col))
            {
                if (t == null)
                    continue;
                var tileScript = t.GetComponent<TileResources>();
                if (check[tileScript.row][tileScript.col])
                    continue;

                check[tileScript.row][tileScript.col] = true;

                if (tileScript.type == toCheckScript.type)
                {
                    toCheck.Add(t);
                    result.Add(t);
                }
            }

            toCheck.RemoveAt(0);
        }

        return result;
    }

    private List<GameObject> GetAdjacent(int row, int col, bool all8Direction = false)
    {
        var result = new List<GameObject>();

        if (row - 1 >= 0)
            result.Add(Grid[row - 1][col]);
        if (col - 1 >= 0)
            result.Add(Grid[row][col - 1]);
        if (row + 1 < Grid.Length)
            result.Add(Grid[row + 1][col]);
        if (col + 1 < Grid[0].Length)
            result.Add(Grid[row][col + 1]);

        if (!all8Direction)
            return result;

        if (row - 1 >= 0 && col - 1 >= 0)
            result.Add(Grid[row - 1][col - 1]);
        if (row + 1 >= 0 && col - 1 >= 0)
            result.Add(Grid[row + 1][col - 1]);
        if (row + 1 >= 0 && col + 1 >= 0)
            result.Add(Grid[row + 1][col + 1]);
        if (row - 1 >= 0 && col + 1 >= 0)
            result.Add(Grid[row - 1][col + 1]);
        return result;
    }

    private float GetSpawnProbability(int x, int y)
    {
        var result = ((cols + rows) / 2f - Math.Abs(x - cols / 2f) - Math.Abs(y - rows / 2f)) /
            (cols + rows) * 2f;
        return result;
    }
}