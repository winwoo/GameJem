using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class UIBase : MonoBehaviour
{
    [SerializeField] 
    private GUI_CATEGORY _category = GUI_CATEGORY.Scene;
    [SerializeField] 
    private bool _isModal = false;
    [SerializeField] 
    private bool _singleton = true;
    [SerializeField] 
    private int _sortingOffset = 0;
    [SerializeField] 
    private Canvas _selfCanvas;

    public GUI_CATEGORY Category => _category;
    public bool IsModal => _isModal;
    public bool IsSingleton => _singleton;
    public int SortingOffset => _sortingOffset;
    public Canvas SelfCanvas => _selfCanvas;

    public virtual void OnCreate(object ctx) { }
    public virtual async UniTask ShowAsync(object args = null) { SetActive(true); await UniTask.Yield(); }
    public virtual async UniTask HideAsync() { SetActive(false); await UniTask.Yield(); }
    public virtual void OnDestroyView() { }

    public async virtual UniTask CloseUI()
    {
        await Managers.UI.Close(this);
    }

    public void SetSortingBase(int baseOrder)
    {
        if (_selfCanvas == null) return;
        _selfCanvas.overrideSorting = true;
        _selfCanvas.sortingOrder = baseOrder + _sortingOffset;
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
        if (_selfCanvas != null)
        {
            _selfCanvas.enabled = active;
        }
    }
}