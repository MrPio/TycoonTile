using UnityEngine;

public class ParabolaInterpolator
{
    float amp;
    public ParabolaInterpolator(float amp)
    {
        this.amp = amp;
    }
    public float interpolator(float t)
    {
        return Mathf.Pow(t/amp,3);
    }
}
