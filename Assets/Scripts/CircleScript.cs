using UnityEngine;

public class CircleScript : MonoBehaviour
{
    [Range(1, 100)] public static float Velocity = 14.0f;

    private float _elapsed;
    private const float Duration = 0.8f;

    private void Update()
    {
        _elapsed += Time.deltaTime;
        var localScale = transform.localScale;
        localScale = new Vector2(localScale.x, localScale.y - localScale.y * _elapsed / (Duration * 4.6f));
        if (_elapsed > Duration)
            Destroy(gameObject);
    }
}