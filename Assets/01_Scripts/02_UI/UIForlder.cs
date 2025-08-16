using Client;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIForlder : UIBase
{
    [SerializeField]
    private Sprite _playerFolderBackground;
    [SerializeField]
    private Sprite _bossFolderBackground;
    [SerializeField]
    [Link]
    private Image _imgBackground;
    [SerializeField]
    [Link]
    private UICFile _file;
    [SerializeField]
    [Link]
    private Transform _playerFileParent;
    [SerializeField]
    [Link]
    private Transform _bossFileParent;
    [SerializeField]
    [Link]
    private Button _btnApply;

    private ICollection<BattlePlayDesginType> _playerDesignTypes;
    private ICollection<BattlePlayDesginType> _playerDesignTypesApplied;
    private ICollection<BattleBossDesignType> _bossDesignTypes;
    private ICollection<BattleBossDesignType> _bossDesignTypesApplied;


    private List<UICFile> _playerFiles = new List<UICFile>();
    private List<UICFile> _bossFiles = new List<UICFile>();

    private UICFile _selectedPlayerFile;
    private UICFile _selectedBossFile;
    public override void OnCreate(object ctx)
    {
        base.OnCreate(ctx);
        _playerDesignTypes = Managers.BattleDesign.PlayerDesignForlder;
        _bossDesignTypes = Managers.BattleDesign.BossDesignForlder;
        _playerDesignTypesApplied = Managers.BattleDesign.PlayerDesignApplied;
        _bossDesignTypesApplied = Managers.BattleDesign.BossDesignApplied;

        CreateFile(_playerFiles, _playerDesignTypes.Count, _playerFileParent);
        CreateFile(_bossFiles, _bossDesignTypes.Count, _bossFileParent);
        _btnApply.onClick.AddListener(OnClickApply);

        SetPlayerFiles();
        SetBossFiles();
        _imgBackground.sprite = _playerFolderBackground; // 플레이어 폴더 배경 설정
        _playerFileParent.gameObject.SetActive(true);
        _bossFileParent.gameObject.SetActive(false);
        _file.gameObject.SetActive(false); // 원본 파일은 비활성화
    }

    private void CreateFile(List<UICFile> files, int count, Transform parent)
    {
        // 원본 _file을 복제하여 새로운 UICFile 인스턴스를 생성
        for (int i = 0; i < count; i++)
        {
            UICFile newFile = Instantiate(_file, transform);
            newFile.name = $"UICFile_{i + 1}";
            newFile.transform.SetParent(parent, false); // 부모 설정
            newFile.gameObject.SetActive(true); // 활성화
            files.Add(newFile);
        }
    }

    private void SetPlayerFiles()
    {
        var fileTypes = _playerDesignTypes.ToList();
        for(int i = 0; i < fileTypes.Count; i++)
        {
            _playerFiles[i].SetText(GetPlayerDesignTypeText(fileTypes[i]));
            _playerFiles[i].AddEvent(OnClickPlayerFile, i);
        }
    }

    private void SetBossFiles()
    {
        var fileTypes = _bossDesignTypes.ToList();
        for(int i = 0; i < fileTypes.Count; i++)
        {
            _bossFiles[i].SetText(GetBossDesignTypeText(fileTypes[i]));
            _bossFiles[i].AddEvent(OnClickBossFile, i);
        }
    }

    private string GetPlayerDesignTypeText(BattlePlayDesginType type)
    {
        switch(type)
        {             
            case BattlePlayDesginType.Dash:
                return "Dash";
            case BattlePlayDesginType.Special1:
                return "Special 1";
            case BattlePlayDesginType.Special2:
                return "Special 2";
            default:
                return "Unknown";
        }
    }

    private string GetBossDesignTypeText(BattleBossDesignType type)
    {
        switch(type)
        {             
            case BattleBossDesignType.Special1:
                return "Special 1";
            case BattleBossDesignType.Special2:
                return "Special 2";
            case BattleBossDesignType.Special3:
                return "Special 3";
            default:
                return "Unknown";
        }
    }

    private void OnClickPlayerFile(UICFile file)
    {
        if (_selectedPlayerFile != null)
        {
            _selectedPlayerFile.SetSelct(false); // 이전 선택 해제
        }
        _selectedPlayerFile = file;
        _selectedPlayerFile.SetSelct(true);
    }

    private void OnClickBossFile(UICFile file)
    {
        if (_selectedBossFile != null)
        {
            _selectedBossFile.SetSelct(false); // 이전 선택 해제
        }
        _selectedBossFile = file;
        _selectedBossFile.SetSelct(true);
    }

    private void OnClickApply()
    {
        if (_playerFileParent.gameObject.activeSelf && _selectedPlayerFile != null)
        {
            var playerType = _playerDesignTypes.ElementAt(_selectedPlayerFile.Index);
            _playerDesignTypes.Remove(playerType); // 플레이어 타입을 보스 디자인 타입에서 제거
            _playerDesignTypesApplied.Add(playerType);
            _playerFileParent.gameObject.SetActive(false);
            _bossFileParent.gameObject.SetActive(true); // 보스 파일 선택 화면으로 전환
            _imgBackground.sprite = _bossFolderBackground; // 보스 폴더 배경으로 변경
        }

        if (_bossFileParent.gameObject.activeSelf && _selectedBossFile != null)
        {
            var bossType = _bossDesignTypes.ElementAt(_selectedBossFile.Index);
            _bossDesignTypes.Remove(bossType); // 보스 타입을 플레이어 디자인 타입에서 제거
            _bossDesignTypesApplied.Add(bossType);
            _bossFileParent.gameObject.SetActive(false);
            Managers.Scene.LoadSceneAsync(Define.Scene.Game).Forget(); // 게임 씬으로 전환
        }
        DebugDesignData();
    }

    private void DebugDesignData()
    {
        string _playerDesignTypesText = string.Join(", ", _playerDesignTypes.Select(t => GetPlayerDesignTypeText(t)));
        string _bossDesignTypesText = string.Join(", ", _bossDesignTypes.Select(t => GetBossDesignTypeText(t)));
        Debug.Log($"Player Design Types: {_playerDesignTypesText}");
        Debug.Log($"Boss Design Types: {_bossDesignTypesText}");
        Debug.Log($"Player Design Types Applied: {string.Join(", ", _playerDesignTypesApplied.Select(t => GetPlayerDesignTypeText(t)))}");
        Debug.Log($"Boss Design Types Applied: {string.Join(", ", _bossDesignTypesApplied.Select(t => GetBossDesignTypeText(t)))}");
    }
}
