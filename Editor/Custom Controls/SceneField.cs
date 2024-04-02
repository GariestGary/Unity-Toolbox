using UnityEditor.IMGUI.Controls;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System;

namespace VolumeBox.Toolbox.Editor
{
    public class SceneField: BindableElement, INotifyValueChanged<string>, IBindable
    {
        private string m_CurrentSceneName;

        public SceneField()
        {
            DrawGUI(string.Empty);
        }

        public SceneField(string label = "")
        {
            DrawGUI(label);
        }

        private void DrawGUI(string label = "")
        {
            var buttonContainer = new IMGUIContainer(() =>
            {
                var rect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none);

                if(label.IsValuable())
                {
                    rect.x += 3;
                    rect.y += 1;
                    var labelRect = rect;
                    var a = GUI.skin.label.CalcSize(new GUIContent(label));
                    //TODO: figure out how to render field as same width aspect as other
                    labelRect.width = EditorGUIUtility.labelWidth;
                    EditorGUI.LabelField(labelRect, label);
                    rect.x += labelRect.width;
                    rect.width -= labelRect.width;
                }

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

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                SceneField sceneField = (SceneField)ve;
                sceneField.SetValueWithoutNotify(m_Value.GetValueFromBag(bag, cc));
            }
        }
    }
}
