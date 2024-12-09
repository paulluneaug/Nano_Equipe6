using System.Runtime.CompilerServices;
using UnityEngine;

public static class Vector2Utils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 XY0(this Vector2 v2)
    {
        return new Vector3(v2.x, v2.y);
    }
}
