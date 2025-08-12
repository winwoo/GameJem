using System;
using UnityEngine;


[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class LinkAttribute : PropertyAttribute
{
    public string ObjectName;

    public bool UseFieldNameAsObjectName => string.IsNullOrEmpty(ObjectName);

    public LinkAttribute()
    {
        ObjectName = null;
    }

    public LinkAttribute(string objectName)
    {
        ObjectName = objectName;
    }
}
