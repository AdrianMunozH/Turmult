using MLAPI;

namespace Prefabs.Enemies
{
    public class TestObject : NetworkBehaviour
    {
        public override void NetworkStart()
        {
            if (!IsServer) return;
        }
    }
}
