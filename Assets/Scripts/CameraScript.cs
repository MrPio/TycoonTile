using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField]
    public static float hitStrength=10f;
    [SerializeField]
    public static float digSpeed = 0.1f;
    [SerializeField]
    private float dragSpeed = 2;
    private Vector3 posInit;
    private float animationBorderTime;

    // Start is called before the first frame update
    void Start()
    {

    }

    void Update()
    {
        /*if ((Camera.main.ScreenToViewportPoint(Input.mousePosition).x < 0.2f || Camera.main.ScreenToViewportPoint(Input.mousePosition).x > 0.8f ||
            Camera.main.ScreenToViewportPoint(Input.mousePosition).y < 0.3f || Camera.main.ScreenToViewportPoint(Input.mousePosition).y > 0.7f))
        {*/
            var direction = ((Vector2)Camera.main.ScreenToViewportPoint(Input.mousePosition) - new Vector2(0.5f, 0.5f));
            animationBorderTime += Time.deltaTime;
            transform.position = new Vector3(0, 0, -10) + (Vector3)direction.normalized *
            new ParabolaInterpolator(0.71f).interpolator(direction.magnitude) * 0.8f;
        //}

    }

    void OnGUI()
    {
        if (32.7f < GetComponent<Camera>().fieldOfView - Input.mouseScrollDelta.y &&
            GetComponent<Camera>().fieldOfView - Input.mouseScrollDelta.y < 80f)
            GetComponent<Camera>().fieldOfView -= Input.mouseScrollDelta.y;
    }
}
