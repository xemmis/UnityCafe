#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Models;

[CustomEditor(typeof(ActionsSO))]
public sealed class NpcSOEditor : Editor
{
    private GUIStyle _headerStyle;
    private GUIStyle _groupBoxStyle;
    private GUIStyle _actionBoxStyle;
    private GUIStyle _badgeStyle;
    private bool _stylesInitialized;

    private void InitStyles()
    {
        if (_stylesInitialized) return;

        _headerStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 13,
            normal = { textColor = new Color(0.9f, 0.9f, 0.9f) }
        };

        _groupBoxStyle = new GUIStyle(GUI.skin.box)
        {
            padding = new RectOffset(8, 8, 6, 6),
            margin = new RectOffset(0, 0, 4, 4)
        };

        _actionBoxStyle = new GUIStyle(GUI.skin.box)
        {
            padding = new RectOffset(6, 6, 4, 4),
            margin = new RectOffset(0, 0, 2, 2),
            normal = { background = MakeTex(1, 1, new Color(0.2f, 0.2f, 0.2f, 0.3f)) }
        };

        _badgeStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
        {
            fontStyle = FontStyle.Bold
        };

        _stylesInitialized = true;
    }

    public override void OnInspectorGUI()
    {
        InitStyles();
        serializedObject.Update();

        SerializedProperty actionsList = serializedObject.FindProperty("<Actions>k__BackingField");

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Action Groups", _headerStyle);
        GUILayout.FlexibleSpace();
        EditorGUILayout.LabelField($"{actionsList.arraySize} groups", _badgeStyle, GUILayout.Width(60));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(4);

        DrawActionGroups(actionsList);

        EditorGUILayout.Space(6);

        if (GUILayout.Button("＋  Add Actions Group", GUILayout.Height(30)))
            actionsList.InsertArrayElementAtIndex(actionsList.arraySize);

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawActionGroups(SerializedProperty actionsList)
    {
        for (int i = 0; i < actionsList.arraySize; i++)
        {
            SerializedProperty group = actionsList.GetArrayElementAtIndex(i);
            SerializedProperty innerList = group.FindPropertyRelative("<Actions>k__BackingField");

            EditorGUILayout.BeginVertical(_groupBoxStyle);

            EditorGUILayout.BeginHorizontal();
            group.isExpanded = EditorGUILayout.Foldout(group.isExpanded,
                $"Group {i}  ({innerList.arraySize} actions)", true, EditorStyles.foldoutHeader);

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("＋", GUILayout.Width(26), GUILayout.Height(18)))
            {
                innerList.InsertArrayElementAtIndex(innerList.arraySize);
                group.isExpanded = true;
            }

            GUI.color = new Color(1f, 0.4f, 0.4f);
            if (GUILayout.Button("✕", GUILayout.Width(26), GUILayout.Height(18)))
            {
                actionsList.DeleteArrayElementAtIndex(i);
                GUI.color = Color.white;
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                break;
            }
            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();

            if (group.isExpanded)
            {
                EditorGUI.indentLevel++;
                DrawActions(innerList);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();
        }
    }

    private void DrawActions(SerializedProperty innerList)
    {
        if (innerList.arraySize == 0)
        {
            EditorGUILayout.HelpBox("No actions. Press ＋ to add.", MessageType.Info);
            return;
        }

        for (int j = 0; j < innerList.arraySize; j++)
        {
            SerializedProperty action = innerList.GetArrayElementAtIndex(j);
            SerializedProperty dialogue = action.FindPropertyRelative("<Dialogue>k__BackingField");
            SerializedProperty stateType = action.FindPropertyRelative("<StateType>k__BackingField");
            SerializedProperty walkType = action.FindPropertyRelative("<WalkType>k__BackingField");
            SerializedProperty foodRecipe = action.FindPropertyRelative("<FoodRecipe>k__BackingField");
            SerializedProperty intData = action.FindPropertyRelative("<IntData>k__BackingField");

            EditorGUILayout.BeginVertical(_actionBoxStyle);

            EditorGUILayout.BeginHorizontal();
            action.isExpanded = EditorGUILayout.Foldout(action.isExpanded,
                $"Action {j}  —  {(StateType)stateType.enumValueIndex}",
                true);
            GUILayout.FlexibleSpace();
            GUI.color = new Color(1f, 0.4f, 0.4f);
            if (GUILayout.Button("✕", GUILayout.Width(26), GUILayout.Height(16)))
            {
                innerList.DeleteArrayElementAtIndex(j);
                GUI.color = Color.white;
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                break;
            }
            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();

            if (!action.isExpanded)
            {
                EditorGUILayout.EndVertical();
                continue;
            }

            EditorGUI.indentLevel++;

            // Dialogue — всегда
            EditorGUILayout.PropertyField(dialogue, new GUIContent("Dialogue Tree"));

            // StateType — всегда
            EditorGUILayout.PropertyField(stateType, new GUIContent("State"));

            StateType currentState = (StateType)stateType.enumValueIndex;

            // IntData — теперь всегда
            EditorGUILayout.PropertyField(intData, new GUIContent("Int Data"));

            // WalkType — только при Walk
            if (currentState == StateType.Walk)
            {
                EditorGUILayout.PropertyField(walkType, new GUIContent("Walk Type"));
            }

            // FoodRecipe — только при MakeOrder
            if (currentState == StateType.MakeOrder)
            {
                EditorGUILayout.PropertyField(foodRecipe, new GUIContent("Food Recipe"));
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }
    }

    private static Texture2D MakeTex(int w, int h, Color col)
    {
        var tex = new Texture2D(w, h);
        tex.SetPixel(0, 0, col);
        tex.Apply();
        return tex;
    }
}
#endif