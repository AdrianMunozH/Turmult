using Singleplayer.Field;
using UnityEngine;

namespace Singleplayer.Ui.Input
{
    public abstract class ModiState
    {
        private Resource.ResourceType _currentResource;
        private int _currentTurretIndex = 0;
        
        public string name;
        protected Player.Player _playerStats;
        
        // setter getter
        public Resource.ResourceType CurrentResource
        {
            get => _currentResource;
            set => _currentResource = value;
        }
        
        public int CurrentTurretIndex
        {
            get => _currentTurretIndex;
            set => _currentTurretIndex = value;
        }
        //
        
        public abstract void Start();
        
        public virtual void BuyMinion()
        {
            return;
        }

        public virtual void BuyTurret(HCell cell, Resource.ResourceType ressourceEnum, int turret)
        {
            return;
        }

        public virtual void AcquireField(HCell cell)
        {
            return;
        }

        public virtual void OnDestroy()
        {
            return;
        }

        public abstract void Input();



        public void SetPlayer(Player.Player player)
        {
            _playerStats = player;
        }
        
    }
}