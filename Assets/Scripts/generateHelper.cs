using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class generateHelper
{
    public static void ClearChildern(this Transform Base)
    {
        var list = Base.GetComponentsInChildren<Transform>();
        foreach(var e in list)
        {
        }
    }
}
