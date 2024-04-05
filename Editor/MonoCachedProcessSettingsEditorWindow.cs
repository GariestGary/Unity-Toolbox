using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace VolumeBox.Toolbox.Editor
{
    public class MonoCachedProcessSettingsEditorWindow : EditorWindow
    {
        private static MonoCachedProcessSettingsEditorWindow instance;
        private MonoCached m_Target;
        
        public void DrawTarget(MonoCached mono)
        {
            m_Target = mono;

            var inHierarchy = new Toggle("Process If Inactive In Hierarchy");
            var self = new Toggle("Process If Inactive Self");
            var serializedObject = new SerializedObject(m_Target);
            var selfProperty = serializedObject.FindProperty("processIfInactiveSelf");
            var inHierarchyProperty = serializedObject.FindProperty("processIfInactiveInHierarchy");
            inHierarchy.BindProperty(inHierarchyProperty);
            self.BindProperty(selfProperty);
            self.style.flexShrink = new StyleFloat(1);
            rootVisualElement.Add(self);
            rootVisualElement.Add(inHierarchy);
        }

        [MenuItem("CONTEXT/MonoCached/Process Settings")]
        public static void Open(MenuCommand command)
        {
            if(instance != null)
            {
                instance.Close();
            }

            MonoCached mono = command.context as MonoCached;
            var window = GetWindow<MonoCachedProcessSettingsEditorWindow>(mono.GetType().ToString() + " Settings");
            window.maxSize = new UnityEngine.Vector2(300, 100);
            instance = window;
            window.minSize = new UnityEngine.Vector2(300, 100);
            window.DrawTarget(mono);
            window.Show();
        }
    }
}
