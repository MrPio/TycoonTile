using System;
using System.Collections;
using System.Collections.Generic;
using Classes;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class TileResources : MonoBehaviour
{
    public enum TileType
    {
        Wood
    }

    public static Dictionary<TileType, Tile[]> TilesDictionary = new()
    {
        [TileType.Wood] = new[]
        {
            new Tile("Acacia", "logs", 1, "#F2C791"),
            new Tile("Willow", "logs", 1, "#F2994B"),
            new Tile("Birch", "logs", 1, "#F2A97E"),
            new Tile("Pine tree", "logs", 1, "#8C533E"),
            new Tile("Oak", "logs", 1, "#BF7E78")
        }
    };

    public float healthMax = 100f;
    public int row, col;
    public int type;
    public TileType tileType;

    private float _offsetY;
    private AudioClip[] _mouseDownAudioClip;
    private AudioClip[] _mouseUpAudioClip;
    private AudioClip[] _damageAudioClip;
    private AudioClip[] _destroyedAudioClip;
    private float _health;
    private bool _down;
    private float _downTime;
    private bool _destroying;
    private GameObject _menuInfo;
    private static readonly string[] Ages = {"Young", "Adult", "Old"};

    private void Start()
    {
        _mouseDownAudioClip = new[]
        {
            Resources.Load("Sounds/click_down_001") as AudioClip,
            Resources.Load("Sounds/click_down_002") as AudioClip,
            Resources.Load("Sounds/click_down_003") as AudioClip
        };
        _mouseUpAudioClip = new[]
        {
            Resources.Load("Sounds/click_up_001") as AudioClip,
            Resources.Load("Sounds/click_up_002") as AudioClip,
            Resources.Load("Sounds/click_up_003") as AudioClip
        };
        _damageAudioClip = new[]
        {
            Resources.Load("Sounds/wood_cut_001") as AudioClip,
            Resources.Load("Sounds/wood_cut_002") as AudioClip,
            Resources.Load("Sounds/wood_cut_003") as AudioClip,
            Resources.Load("Sounds/wood_cut_004") as AudioClip,
            Resources.Load("Sounds/wood_cut_005") as AudioClip,
            Resources.Load("Sounds/wood_cut_006") as AudioClip,
            Resources.Load("Sounds/wood_cut_007") as AudioClip,
            Resources.Load("Sounds/wood_cut_008") as AudioClip
        };
        _destroyedAudioClip = new[]
        {
            Resources.Load("Sounds/wood1") as AudioClip,
            Resources.Load("Sounds/wood2") as AudioClip,
            Resources.Load("Sounds/wood3") as AudioClip,
            Resources.Load("Sounds/wood4") as AudioClip
        };
        _health = healthMax;
    }

    void Update()
    {
        if (!_down) return;
        _downTime += Time.deltaTime;
        if (_downTime > CameraScript.digSpeed)
        {
            //DAMAGE
            _health -= CameraScript.hitStrength;
            if (_health <= 0)
            {
                if (_destroying) return;
                _destroying = true;
                Destroy();
                return;
            }

            gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite =
                Resources.Load("destroy_stage_" + (9 - _health / healthMax * 10), typeof(Sprite)) as Sprite;
            _downTime = 0f;
            GetComponent<AudioSource>().PlayOneShot(_damageAudioClip[Random.Range(0, _damageAudioClip.Length)]);
        }
    }

    void OnMouseDown()
    {
        _down = true;
        _downTime = 0;

        gameObject.transform.position = (Vector2) transform.position + new Vector2(0f, _offsetY);
        GetComponent<AudioSource>().clip = _mouseDownAudioClip[Random.Range(0, _mouseDownAudioClip.Length)];
        GetComponent<AudioSource>().Play();

        if (_menuInfo != null)
            Destroy(_menuInfo);
    }

    private void OnMouseUp()
    {
        _down = false;

        gameObject.transform.position = (Vector2) transform.position - new Vector2(0f, _offsetY);
        GetComponent<AudioSource>().clip = _mouseUpAudioClip[Random.Range(0, _mouseUpAudioClip.Length)];
        GetComponent<AudioSource>().Play();
        _health = healthMax;
        gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
    }

    private void OnMouseEnter()
    {
        _menuInfo = Instantiate(Resources.Load("SelectedItemInfo") as GameObject, GameObject.Find("Canvas").transform, true);
        var mouse = Input.mousePosition;
        mouse.z = 10;
        var mousePos = (Vector2) Camera.main!.ScreenToWorldPoint(mouse);
        if (mousePos.Equals(Vector3.zero)) return;
        _menuInfo.transform.position = (Vector3) mousePos + _menuInfo.transform.localScale / 1f;
        _menuInfo.gameObject.transform.Find("title").GetComponent<TextMeshProUGUI>().text=TilesDictionary[tileType][type].name;
        _menuInfo.gameObject.transform.Find("title").GetComponent<TextMeshProUGUI>().color=HexToColor(TilesDictionary[tileType][type].color);
        _menuInfo.gameObject.transform.Find("logs").GetComponent<TextMeshProUGUI>().text=TilesDictionary[tileType][type].resourcesCount.ToString();
        _menuInfo.gameObject.transform.Find("age").GetComponent<TextMeshProUGUI>().text=Ages[Random.Range(0,Ages.Length)];
    }

    private void OnMouseExit()
    {
        Destroy(_menuInfo);
    }

    private void OnMouseOver()
    {
        if (_menuInfo == null)
            return;

        var mouse = Input.mousePosition;
        mouse.z = 10;
        if (Camera.main == null) return;
        var mousePos = (Vector2) Camera.main.ScreenToWorldPoint(mouse);
        var l2 = _menuInfo.transform.localScale / 1f;
        if (mousePos.Equals(Vector3.zero)) return;
        _menuInfo.transform.position = (Vector3) mousePos + l2;
    }

    public void SetOffsetY(float offsetY)
    {
        _offsetY = offsetY;
    }

    private void Destroy()
    {
        GameObject.Find("Main Camera").GetComponent<AudioSource>()
            .PlayOneShot(_destroyedAudioClip[Random.Range(0, _destroyedAudioClip.Length)]);
        gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);

        GameObject.Find("GridMain").GetComponent<GridSystem>().Grid[row][col] = null;

        DestroyEffect(Random.Range(6, 12), HexToColor(TilesDictionary[tileType][type].color));

        var color = GetComponent<SpriteRenderer>().color;
        color.a = 0f;
        GetComponent<SpriteRenderer>().color = color;

        StartCoroutine(ChainDestroy());
    }

    private IEnumerator ChainDestroy()
    {
        foreach (var tile in GameObject.Find("GridMain").GetComponent<GridSystem>().GetFlow(gameObject))
        {
            yield return new WaitForSeconds(0.1f);
            GameObject.Find("Main Camera").GetComponent<AudioSource>()
                .PlayOneShot(_destroyedAudioClip[Random.Range(0, _destroyedAudioClip.Length)]);
            var script = tile.GetComponent<TileResources>();
            GameObject.Find("GridMain").GetComponent<GridSystem>().Grid[script.row][script.col] = null;
            script.DestroyEffect(Random.Range(6, 12), HexToColor(TilesDictionary[tileType][tile.GetComponent<TileResources>().type].color));
            Destroy(tile);
        }

        Destroy(gameObject);
    }

    private void DestroyEffect(int num, Color color)
    {
        Circle circle = new Circle(0.2f, transform.position);
        float alpha = 0;
        for (int i = 0; i < num; ++i)
        {
            var circleGo = Instantiate(Resources.Load("circle") as GameObject);
            circleGo.transform.SetPositionAndRotation(circle.getPointFromAngle(alpha),
                Quaternion.Euler(0, 0, Mathf.Rad2Deg * alpha));
            circleGo.transform.localScale = new Vector2(0.16f, 0.16f);
            circleGo.GetComponent<SpriteRenderer>().sortingLayerName = "Grid";
            circleGo.GetComponent<SpriteRenderer>().sortingOrder = 99;
            circleGo.GetComponent<SpriteRenderer>().color = color;

            var direction = circle.getPointFromAngle(alpha) - circle.center;
            circleGo.GetComponent<Rigidbody2D>().velocity = direction * CircleScript.Velocity;
            circleGo.GetComponent<Rigidbody2D>().AddForce(-direction*1600f);

            alpha += (2 * Mathf.PI) / num;
        }
    }

    private static Color HexToColor(string hex)
    {
        return ColorUtility.TryParseHtmlString(hex, out var newCol) ? newCol : Color.black;
    }
}