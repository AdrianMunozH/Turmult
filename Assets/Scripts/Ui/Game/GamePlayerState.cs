using MLAPI.Serialization;

namespace Ui
{
    public struct GamePlayerState : INetworkSerializable
    {
        public ulong ClientId;
        public string PlayerName;
        public int Lifes;

        public GamePlayerState(ulong clientId, string playerName, int lifes)
        {
            ClientId = clientId;
            PlayerName = playerName;
            Lifes = lifes;
        }

        public void NetworkSerialize(NetworkSerializer serializer)
        {
            serializer.Serialize(ref ClientId);
            serializer.Serialize(ref PlayerName);
            serializer.Serialize(ref Lifes);
        }
    }
}