using BonLib.DependencyInjection;
using BonLib.Events;
using Core.Runtime.Events.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Runtime.Managers
{
    public class SceneLoader : MonoBehaviour, 
        IEventHandler<LoadSceneEvent>
    {
        private EventManager m_eventManager;
        
        private void Awake()
        {
            m_eventManager = DI.Resolve<EventManager>();
        }

        private void OnEnable()
        {
            m_eventManager.AddListener<LoadSceneEvent>(this, Priority.Critical);

            var evt = new SceneLoaderReadyEvent();
            m_eventManager.SendEvent(ref evt);
        }

        private void OnDisable()
        {
            m_eventManager.RemoveListener<LoadSceneEvent>(this);
        }

        public void OnEventReceived(ref LoadSceneEvent evt)
        {
            LoadScene(evt.SceneIndex);
        }

        private void LoadScene(int index)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(index);
            
            operation.completed += (obj) =>
            {
                OnSceneLoaded(index);
            };
        }

        private void OnSceneLoaded(int sceneIndex)
        {
            var evt = new SceneLoadedEvent(sceneIndex);
            m_eventManager.SendEvent(ref evt);
        }
    }

}