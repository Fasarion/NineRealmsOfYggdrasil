using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class Extensions
{
    public static List<MethodInfo> GetMethods(this MonoBehaviour mb, Type returnType, Type[] paramTypes, BindingFlags flags)
    {
        return mb.GetType().GetMethods(flags)
            .Where(m => m.ReturnType == returnType)
            .Select(m => new { m, Params = m.GetParameters() })
            .Where(x =>
            {
                return paramTypes == null ? // in case we want no params
                    x.Params.Length == 0 :
                    x.Params.Length == paramTypes.Length &&
                    x.Params.Select(p => p.ParameterType).ToArray().IsEqualTo(paramTypes);
            })
            .Select(x => x.m)
            .ToList();
    }
    public static List<MethodInfo> GetMethods(this GameObject go, Type returnType, Type[] paramTypes, BindingFlags flags)
    {
        var mbs = go.GetComponents<MonoBehaviour>();
        List<MethodInfo> list = new List<MethodInfo>();
        foreach (var mb in mbs) {
            list.AddRange(mb.GetMethods(returnType, paramTypes, flags));
        }
        return list;
    }
    
    public static bool IsEqualTo<T>(this IList<T> list, IList<T> other)
    {
        if (list.Count != other.Count) return false;
        for (int i = 0, count = list.Count; i < count; i++) {
            if (!list[i].Equals(other[i])) {
                return false;
            }
        }
        return true;
    }
}
