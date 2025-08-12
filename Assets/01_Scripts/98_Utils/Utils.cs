using System.Text;
using System.Xml;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace Client
{
    public enum LogColor
    {
        Black,
        DarkBlue,
        DarkGreen,
        DarkCyan,
        DarkRed,
        DarkMagenta,
        DarkYellow,
        Gray,
        DarkGray,
        Blue,
        Green,
        Cyan,
        Red,
        Magenta,
        Yellow,
        White
    }

    public enum LogLevel
    {
        Info,
        Debug,
        Error
    }

    public static class Utils
    {
    }

    public static class Logger
    {
        public static void Log(string message)
        {
            Log(string.Empty, message, LogLevel.Debug, LogColor.White);
        }

        public static void Log(string title, string message)
        {
            Log(title, message, LogLevel.Debug, LogColor.White);
        }

        public static void LogInfo(string message)
        {
            Log(string.Empty, message, LogLevel.Info, LogColor.Yellow);
        }

        public static void LogInfo(string title, string message)
        {
            Log(title, message, LogLevel.Info, LogColor.Yellow);
        }

        public static void LogDebug(string message)
        {
            Log(string.Empty, message, LogLevel.Debug, LogColor.Green);
        }

        public static void LogDebug(string title, string message)
        {
            Log(title, message, LogLevel.Debug, LogColor.Green);
        }

        public static void LogError(string message)
        {
            Log(string.Empty, message, LogLevel.Error, LogColor.Red);
        }

        public static void LogError(string title, string message)
        {
            Log(title, message, LogLevel.Error, LogColor.Red);
        }

        public static void Log(string title, string message, LogLevel level, LogColor titleColor)
        {
            title = $"<color={titleColor}>{title}</color>";
            switch(level)
            {
                case LogLevel.Info:
                    Debug.LogWarning($"{title}: {message}");
                    break;
                case LogLevel.Debug:
                    Debug.Log($"{title}: {message}");
                    break;
                case LogLevel.Error:
                    Debug.LogError($"{title}: {message}");
                    break;
            }
        }
    }

    public class PriorityRandom<T> where T : notnull
    {
        T[] _priorItems;
        int _totalPrior = 0;
        float[] _priorsRates;
        Dictionary<T, float> _probabilityDic;
        System.Random _random = new System.Random();
        public PriorityRandom(T[] priorItems, Func<T, int> predicate)
        {
            _priorItems = priorItems;
            for (int i = 0; i < _priorItems.Length; ++i)
            {
                _totalPrior += predicate(_priorItems[i]);
            }

            _priorsRates = new float[priorItems.Length];
            _probabilityDic = new Dictionary<T, float>();
            //[10, 40, 20, 30]
            // 10 / 100 = 0.1
            // 0.1 + (40 / 100) = 0.5
            // 0.5 + (20 / 100) = 0.7
            // 0.7 + (30 / 100) = 1.0
            float cumulative = 0f;
            for (int i = 0; i < priorItems.Length; ++i)
            {
                float probabiliry = ((float)predicate(_priorItems[i]) / _totalPrior);
                _probabilityDic.Add(priorItems[i], probabiliry);
                _priorsRates[i] = cumulative + probabiliry;
                cumulative = _priorsRates[i];
            }
        }

        public T GetRandom()
        {
            double rand = _random.NextDouble();
            for (int i = 0; i < _priorsRates.Length; ++i)
            {
                if (rand <= _priorsRates[i])
                {
                    // 선택됨
                    return _priorItems[i];
                }
            }
            return default;
        }

        public float GetProbability(T item)
        {
            _probabilityDic.TryGetValue(item, out float probability);
            return probability;
        }

        public Dictionary<T, float> GetProbabilities()
        {
            return _probabilityDic;
        }
    }

    public class ArrayRandom
    {
        System.Random _random = new System.Random();

        public T SelectRandomElement<T>(T[] array)
        {
            int index = SelectRandomIndex(array);
            return array[index];
        }

        public int SelectRandomIndex<T>(T[] array)
        {
            return _random.Next(array.Length);
        }

        public T SelectRandomElement<T>(List<T> array)
        {
            int index = SelectRandomIndex(array);
            return array[index];
        }

        public int SelectRandomIndex<T>(List<T> array)
        {
            return _random.Next(array.Count);
        }

        public List<int> SelectRandomIndexes(int totalCount, int count)
        {
            if (count > totalCount)
                count = totalCount;

            List<int> temp = new List<int>(totalCount);
            for (int i = 0; i < totalCount; ++i)
            {
                temp.Add(i);
            }

            List<int> result = new List<int>();
            for (int i = 0; i < count; ++i)
            {
                int index = SelectRandomIndex(temp);
                result.Add(temp[index]);
                temp.RemoveAt(index);
            }

            return result;
        }
    }

    public class PriorityQueue<T> where T : IComparable<T>
    {
        List<T> _heap = new List<T>();
        public int Count { get { return _heap.Count; } }

        // O(logN)
        public void Push(T data)
        {
            // 힙의 맨 끝에 새로운 데이터를 삽입한다
            _heap.Add(data);

            int now = _heap.Count - 1;
            // 도장깨기를 시작
            while (now > 0)
            {
                // 도장깨기를 시도
                int next = (now - 1) / 2;
                if (_heap[now].CompareTo(_heap[next]) < 0)
                    break; // 실패

                // 두 값을 교체한다
                T temp = _heap[now];
                _heap[now] = _heap[next];
                _heap[next] = temp;

                // 검사 위치를 이동한다
                now = next;
            }
        }

        // O(logN)
        public T Pop()
        {
            // 반환할 데이터를 따로 저장
            T ret = _heap[0];

            // 마지막 데이터를 루트로 이동한다
            int lastIndex = _heap.Count - 1;
            _heap[0] = _heap[lastIndex];
            _heap.RemoveAt(lastIndex);
            lastIndex--;

            // 역으로 내려가는 도장깨기 시작
            int now = 0;
            while (true)
            {
                int left = 2 * now + 1;
                int right = 2 * now + 2;

                int next = now;
                // 왼쪽값이 현재값보다 크면, 왼쪽으로 이동
                if (left <= lastIndex && _heap[next].CompareTo(_heap[left]) < 0)
                    next = left;
                // 오른값이 현재값(왼쪽 이동 포함)보다 크면, 오른쪽으로 이동
                if (right <= lastIndex && _heap[next].CompareTo(_heap[right]) < 0)
                    next = right;

                // 왼쪽/오른쪽 모두 현재값보다 작으면 종료
                if (next == now)
                    break;

                // 두 값을 교체한다
                T temp = _heap[now];
                _heap[now] = _heap[next];
                _heap[next] = temp;
                // 검사 위치를 이동한다
                now = next;
            }

            return ret;
        }

        public T Peek()
        {
            if (_heap.Count == 0)
                return default(T);
            return _heap[0];
        }

        public void ForEach(Action<T> callback)
        {
            foreach (T item in _heap)
            {
                callback(item);
            }
        }
    }
}
