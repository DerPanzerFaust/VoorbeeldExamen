using UnityEngine;

namespace MenuHandler
{
    public class Menu : MonoBehaviour
    {
        //-------------------Private-------------------//
        [SerializeField]
        private string _name;
        [SerializeField]
        private bool _open;
        
        //-------------------Public-------------------//
        public bool Open
        {
            get => _open;
            set => _open = value;
        }
        
        //-------------------Functions-------------------//
        /// <summary>
        /// Opent huidig menu aangeroepen door gedrukte knop.
        /// </summary>
        public void OpenMenu()
        {
            Open = true;
            gameObject.SetActive(true);
        }
        /// <summary>
        /// Sluit het huidige menu.
        /// </summary>
        public void CloseMenu()
        {
            Open = false;
            gameObject.SetActive(false);
        }

    }
}
