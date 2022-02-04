using UnityEngine;

public class CircleScript : MonoBehaviour
{
    [Range(1, 100)] public const float Velocity = 14.0f;

    private float _elapsed;
    private const float Duration = 0.6f;

    private void Update()
    {
        _elapsed += Time.deltaTime;
        var localScale = transform.localScale;
        localScale = new Vector2(localScale.x,
            localScale.y - localScale.y * _elapsed / (Duration * Random.Range(60,100)));
        transform.localScale = localScale;
        var color = GetComponent<SpriteRenderer>().color;
        color.a = 1-_elapsed / Duration;
        GetComponent<SpriteRenderer>().color = color;
        if (_elapsed > Duration)
            Destroy(gameObject);
    }
}