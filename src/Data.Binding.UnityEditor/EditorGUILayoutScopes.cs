using LWJ.Unity;
using System;
using UnityEditor;
using UnityEngine;

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
