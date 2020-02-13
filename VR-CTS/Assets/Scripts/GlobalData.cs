using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalData
{
    private static string _levelName = null;
    public static string LevelName { get { return _levelName; } set { _levelName = value; } }
}
