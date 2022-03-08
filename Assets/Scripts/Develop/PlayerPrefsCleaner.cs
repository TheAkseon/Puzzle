using UnityEngine;

namespace Develop
{
    public class PlayerPrefsCleaner : MonoBehaviour
    {
        private void Start()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }
    }
}
