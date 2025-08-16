using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine.Events;
using Unity.VisualScripting;
using Cysharp.Threading.Tasks;
using System;

namespace MorningBird.UI
{
    public class TMProController : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _textMesh;
        [SerializeField] string _debugField;

        public UnityEvent OnStartEvent;
        public UnityEvent OnEnableEvent;

        Task _currentTask;
        CancellationTokenSource _currentTaskCTS;
        bool _isCancle;
        int _delayTime;

        public bool IsTaskDone
        {
            get
            {
                if(_currentTask == null)
                {
                    return true;
                }
                else
                {
                    if(_currentTask.IsCompleted == true)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

    #if UNITY_EDITOR
        [SerializeField] bool _doUpdate;
        [SerializeField] bool _textShowImediatly;
        [SerializeField] bool _cancellation;
        [SerializeField] bool _setTaskAsNull;

        void Update()
        {

            if(_setTaskAsNull == true)
            {
                _setTaskAsNull = false;
                _currentTask = null;
            }

            if(_doUpdate == true)
            {
                _doUpdate = false;
                if (_currentTask == null)
                {
                    ShowText(_debugField, 200);
                }
            }

            if(_cancellation == true)
            { 
                if( _currentTaskCTS != null)
                {
                    _currentTaskCTS.Cancel();
                    _cancellation = false;
                }
            }

            if (_textShowImediatly == true)
            {
                _textShowImediatly = false;
                ShowTextImediatly();
            }
        }
#endif

        private void Start()
        {
            if(OnStartEvent != null)
            {
                OnStartEvent.Invoke();
            }
        }

        private void OnEnable()
        {
            if(OnEnableEvent != null)
            {
                OnEnableEvent.Invoke();
            }
        }

        #region Contorl Text String

        public void ShowTextImediatly()
        {
            ResetCurrentTask();
            
            _textMesh.text = _debugField;
        }

        public void ShowTextImediatly(string text)
        {
            ResetCurrentTask();

            _debugField = text;
            _textMesh.text = text;
        }

        public void ShowTextForEvent(string text) => ShowText(text);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="speed">Lower is much fatser. This use delay for showing text.</param>
        public async void ShowText(string text, int delayEachLatter = 10)
        {
            
            if(_currentTask != null)
            {
                ResetCurrentTask();
                await UniTask.Delay(_delayTime + 2);
            }


            _delayTime = delayEachLatter;

            _debugField = text;
            _currentTaskCTS = new CancellationTokenSource();
            await (_currentTask = ShowTextTask(delayEachLatter, _currentTaskCTS.Token));

        }

        private async Task ShowTextTask(int delayEachLatter, CancellationToken token)
        {
            _textMesh.text = _debugField[0].ToString();

            for (int i = 1; i < _debugField.Length; i++)
            {
                await Task.Delay(delayEachLatter);
                if (token.IsCancellationRequested == true)
                {
                    
                    break;
                }

                _textMesh.text = _textMesh.text.Insert(_textMesh.text.Length, _debugField[i].ToString());

            }

        }

        public void CancleCurrentTask()
        {
            if (_currentTask != null && _currentTaskCTS != null)
            {
                _currentTaskCTS.Cancel();
                _currentTaskCTS.Dispose();
                _currentTaskCTS = null;
            }
        }

        void ResetCurrentTaskCancellationTokenSourceAsync()
        {
            if( _currentTaskCTS != null )
            {
                _currentTaskCTS.Cancel();
                _currentTaskCTS.Dispose();
                _currentTaskCTS = null;
            }
        }

        void ResetCurrentTask()
        {
            ResetCurrentTaskCancellationTokenSourceAsync();


            if (_currentTask != null)
            {
                _currentTask = null;
            }
        }

        #endregion

        #region Change Text Alpha

        public void ChangeTextAlpharForEvent(bool alphaIncrease) => ChangeTextAlpha(alphaDecreasing : !alphaIncrease);

        public void ChangeTextAlpha(float changeSpeed = 0.5f, float waitTimeBeforeStart = 1f, bool alphaDecreasing = true)
        {
            StartCoroutine(ChangeAlpha(changeSpeed, waitTimeBeforeStart, alphaDecreasing));
        }

        IEnumerator ChangeAlpha(float changeSpeed, float waitTimeBeforeStart, bool alphaDecreasing)
        {
            yield return new WaitForSeconds(waitTimeBeforeStart);

            if (alphaDecreasing == true)
            {
                while (_textMesh.color.a >= 0f)
                {
                    yield return new WaitForFixedUpdate();

                    _textMesh.color = new Color(_textMesh.color.r, _textMesh.color.g, _textMesh.color.b, _textMesh.color.a - changeSpeed * Time.deltaTime);
                }
            }
            else
            {
                while (_textMesh.color.a <= 1f)
                {
                    yield return new WaitForFixedUpdate();

                    _textMesh.color = new Color(_textMesh.color.r, _textMesh.color.g, _textMesh.color.b, _textMesh.color.a + changeSpeed * Time.deltaTime);
                }
            }

        }

        #endregion



    }

}

