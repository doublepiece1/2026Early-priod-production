using UnityEngine;

namespace kounosuke
{
    public class ShutDown : MonoBehaviour
    {
        public void ShutDownGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
        }
    }
}