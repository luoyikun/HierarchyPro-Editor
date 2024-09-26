using Examples.HierarchyPlugin;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using static EMX.HierarchyPlugin.Editor.Tools;

namespace MixFramework
{
    public class EditorFindBind : Editor
    {
        static Type serType2 = typeof(System.NonSerializedAttribute);
        static Type serType = typeof(SerializeField);
        //需要遍历的目标脚本
        static List<Type> listTargetComponent = new List<Type>() {
                typeof(Functions_DrawInHier_Example)
            };

        static Dictionary<int, UnityEngine.Object> dicChildInstanceID = new Dictionary<int, UnityEngine.Object>(8);

        [MenuItem("GameObject/查找绑定关系 %Q", priority = 1)]
        public static void DoEditorFindBind()
        {
            if (Selection.activeGameObject == null)
            {
                return;
            }
            //记录所有组件GetInstaneID
            dicChildInstanceID.Clear();
            int objInstaneID = Selection.activeGameObject.GetInstanceID();
            dicChildInstanceID.Add(objInstaneID, Selection.activeGameObject);
            Component[] comps = Selection.activeGameObject.GetComponents<Component>();
            for (int i = 0; i < comps.Length; i++)
            {
                dicChildInstanceID.Add(comps[i].GetInstanceID(), comps[i]);
            }

            //先上找父物体
            Transform par = Selection.activeGameObject.transform.parent;
            while (par != null)
            {
                DoEditorParentFindBind(par);
                par = par.parent;
            }
            
        }

        static void DoEditorParentFindBind(Transform par)
        {
            Component[] comps = par.GetComponents<Component>();
            for (int i = 0; i < comps.Length; i++)
            {
                Component com = comps[i];
                Type type = com.GetType();
                if (IsTargetComponent(type))
                {
                    //遍历某组件中公开字段
                    FieldInfo[] arrField = type.GetFields(~BindingFlags.Static);

                    for (int filedIdx = 0; filedIdx < arrField.Length; filedIdx++)
                    {
                        var item = arrField[filedIdx];
                        if (item.FieldType.IsPrimitive) continue;
                        if (!item.IsPublic) if (!item.IsDefined(serType, true)) continue;
                        if (item.IsDefined(serType2, true)) continue;
                        //Debug.Log($"字段名字{item.Name}");

                        //某个字段的处理器
                        var fa = EMX.HierarchyPlugin.Editor.Tools.FieldAdapter.TryToCreate(item, true);

                        object res = null;
                        res = fa.GetValue(com);

                        if (res == null)
                        {
                            continue;
                        }

                        if (fa.isEnumerable)
                        {
                            //数组形式
                            var dic = new Dictionary<string, object>();
                            int resItemIdx = 0;
                            foreach (var resItem in (IEnumerable)res)
                            {

                                if (resItem != null)
                                {
                                    UnityEngine.Object objResItem = resItem as UnityEngine.Object;
                                    int instanceID = objResItem.GetInstanceID();
                                    if (dicChildInstanceID.ContainsKey(instanceID))
                                    {
                                        Debug.Log($"物体{par.name}中组件{type.Name}中字段{item.Name}包含{dicChildInstanceID[instanceID]},索引为{resItemIdx}");
                                    }
                                }
                                resItemIdx++;
                            }
                        }
                        else if (fa.isObject)
                        {
                            //字段为UnityEngine.Object
                            if (res != null)
                            {
                                UnityEngine.Object objResItem = res as UnityEngine.Object;
                                int instanceID = objResItem.GetInstanceID();
                                if (dicChildInstanceID.ContainsKey(instanceID))
                                {
                                    Debug.Log($"物体{par.name}中组件{type.Name}中字段{item.Name}包含{dicChildInstanceID[instanceID]}");
                                }
                            }

                        }
                    }
                }
            }
        }


        static bool IsTargetComponent(Type type)
        {
            for (int i = 0; i < listTargetComponent.Count; i++)
            {
                if (type == listTargetComponent[i])
                {
                    return true;
                }
            }
            return false;
        }
        public static void DoEditorBindFindOri()
        {
            //快捷键不会显示，但是还是可以使用
            Debug.Log("查找绑定关系");
            if (Selection.activeGameObject == null)
            {
                return;
            }

            //记录物体所有组件
            //Component[] comps = (Selection.activeGameObject as GameObject).GetComponents<Component>().Where(c => c).ToArray();
            Component[] comps = Selection.activeGameObject.GetComponents<Component>();
            Dictionary<int, List<FieldAdapter>> dicField = new Dictionary<int, List<FieldAdapter>>(8);
            Dictionary<int, Component> dicComponent = new Dictionary<int, Component>();
            for (int i = 0; i < comps.Length; i++)
            {
                Debug.Log($"组件名字{comps[i].GetType().Name}");
                Type type = comps[i].GetType();
                Component com = comps[i];
                int instanceID = com.GetInstanceID();
                //遍历某组件中公开字段
                FieldInfo[] arrField = type.GetFields(~BindingFlags.Static);

                for (int filedIdx = 0; filedIdx < arrField.Length; filedIdx++)
                {
                    var item = arrField[filedIdx];
                    if (item.FieldType.IsPrimitive) continue;
                    if (!item.IsPublic) if (!item.IsDefined(serType, true)) continue;
                    if (item.IsDefined(serType2, true)) continue;
                    Debug.Log($"字段名字{item.Name}");

                    var fa = EMX.HierarchyPlugin.Editor.Tools.FieldAdapter.TryToCreate(item, true);

                    if (dicComponent.ContainsKey(instanceID) == false)
                    {
                        dicComponent.Add(instanceID, com);
                    }

                    if (dicField.ContainsKey(instanceID) == false)
                    {
                        dicField.Add(instanceID, new List<FieldAdapter>());
                    }
                    dicField[instanceID].Add(fa);
                }

            }

            foreach (var item in dicField)
            {
                for (int i = 0; i < item.Value.Count; i++)
                {
                    object res = null;
                    res = item.Value[i].GetValue(dicComponent[item.Key]);
                    int test = 0;
                }
                
            }


        }
    }
}