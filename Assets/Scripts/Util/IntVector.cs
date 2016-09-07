using UnityEngine;
using System.Collections;

public class IntVector
{
    public int x, y;

    public IntVector(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override bool Equals(System.Object obj)
    {
        if (obj == null)
        {
            return false;
        }
        
        IntVector vector = obj as IntVector;
        if ((System.Object)vector == null)
        {
            return false;
        }
        
        return (x == vector.x) && (y == vector.y);
    }

    public override int GetHashCode()
    {
        return x * 7 + y * 9;
    }

    public override string ToString()
    {
        return "(" + x + ", " + y + ")";
    }
}
