using BonLib.Managers;
using Core.Runtime.Events.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Runtime.Managers
{

    public class UIManager : Manager<UIManager>
    {
        [SerializeField]
        private Button m_spinButton;

        public override void Initialize()
        {
            base.Initialize();
            
            m_spinButton.onClick.AddListener(Spin);
        }

        public void Spin()
        {
            var evt = new SpinButtonPressedEvent();
            EventManager.SendEvent(ref evt);
        }
    }

}