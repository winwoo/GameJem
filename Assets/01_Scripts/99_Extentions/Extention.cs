using UnityEngine.Diagnostics;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace Client
{
    public static class Extention
    {
        public static Vector3 ZERO = new Vector3(0f, 0f, 0f);
        public static Vector3 UP = new Vector3(0f, 1f, 0f);
        public static Vector3 DOWN = new Vector3(0f, -1f, 0f);
        public static Vector3 BACK = new Vector3(0f, 0f, -1f);
        public static Vector3 RIGHT = new Vector3(1f, 0f, 0f);

        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            T component = go.GetComponent<T>();
            if (component == null)
                component = go.AddComponent<T>();
            return component;
        }

        public static T FindChild<T>(this Transform transform, string name) where T : Component
        {
            T[] ts = transform.GetComponentsInChildren<T>(true);
            foreach (T t in ts)
            {
                if (string.IsNullOrEmpty(name) || t.name == name)
                {
                    return t;
                }
            }
            return null;
        }

        public static void ResetTransform(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        public static void ResetRectTransform(this Transform transform)
        {
            RectTransform tr = transform as RectTransform;
            if (null == tr) return;

            tr.anchorMin = Vector2.zero;
            tr.anchorMax = Vector2.one;
            tr.offsetMin = Vector2.zero;
            tr.offsetMax = Vector2.zero;
        }

        public static Vector2 GetRelativeSize(this RectTransform rectTransform)
        {
            return new Vector2(GetRelativeSizeX(rectTransform), GetRelativeSizeY(rectTransform));
        }

        public static float GetRelativeSizeX(this RectTransform rectTransform)
        {
            if (rectTransform.anchorMin.x != rectTransform.anchorMax.x)
            {
                RectTransform parent = rectTransform.parent as RectTransform;
                if (null != parent)
                {
                    float parentSize = GetRelativeSizeX(parent);

                    return parentSize + rectTransform.sizeDelta.x;
                }
                else
                {
                    return 1920;
                }
            }
            else
                return rectTransform.sizeDelta.x;
        }

        public static float GetRelativeSizeY(this RectTransform rectTransform)
        {
            if (rectTransform.anchorMin.y != rectTransform.anchorMax.y)
            {
                RectTransform parent = rectTransform.parent as RectTransform;
                if (null != parent)
                {
                    float parentSize = GetRelativeSizeY(parent);

                    return parentSize + rectTransform.sizeDelta.y;
                }
                else
                {
                    return 1080;
                }
            }
            else
                return rectTransform.sizeDelta.y;
        }

        /// <summary>
        /// 대소문자 구분 하지않는 옵션 넣을수있는 string비교 함수
        /// </summary>
        /// <param name="isIgnoreCase">true로 할경우 대소문자 무시</param>
        /// <returns></returns>
        public static bool Contains(this string source, string toCheck, bool isIgnoreCase)
        {
            return source.Contains(toCheck, isIgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.CurrentCulture);
        }

        //리스트 섞기
        public static void Shuffle<T>(this IList<T> list)
        {
            System.Random rng = new System.Random();
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

        // Vector3 Local Position
        public static void setLocalPositionX(this Transform transform, float value)
        {
            transform.localPosition = new Vector3(value,
                                                  transform.localPosition.y,
                                                  transform.localPosition.z);
        }

        public static void setLocalPositionY(this Transform transform, float value)
        {
            transform.localPosition = new Vector3(transform.localPosition.x,
                                                  value,
                                                  transform.localPosition.z);
        }

        public static void setLocalPositionZ(this Transform transform, float value)
        {
            transform.localPosition = new Vector3(transform.localPosition.x,
                                                  transform.localPosition.y,
                                                  value);
        }


        // Vector3 Local Rotation
        public static void setLocalRotationX(this Transform transform, float value)
        {
            transform.localEulerAngles = new Vector3(value,
                                                     transform.localRotation.y,
                                                        transform.localRotation.z);
        }

        public static void setLocalRotationY(this Transform transform, float value)
        {
            transform.localEulerAngles = new Vector3(transform.localRotation.x,
                                                     value,
                                                     transform.localRotation.z);
        }

        public static void setLocalRotationZ(this Transform transform, float value)
        {
            transform.localEulerAngles = new Vector3(transform.localRotation.x,
                                                     transform.localRotation.y,
                                                     value);
        }


        // Vector3 Local Scale
        public static void setLocalScaleX(this Transform transform, float value)
        {
            transform.localScale = new Vector3(value,
                                               transform.localScale.y,
                                               transform.localScale.z);
        }

        public static void setLocalScaleY(this Transform transform, float value)
        {
            transform.localScale = new Vector3(transform.localScale.x,
                                               value,
                                               transform.localScale.z);
        }

        public static void setLocalScaleZ(this Transform transform, float value)
        {
            transform.localScale = new Vector3(transform.localScale.x,
                                               transform.localScale.y,
                                               value);
        }


        // Vector3 Real World Position
        public static Vector3 GetWorldPosition(this Transform transform)
        {
            try
            {
                return new Vector3(
                    transform.position.x / transform.lossyScale.x,
                    transform.position.y / transform.lossyScale.y,
                    transform.position.z / transform.lossyScale.z
                    );
            }

            catch (DivideByZeroException)
            {
                return ZERO;
            }
        }


        public static GameObject Create(this GameObject prefab, Transform parent, bool bPosZero = false)
        {
            GameObject ob = GameObject.Instantiate(prefab) as GameObject;
            SetLayer(ob.transform, parent.gameObject.layer);
            ob.transform.parent = parent;
            ob.transform.localPosition = bPosZero ? ZERO : prefab.transform.localPosition;
            ob.transform.localScale = prefab.transform.localScale;
            return ob;
        }

        public static void SetLayer(Transform kChild, int iLayer)
        {
            kChild.gameObject.layer = iLayer;
            int iChildCnt = kChild.childCount;
            for (int i = 0; i < iChildCnt; ++i)
            {
                SetLayer(kChild.GetChild(i), iLayer);
            }
        }

        public static string SymbolColor(this string text, string start, string end, string code)
        {
            return text.Replace(start, $"<color=#{code}>").Replace(end, "</color>");
        }
    }
}