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
            private int indentLevel;

            public IndentLevel()
            {
                indentLevel = EditorGUI.indentLevel++;
                //EditorGUILayout.Space();
            }

            public IndentLevel(int indentLevel)
            {
                this.indentLevel = indentLevel;
                EditorGUI.indentLevel = indentLevel;
            }

            public void Dispose()
            {
                EditorGUI.indentLevel = indentLevel;
            }

        }
    }
}
