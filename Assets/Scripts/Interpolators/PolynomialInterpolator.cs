public class PolynomialInterpolator
{
    float speed;
    public PolynomialInterpolator(float speed)
    {
        this.speed = speed;
    }
    public float interpolator(float t)
    {
        return (speed*t*t)/(speed*t*t+t);
    }
}
