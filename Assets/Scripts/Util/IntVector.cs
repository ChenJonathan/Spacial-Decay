/// <summary>
/// Unity's Vector2, but with integers.
/// </summary>
public class IntVector
{
    public int x, y;

    /// <summary>
    /// Constructs an IntVector
    /// </summary>
    /// <param name="x">The x value</param>
    /// <param name="y">The y value</param>
    public IntVector(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    /// <summary>
    /// Checks if another object is equal to this one.
    /// </summary>
    /// <param name="obj">The object to be checked with</param>
    /// <returns>Whether the objects are equal or not</returns>
    public override bool Equals(System.Object obj)
    {
        if(obj == null)
        {
            return false;
        }
        
        IntVector vector = obj as IntVector;
        if((System.Object)vector == null)
        {
            return false;
        }
        
        return (x == vector.x) && (y == vector.y);
    }

    /// <summary>
    /// Returns a hash value representing the object.
    /// </summary>
    /// <returns>The hash value representing the object</returns>
    public override int GetHashCode()
    {
        return x * 7 + y * 9;
    }

    /// <summary>
    /// Returns a string representing the object.
    /// </summary>
    /// <returns>The string representing the object</returns>
    public override string ToString()
    {
        return "(" + x + ", " + y + ")";
    }
}