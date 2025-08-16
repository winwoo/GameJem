using Client;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
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

    private List<UICFolder> _folders = new List<UICFolder>();
    private UICFolder _selectFolder;
    private DateTime _selectTime;
    private List<InitBugTypeData> _originBugs;
    public override void OnCreate(object ctx)
    {
        base.OnCreate(ctx);
        _btnBack1.onClick.AddListener(OnBack);
        _btnBack2.onClick.AddListener(OnBack);
        _btnUpdate.onClick.AddListener(OnUpdate);
        _originBugs = Managers.Instance.InitBugSetting.InitBugData.ToList();
        CreateFolder(_folder, _content.transform);
    }

    private void CreateFolder(UICFolder origin, Transform parent)
    {
        var settings = Managers.Instance.InitBugSetting.InitBugData;

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
    }

    private void OnExitFolder(UICFolder exitFolder)
    {
        foreach (var folder in _folders)
        {
            if (folder == exitFolder)
                continue; 

            folder.gameObject.SetActive(true);
        }
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

    private void OnUpdate()
    {
        var datas = Managers.Instance.InitBugSetting.InitBugData;
        bool noModified = true;
        foreach (var data in datas)
        {
            if (data.IsBug != _originBugs.Find(x => x.Type == data.Type).IsBug)
            {
                noModified = false;
                break;
            }
        }

        if (noModified)
        {
            return;
        }
    }
}
