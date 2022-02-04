using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] List<Object> tileList = new List<Object>();
    [SerializeField] private bool chessTiles;
    [SerializeField] private bool shade;
    [SerializeField] [Range(0.3f, 0.3f)] private float shadeOpacity = 0.4f;
    [SerializeField] [Range(-0.3f, 0.3f)] private float shadeOffsetX = 0.1f;
    [SerializeField] [Range(-0.3f, 0.3f)] private float shadeOffsetY = 0.1f;
    [SerializeField] private bool clickable;
    [SerializeField] private GridType gridType;
    public GameObject[][] Grid;
    public Vector2 bottomLeftPoint;
    public Vector2 size;

    // Start is called before the first frame update
    void Start()
    {
        Grid = new GameObject[rows][];
        for (var i = 0; i < Grid.Length; i++)
            Grid[i++] = new GameObject[cols];
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < cols; col++)
            {
                float posX = col;
                float posY = row;
                if (shade)
                {
                    var shadeGo = (GameObject) Instantiate(Resources.Load("tile_shade"), transform);
                    shadeGo.transform.position = new Vector2(posX, posY) + new Vector2(shadeOffsetX, shadeOffsetY);
                    shadeGo.transform.localScale = new Vector2(tileSize, tileSize);
                    shadeGo.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, shadeOpacity);
                }

                int typeNum;
                if (chessTiles)
                    typeNum = (row + col) % 2;
                else
                    typeNum = Random.Range(0, tileList.Count);
                var tile = (GameObject) Instantiate(tileList[typeNum], transform);
                tile.transform.position = new Vector2(posX, posY);
                tile.transform.localScale = new Vector2(tileSize, tileSize);
                if (clickable)
                {
                    tile.AddComponent<TileResources>();
                    tile.AddComponent<AudioSource>();
                    tile.GetComponent<TileResources>().SetOffsetY(shadeOffsetY);
                    tile.GetComponent<TileResources>().row = row;
                    tile.GetComponent<TileResources>().col = col;
                    tile.GetComponent<TileResources>().type = typeNum;
                }

                Grid[row][col] = tile;
            }
        }

        float gridW = cols * 1;
        float gridH = rows * 1;
        size = new Vector2(gridW, gridH);
        bottomLeftPoint = new Vector2(-gridW / 2 + 1f / 2, -gridH / 2 + 1f / 2);
        transform.position = bottomLeftPoint;

        gameObject.AddComponent<BoxCollider2D>();
        gameObject.GetComponent<BoxCollider2D>().size = new Vector2(gridW, gridH);
        gameObject.GetComponent<BoxCollider2D>().offset = -bottomLeftPoint;
        gameObject.GetComponent<BoxCollider2D>().enabled = false;

        if (gridType == GridType.GridGrass)
        {
            gameObject.GetComponent<GrassScript>().SpawnTree();
            gameObject.GetComponent<GrassScript>().SpawnFlowers();
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

    private List<GameObject> GetAdjacent(int row, int col)
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
        return result;
    }
}