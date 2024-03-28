using System.Collections;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Linq;
using UnityEditor.UIElements;

namespace VolumeBox.Toolbox.Editor
{
    public class SceneField: BaseField<string>, INotifyValueChanged<string>
    {
        public string value
        {
            get
            {
                return m_CurrentSceneName;
            }
            set
            {
                if (value == this.value)
                    return;

                var previous = this.value;
                SetValueWithoutNotify(value);

                m_CurrentSceneName = value;

                using (var evt = ChangeEvent<string>.GetPooled(previous, value))
                {
                    evt.target = this;
                    SendEvent(evt);
                }
            }
        }

        private string m_CurrentSceneName;

        public SceneField(string label, VisualElement visualInput) : base(label, visualInput)
        {
            var buttonContainer = new IMGUIContainer(() =>
            {
                var rect = GUILayoutUtility.GetRect(new GUIContent(""), EditorStyles.largeLabel);
                if (GUI.Button(rect, new GUIContent(m_CurrentSceneName.IsValuable() ? m_CurrentSceneName : "Select scene..."), EditorStyles.miniPullDown))
                {
                    var dropdown = new SceneDropdown(new AdvancedDropdownState());
                    dropdown.Show(rect);
                }
            });

            Add(buttonContainer);
        }

        public new class UxmlFactory: UxmlFactory<SceneField> { }
        public new class UxmlTraits: BindableElement.UxmlTraits { }

        public SceneField() : base(string.Empty, null)
        {

        }

        public void SetValueWithoutNotify(string newValue)
        {
            m_CurrentSceneName = newValue;
        }
    }
}
