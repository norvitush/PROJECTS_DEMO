
using UnityEngine;



public struct SafeInt
{
    private int value;
    private int priprava;

    public SafeInt(int value = 0)
    {
        priprava = Random.Range(int.MinValue / 6, int.MaxValue / 6);
        this.value = value ^ priprava;
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    public override bool Equals(object obj)
    {
        try
        {
            return (int)this == (int)obj;
        }
        catch (System.Exception)
        {

            return false;
        }
        

    }
    public override string ToString()
    {
        return ((int)this).ToString();
    }

    public static implicit operator int(SafeInt safeInt)
    {
        return safeInt.value ^ safeInt.priprava;
    }

    public static implicit operator SafeInt(int normalInt)
    {
        return new SafeInt(normalInt);
    }

    public static explicit operator bool(SafeInt normalInt)
    {
        return normalInt==1;
    }
}
