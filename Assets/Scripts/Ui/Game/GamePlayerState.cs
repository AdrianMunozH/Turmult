using MLAPI.Serialization;

namespace Ui
{
    public class GamePlayerState : INetworkSerializable
    {
        public ulong ClientId;
        public string PlayerName;
        public int Gold;
        public int Swamp;
        public int Forest;
        public int Moutain;
        public int Lifes;

        public GamePlayerState(ulong clientId, string playerName, int gold, int swamp, int forest, int moutain,int lifes)
        {
            ClientId = clientId;
            PlayerName = playerName;
            Gold = gold;
            Swamp = swamp;
            Forest = forest;
            Moutain = moutain;
            Lifes = lifes;
        }

        public void NetworkSerialize(NetworkSerializer serializer)
        {
            serializer.Serialize(ref ClientId);
            serializer.Serialize(ref PlayerName);
            serializer.Serialize(ref Gold);
            serializer.Serialize(ref Swamp);
            serializer.Serialize(ref Forest);
            serializer.Serialize(ref Moutain);
            serializer.Serialize(ref Lifes);
        }
    }
}