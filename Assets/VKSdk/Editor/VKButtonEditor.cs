using UnityEditor;
using VKSdk.UI;

[CustomEditor(typeof(VKButton))]
public class VKButtonEditor : UnityEditor.UI.ButtonEditor
{
    public override void OnInspectorGUI()
    {
        VKButton component = (VKButton)target;

        base.OnInspectorGUI();

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("GO CONFIG");
        var propertyGoActive = serializedObject.FindProperty("goActives");
        var propertyGoInActive = serializedObject.FindProperty("goInActives");
        serializedObject.Update();
        EditorGUILayout.PropertyField(propertyGoActive, true);
        EditorGUILayout.PropertyField(propertyGoInActive, true);
        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("IMAGE CONFIG");
        var property1 = serializedObject.FindProperty("imageAll");
        serializedObject.Update();
        EditorGUILayout.PropertyField(property1, true);
        serializedObject.ApplyModifiedProperties();

        var property3 = serializedObject.FindProperty("colorImageAll");
        serializedObject.Update();
        EditorGUILayout.PropertyField(property3, true);
        serializedObject.ApplyModifiedProperties();

        var property4 = serializedObject.FindProperty("spriteImageAll");
        serializedObject.Update();
        EditorGUILayout.PropertyField(property4, true);
        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("TEXT CONFIG");
        var property2 = serializedObject.FindProperty("textAll");
        serializedObject.Update();
        EditorGUILayout.PropertyField(property2, true);
        serializedObject.ApplyModifiedProperties();

        var property5 = serializedObject.FindProperty("colorTextAll");
        serializedObject.Update();
        EditorGUILayout.PropertyField(property5, true);
        serializedObject.ApplyModifiedProperties();

        var property6 = serializedObject.FindProperty("valueTextAll");
        serializedObject.Update();
        EditorGUILayout.PropertyField(property6, true);
        serializedObject.ApplyModifiedProperties();


        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("OUTLINE CONFIG");
        var property7 = serializedObject.FindProperty("outlineTextAll");
        serializedObject.Update();
        EditorGUILayout.PropertyField(property7, true);
        serializedObject.ApplyModifiedProperties();

        var property8 = serializedObject.FindProperty("colorOutlineTextAll");
        serializedObject.Update();
        EditorGUILayout.PropertyField(property8, true);
        serializedObject.ApplyModifiedProperties();
    }
}
