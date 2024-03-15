using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace VolumeBox.Toolbox
{
    public static class ToolboxExtensions
    {
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

        /// <summary>
        /// Cancels and disposes current TokenSource if it exists and returns new one of same type
        /// </summary>
        public static T CancelAndCreate<T>(this T source) where T : CancellationTokenSource
        {
            source?.Cancel();
            source?.Dispose();
            return default(T);
        }
        
        /// <summary>
        /// Copies file from one path to other (Currently used by internal logic but i think you can use it)
        /// </summary>
        public static async Task CopyFileAsync(string sourceFile, string destinationFile)
        {
            await using var sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan);
            await using var destinationStream = new FileStream(destinationFile, FileMode.CreateNew, FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan);
            await sourceStream.CopyToAsync(destinationStream);
        }

        /// <summary>
        /// Trims path restricted characters (does not changes source string)
        /// </summary>
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

        /// <summary>
        /// Enables or disables interactable and blockRaycasts flag of CanvasGroup
        /// </summary>
        public static void SetInteractions(this CanvasGroup group, bool value)
        {
            group.interactable = value;
            group.blocksRaycasts = value;
        }

        /// <summary>
        /// Checks if string has actual value (string consisting only of whitespaces will return false)
        /// </summary>
        public static bool IsValuable(this string text)
        {
            return !string.IsNullOrEmpty(text) && !string.IsNullOrWhiteSpace(text);
        }


        /// <summary>
        /// Shuffles the elements in the specified list randomly.
        /// </summary>
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

        /// <summary>
        /// Splits the input list into multiple sub-lists, each containing approximately the same number of elements.
        /// </summary>
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