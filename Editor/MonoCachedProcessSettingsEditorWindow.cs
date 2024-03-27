using Alchemy.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace VolumeBox.Toolbox.Editor
{
    public class MonoCachedProcessSettingsEditorWindow : AlchemyEditorWindow
    {
        private static MonoCachedProcessSettingsEditorWindow instance;

        private MonoCached m_Target;

        public void SetTarget(MonoCached mono)
        {
            m_Target = mono;
        }

        protected override void CreateGUI()
        {
            var inHierarchy = new Toggle("Process If Inactive In Hierarchy");
            //var inHierarchyProperty = new SerializedProperty();
            //inHierarchy.BindProperty();
            rootVisualElement.Add(inHierarchy);
        }

        [MenuItem("CONTEXT/MonoCached/Process Settings")]
        static void Open(MenuCommand command)
        {
            if(instance != null)
            {
                instance.Close();
            }

            MonoCached mono = command.context as MonoCached;
            var window = GetWindow<MonoCachedProcessSettingsEditorWindow>(mono.GetType().ToString() + " Process Settings");
            window.SetTarget(mono);
            window.Show();
            instance = window;
        }
    }
}
