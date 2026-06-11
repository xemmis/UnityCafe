#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Models;
using Models.Food;
using Models.Npc;

[CustomEditor(typeof(FoodRecipe))]
public sealed class FoodRecipeEditor : Editor
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

        SerializedProperty ingredients = serializedObject.FindProperty("<Ingredients>k__BackingField");
        SerializedProperty foodOutput = serializedObject.FindProperty("<FoodOutput>k__BackingField");
        SerializedProperty npcActions = serializedObject.FindProperty("<NpcActions>k__BackingField");
        SerializedProperty cookTime = serializedObject.FindProperty("<CookTime>k__BackingField");

        // ── Header ──────────────────────────
        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("🍽 Recipe Configuration", _headerStyle);
        EditorGUILayout.Space(4);

        // ── Cook Time ───────────────────────
        EditorGUILayout.PropertyField(cookTime, new GUIContent("⏱ Cook Time (s)"));

        EditorGUILayout.Space(6);

        // ── Ingredients ─────────────────────
        EditorGUILayout.BeginVertical(_groupBoxStyle);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("📦 Ingredients", _headerStyle);
        GUILayout.FlexibleSpace();
        EditorGUILayout.LabelField($"{ingredients.arraySize} items", _badgeStyle, GUILayout.Width(55));
        EditorGUILayout.EndHorizontal();

        EditorGUI.indentLevel++;
        for (int i = 0; i < ingredients.arraySize; i++)
        {
            SerializedProperty item = ingredients.GetArrayElementAtIndex(i);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(item, GUIContent.none);

            GUI.color = new Color(1f, 0.4f, 0.4f);
            if (GUILayout.Button("✕", GUILayout.Width(26), GUILayout.Height(18)))
            {
                ingredients.DeleteArrayElementAtIndex(i);
                GUI.color = Color.white;
                EditorGUILayout.EndHorizontal();
                break;
            }
            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();
        }
        EditorGUI.indentLevel--;

        if (GUILayout.Button("＋ Add Ingredient", GUILayout.Height(24)))
            ingredients.InsertArrayElementAtIndex(ingredients.arraySize);

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(6);

        // ── Food Output ─────────────────────
        EditorGUILayout.BeginVertical(_groupBoxStyle);
        EditorGUILayout.LabelField("🍛 Result Dish", _headerStyle);
        EditorGUILayout.PropertyField(foodOutput, new GUIContent("Food Item"));
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(6);

        // ── NPC Actions ─────────────────────
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("👤 NPC Actions", _headerStyle);
        GUILayout.FlexibleSpace();
        EditorGUILayout.LabelField($"{npcActions.arraySize} actions", _badgeStyle, GUILayout.Width(60));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(2);
        DrawNpcActions(npcActions);

        EditorGUILayout.Space(4);
        if (GUILayout.Button("＋ Add NPC Action", GUILayout.Height(28)))
            npcActions.InsertArrayElementAtIndex(npcActions.arraySize);

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawNpcActions(SerializedProperty actionsList)
    {
        for (int i = 0; i < actionsList.arraySize; i++)
        {
            SerializedProperty action = actionsList.GetArrayElementAtIndex(i);
            SerializedProperty dialogue = action.FindPropertyRelative("<Dialogue>k__BackingField");
            SerializedProperty stateType = action.FindPropertyRelative("<StateType>k__BackingField");
            SerializedProperty walkType = action.FindPropertyRelative("<WalkType>k__BackingField");
            SerializedProperty foodRecipe = action.FindPropertyRelative("<FoodRecipe>k__BackingField");
            SerializedProperty intData = action.FindPropertyRelative("<IntData>k__BackingField");

            EditorGUILayout.BeginVertical(_actionBoxStyle);

            // ── Header строки ────────────────
            EditorGUILayout.BeginHorizontal();
            action.isExpanded = EditorGUILayout.Foldout(action.isExpanded,
                $"Action {i}  —  {(StateType)stateType.enumValueIndex}",
                true);
            GUILayout.FlexibleSpace();
            GUI.color = new Color(1f, 0.4f, 0.4f);
            if (GUILayout.Button("✕", GUILayout.Width(26), GUILayout.Height(16)))
            {
                actionsList.DeleteArrayElementAtIndex(i);
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
            EditorGUILayout.PropertyField(dialogue, new GUIContent("💬 Dialogue Tree"));

            // StateType — всегда
            EditorGUILayout.PropertyField(stateType, new GUIContent("🔁 State"));

            StateType currentState = (StateType)stateType.enumValueIndex;

            // IntData — теперь всегда
            EditorGUILayout.PropertyField(intData, new GUIContent("🔢 Int Data"));

            // WalkType — только Walk
            if (currentState == StateType.Walk)
            {
                EditorGUILayout.PropertyField(walkType, new GUIContent("🚶 Walk Type"));
            }

            // FoodRecipe — только MakeOrder
            if (currentState == StateType.MakeOrder)
                EditorGUILayout.PropertyField(foodRecipe, new GUIContent("🍽 Food Recipe"));
            else
                EditorGUILayout.PropertyField(foodRecipe, new GUIContent("🍽 Food Recipe (ignored)"));

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

        if (actionsList.arraySize == 0)
        {
            EditorGUILayout.HelpBox("No NPC actions. Press ＋ to add.", MessageType.Info);
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