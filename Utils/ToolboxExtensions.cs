using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VolumeBox.Toolbox;
using Random = System.Random;

namespace VolumeBox.Toolbox
{
    public static class ToolboxExtensions
    {
        public static void Resolve(this object mono)
        {
            Resolver.Inject(mono);
        }

        /// <summary>
        /// Gets index of first active toggle (you need to place all toggles as children of group)
        /// </summary>
        public static int GetActiveIndex(this ToggleGroup group)
        {
            var toggles = group.GetComponentsInChildren<Toggle>().ToList();
            var active = group.GetFirstActiveToggle();

            for (int i = 0; i < toggles.Count; i++)
            {
                if (toggles[i] == active)
                {
                    return i;
                }
            }

            return -1;
        }
        
        public static async Task CopyFileAsync(string sourceFile, string destinationFile)
        {
            await using var sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan);
            await using var destinationStream = new FileStream(destinationFile, FileMode.CreateNew, FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan);
            await sourceStream.CopyToAsync(destinationStream);
        }

        public static string FormatStringForSave(this string source)
        {
            string restrictChars = "#<$+%>!`&*'|{?\"=}/:\\@";
            string format = source;
            
            restrictChars.ToList().ForEach(c =>
            {
                format = format.Replace(c.ToString(), "");
            });

            return format;
        }

        public static void SetInteractions(this CanvasGroup group, bool value)
        {
            group.interactable = value;
            group.blocksRaycasts = value;
        }

        public static bool IsValuable(this string text)
        {
            return !string.IsNullOrEmpty(text) && !string.IsNullOrWhiteSpace(text);
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            var rng = new Random();
            var n = list.Count;  
            while (n > 1) {  
                n--;  
                var k = rng.Next(n + 1);  
                (list[k], list[n]) = (list[n], list[k]);
            }  
        }
        
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> list, int parts)
        {
            int i = 0;
            var splits = from item in list
                group item by i++ % parts into part
                select part.AsEnumerable();
            return splits;
        }
    }
}