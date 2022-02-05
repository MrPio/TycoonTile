using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public static readonly float hitStrength=10f;
    public static readonly float digSpeed = 0.1f;

    // Start is called before the first frame update
    void Start()
    {

    }

    void Update()
    {
        /*if ((Camera.main.ScreenToViewportPoint(Input.mousePosition).x < 0.2f || Camera.main.ScreenToViewportPoint(Input.mousePosition).x > 0.8f ||
            Camera.main.ScreenToViewportPoint(Input.mousePosition).y < 0.3f || Camera.main.ScreenToViewportPoint(Input.mousePosition).y > 0.7f))
        {*/
            var direction = ((Vector2)Camera.main!.ScreenToViewportPoint(Input.mousePosition) - new Vector2(0.5f, 0.5f));
            transform.position = new Vector3(0, 0, -10) + (Vector3)direction.normalized *
            new ParabolaInterpolator(0.71f).interpolator(direction.magnitude) * 0.8f;
        //}

    }

    void OnGUI()
    {
        var sensorSize = GetComponent<Camera>().orthographicSize;
        var scrollDelta = Input.mouseScrollDelta.y / 3f;
        if (3.5f < sensorSize - scrollDelta && sensorSize - scrollDelta < 15f)
            GetComponent<Camera>().orthographicSize -= scrollDelta;
    }
}
