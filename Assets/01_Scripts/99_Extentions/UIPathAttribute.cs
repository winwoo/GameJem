using System;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class UIPathAttribute : Attribute
{
    public readonly string Path; // ¿¹) "UI/Popup/{UIBase_Name}"
    public UIPathAttribute(string path) => Path = path;
}
