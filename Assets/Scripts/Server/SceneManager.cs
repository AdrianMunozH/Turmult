using MLAPI;
using MLAPI.SceneManagement;

namespace Server
{
    public class SceneManager : NetworkBehaviour
    {
        public override void NetworkStart()
        {
            base.NetworkStart();
            if (IsClient)
            {
                enabled = false;
                return;
            }
        }

        public void ChangeScene(string name)
        {
            NetworkSceneManager.SwitchScene(name);
        }

    }
}
