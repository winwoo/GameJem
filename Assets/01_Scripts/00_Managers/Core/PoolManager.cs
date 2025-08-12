using System.Collections.Generic;
using UnityEngine;
using Client;
using Logger = Client.Logger;
using Cysharp.Threading.Tasks;
using System;
using Object = UnityEngine.Object;
using Unity.VisualScripting;
using static UnityEngine.UI.Image;
using System.Linq;

public class PoolManager : BaseManager
{
    #region Pool
    class Pool : IDisposable
    {
        public string PoolName => Original.name;
        public GameObject Original { get; private set; }
        public Transform Root { get; set; }

        private readonly Stack<Poolable> _free = new();
        private readonly HashSet<Poolable> _using = new();

        public void Init(GameObject original, int count = 5, Transform root = null)
        {
            Original = original;
            // Root ����/����
            Root = root != null ? root : new GameObject($"{original.name}_Root").transform;

            Original.SetActive(false);
            Original.transform.SetParent(Root, false);

            for (int i = 0; i < count; i++)
                Release(Create());
        }

        private Poolable Create(bool inactive = true)
        {
            // ���� ��� ��Ȱ��ȭ�ؼ� OnEnable ��� ����
            GameObject go = Object.Instantiate(Original, Root);
            if (inactive) go.SetActive(false);
            go.name = PoolName;

            Poolable poolable = go.GetOrAddComponent<Poolable>();
            poolable.PoolName = PoolName;
            poolable.IsUsing = false;

            return poolable;
        }

        public void Release(Poolable poolable)
        {
            if (poolable == null)
                return;

            _using.Remove(poolable);
            poolable.IsUsing = false;
            poolable.gameObject.SetActive(false);

            if (poolable.transform.parent != Root)
            {
                poolable.transform.SetParent(Root, false);
            }

            _free.Push(poolable);
        }

        public Poolable Get(Transform parent)
        {
            Poolable poolable;

            if (_free.Count > 0)
                poolable = _free.Pop();
            else
                poolable = Create();

            if (parent != null && poolable.transform.parent != parent)
            {
                poolable.transform.SetParent(parent, false);
            }

            poolable.IsUsing = true;
            poolable.gameObject.SetActive(true);
            _using.Add(poolable);
            return poolable;
        }

        public void Dispose()
        {
            DestroyObjects(_using);
            DestroyObjects(_free);
            _using.Clear();
            _free.Clear();

            if (Original != null)
            {
                if (Original.transform.parent == Root)
                    Original.transform.SetParent(null, false);
                Object.Destroy(Original);
                Original = null;
            }

            if (Root != null && Root.childCount == 0) 
                Object.Destroy(Root.gameObject);
            Root = null;
        }

        private void DestroyObjects(IEnumerable<Poolable> collection)
        {
            foreach(Poolable poolable in collection)
            {
                if(poolable == null)
                    continue;

                if(poolable.transform.parent == Root)
                    poolable.transform.SetParent(null, false); // �θ� �����Ͽ� Root���� �и�

                Object.Destroy(poolable.gameObject);
            }
        }
    }
    #endregion

    private Dictionary<string, Pool> _pool = new Dictionary<string, Pool>();
    private Transform _root;

    public async override UniTask Init()
    {
        if (_root == null)
        {
            _root = new GameObject { name = "@Pool_Root" }.transform;
            Object.DontDestroyOnLoad(_root);
        }
        await UniTask.CompletedTask;
    }

    public async override UniTask Dispose()
    {
        DisposeAllPools();
        await UniTask.CompletedTask;
    }

    /// <summary>
    /// Pool ���� �� �񵿱�� Asset�� Load�Ͽ� Ǯ�� �����մϴ�.
    /// </summary>
    /// <param name="key">
    /// Resources : Resources ���� ��� (��: "Textures/myTexture")
    /// Addressables : Addressable Ű (��: "myTextureKey")
    /// </param>
    /// <param name="count">���� ���� ����</param>
    /// <returns></returns>
    public async UniTask CreatePool(string key, int count = 5)
    {
        GameObject original = GetOriginal(key);
        if (original != null)
            return;

        GameObject obj = await Managers.Resource.LoadAsync<GameObject>(key);
        if (obj == null)
        {
            Logger.LogError("CreatePool", $"Not Found Resource : {key}");
            return;
        }
        original = GameObject.Instantiate(obj);
        original.name = key;
        original.SetActive(false);

        CreatePool(original, count);
    }

    /// <summary>
    /// Ǯ ����
    /// </summary>
    /// <param name="original">����</param>
    /// <param name="count">���� ���� ����</param>
    /// <param name="root">���� root�� �ִ� ���</param>
    public void CreatePool(GameObject original, int count = 5, Transform root = null)
    {
        if (original == null)
        {
            Logger.LogError("CreatePool", "���� ������Ʈ�� null�Դϴ�.");
            return;
        }
        if (_pool.ContainsKey(original.name))
        {
            Logger.LogInfo("CreatePool", $"���� ������Ʈ�� �����մϴ�. _ ObjectName : {original.name}");
            return;
        }

        Pool pool = new Pool();
        pool.Init(original, count, root);
        if (root == null)
        {
            // ���� root�� ������ �⺻(_root) root�� �߰�
            pool.Root.parent = _root;
        }
        _pool.Add(original.name, pool);
        original.SetActive(false);
    }

    public void Release(Poolable poolable)
    {
        if (poolable == null)
            return;

        string name = poolable.PoolName;
        if(_pool.TryGetValue(name, out Pool pool))
        {
            pool.Release(poolable);
            return;
        }
        GameObject.Destroy(poolable.gameObject);
    }

    public Poolable Get(GameObject original, Transform parent = null)
    {
        if (_pool.ContainsKey(original.name) == false)
            CreatePool(original);

        return _pool[original.name].Get(parent);
    }

    public GameObject GetOriginal(string name)
    {
        if (_pool.ContainsKey(name) == false)
            return null;
        return _pool[name].Original;
    }

    public void DisposePool(string poolName)
    {
        if (!_pool.TryGetValue(poolName, out Pool pool))
            return;

        pool.Dispose();
        _pool.Remove(poolName);
        Managers.Resource.Release<GameObject>(poolName);
    }

    public void DisposeAllPools()
    {
        var keys = _pool.Keys.ToList();            // ������
        foreach (var key in keys)
        {
            var pool = _pool[key];
            pool.Dispose();
            Managers.Resource.Release<GameObject>(key); // Ű�� Release
        }
        _pool.Clear();
    }
}
