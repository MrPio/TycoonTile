using System.Collections.Generic;
using UnityEngine;

public class GrassScript : MonoBehaviour
{
    [SerializeField] [Range(0, 8000)] int treeCount = 2000;
    [SerializeField] [Range(0, 800)] int flowerCount = 2000;
    [SerializeField] List<GameObject> treeAvaibleList;
    [SerializeField] List<GameObject> flowersAvaibleList;
    List<GameObject> treeList = new List<GameObject>();

    public void SpawnTree()
    {
        spawn(treeCount, treeAvaibleList, 1.4f, new Vector2(2f, 4f));
    }

    public void SpawnFlowers()
    {
        spawn(flowerCount, flowersAvaibleList, 1.5f, new Vector2(1f, 2f));
    }

    void spawn(int howMany, List<GameObject> gameObjects, float scaleAroundGrid, Vector2 sizeRange)
    {
        var script = GameObject.Find("GridGrass").GetComponent<GridSystem>();
        var boxCollider = GameObject.Find("GridMain").GetComponent<BoxCollider2D>();
        boxCollider.size = boxCollider.size * scaleAroundGrid;
        boxCollider.enabled = true;
        int sqrt = (int) Mathf.Sqrt(howMany);
        for (int i = 0; i < sqrt; ++i)
        {
            for (int j = 0; j < sqrt; ++j)
            {
                Vector2 spawnPoint;
                do
                {
                    spawnPoint = new Vector2(Random.Range(-script.size.x / 2, script.size.x / 2),
                        script.size.y * i / sqrt - script.size.y / 2);
                } while (boxCollider.OverlapPoint(spawnPoint));

                var tree = Instantiate(gameObjects[Random.Range(0, gameObjects.Count)]);
                tree.transform.position = spawnPoint;
                var size = Random.Range(sizeRange.x, sizeRange.y);
                tree.transform.localScale = new Vector2(size, size);
                tree.GetComponent<SpriteRenderer>().sortingLayerName = "Grid";
                tree.GetComponent<SpriteRenderer>().sortingOrder = sqrt - i + 1;
                treeList.Add(tree);
            }
        }

        boxCollider.enabled = false;
        boxCollider.size = boxCollider.size / scaleAroundGrid;
    }
}