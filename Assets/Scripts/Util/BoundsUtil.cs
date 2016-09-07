using UnityEngine;
using System.Collections;
using DanmakU;

public class BoundsUtil
{
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
