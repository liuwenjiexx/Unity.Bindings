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
using LWJ.Unity;

namespace LWJ.UnityEditor
{

    public static partial class EditorGUIHelper
    {

        static GUIStyle whiteTextureStyle;
        static Dictionary<Type, GUIContent[]> propertys;
        static Dictionary<Type, GUIContent[]> type_members;

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
        static GUIContent[] GetPropertyContents(Type type, bool canWrite = false)
        {
            if (type == null)
                return EmptyArray;

            if (propertys == null)
                propertys = new Dictionary<Type, GUIContent[]>();

            return propertys.GetOrCreateValue(type, (t) =>
            {
                return (from o in ((from o in t.GetProperties()
                                    orderby o.Name
                                    select string.Format("{0}", o.Name, o.PropertyType.Name))/*.Concat((from o in t.GetFields()
                                                            select o.Name))*/)
                        select new GUIContent(o)).ToArray();
            });
        }
        static GUIContent[] GetWritablePropertyContents(Type type)
        {
            if (type == null)
                return EmptyArray;

            if (type_members == null)
                type_members = new Dictionary<Type, GUIContent[]>();

            return type_members.GetOrCreateValue(type, (t) =>
            {
                return (from o in ((from o in t.GetProperties()
                                    where o.CanWrite
                                    orderby o.Name
                                    select string.Format("{0}({1})", o.Name, o.PropertyType.Name))/*.Concat((from o in t.GetFields()
                                                            select o.Name))*/)
                        select new GUIContent(o)).ToArray();
            });
        }

        public static void MemberPopup(GUIContent label, Type type, MemberPopupFlags memberPopupFlags, SerializedProperty prop)
        {
            prop.stringValue = MemberPopup(label, type, memberPopupFlags, prop.stringValue);
        }


        public static string MemberPopup(GUIContent label, Type type, MemberPopupFlags memberPopupFlags, string memberName)
        {
            using (new GUILayout.VerticalScope()) { }
            Rect rect = GUILayoutUtility.GetLastRect();
            float height = EditorGUI.GetPropertyHeight(SerializedPropertyType.String, label);
            rect = GUILayoutUtility.GetRect(0, rect.width, height, height);

            return MemberPopup(rect, label, type, memberPopupFlags, memberName);
        }

        public static void MemberPopup(Rect rect, GUIContent label, Type type, MemberPopupFlags memberPopupFlags, SerializedProperty prop)
        {
            prop.stringValue = MemberPopup(rect, label, type, memberPopupFlags, prop.stringValue);
        }

        public static string MemberPopup(Rect rect, GUIContent label, Type type, MemberPopupFlags memberPopupFlags, string memberName)
        {
            int w = (int)(rect.width * 0.3f);
            memberName = EditorGUI.TextField(new Rect(rect.x, rect.y, rect.width - w, EditorGUI.GetPropertyHeight(SerializedPropertyType.String, label)), label, memberName);
            GUIContent[] contents = GetPropertyContents(type);
            int selectedIndex = -1;

            selectedIndex = EditorGUI.Popup(new Rect(rect.x + rect.width - w, rect.y, w, EditorGUI.GetPropertyHeight(SerializedPropertyType.String, label)), selectedIndex, contents);
            if (selectedIndex != -1)
                memberName = contents[selectedIndex].text;

            return memberName;
        }


        public static void ComponentPopup(GUIContent label, SerializedProperty prop)
        {
            prop.objectReferenceValue = ComponentPopup(label, prop.objectReferenceValue);
        }

        public static UnityEngine.Object ComponentPopup(GUIContent label, UnityEngine.Object obj)
        {

            using (new GUILayout.VerticalScope()) { }
            Rect rect = GUILayoutUtility.GetLastRect();
            float height = EditorGUI.GetPropertyHeight(SerializedPropertyType.ObjectReference, label);
            rect = GUILayoutUtility.GetRect(0, rect.width, height, height);
            return ComponentPopup(rect, label, obj);
        }

        public static void ComponentPopup(Rect position, GUIContent label, SerializedProperty prop)
        {

            //position= EditorGUI.PrefixLabel(position, label);
            //int w = (int)(position.width * 0.5);
            prop.objectReferenceValue = ComponentPopup(position, label, prop.objectReferenceValue);
        }

        public static Component ComponentPopup(Rect rect, GUIContent label, UnityEngine.Object value)
        {

            int w = (int)(rect.width * 0.3);
            value = EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width - w, rect.height), label, value, typeof(Component), false);

            rect = new Rect(rect.x + rect.width - w, rect.y, w, rect.height);

            int selectedIndex = -1;
            Component[] values = null;
            string[] displayNames = null;

            if (value)
            {
                if (value is GameObject)
                {
                    values = ((GameObject)value).GetComponents<Component>();
                }
                else
                {
                    value = value as Component;
                    if (value)
                        values = ((Component)value).GetComponents<Component>();
                    else
                        values = new Component[0];
                }
                values = (from o in values
                          orderby o.GetType().Name
                          select o).ToArray();
                displayNames = (from o in values
                                select o.GetType().Name).ToArray();
                selectedIndex = values.IndexOf(value);
            }
            else
            {
                displayNames = new string[] { "" };
                selectedIndex = 0;
            }

            int newIndex;


            newIndex = EditorGUI.Popup(new Rect(rect.xMin, rect.y, rect.width, (int)EditorGUI.GetPropertyHeight(SerializedPropertyType.ObjectReference, GUIContent.none)), selectedIndex, displayNames);

            if (newIndex != selectedIndex)
            {
                if (selectedIndex >= 0)
                    value = values[newIndex];
            }

            if (value)
            {
                if (!(value is Component) && values != null && values.Length > 0)
                {
                    value = values[0];
                }
            }
            return value as Component;
        }


        //public static Object ComponentAndGameObjectPop(string label, GameObject go, Object value, bool allowSetObject, params GUILayoutOption[] options)
        //{
        //    if (allowSetObject)
        //    {
        //        using (new GUILayout.HorizontalScope())
        //        {

        //            value = EditorGUILayout.ObjectField(label, value, typeof(Object), true);

        //            value = ComponentAndGameObjectPop(go, value, GUILayout.MaxWidth(120));

        //        }
        //    }
        //    else
        //    {
        //        value = ComponentAndGameObjectPop(label, go, value, GUILayout.MaxWidth(120));
        //    }
        //    return value;
        //}

        //public static Object ComponentAndGameObjectPop(GameObject go, Object value, bool allowSetObject, params GUILayoutOption[] options)
        //{
        //    if (allowSetObject)
        //    {
        //        using (new GUILayout.HorizontalScope())
        //        {

        //            value = EditorGUILayout.ObjectField(value, typeof(Object), true);

        //            value = ComponentAndGameObjectPop(go, value, GUILayout.MaxWidth(120));

        //        }
        //    }
        //    else
        //    {
        //        value = ComponentAndGameObjectPop(go, value, GUILayout.MaxWidth(120));
        //    }
        //    return value;
        //}


        //public static Object ComponentAndGameObjectPop(string label, GameObject go, Object value, params GUILayoutOption[] options)
        //{
        //    return ComponentAndGameObjectPop(true, label, go, value, options);
        //}

        //public static Object ComponentAndGameObjectPop(GameObject go, Object value, params GUILayoutOption[] options)
        //{
        //    return ComponentAndGameObjectPop(false, null, go, value, options);
        //}
        //private static Object ComponentAndGameObjectPop(bool hasLabel, string label, GameObject go1, Object value, params GUILayoutOption[] options)
        //{
        //    int selectedIndex = -1;
        //    Object[] values = null;
        //    string[] displayNames = null;

        //    GameObject go = null;

        //    if (value != null)
        //    {
        //        if (value is Component)
        //        {
        //            go = ((Component)value).gameObject;
        //        }
        //        else
        //        {
        //            go = value as GameObject;
        //        }
        //    }

        //    if (go != null)
        //    {
        //        values = go.GetComponents<Component>().Union(new Object[] { go }).OrderBy(o => o.GetType().Name).ToArray();
        //        displayNames = values.Select(o => o.GetType().Name).ToArray();
        //        if (value != null)
        //        {
        //            for (int i = 0; i < values.Length; i++)
        //            {
        //                if (values[i] == value)
        //                {
        //                    selectedIndex = i;
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        displayNames = new string[] { };

        //    }

        //    int newIndex;
        //    if (hasLabel)
        //        newIndex = EditorGUILayout.Popup(label, selectedIndex, displayNames, options);
        //    else
        //        newIndex = EditorGUILayout.Popup(selectedIndex, displayNames, options);

        //    if (newIndex != selectedIndex)
        //    {
        //        value = values[newIndex];
        //    }
        //    return value;

        //} 




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