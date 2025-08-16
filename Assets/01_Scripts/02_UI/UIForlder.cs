using Client;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIForlder : UIBase
{
    [SerializeField]
    [Link]
    private UICFolder _folder;

    [SerializeField]
    [Link]
    private Transform _content;

    [SerializeField]
    [Link]
    private Button _btnBack1;
    [SerializeField]
    [Link]
    private Button _btnBack2;
    [SerializeField]
    [Link]
    private Button _btnUpdate;
    [SerializeField]
    [Link]
    private TextMeshProUGUI _textDesc;
    [SerializeField]
    [Link]
    private Image _imgBlank;
    [SerializeField]
    [Link]
    private TextMeshProUGUI _textPath;

    private List<UICFolder> _folders = new List<UICFolder>();
    private UICFolder _selectFolder;
    private DateTime _selectTime;
    public override void OnCreate(object ctx)
    {
        base.OnCreate(ctx);
        _btnBack1.onClick.AddListener(OnBack);
        _btnBack2.onClick.AddListener(OnBack);
        _btnUpdate.onClick.AddListener(OnUpdate);
        Managers.Instance.OriginBugs = Managers.Instance.InitBugSetting
            .ToDictionary(data => data.Type, data => data.IsBug);
        CreateFolder(_folder, _content.transform);
        _textDesc.gameObject.SetActive(false); // 설명 텍스트 비활성화
        _imgBlank.gameObject.SetActive(true); 
        _textPath.gameObject.SetActive(false); 
    }

    private void CreateFolder(UICFolder origin, Transform parent)
    {
        var settings = Managers.Instance.InitBugSetting;

        for(int i = 0; i < settings.Length; i++)
        {
            InitBugTypeData data = settings[i];
            UICFolder newFolder = Instantiate(origin, parent);
            newFolder.name = data.FolderName;
            newFolder.transform.SetParent(parent, false); // 부모 설정
            newFolder.gameObject.SetActive(true); // 활성화
            newFolder.InitFolder(data, OnClickFolder, i);
            _folders.Add(newFolder);
        }
        origin.gameObject.SetActive(false); // 원본 폴더 비활성화
    }

    private void OnClickFolder(UICFolder folder)
    {
        if(_selectFolder == null)
        {
            _selectFolder = folder;
            _selectTime = DateTime.Now;
            _selectFolder.SetSelect(true);
            return;
        }

        if (_selectFolder == folder)
        {
            if ((DateTime.Now - _selectTime).TotalSeconds < 0.5f)
            {
                // 폴더 선택 해제
                // 폴더 들어가기
                EnterFolder();
                _selectFolder.SetSelect(false);
            }
            else
            {
                // 폴더 선택 유지
                _selectTime = DateTime.Now;
            }
        }
        else
        {
            // 다른 폴더 선택
            _selectFolder.SetSelect(false);
            _selectFolder = folder;
            _selectTime = DateTime.Now;
            _selectFolder.SetSelect(true);
        }
    }

    private void EnterFolder()
    {
        if (_selectFolder == null)
        {
            Debug.LogWarning("선택된 폴더가 없습니다.");
            return;
        }
        foreach (var folder in _folders)
        {
            if (folder == _selectFolder)
                continue; 
            folder.gameObject.SetActive(false);
        }
        _selectFolder.EnterFolder(OnExitFolder);
        _btnUpdate.gameObject.SetActive(false); // 업데이트 버튼 비활성화
        _textDesc.gameObject.SetActive(true);
        _imgBlank.gameObject.SetActive(false); 
        _textPath.gameObject.SetActive(true);
        _textPath.text = _selectFolder.FolderName; // 현재 폴더 경로 표시
    }

    private void OnExitFolder(UICFolder exitFolder)
    {
        foreach (var folder in _folders)
        {
            if (folder == exitFolder)
                continue; 

            folder.gameObject.SetActive(true);
        }
        _btnUpdate.gameObject.SetActive(true); // 업데이트 버튼 활성화
        _textDesc.gameObject.SetActive(false);
        _imgBlank.gameObject.SetActive(true);
        _textPath.gameObject.SetActive(false);
        DebugPrint();
    }

    private void OnBack()
    {
        if (_selectFolder == null)
        {
            Debug.LogWarning("선택된 폴더가 없습니다.");
            return;
        }
        _selectFolder.ExitFolder();
    }

    private async void OnUpdate()
    {
        var datas = Managers.Instance.InitBugSetting;
        bool noModified = true;
        foreach (var data in datas)
        {
            if(Managers.Instance.OriginBugs[data.Type] == data.IsBug)
                continue;

            noModified = false;
            break;
        }

        if (noModified)
        {
            Action<bool> action = async (isOk) =>
            {
                if(!isOk)
                    return;

                await Managers.Scene.LoadSceneAsync(Define.Scene.Game);
                await CloseUI();
            };
            await Managers.UI.Open<UISystem>(action);
            return;
        }

        await Managers.Scene.LoadSceneAsync(Define.Scene.Game);
        await CloseUI();
    }

    private void DebugPrint()
    {
        string origin = "";
        foreach (var data in Managers.Instance.OriginBugs)
        {
            origin += $"{data.Key}: {data.Value}\n";
        }
        string current = "";
        foreach (var data in Managers.Instance.InitBugSetting)
        {
            current += $"{data.Type}: {data.IsBug}\n";
        }
        Debug.Log($"Origin Bugs:\n{origin}\nCurrent Bugs:\n{current}");
    }
}
