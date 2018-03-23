/**************************************************************
 *  Filename:    EditorGUIHelper.cs
 *  Description: LWJ.UnityEditor ClassFile
 *  @author:     WenJie Liu
 *  @version     2017/3/9
 **************************************************************/
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;


namespace LWJ.UnityEditor
{

    public static partial class EditorGUIHelper
    {

        static GUIStyle whiteTextureStyle;
        static Dictionary<Type, GUIContent[]> propertys;

        private static Dictionary<string, GUIContent> contents;


        public static readonly GUIContent NullContent = new GUIContent("<none>");

        static EditorGUIHelper()
        {
            contents = new Dictionary<string, GUIContent>();
        }

        public static readonly GUIContent[] EmptyArray = new GUIContent[0];

        public static GUIContent InfoIcon { get => GetIconContent(ContentNames.InfoIcon); }

        public static GUIContent WarningIcon { get => GetIconContent(ContentNames.WarningIcon); }

        public static GUIContent ErrorIcon { get => GetIconContent(ContentNames.ErrorIcon); }

        public static GUIContent IconToolbarMinus
        {
            get =>/* new GUIContent(EditorGUIUtility.IconContent("Toolbar Minus")); */GetIconContent(ContentNames.IconToolbarMinus);
        }

        public static GUIStyle WhiteTextureStyle
        {
            get
            {
                if (whiteTextureStyle == null)
                {
                    whiteTextureStyle = new GUIStyle();
                    whiteTextureStyle.normal.background = EditorGUIUtility.whiteTexture;
                }
                return whiteTextureStyle;
            }
        }

        public static GUIContent GetIconContent(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            GUIContent content;
            if (!contents.TryGetValue(name, out content))
            {
                content = EditorGUIUtility.IconContent(name);
                if (content == null) throw new Exception("GUIＣontent Null <{0}>".FormatArgs(name));
                contents[name] = content;
            }

            return content;
        }

        public static GUIContent GetContentByFileName(string fileName)
        {
            string extension = Path.GetExtension(fileName);
            GUIContent icon = null;
            string name;
            switch (extension)
            {
                case ".js":
                    name = ContentNames.JsFileIcon;
                    break;
                case ".cs":
                    name = ContentNames.CsFileIcon;
                    break;
                case ".boo":
                    name = ContentNames.BooFileIcon;
                    break;
                case ".shader":
                    name = ContentNames.ShaderFileIcon;
                    break;
                default:
                    name = ContentNames.TextFileIcon;
                    break;
            }
            icon = GetIconContent(name);
            return icon;
        }

        public static GUIContent AssetPathToContent(string assetPath)
        {
            var obj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object));
            if (obj == null)
                return new GUIContent("");
            return EditorGUIUtility.ObjectContent(obj, obj.GetType());
        }



        public static T CreateAsset<T>(string assetName)
            where T : ScriptableObject
        {
            if (assetName == null) throw new ArgumentNullException(nameof(assetName));

            T obj = ScriptableObject.CreateInstance<T>();
            if (obj == null)
                throw new Exception(string.Format("ScriptableObject.CreateInstance  null ，type:{0} asset name:{1}", typeof(T), assetName));

            ProjectWindowUtil.CreateAsset(obj, assetName);

            return obj;
        }

        public static void DrawIconFromAssetPath(string assetPath, ScaleMode scaleMode = ScaleMode.ScaleToFit)
        {
            Vector2 iconSize = EditorGUIUtility.GetIconSize();

            Rect rect = GUILayoutUtility.GetRect(iconSize.x, iconSize.y, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));

            var icon = AssetPathToContent(assetPath).image;
            if (icon != null)
                GUI.DrawTexture(rect, icon, scaleMode);
        }
        static GUIContent[] GetPropertyContents(Type type)
        {
            if (type == null)
                return EmptyArray;

            if (propertys == null)
                propertys = new Dictionary<Type, GUIContent[]>();

            return propertys.GetOrCreateValue(type, (t) =>
            {
                return (from o in ((from o in t.GetProperties()
                                    select o.Name)/*.Concat((from o in t.GetFields()
                                                            select o.Name))*/)
                        orderby o
                        select new GUIContent(o)).ToArray();
            });
        }

        public static string PropertyNamesField(string label, Type type, string propertyName, params GUILayoutOption[] options)
        {

            return PropertyNamesField(true, label, type, propertyName, options);
        }
        public static string PropertyNamesField(Type type, string propertyName, params GUILayoutOption[] options)
        {
            return PropertyNamesField(false, null, type, propertyName, options);
        }
        private static string PropertyNamesField(bool hasLabel, string label, Type type, string propertyName, params GUILayoutOption[] options)
        {

            using (new GUILayout.HorizontalScope())
            {
                if (hasLabel)
                    propertyName = EditorGUILayout.TextField(label, propertyName);
                else
                    propertyName = EditorGUILayout.TextField(propertyName);
                GUIContent[] contents = GetPropertyContents(type);
                int selectedIndex = -1;
                int width = 30;

                selectedIndex = EditorGUILayout.Popup(selectedIndex, contents, GUILayout.Width(width), GUILayout.MaxWidth(width), GUILayout.ExpandWidth(false));
                if (selectedIndex != -1)
                    propertyName = contents[selectedIndex].text;

            }
            return propertyName;
        }




        public static Object ComponentAndGameObjectPop(string label, Object value, bool allowSetObject, params GUILayoutOption[] options)
        {
            if (allowSetObject)
            {
                using (new GUILayout.HorizontalScope())
                {

                    value = EditorGUILayout.ObjectField(label, value, typeof(Object), true);

                    value = ComponentAndGameObjectPop(value, GUILayout.MaxWidth(120));

                }
            }
            else
            {
                value = ComponentAndGameObjectPop(label, value, GUILayout.MaxWidth(120));
            }
            return value;
        }

        public static Object ComponentAndGameObjectPop(Object value, bool allowSetObject, params GUILayoutOption[] options)
        {
            if (allowSetObject)
            {
                using (new GUILayout.HorizontalScope())
                {

                    value = EditorGUILayout.ObjectField(value, typeof(Object), true);

                    value = ComponentAndGameObjectPop(value, GUILayout.MaxWidth(120));

                }
            }
            else
            {
                value = ComponentAndGameObjectPop(value, GUILayout.MaxWidth(120));
            }
            return value;
        }


        public static Object ComponentAndGameObjectPop(string label, Object value, params GUILayoutOption[] options)
        {
            return ComponentAndGameObjectPop(true, label, value, options);
        }

        public static Object ComponentAndGameObjectPop(Object value, params GUILayoutOption[] options)
        {
            return ComponentAndGameObjectPop(false, null, value, options);
        }
        private static Object ComponentAndGameObjectPop(bool hasLabel, string label, Object value, params GUILayoutOption[] options)
        {
            int selectedIndex = -1;
            Object[] values = null;
            string[] displayNames = null;

            GameObject go = null;

            if (value != null)
            {
                if (value is Component)
                {
                    go = ((Component)value).gameObject;
                }
                else
                {
                    go = value as GameObject;
                }
            }

            if (go != null)
            {
                values = go.GetComponents<Component>().Union(new Object[] { go }).ToArray();
                displayNames = values.Select(o => o.GetType().Name).ToArray();
                if (value != null)
                    selectedIndex = displayNames.IndexOf(value.GetType().Name);
            }
            else
            {
                displayNames = new string[] { };

            }

            int newIndex;
            if (hasLabel)
                newIndex = EditorGUILayout.Popup(label, selectedIndex, displayNames, options);
            else
                newIndex = EditorGUILayout.Popup(selectedIndex, displayNames, options);
            if (newIndex != selectedIndex)
            {
                value = values[newIndex];
            }
            return value;

        }
        public static Component ComponentPop(string label, GameObject go, Component value, bool allowSetObject, params GUILayoutOption[] options)
        {
            return ComponentPop(true, label, go, value, allowSetObject, options);
        }
        public static Component ComponentPop(GameObject go, Component value, bool allowSetObject, params GUILayoutOption[] options)
        {
            return ComponentPop(false, null, go, value, allowSetObject, options);
        }
        private static Component ComponentPop(bool hasLabel, string label, GameObject go, Component value, bool allowSetObject, params GUILayoutOption[] options)
        {
            if (allowSetObject)
            {
                using (new GUILayout.HorizontalScope())
                {

                    GameObject newGo = null;
                    if (hasLabel)
                        newGo = EditorGUILayout.ObjectField(label, go, typeof(GameObject), true) as GameObject;
                    else
                        newGo = EditorGUILayout.ObjectField(go, typeof(GameObject), true) as GameObject;
                    if (newGo != go)
                    {
                        go = newGo;
                        if (go != null)
                            value = go.GetComponent<Component>();
                        else
                            value = null;
                    }
                    value = ComponentPop(go, value, GUILayout.MaxWidth(120));

                }
            }
            else
            {
                if (hasLabel)
                    value = ComponentPop(label, go, value, options);
                else
                    value = ComponentPop(go, value, options);
            }
            return value;
        }

        public static Component ComponentPop(string label, GameObject go, Component value, params GUILayoutOption[] options)
        {
            return ComponentPop(true, label, go, value, options);
        }
        public static Component ComponentPop(GameObject go, Component value, params GUILayoutOption[] options)
        {
            return ComponentPop(false, null, go, value, options);
        }

        private static Component ComponentPop(bool hasLabel, string label, GameObject go, Component value, params GUILayoutOption[] options)
        {
            int selectedIndex = -1;
            Component[] values = null;
            string[] displayNames = null;


            if (go != null)
            {
                values = go.GetComponents<Component>();
                displayNames = values.Select(o => o.GetType().Name).OrderBy(o => o).ToArray();
                selectedIndex = values.IndexOf(value);
            }
            else
            {
                displayNames = new string[] { "" };
                selectedIndex = 0;
            }

            int newIndex;
            if (hasLabel)
                newIndex = EditorGUILayout.Popup(label, selectedIndex, displayNames, options);
            else
                newIndex = EditorGUILayout.Popup(selectedIndex, displayNames, options);
            if (newIndex != selectedIndex)
            {
                value = values[newIndex];

            }
            return value;
        }


        public class ContentNames
        {
            public const string InfoIcon = "console.infoicon";
            public const string WarningIcon = "console.warnicon";
            public const string ErrorIcon = "console.erroricon";
            public const string IconToolbarMinus = "Toolbar Minus";

            /// <summary>
            /// .js
            /// </summary>
            public const string JsFileIcon = "js Script Icon";

            /// <summary>
            /// .cs
            /// </summary>
            public const string CsFileIcon = "cs Script Icon";

            /// <summary>
            /// .boo
            /// </summary>
            public const string BooFileIcon = "boo Script Icon";
            /// <summary>
            ///  .shader
            /// </summary>
            public const string ShaderFileIcon = "Shader Icon";
            /// <summary>
            /// .txt
            /// </summary>
            public const string TextFileIcon = "TextAsset Icon";
        }

    }


}