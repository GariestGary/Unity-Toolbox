using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using VolumeBox.Toolbox;

[InitializeOnLoad]
public static class CodeGenerator
{
    private static List<string> messageTypes;

    private static int _currentTypesCount;

    [MenuItem("Tools/VolumeBox/Update Messages Types")]
    private static void Update()
    {
        if (Application.isPlaying) return;

        List<Type> types = Assembly.GetAssembly(typeof(Message)).GetTypes().Where(t => t.IsSubclassOf(typeof(Message)) && !t.IsAbstract).ToList();
        List<Type> baseNamespaceTypes = Assembly.GetAssembly(typeof(CodeGenerator)).GetTypes().Where(t => t.IsSubclassOf(typeof(Message)) && !t.IsAbstract).ToList();
        types = types.Concat(baseNamespaceTypes).ToList();

        if(_currentTypesCount == types.Count)
        {
            return;
        }

        if(messageTypes == null)
        {
            messageTypes = new List<string>();
        }

        foreach(var type in types)
        {
            string typeName = type.Name;

            if(messageTypes.Contains(typeName))
            {
                continue;
            }
            else
            {
                messageTypes.Add(typeName);
            }
        }

        WriteCodeFile();

        _currentTypesCount = types.Count;
    }

    private static void WriteCodeFile()
    {

        // the path we want to write to
        string path = string.Concat(Application.dataPath,
            Path.DirectorySeparatorChar,
            "MessagesContainer.cs");

        try
        {
            // opens the file if it already exists, creates it otherwise
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("// ----- AUTO GENERATED CODE ----- //");
            builder.AppendLine("using UnityEngine;");
            builder.AppendLine("using VolumeBox.Toolbox;");
            builder.AppendLine("[System.Serializable]");
            builder.AppendLine("public partial class MessagesContainer");
            builder.AppendLine("{");
            foreach (string message in messageTypes)
            {
                builder.AppendLine($"\t[SerializeField] public {message} {message} = new {message}();");
            }

            builder.AppendLine("}");

            File.WriteAllText(path, builder.ToString());
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);

            // if we have an error, it is certainly that the file is screwed up. Delete to be save
            if (File.Exists(path) == true)
                File.Delete(path);
        }

        AssetDatabase.Refresh();
    }
}
