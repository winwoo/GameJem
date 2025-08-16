using Cysharp.Threading.Tasks;
using MorningBird.UI;
using UnityEngine;

namespace ScenarioScene
{
    [System.Serializable]
    public class TimeTextCollaborate
    {
        [SerializeField] public float WaitTimeForNextText;
        [SerializeField] public string Text;
    }
    
    public class ScenarioSceneManager : MonoBehaviour
    {
        [SerializeField] TMProController _tmProController;
        
        [SerializeField] TimeTextCollaborate[] _timeTextCollaborates;

        async void Start()
        {

            for (int i = 0; i < _timeTextCollaborates.Length; i++)
            {
                _tmProController.ShowText(_timeTextCollaborates[i].Text);
                await UniTask.WaitForSeconds(_timeTextCollaborates[i].WaitTimeForNextText);
            }
            
            GoNextScene();
        }

        void GoNextScene()
        {
            
        }
        
        
        

    }
}
