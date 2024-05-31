using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using System.Linq.Expressions;

namespace DarkcupGames
{
    public static class Utils
    {
        public static WaitForSeconds WAIT_1_SECOND = new WaitForSeconds(1f);
        public static WaitForSeconds WAIT_2_SECOND = new WaitForSeconds(2f);
        public static WaitForSeconds WAIT_3_SECOND = new WaitForSeconds(3f);
        public static WaitForSeconds WAIT_0_DOT_02_SECOND = new WaitForSeconds(0.02f);
        public static WaitForSeconds WAIT_0_DOT_2_SECOND = new WaitForSeconds(0.2f);
        public static WaitForSeconds WAIT_0_DOT_1_SECOND = new WaitForSeconds(0.1f);
        private static System.Random rng = new System.Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static T RandomElement<T>(this IList<T> list)
        {
            if (list.Count == 0)
            {
                Debug.LogError("Get random element on a null list");
                return default(T);
            }
            return list[UnityEngine.Random.Range(0, list.Count)];
        }

        public static void UpdateSelected<T>(this List<T> list, int index, Action<T> selected, Action<T> notSelected)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (i == index)
                {
                    selected(list[i]);
                }
                else
                {
                    notSelected(list[i]);
                }
            }
        }

        public static void UpdateSelected(this Transform thisTransform, int index, Action<Transform> notSelected, Action<Transform> selected)
        {
            for (int i = 0; i < thisTransform.childCount; i++)
            {
                if (i == index)
                {
                    selected(thisTransform.GetChild(i));
                }
                else
                {
                    notSelected(thisTransform.GetChild(i));
                }
            }
        }

        public static void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public static void ChangeScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public static int NextIndex<T>(this List<T> list, int currentIndex)
        {
            currentIndex++;
            if (currentIndex >= list.Count)
            {
                currentIndex = 0;
            }
            return currentIndex;
        }

        public static int PreviousIndex<T>(this List<T> list, int currentIndex)
        {
            currentIndex--;
            if (currentIndex < 0)
            {
                currentIndex = list.Count - 1;
            }
            return currentIndex;
        }

        public static void SetEnableChild(this Transform thisTransform, int index)
        {
            if (index >= thisTransform.childCount) return;

            for (int i = 0; i < thisTransform.childCount; i++)
            {
                thisTransform.GetChild(i).gameObject.SetActive(i == index);
            }
        }

        #region GET_COMPONENT_BY_PATH
        public static T GetChildComponent<T>(this GameObject obj, string name)
        {
            if (name.Contains("/"))
            {
                var find = GetGameObjectWithPath(obj, name);
                if (find == null)
                {
                    return default(T);
                }
                else
                {
                    return GetComponentFromObject<T>(find.gameObject);
                }
            }

            foreach (Transform child in obj.transform)
            {
                if (child.name == name)
                {
                    return GetComponentFromObject<T>(child.gameObject);
                }
            }
            Debug.LogError($"Not found object with name {name} in {obj.name}'s childrens!");
            return default(T);
        }

        static T GetComponentFromObject<T>(GameObject baseObj)
        {
            if (typeof(T) == typeof(GameObject))
            {
                return (T)((object)baseObj.gameObject);
            }
            var result = baseObj.GetComponent<T>();

            if (result == null)
            {
                Debug.LogError("Component not found!");
            }
            return result;
        }

        public static GameObject GetChildGameObject(this GameObject obj, string name)
        {
            foreach (Transform child in obj.transform)
            {
                if (child.name == name)
                {
                    return child.gameObject;
                }
            }
            Debug.LogError($"Not found object with name {name} in {obj.name}'s childrens!");
            return null;
        }

        public static T GetComponentWithPath<T>(this GameObject obj, string path)
        {
            GameObject child = GetGameObjectWithPath(obj, path);

            if (child == null)
            {
                Debug.LogError($"Cannot found any game object with path {path} in {obj.gameObject.name}'s child!");
                return default(T);
            }
            else
            {
                var result = child.GetComponent<T>();
                if (result == null)
                {
                    Debug.LogError($"Cannot found given type of component in object {child}!");
                }
                return result;
            }
        }

        public static T GetOrCreateComponent<T>(this GameObject obj) where T : Component
        {
            var result = obj.GetComponent<T>();
            if (result == null)
            {
                result = obj.AddComponent<T>();
            }
            return result;
        }

        private static string[] _paths;

        public static GameObject GetGameObjectWithPath(this GameObject root, string path)
        {
            _paths = path.Split('/');
            GameObject result = root;
            for (int i = 0; i < _paths.Length; i++)
            {
                result = FindObjectInChildren(result, _paths[i]);
                if (!result)
                {
                    Debug.LogError($"Cant find {_paths[i]} in {result.name}");
                    return null;
                }
            }

            return result;
        }

        public static GameObject FindObjectInChildren(GameObject root, string name)
        {
            for (int i = 0; i < root.transform.childCount; i++)
            {
                if (root.transform.GetChild(i).name == name)
                {
                    return root.transform.GetChild(i).gameObject;
                }
            }
            Debug.LogError($"Cant find {name} in {root.name}");
            return null;
        }

        public static GameObject FindObjectIncludingInactive(GameObject root, string name)
        {
            if (root.transform.childCount <= 0)
            {
                Debug.LogError($"{root} have no child!");
                return null;
            }
            GameObject result = null;

            if (root.name == name)
            {
                return root;
            }

            for (int i = 0; i < root.transform.childCount; i++)
            {
                result = FindObjectIncludingInactive(root.transform.GetChild(i).gameObject, name);
                if (result)
                {
                    return result;
                }
            }

            Debug.LogError($"Cant find {name} in {root.name}");
            return null;
        }

        public static GameObject FindObjectInRoot(string name)
        {
            return GameObject.Find(name);
        }

        public static GameObject FindObjectInRootIncludingInactive(string name)
        {
            Scene scene = SceneManager.GetActiveScene();
            if (!scene.isLoaded)
            {
                Debug.LogError("No scene loaded");
                return null;
            }

            var roots = new List<GameObject>();
            scene.GetRootGameObjects(roots);

            foreach (GameObject obj in roots)
            {
                if (obj.transform.name == name) return obj;
            }

            Debug.LogError($"Cant find {name} in root");
            return null;
        }
        #endregion
    }
}