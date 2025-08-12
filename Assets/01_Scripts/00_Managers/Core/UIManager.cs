using Client;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using Object = UnityEngine.Object;
using UnityEngine.EventSystems;
public class UIManager : BaseManager
{
    private Transform _root;

    private readonly Dictionary<GUI_CATEGORY, Canvas> _layers = new();
    private readonly Dictionary<string, List<UIBase>> _opened = new();
    private readonly Stack<UIBase> _popupStack = new();
    private Canvas _modalMaskCanvas;

    const string BASE_PATH = "00_GUI";

    #region Init
    public async override UniTask Init()
    {
        if (_root != null)
            return;

        GameObject root = GameObject.Find("@UI_Root");
        if (root == null)
            _root = new GameObject { name = "@UI_Root" }.transform;
        else
            _root = root.transform;

        if (Application.isPlaying)
        {
            Object.DontDestroyOnLoad(_root);
            CreateLayer(GUI_CATEGORY.Common, 0);
            CreateLayer(GUI_CATEGORY.Scene, 100);
            CreateLayer(GUI_CATEGORY.Popup, 500);
            CreateLayer(GUI_CATEGORY.System, 900);
            _modalMaskCanvas = CreateModalMask(_layers[GUI_CATEGORY.Popup].transform, 999);
            _modalMaskCanvas.gameObject.SetActive(false);

            if (EventSystem.current == null)
            {
                var es = new GameObject("EventSystem");
                es.AddComponent<EventSystem>();
                es.AddComponent<StandaloneInputModule>();
                es.transform.SetParent(_root, false);
            }
        }
        await UniTask.CompletedTask;
    }

    public async override UniTask Dispose()
    {
        // 이미 초기화 안 된 상태면 끝
        if (_root == null)
            return;

        // 1) 모달/팝업: 스택 Top부터 안전하게 닫기
        while (_popupStack.Count > 0)
        {
            var top = _popupStack.Peek();      // Close에서 Pop 처리됨
            if (top == null) { _popupStack.Pop(); continue; }
            await Close(top);
        }

        // 2) 나머지 열린 UI 모두 닫기(스냅샷 역순)
        //    Close 호출 중 _opened가 변하므로 스냅샷이 필수
        var remain = _opened.Values.SelectMany(list => list).ToList();
        for (int i = remain.Count - 1; i >= 0; --i)
        {
            var view = remain[i];
            if (view != null)                 // 이미 파괴되었을 수 있음
                await Close(view);
        }
        _opened.Clear();

        // 3) 모달 마스크 정리
        if (_modalMaskCanvas != null)
        {
            var go = _modalMaskCanvas.gameObject;
            _modalMaskCanvas = null;
            if (go) GameObject.Destroy(go);
        }

        // 4) 레이어 캔버스 정리
        if (_layers.Count > 0)
        {
            foreach (var kv in _layers)
                if (kv.Value != null && kv.Value.gameObject != null)
                    GameObject.Destroy(kv.Value.gameObject);
            _layers.Clear();
        }

        // 5) 루트 정리
        if (_root != null)
        {
            // _root 하위에 EventSystem을 우리가 생성해 붙여놨다면 함께 제거됨
            var rootGo = _root.gameObject;
            _root = null;
            if (rootGo) GameObject.Destroy(rootGo);
        }

        await UniTask.CompletedTask;
    }

    private void CreateLayer(GUI_CATEGORY cat, int orderBase)
    {
        var go = new GameObject($"{cat}Layer");
        go.transform.SetParent(_root, false);

        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = orderBase;
        var scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        go.AddComponent<GraphicRaycaster>();
        _layers[cat] = canvas;
    }

    private Canvas CreateModalMask(Transform parent, int orderOffset)
    {
        var go = new GameObject("ModalMask");
        go.transform.SetParent(parent, false);
        var img = go.AddComponent<Image>();
        img.raycastTarget = true; img.color = new Color(0, 0, 0, 0.6f);
        var c = go.AddComponent<Canvas>();
        c.overrideSorting = true;
        c.sortingOrder = _layers[GUI_CATEGORY.Popup].sortingOrder + orderOffset;
        go.AddComponent<GraphicRaycaster>();
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one; rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
        return c;
    }
    #endregion

    #region Open/Close
    public async UniTask<T> Open<T>(object args = null) where T : UIBase
    {
        var type = typeof(T);
        string pathKey = ResolveKey(type);
        string path = $"{BASE_PATH}/{pathKey}";
        GameObject prefab = await Managers.Resource.LoadAsync<GameObject>(path);
        if (prefab == null)
            throw new Exception($"UI prefab not found: {path}");

        var prefabView = prefab.GetComponent<UIBase>();
        if (prefabView == null)
            throw new Exception($"UIBase component missing on prefab {prefab.name}");

        var layer = _layers[prefabView.Category];

        // 싱글턴 중복 방지
        if (prefabView.IsSingleton && TryGetTop(pathKey, out var exist))
        {
            BringToFront(exist);
            return exist as T;
        }

        var go = GameObject.Instantiate(prefab, layer.transform, false);
        var view = go.GetComponent<T>();
        view.SetSortingBase(layer.sortingOrder);
        view.OnCreate(args);
        Register(pathKey, view);

        if (view.Category == GUI_CATEGORY.Popup)
            _popupStack.Push(view);

        UpdateModalMask();
        await view.ShowAsync(args);
        return view;
    }

    public async UniTask Close(UIBase view)
    {
        if (view == null) return;
        var key = ResolveKey(view.GetType());
        await view.HideAsync();
        Unregister(key, view);

        if (_popupStack.Count > 0)
        {
            if (_popupStack.Peek() == view)
            {
                _popupStack.Pop();
            }
            else
            {
                // close시 stack 중간에 있는 팝업이 재거될 경우
                // stack 재구성
                var rebuilt = new Stack<UIBase>(_popupStack.Reverse().Where(x => x != view));
                _popupStack.Clear();
                foreach (var x in rebuilt) _popupStack.Push(x);
            }
        }

        view.OnDestroyView();
        GameObject.Destroy(view.gameObject);
        UpdateModalMask();
    }
    #endregion

    #region Registry
    private void Register(string key, UIBase v)
    {
        if (!_opened.TryGetValue(key, out var list))
            _opened[key] = list = new List<UIBase>(2);
        list.Add(v);
    }

    private void Unregister(string key, UIBase v)
    {
        if (!_opened.TryGetValue(key, out var list)) return;
        list.Remove(v);
        if (list.Count == 0) _opened.Remove(key);
    }

    private bool TryGetTop(string key, out UIBase v)
    {
        v = null;
        if (_opened.TryGetValue(key, out var list) && list.Count > 0)
        {
            v = list[list.Count - 1];
            return true;
        }
        return false;
    }

    private void UpdateModalMask()
    {
        var top = _popupStack.Count > 0 ? _popupStack.Peek() : null;
        if (top != null && top.IsModal)
        {
            _modalMaskCanvas.gameObject.SetActive(true);
            int baseOrder = _layers[GUI_CATEGORY.Popup].sortingOrder;

            // 최상단 팝업 바로 아래로(시각: Dim은 뒤, 입력: 뒤에서 막음)
            int topOrder = top.SelfCanvas != null ? top.SelfCanvas.sortingOrder : baseOrder + 1;
            _modalMaskCanvas.overrideSorting = true;
            _modalMaskCanvas.sortingOrder = Mathf.Max(baseOrder, topOrder - 1);
        }
        else
        {
            _modalMaskCanvas.gameObject.SetActive(false);
        }
    }

    public void BringToFront(UIBase view)
    {
        if (view?.SelfCanvas == null) 
            return;

        var layer = _layers[view.Category];

        var maxOrder = _opened.Values.SelectMany(x => x)
            .Where(v => v.Category == view.Category)
            .Select(v => v.SelfCanvas != null ? v.SelfCanvas.sortingOrder : layer.sortingOrder)
            .DefaultIfEmpty(layer.sortingOrder)
            .Max();

        view.SelfCanvas.sortingOrder = maxOrder + 1;

        if (view.Category == GUI_CATEGORY.Popup)
            UpdateModalMask();
    }

    static string ResolveKey(Type t)
    {
        // Attribute 경로가 있으면 그걸 키로, 없으면 타입명
        var attr = (UIPathAttribute)Attribute.GetCustomAttribute(t, typeof(UIPathAttribute));
        return attr != null ? $"{attr.Path}/{t.Name}" : t.Name;
    }
    #endregion
}
