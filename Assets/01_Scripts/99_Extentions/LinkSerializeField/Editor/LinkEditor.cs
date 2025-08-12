using System;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MonoBehaviour), true)]
public class LinkEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var targetObject = target as MonoBehaviour;
        var type = targetObject.GetType();
        var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        bool updated = false;

        foreach (var field in fields)
        {
            var attr = field.GetCustomAttribute<LinkAttribute>();
            if (attr == null) continue;

            string objectName = attr.ObjectName;
            Type componentType = field.FieldType;
            if (attr.UseFieldNameAsObjectName)
            {
                objectName = field.Name.Replace("_", "");
            }

            // 이미 값이 들어있으면 스킵
            object fieldValue = field.GetValue(targetObject);
            if (fieldValue is Component component && component != null)
            {
                // 이미 컴포넌트가 할당되어 있으면 스킵
                if(!component.name.Equals(objectName, StringComparison.OrdinalIgnoreCase))
                {
                    // 이름이 변경됐거나 타입이 변경됐으면 연결 끊음
                    Debug.LogWarning($"[LinkSerialized] Object name mismatch: expected '{objectName}', but found '{component.name}'. Clearing field.");
                    field.SetValue(targetObject, null);
                    EditorUtility.SetDirty(targetObject);
                    Undo.RecordObject(targetObject, "LinkSerialized Assignment");
                    updated = true;
                }
                continue;
            }

            // 모든 컴포넌트를 찾는다 (비활성 포함)
            var components = Resources.FindObjectsOfTypeAll(componentType);
            if (components == null || components.Length == 0)
            {
                Debug.LogWarning($"[LinkSerialized] Component type '{componentType}' not found in any object.");
                continue;
            }

            // 이름이 일치하는 오브젝트를 가진 컴포넌트를 찾는다
            var comp = components
                .Cast<Component>()
                .FirstOrDefault(c => c.gameObject.name.Equals(objectName, StringComparison.OrdinalIgnoreCase));

            if (comp == null)
            {
                Debug.LogWarning($"[LinkSerialized] GameObject '{objectName}' with '{componentType}' not found.");
                continue;
            }

            field.SetValue(targetObject, comp);
            EditorUtility.SetDirty(targetObject);
            Undo.RecordObject(targetObject, "LinkSerialized Assignment");
            updated = true;
        }

        if (updated)
        {
            serializedObject.Update();
        }
    }
}
