using System;
using System.Collections;
using System.Collections.Generic;
using Codice.CM.Common.Serialization;
using DS.ScriptableObjects;
using KKD;
using QS.Elements;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEditor.VersionControl;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace QS.Utilities
{
    public static class QSElementUtility
    {
        public static Button CreateButton(string text, Action onClick = null)
        {
            Button button = new Button(onClick)
            {
                text = text
            };
            return button;
        }
        public static Foldout CreateFoldout(string title, bool collapsed = false)
        {
            Foldout foldout = new Foldout()
            {
                text = title,
                value = !collapsed

            };
            return foldout;
        }
        public static Port CreatePort(this QSNode node, string portName = "",
            Orientation orientation = Orientation.Horizontal, Direction direction = Direction.Output, Port.Capacity capacity = Port.Capacity.Single)
        {
            Port port = node.InstantiatePort(orientation, direction, capacity, typeof(bool));
            port.portName = portName;
            return port;
        }
        
        
        public static TextField CreateTextField(string value = null, string label = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            TextField textField = new TextField()
            {
                value = value,
                label = label
            };

            if (onValueChanged != null)
            {
                textField.RegisterValueChangedCallback(onValueChanged);
            }

            return textField;
        }

        public static TextField CreateTextArea(string value = null, string label = null,
            EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            TextField textArea = CreateTextField(value, label, onValueChanged);
            textArea.multiline = true;
            return textArea;
        }

        public static FloatField CreateFloatField(float value = 0, string label = null,
            EventCallback<ChangeEvent<float>> onValueChanged = null)
        {
            FloatField floatField = new FloatField()
            {
                value = value,
                label = label,
            };
            if (onValueChanged != null)
            {
                floatField.RegisterValueChangedCallback(onValueChanged);
            }

            return floatField;
        }
        
        public static ObjectField CreateObjectField(EventCallback<ChangeEvent<Object>> onValueChanged = null)
        {
            //QuestHandler handler;
            
            //Type nodeType = Type.GetType($"DS.Elements.DS{dialogueType}Node");
            ObjectField objectField = new ObjectField();

            if (onValueChanged != null)
            {
                objectField.RegisterValueChangedCallback(onValueChanged);
            }
            return objectField;
            //field.value = AssetUtility.LoadAssetBundle(name, path);
        }

        /*public static ObjectField CreateQSActivatorNodeField()
        {
            
        }*/
        
        public static ObjectField CreateQuestHandlerField(EventCallback<ChangeEvent<Object>> onValueChanged = null)
        {
            //QuestHandler handler;
            
            //Type nodeType = Type.GetType($"DS.Elements.DS{dialogueType}Node");
            ObjectField objectField = new ObjectField();
            Type type = typeof(QuestHandler);//Type.GetType("KKD.QuestHandler");
            objectField.objectType = type;

            if (onValueChanged != null)
            {
                objectField.RegisterValueChangedCallback(onValueChanged);
            }
            return objectField;
            //field.value = AssetUtility.LoadAssetBundle(name, path);
        }
        
        public static ObjectField CreateDialogueGraphField(EventCallback<ChangeEvent<Object>> onValueChanged = null)
        {
            //QuestHandler handler;
            
            //Type nodeType = Type.GetType($"DS.Elements.DS{dialogueType}Node");
            ObjectField objectField = new ObjectField();
            Type type = typeof(DSDialogueContainerSO);//Type.GetType("KKD.QuestHandler");
            objectField.objectType = type;

            if (onValueChanged != null)
            {
                objectField.RegisterValueChangedCallback(onValueChanged);
            }
            return objectField;
            //field.value = AssetUtility.LoadAssetBundle(name, path);
        }
        
        
        
        public static PropertyField CreatePropertyField(SerializedProperty serializedProperty, EventCallback<SerializedPropertyChangeEvent> onValueChanged = null)
        {
            PropertyField propertyField = new PropertyField();
            if (onValueChanged != null)
            {
                propertyField.RegisterValueChangeCallback(onValueChanged);
            }
            return propertyField;
            //field.value = AssetUtility.LoadAssetBundle(name, path);
        }
    }

}
