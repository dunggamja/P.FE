using UnityEngine;
using UnityEditor;
using System.Reflection;
using UnityEngine.InputSystem;
using System.Linq;
using System;
using UnityEngine.Events;

[CustomEditor(typeof(InputManager))]
public class InputManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Auto Bind Input"))
        {
            AutoBindInput();
        }
        
    }

    void AutoBindInput()
    {
        var input_manager = target as InputManager;
        if (input_manager == null)
            return;

        var player_input = input_manager.GetComponent<PlayerInput>();
        if (player_input == null)
            return;

        var serialized_player_input = new SerializedObject(player_input);
        var serialized_property     = serialized_player_input.FindProperty("m_ActionEvents");

        MethodInfo[] methods_array = input_manager
        .GetType()
        .GetMethods( BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        foreach (var method in methods_array)
        {
            var attributes = method.GetCustomAttribute<InputBindingAttribute>();
            if (attributes == null)
                continue; 

            for (int i = 0; i < serialized_property.arraySize; i++)
            {
                var event_property      = serialized_property.GetArrayElementAtIndex(i);
                var event_name_property = event_property.FindPropertyRelative("m_ActionName");

                var event_name = event_name_property.stringValue;
                var bracket_index = event_name.IndexOf('[');
                if (bracket_index != -1)
                    event_name = event_name.Substring(0, bracket_index);

                event_name = event_name.Split('/').Last();

                Debug.Log($"{event_name_property.stringValue}:{event_name}");

                if (event_name != attributes.ActionName)
                    continue;

                Debug.Log($"{event_name_property.stringValue}:{event_name}:{attributes.ActionName}");

                var call_property       = event_property.FindPropertyRelative("m_PersistentCalls");
                var call_array_property = call_property.FindPropertyRelative("m_Calls");
                
                // 기존 이벤트 제거
                call_array_property.ClearArray();

                // 새 이벤트 추가.
                call_array_property.InsertArrayElementAtIndex(0);
                var call_element_property = call_array_property.GetArrayElementAtIndex(0);

                // 생각보다 초 하드코딩이구먼...
                call_element_property.FindPropertyRelative("m_Target").objectReferenceValue = input_manager;
                call_element_property.FindPropertyRelative("m_MethodName").stringValue = method.Name;
                call_element_property.FindPropertyRelative("m_Mode").enumValueIndex = (int)PersistentListenerMode.EventDefined;
                call_element_property.FindPropertyRelative("m_CallState").enumValueIndex = (int)UnityEventCallState.RuntimeOnly;
                call_element_property.FindPropertyRelative("m_Arguments.m_ObjectArgumentAssemblyTypeName").stringValue = "UnityEngine.InputSystem.InputAction+CallbackContext, Unity.InputSystem";
            }
        }

        serialized_player_input.ApplyModifiedProperties();
        EditorUtility.SetDirty(player_input);

        // 변경사항을 즉시 저장
        // AssetDatabase.SaveAssets();
    }
    
}
