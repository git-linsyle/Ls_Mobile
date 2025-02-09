﻿/****************************************************
	文件：MonoSingletonCreator.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/01/14 9:57   	
	Features：
*****************************************************/
using System.Reflection;
using UnityEngine;

namespace EdgeFramework
{
    class MonoSingletonCreator
    {
        public static bool IsUnitTestMode { get; set; }
        public static T CreateMonoSingleton<T>() where T : MonoBehaviour, ISingleton
        {
            T instance = null;

            if (!IsUnitTestMode && !Application.isPlaying) return instance;
            instance = Object.FindObjectOfType<T>();

            if (instance != null) return instance;
            MemberInfo info = typeof(T);
            var attributes = info.GetCustomAttributes(true);
            foreach (var atribute in attributes)
            {
                var defineAttri = atribute as MonoSingletonPath;
                if (defineAttri == null)
                {
                    continue;
                }

                instance = CreateComponentOnGameObject<T>(defineAttri.PathInHierarchy, true);
                break;
            }
            if (instance == null)
            {
                var obj = new GameObject(typeof(T).Name);
                if (!IsUnitTestMode)
                    Object.DontDestroyOnLoad(obj);
                instance = obj.AddComponent<T>();
            }

            instance.OnSingletonInit();

            return instance;
        }
        private static T CreateComponentOnGameObject<T>(string path, bool dontDestroy) where T : MonoBehaviour
        {
            var obj = FindGameObject(path, true, dontDestroy);
            if (obj == null)
            {
                obj = new GameObject("Singleton of " + typeof(T).Name);
                if (dontDestroy && !IsUnitTestMode)
                {
                    Object.DontDestroyOnLoad(obj);
                }
            }
            return obj.AddComponent<T>();
        }
        private static GameObject FindGameObject(string path, bool build, bool dontDestroy)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            var subPath = path.Split('/');
            if (subPath == null || subPath.Length == 0)
            {
                return null;
            }
            return FindGameObject(null, subPath, 0, build, dontDestroy);
        }
        private static GameObject FindGameObject(GameObject root, string[] subPath, int index, bool build, bool dontDestroy)
        {
            GameObject client = null;
            if (root == null)
            {
                client = GameObject.Find(subPath[index]);
            }
            else
            {
                var child = root.transform.Find(subPath[index]);
                if (child != null)
                {
                    client = child.gameObject;
                }
            }
            if (client == null)
            {
                if (build)
                {
                    client = new GameObject(subPath[index]);
                    if (root != null)
                    {
                        client.transform.SetParent(root.transform);
                    }
                    if (dontDestroy && index == 0 && !IsUnitTestMode)
                    {
                        GameObject.DontDestroyOnLoad(client);
                    }
                }
            }
            if (client == null)
            {
                return null;
            }
            return ++index == subPath.Length ? client : FindGameObject(client, subPath, index, build, dontDestroy);
        }
    }

}
