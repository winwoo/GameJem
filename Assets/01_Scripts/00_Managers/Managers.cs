using Cysharp.Threading.Tasks;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers s_instance;
    public static Managers Instance { get { Init().GetAwaiter().GetResult(); return s_instance; } }
    #region Core
    private readonly PoolManager _pool = new PoolManager();
    private readonly ResourceManager _resource = new ResourceManager();
    private readonly SceneManager _scene = new SceneManager();
    private readonly DataManager _data = new DataManager();
    private readonly UIManager _ui = new UIManager();
    private readonly SoundManager _sound = new SoundManager();

    public static PoolManager Pool => Instance._pool;
    public static ResourceManager Resource => Instance._resource;
    public static SceneManager Scene => Instance._scene;
    public static DataManager Data => Instance._data;
    public static UIManager UI => Instance._ui;
    public static SoundManager Sound => Instance._sound;
    #endregion

    #region Contents
    private readonly BattleDesignManager _battleDesign = new BattleDesignManager();

    public static BattleDesignManager BattleDesign => Instance._battleDesign;
    #endregion

    private async void Awake()
    {
        await Init();
    }

    private void Start()
    {
        SetResolution(1920, 1080); // 원하는 해상도로 설정
    }

    #region Init
    public void SetResolution(int width, int height)
    {
        int setWidth = width; // 사용자 설정 너비
        int setHeight = height; // 사용자 설정 높이

        int deviceWidth = Screen.width; // 기기 너비 저장
        int deviceHeight = Screen.height; // 기기 높이 저장

        Screen.SetResolution(setWidth, (int)(((float)deviceHeight / deviceWidth) * setWidth), true); // SetResolution 함수 제대로 사용하기

        // 카메라의 Rect 설정
        Camera camera = Camera.main;
        if (camera == null)
            return;

        if ((float)setWidth / setHeight < (float)deviceWidth / deviceHeight) // 기기의 해상도 비가 더 큰 경우
        {
            float newWidth = ((float)setWidth / setHeight) / ((float)deviceWidth / deviceHeight); // 새로운 너비
            camera.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f); // 새로운 Rect 적용
        }
        else // 게임의 해상도 비가 더 큰 경우
        {
            float newHeight = ((float)deviceWidth / deviceHeight) / ((float)setWidth / setHeight); // 새로운 높이
            camera.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight); // 새로운 Rect 적용
        }
    }

    static async UniTask Init()
    {
        if(s_instance != null)
            return;

        GameObject go = GameObject.Find("@Managers");
        if (go == null)
        {
            go = new GameObject { name = "@Managers" };
            go.AddComponent<Managers>();
        }

        s_instance = go.GetComponent<Managers>();
        if (Application.isPlaying)
        {
            DontDestroyOnLoad(go);

            // Manager 초기화
            await s_instance._resource.Init();
            await s_instance._data.Init();
            await s_instance._pool.Init();
            await s_instance._scene.Init();
            await s_instance._ui.Init();
            await s_instance._sound.Init();
            await s_instance._battleDesign.Init();
        }
    }

    public async UniTask Dispose()
    {
        await s_instance._ui.Dispose();
        await s_instance._scene.Dispose();
        await s_instance._pool.Dispose();
        await s_instance._data.Dispose();
        await s_instance._resource.Dispose();
        await s_instance._sound.Dispose();
        await s_instance._battleDesign.Dispose();
        s_instance = null;
    }
    #endregion
}
