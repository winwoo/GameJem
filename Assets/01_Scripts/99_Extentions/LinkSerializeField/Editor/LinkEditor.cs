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

            // �̹� ���� ��������� ��ŵ
            object fieldValue = field.GetValue(targetObject);
            if (fieldValue is Component component && component != null)
            {
                // �̹� ������Ʈ�� �Ҵ�Ǿ� ������ ��ŵ
                if(!component.name.Equals(objectName, StringComparison.OrdinalIgnoreCase))
                {
                    // �̸��� ����ưų� Ÿ���� ��������� ���� ����
                    Debug.LogWarning($"[LinkSerialized] Object name mismatch: expected '{objectName}', but found '{component.name}'. Clearing field.");
                    field.SetValue(targetObject, null);
                    EditorUtility.SetDirty(targetObject);
                    Undo.RecordObject(targetObject, "LinkSerialized Assignment");
                    updated = true;
                }
                continue;
            }

            // ��� ������Ʈ�� ã�´� (��Ȱ�� ����)
            var components = Resources.FindObjectsOfTypeAll(componentType);
            if (components == null || components.Length == 0)
            {
                Debug.LogWarning($"[LinkSerialized] Component type '{componentType}' not found in any object.");
                continue;
            }

            // �̸��� ��ġ�ϴ� ������Ʈ�� ���� ������Ʈ�� ã�´�
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
