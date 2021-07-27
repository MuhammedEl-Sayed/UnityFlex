using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(FlexChildren))]
[CanEditMultipleObjects]
public class FlexChildrenEditor : Editor
{
    SerializedProperty childorder;
    SerializedProperty childflexgrow;
    SerializedProperty childflexshrink;
    string[] flexBasisType = new[] { "Auto", "Content", "Custom Percentage" };
    SerializedProperty childFlexTypeIndex;
    SerializedProperty childflexbasis;
    SerializedProperty flexValue;

    SerializedProperty autoheight;
    SerializedProperty autowidth;

    string[] constraintType = new[] { "None", "Custom" };
    SerializedProperty constraintTypeIndex;
    SerializedProperty containerConstraintsHeightx;
    SerializedProperty containerConstraintsHeighty;
    SerializedProperty containerConstraintsWidthx;
    SerializedProperty containerConstraintsWidthy;
    string[] marginType = new[] { "Auto", "Custom" };
    SerializedProperty topMarginType;
    SerializedProperty topMarginValue;
    SerializedProperty bottomMarginType;
    SerializedProperty bottomMarginValue;
    SerializedProperty rightMarginType;
    SerializedProperty rightMarginValue;
    SerializedProperty leftMarginType;
    SerializedProperty leftMarginValue;

    // string[] childalignself = new[] { "Flex-start", "Flex-end", "Center", "Stretch", "Baseline" };
    //int[] childIndexes = new[] { 0, 0, 0 };
    // int childNameIndex = 0;
    void OnEnable()
    {
        childorder = serializedObject.FindProperty("childOrder");
        childflexgrow = serializedObject.FindProperty("childFlexGrow");
        childflexshrink = serializedObject.FindProperty("childFlexShrink");
        flexValue = serializedObject.FindProperty("flexBasisSize");
        childFlexTypeIndex = serializedObject.FindProperty("childFlexTypeIndex");
        childflexbasis = serializedObject.FindProperty("flexBasisSize");
        autoheight = serializedObject.FindProperty("autoHeight");
        autowidth = serializedObject.FindProperty("autoWidth");
        constraintTypeIndex = serializedObject.FindProperty("constraintTypeIndex");
                containerConstraintsHeightx = serializedObject.FindProperty("containerConstraintsHeightx");
        containerConstraintsHeighty = serializedObject.FindProperty("containerConstraintsHeighty");
        containerConstraintsWidthx = serializedObject.FindProperty("containerConstraintsWidthx");
        containerConstraintsWidthy = serializedObject.FindProperty("containerConstraintsWidthy");
        topMarginType = serializedObject.FindProperty("topMarginType");
        topMarginValue = serializedObject.FindProperty("topMarginValue");
        bottomMarginType = serializedObject.FindProperty("bottomMarginType");
        bottomMarginValue = serializedObject.FindProperty("bottomMarginValue");
        rightMarginType = serializedObject.FindProperty("rightMarginType");
        rightMarginValue = serializedObject.FindProperty("rightMarginValue");
        leftMarginType = serializedObject.FindProperty("leftMarginType");
        leftMarginValue = serializedObject.FindProperty("leftMarginValue");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.LabelField("Children Properties");
        // EditorGUILayout.BeginHorizontal();
        childFlexTypeIndex.intValue = EditorGUILayout.Popup("Display Type", childFlexTypeIndex.intValue, flexBasisType);
        if (childFlexTypeIndex.intValue == 2)
        {
            EditorGUILayout.PropertyField(childflexbasis);
        }
        else
        {
            childflexbasis.floatValue = 0;
        }

        EditorGUILayout.BeginHorizontal();
        constraintTypeIndex.vector4Value = new Vector4(EditorGUILayout.Popup(GUIContent.none, (int)constraintTypeIndex.vector4Value.x, constraintType, GUILayout.MaxWidth(100f)), constraintTypeIndex.vector4Value.y, constraintTypeIndex.vector4Value.z, constraintTypeIndex.vector4Value.w);
        if (constraintTypeIndex.vector4Value.x == 1)
        {
            EditorGUILayout.PropertyField(containerConstraintsHeightx, GUIContent.none, GUILayout.ExpandWidth(false));
        }
        EditorGUILayout.LabelField("> Height >");
        constraintTypeIndex.vector4Value = new Vector4(constraintTypeIndex.vector4Value.x, EditorGUILayout.Popup(GUIContent.none, (int)constraintTypeIndex.vector4Value.y, constraintType, GUILayout.MaxWidth(100f)), constraintTypeIndex.vector4Value.z, constraintTypeIndex.vector4Value.w);
        if (constraintTypeIndex.vector4Value.y == 1)
        {
            EditorGUILayout.PropertyField(containerConstraintsHeighty, GUIContent.none, GUILayout.ExpandWidth(false));
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        constraintTypeIndex.vector4Value = new Vector4(constraintTypeIndex.vector4Value.x, constraintTypeIndex.vector4Value.y, EditorGUILayout.Popup(GUIContent.none, (int)constraintTypeIndex.vector4Value.z, constraintType, GUILayout.MaxWidth(100f)), constraintTypeIndex.vector4Value.w);

        if (constraintTypeIndex.vector4Value.z == 1)
        {
            EditorGUILayout.PropertyField(containerConstraintsWidthx, GUIContent.none, GUILayout.ExpandWidth(false));
        }
        EditorGUILayout.LabelField("> Width >");
        constraintTypeIndex.vector4Value = new Vector4(constraintTypeIndex.vector4Value.x, constraintTypeIndex.vector4Value.y, constraintTypeIndex.vector4Value.z, EditorGUILayout.Popup(GUIContent.none, (int)constraintTypeIndex.vector4Value.w, constraintType, GUILayout.MaxWidth(100f)));

        if (constraintTypeIndex.vector4Value.w == 1)
        {
            EditorGUILayout.PropertyField(containerConstraintsWidthy, GUIContent.none, GUILayout.ExpandWidth(false));
        }
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.PropertyField(childorder);
        EditorGUILayout.PropertyField(childflexgrow);
        EditorGUILayout.PropertyField(childflexshrink);

        EditorGUILayout.LabelField("Margins:");
        topMarginType.intValue = EditorGUILayout.Popup("Top Margin Type", topMarginType.intValue, marginType);
        if (topMarginType.intValue == 1)
        {
            EditorGUILayout.PropertyField(topMarginValue, GUILayout.ExpandWidth(false));
        }
        else topMarginValue.floatValue = 0;
        bottomMarginType.intValue = EditorGUILayout.Popup("Bottom Margin Type",bottomMarginType.intValue, marginType);
        if (bottomMarginType.intValue == 1)
        {
            EditorGUILayout.PropertyField(bottomMarginValue, GUILayout.ExpandWidth(false));
        }
         else bottomMarginValue.floatValue = 0;
        rightMarginType.intValue = EditorGUILayout.Popup("Right Margin Type",rightMarginType.intValue, marginType);
        if (rightMarginType.intValue == 1)
        {
            EditorGUILayout.PropertyField(rightMarginValue,GUILayout.ExpandWidth(false));
        }
         else rightMarginValue.floatValue = 0;
        leftMarginType.intValue = EditorGUILayout.Popup("Left Margin Type",leftMarginType.intValue, marginType);
        if (leftMarginType.intValue == 1)
        {
            EditorGUILayout.PropertyField(leftMarginValue,  GUILayout.ExpandWidth(false));
        }
         else leftMarginValue.floatValue = 0;


        serializedObject.ApplyModifiedProperties();

    }

}
