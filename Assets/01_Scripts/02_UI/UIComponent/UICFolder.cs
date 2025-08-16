using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICFolder : MonoBehaviour
{
    [SerializeField]
    [Link]
    private UICFile _file;
    [SerializeField]
    [Link]
    private Image _imgSelect;
    [SerializeField]
    [Link]
    private TextMeshProUGUI _textName;
    [SerializeField]
    [Link]
    private Button _btnIcon;
    [SerializeField]
    [Link]
    private Transform _fileContent;
    [SerializeField]
    private List<UICFile> _files;

    private InitBugTypeData _data;
    private Action<UICFolder> _onClickFolder;
    private Action<UICFolder> _onExitFolder;
    public int Index { get; private set; }
    public void InitFolder(InitBugTypeData data, Action<UICFolder> onClickFolder, int index)
    {
        _data = data;
        _btnIcon.onClick.AddListener(OnClickFolder);
        CreateFile(_file, 2, _fileContent);
        _textName.text = data.FolderName;
        _imgSelect.gameObject.SetActive(false); // 선택 이미지 비활성화
        _onClickFolder = onClickFolder;
    }

    private void CreateFile(UICFile origin, int count, Transform parent)
    {
        // 원본 _file을 복제하여 새로운 UICFile 인스턴스를 생성
        for (int i = 0; i < count; i++)
        {
            UICFile newFile = Instantiate(origin, transform);
            newFile.name = i == 0 ? "정상" : "버그";
            newFile.transform.SetParent(parent, false); // 부모 설정
            newFile.gameObject.SetActive(false); // 활성화
            newFile.InitFile(i == 0 ? _data.NormalCodeImage : _data.BugCodeImage, $"{i + 1}");
            _files.Add(newFile);
        }
    }

    public void EnterFolder(Action<UICFolder> onExitFolder)
    {
        foreach (var file in _files)
        {
            file.gameObject.SetActive(true); // 파일 활성화
        }
        _onExitFolder = onExitFolder;
        SetActiveFolderComponents(false);
    }

    public void ExitFolder()
    {
        foreach (var file in _files)
        {
            file.gameObject.SetActive(false); // 파일 비활성화
        }
        SetActiveFolderComponents(true);
        _onExitFolder?.Invoke(this);
        _onExitFolder = null;
    }

    public void SetSelect(bool active)
    {
        _imgSelect.gameObject.SetActive(active);
    }

    private void OnClickFolder()
    {
        _onClickFolder.Invoke(this);
    }

    private void SetActiveFolderComponents(bool active)
    {
        _imgSelect.gameObject.SetActive(active); // 선택 이미지 활성화
        _textName.gameObject.SetActive(active); // 폴더 이름 텍스트 활성화
        _btnIcon.gameObject.SetActive(active); // 폴더 아이콘 버튼 활성화
    }
}
