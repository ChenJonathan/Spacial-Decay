using UnityEngine;
using DanmakU;

/// <summary>
/// Utility functions involving the boundary box.
/// </summary>
public class BoundsUtil
{
    /// <summary>
    /// Makes sure that the <paramref name="objCenter"/> specified will allow the <paramref name="obj"/> to remain in the <paramref name="field"/>.
    /// Returns the original <paramref name="objCenter"/> if it is valid or a new location otherwise.
    /// </summary>
    /// <param name="objCenter">The location of the object's center</param>
    /// <param name="obj">The object's bounds</param>
    /// <param name="field">The bounding box to fit the object in</param>
    /// <returns>The location to move the object to</returns>
    public static Vector2 VerifyBounds(Vector2 objCenter, Bounds2D obj, Bounds2D field)
    {
        Vector2 maxOffset = field.Max - (objCenter + obj.Extents);
        Vector2 minOffset = field.Min - (objCenter - obj.Extents);

        if (minOffset.x > 0)
            objCenter.x = field.XMin + obj.Extents.x;
        else if (maxOffset.x < 0)
            objCenter.x = field.XMax - obj.Extents.x;
        if (minOffset.y > 0)
            objCenter.y = field.YMin + obj.Extents.y;
        else if (maxOffset.y < 0)
            objCenter.y = field.YMax - obj.Extents.y;
        return objCenter;
    }
}