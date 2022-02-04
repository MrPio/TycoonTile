using UnityEngine;

public class Circle
{
    public float radius { get; }
    public Vector2 center { get; }

    public Circle(float radius, Vector2 center)
    {
        this.radius = radius;
        this.center = center;
    }
    public Circle(Vector2 center)
    {
        radius = 0f;
        this.center = center;
    }


    public Vector2 getPointFromAngle(float angleRadiants)
    {
        var vec= new Vector2((Mathf.Cos(angleRadiants) * radius + center.x),
                           (Mathf.Sin(angleRadiants) * radius + center.y));
        return vec;
    }

    public float getAngleByPoint(Vector2 point)
    {
        float radius = Mathf.Sqrt(Mathf.Pow(center.x - point.x, 2) + Mathf.Pow(center.y - point.y, 2));
        if (point.y > center.y)
            return -(Mathf.Acos((+point.x - center.x) / radius) / Mathf.PI * 180f);
        return Mathf.Acos((+point.x - center.x) / radius) / Mathf.PI * 180f;
    }

    public float getDistaceFromCenter(Vector2 point)
    {
        return Mathf.Sqrt(Mathf.Pow(center.x - point.x, 2) + Mathf.Pow(center.y - point.y, 2));
    }
}
