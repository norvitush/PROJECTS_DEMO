using UnityEngine;
using System.Collections;

public struct SafeFloat
{
    private float value;
    private float priprava;

    public SafeFloat(float value = 0)
    {
        priprava = Random.Range(0.1f, 10f);
        this.value = value * priprava;
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    public override bool Equals(object obj)
    {
        return (float)this == (float)obj;
    }
    public override string ToString()
    {
        return ((float)this).ToString();
    }
    public float GetValue()
    {
        return this.value / this.priprava;
    }
    public static implicit operator float(SafeFloat safeFloat)
    {

        return safeFloat.value / safeFloat.priprava;
    }

    public static implicit operator SafeFloat(float normalFloat)
    {
        return new SafeFloat(normalFloat);
    }

    public static SafeFloat operator +(SafeFloat a, SafeFloat b)
    {
        return new SafeFloat(a.GetValue()+b.GetValue());
    }
    public static SafeFloat operator +(SafeFloat a, float b)
    {
        return new SafeFloat(a.GetValue() + b);
    }
    public static SafeFloat operator -(SafeFloat a, SafeFloat b)
    {       
        return new SafeFloat(a.GetValue() - b.GetValue());
    }
    public static SafeFloat operator -(SafeFloat a, float b)
    {
        return new SafeFloat(a.GetValue() - b);
    }
    public static bool operator >(SafeFloat a, SafeFloat b)
    {
        return a.GetValue() > b.GetValue();
    }
    public static bool operator <(SafeFloat a, SafeFloat b)
    {
        return a.GetValue() < b.GetValue();
    }
    public static bool operator >(SafeFloat a, float b)
    {
        return a.GetValue() > b;
    }
    public static bool operator <(SafeFloat a, float b)
    {
        return a.GetValue() < b;
    }
    public static bool operator >(float a, SafeFloat b)
    {
        return a > b.GetValue();
    }
    public static bool operator <(float a, SafeFloat b)
    {
        return a < b.GetValue();
    }
}
