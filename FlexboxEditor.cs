using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(FlexContainer))]
[CanEditMultipleObjects]
public class FlexboxEditor : Editor
{
    //Container Options
    string[] displaytypes = new[] { "Flex", "Inline-Flex" };
    SerializedProperty display;
    SerializedProperty flexdir;
    SerializedProperty flexwr;
    SerializedProperty justcon;
    SerializedProperty aligni;
    SerializedProperty alignc;
    SerializedProperty containerPreferredSize;

    SerializedProperty RootContainer;
    SerializedProperty ChildContainer;
    string[] flexdirection = new[] { "Row", "Row-reverse", "Column", "Column-reverse" };
    string[] flexwrap = new[] { "Wrap", "No Wrapping", "Wrap-reverse" };
    string[] justifyContent = new[] { "Flex-start", "Flex-end", "Space-evenly" };
    string[] alignitems = new[] { "Flex-start", "Flex-end", "Center", "Stretch", "Baseline" };
    string[] aligncontent = new[] { "Flex-start", "Flex-end", "Center", "Stretch", "Space-between", "Space-around" };
    int[] containerIndexes = new[] { 0, 0, 0, 0, 0, 0 };

    //Child Options



    void OnEnable()
    {
        display = serializedObject.FindProperty("displayTypeIndex");
        flexdir = serializedObject.FindProperty("flexDirectionIndex");
        flexwr = serializedObject.FindProperty("flexWrapIndex");
        justcon = serializedObject.FindProperty("justifyContentIndex");
        aligni = serializedObject.FindProperty("alignItemsIndex");
        alignc = serializedObject.FindProperty("alignContentIndex");
        RootContainer = serializedObject.FindProperty("RootContainer");
        ChildContainer = serializedObject.FindProperty("ChildContainer");

    }
    public override void OnInspectorGUI()
    {
        FlexContainer script = (FlexContainer)target;
        serializedObject.Update();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(RootContainer);
        EditorGUILayout.PropertyField(ChildContainer);
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Get Children of Container"))
        {
            script.GetChildren();

        }



        if (GUILayout.Button("Child Container Fill To Preferred Size"))
        {
            script.SetContainerSize();

        }

        display.intValue = EditorGUILayout.Popup("Display Type", display.intValue, displaytypes);

        flexdir.intValue = EditorGUILayout.Popup("Flex Direction", flexdir.intValue, flexdirection);

        flexwr.intValue = EditorGUILayout.Popup("Flex Wrap", flexwr.intValue, flexwrap);

        justcon.intValue = EditorGUILayout.Popup("Justify Content", justcon.intValue, justifyContent);

        aligni.intValue = EditorGUILayout.Popup("Align Items", aligni.intValue, alignitems);

        alignc.intValue = EditorGUILayout.Popup("Align Content", alignc.intValue, aligncontent);


        serializedObject.ApplyModifiedProperties();

    }

}
