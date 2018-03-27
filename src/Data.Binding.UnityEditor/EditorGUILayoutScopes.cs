using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;

namespace LWJ.UnityEditor
{
    public class EditorGUILayoutScopes
    {
        public class IndentLevel : IDisposable
        {

            public static int IndentPixels;

            public IndentLevel()
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.Space(); 
            }

            public void Dispose()
            {
                EditorGUI.indentLevel--;
            }

        }
    }
}
