using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;

namespace LWJ.UnityEditor
{
    public class EditorGUIScopes
    {
        public class IndentLevel : IDisposable
        {
            public IndentLevel()
            {
                EditorGUI.indentLevel++;
            }

            public void Dispose()
            {
                EditorGUI.indentLevel--;
            }


        }
    }
}
