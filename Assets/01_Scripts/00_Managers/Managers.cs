using Cysharp.Threading.Tasks;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers s_instance;
    public static Managers Instance { get { Init().GetAwaiter().GetResult(); return s_instance; } }
    #region Core
    PoolManager _pool = new PoolManager();
    ResourceManager _resource = new ResourceManager();
    SceneManager _scene = new SceneManager();
    DataManager _data = new DataManager();
    UIManager _ui = new UIManager();

    public static PoolManager Pool => Instance._pool;
    public static ResourceManager Resource => Instance._resource;
    public static SceneManager Scene => Instance._scene;
    public static DataManager Data => Instance._data;
    public static UIManager UI => Instance._ui;
    #endregion

    private async void Awake()
    {
        await Init();
    }

    private void Start()
    {
        SetResolution(1920, 1080); // ���ϴ� �ػ󵵷� ����
    }

    private async void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            await UI.Open<UISample>();
        }
    }

    #region Init
    public void SetResolution(int width, int height)
    {
        int setWidth = width; // ����� ���� �ʺ�
        int setHeight = height; // ����� ���� ����

        int deviceWidth = Screen.width; // ��� �ʺ� ����
        int deviceHeight = Screen.height; // ��� ���� ����

        Screen.SetResolution(setWidth, (int)(((float)deviceHeight / deviceWidth) * setWidth), true); // SetResolution �Լ� ����� ����ϱ�

        // ī�޶��� Rect ����
        Camera camera = Camera.main;
        if (camera == null)
            return;

        if ((float)setWidth / setHeight < (float)deviceWidth / deviceHeight) // ����� �ػ� �� �� ū ���
        {
            float newWidth = ((float)setWidth / setHeight) / ((float)deviceWidth / deviceHeight); // ���ο� �ʺ�
            camera.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f); // ���ο� Rect ����
        }
        else // ������ �ػ� �� �� ū ���
        {
            float newHeight = ((float)deviceWidth / deviceHeight) / ((float)setWidth / setHeight); // ���ο� ����
            camera.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight); // ���ο� Rect ����
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

            // Manager �ʱ�ȭ
            await s_instance._resource.Init();
            await s_instance._data.Init();
            await s_instance._pool.Init();
            await s_instance._scene.Init();
            await s_instance._ui.Init();
        }
    }

    public async UniTask Dispose()
    {
        await s_instance._ui.Dispose();
        await s_instance._scene.Dispose();
        await s_instance._pool.Dispose();
        await s_instance._data.Dispose();
        await s_instance._resource.Dispose();
        s_instance = null;
    }
    #endregion
}
