using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

public class PlayModeStateCheckerWindow : EditorWindow
{

    public ChoiceDataScriptableObject choiceData;
    [MenuItem("Window/MyEditorWindow")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(PlayModeStateCheckerWindow)).Show();
    }
    
    
    public static ObjectField CreateChoiceDataField(EventCallback<ChangeEvent<Object>> onValueChanged = null)
    {
        //QuestHandler handler;
            
        //Type nodeType = Type.GetType($"DS.Elements.DS{dialogueType}Node");
        ObjectField objectField = new ObjectField();
        Type type = typeof(ChoiceDataScriptableObject);//Type.GetType("KKD.QuestHandler");
        objectField.objectType = type;

        if (onValueChanged != null)
        {
            objectField.RegisterValueChangedCallback(onValueChanged);
        }
        return objectField;
        //field.Value = AssetUtility.LoadAssetBundle(name, path);
    }
    
    
    public static PropertyField CreatePropertyField(SerializedProperty serializedProperty, EventCallback<SerializedPropertyChangeEvent> onValueChanged = null)
    {
        PropertyField propertyField = new PropertyField();
        if (onValueChanged != null)
        {
            propertyField.RegisterValueChangeCallback(onValueChanged);
        }
        return propertyField;
        //field.Value = AssetUtility.LoadAssetBundle(name, path);
    }

    public void DrawBoolProperty(VisualElement targetElement)
    {
        VisualElement objectElement = new VisualElement();
        VisualElement propertyElement = new VisualElement();
        ObjectField testValueObjectField = CreateChoiceDataField(callback =>
        {
            ObjectField target = (ObjectField) callback.target;
            choiceData = (ChoiceDataScriptableObject) target.value;
            if (choiceData != null)
            {
                SerializedObject choiceDataSerialized = new SerializedObject(choiceData);
                SerializedProperty resetRoomProgressionProperty =
                    choiceDataSerialized.FindProperty("resetRoomProgression");
                PropertyField testValuePropertyField = CreatePropertyField(resetRoomProgressionProperty, callback =>
                {
                    PropertyField target = (PropertyField)callback.target;
                });
                
                propertyElement.Add(testValuePropertyField);
                //resetRoomProgressionProperty.boolValue
            }
        });
        objectElement.Add(testValueObjectField);
        objectElement.Add(propertyElement);
        targetElement.Add(objectElement);

    }
    
    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy
        Label label = new Label("Hello World!");
        root.Add(label);
        //SerializedObject serializedObject = new SerializedObject();

        // Create button
        Button button = new Button();
        button.name = "button";
        button.text = "Button";
        root.Add(button);

        // Create toggle
        Toggle toggle = new Toggle();
        toggle.name = "toggle";
        toggle.label = "Toggle";
        root.Add(toggle);

        DrawBoolProperty(root);
    }
    
    private void OnEnable()
    {
        Debug.Log("OnEnable: registering playModeStateChanged");
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }
 
    private void OnDisable()
    {
        Debug.Log("OnDisable: unregistering playModeStateChanged");
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }
 
    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        Debug.Log("PlayModeStateChanged: " + state);
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            
        }
    }
}
