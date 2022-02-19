using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class NetworkConstant
{
    #region netcode constant
    public const int senddata = 1;

    #endregion

    public static Dictionary<Type, int> EntityIds = new Dictionary<Type, int>();
    public static Dictionary<int, MethodInfo> Methods = new Dictionary<int, MethodInfo>();
    private static int id = 0;

    public static void Init()
    {
        var types = typeof(Entity).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(Entity))).ToList();
        foreach(var t in types)
        {
            id++;
            EntityIds.Add(t, id);
            var method = t.GetMethod("CreateInstance", BindingFlags.Public | BindingFlags.Static);
            Methods.Add(id,method);
        }
    }
}
