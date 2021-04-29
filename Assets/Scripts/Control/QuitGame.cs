using UnityEngine;

namespace Control
{
    public class QuitGame : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetKey("escape"))
            {
                Application.Quit();
            }
        }
    }
}
