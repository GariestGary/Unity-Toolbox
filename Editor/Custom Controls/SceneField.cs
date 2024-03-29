using System.Collections;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Linq;
using UnityEditor.UIElements;
using Unity.Plastic.Newtonsoft.Json.Linq;

namespace VolumeBox.Toolbox.Editor
{
    public class SceneField: BindableElement, INotifyValueChanged<string>, IBindable
    {
        private string m_CurrentSceneName;

        public SceneField()
        {
            var buttonContainer = new IMGUIContainer(() =>
            {
                var rect = GUILayoutUtility.GetRect(new GUIContent(""), EditorStyles.largeLabel);
                if (GUI.Button(rect, new GUIContent(m_CurrentSceneName.IsValuable() ? m_CurrentSceneName : "Select scene..."), EditorStyles.miniPullDown))
                {
                    var dropdown = new SceneDropdown(new AdvancedDropdownState());
                    dropdown.OnItemSelected += item =>
                    {
                        value = item.SceneName;
                    };
                    dropdown.Show(rect);
                }
            });

            Add(buttonContainer);
        }

        public string value 
        {
            get => m_CurrentSceneName;
            set
            {
                if(m_CurrentSceneName != value)
                {
                    using (ChangeEvent<string> evt = ChangeEvent<string>.GetPooled(m_CurrentSceneName, value))
                    {
                        evt.target = this;
                        SetValueWithoutNotify(value);
                        SendEvent(evt);
                    }
                }
            }
        }

        public void SetValueWithoutNotify(string newValue)
        {
            m_CurrentSceneName = newValue;
        }

        public new class UxmlFactory: UxmlFactory<SceneField> { }

        public new class UxmlTraits: BaseField<Enum>.UxmlTraits
        {
            private UxmlStringAttributeDescription m_Value = new UxmlStringAttributeDescription { defaultValue = string.Empty };
#pragma warning restore 414

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                SceneField sceneField = (SceneField)ve;
                sceneField.SetValueWithoutNotify(m_Value.GetValueFromBag(bag, cc));
            }
        }
    }
}
