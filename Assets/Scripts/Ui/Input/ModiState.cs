using Field;
using UnityEngine;

namespace Ui.Input
{
    public abstract class ModiState 
    {
        public string name;
        protected LayerMask _layerMask;
        protected Player.Player _playerStats;
        public abstract void Start();
        
        public virtual void BuyMinion()
        {
            return;
        }

        public virtual void BuyTurret(HCell cell, Ressource.RessourceType ressourceEnum, int turret)
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

        public void SetLayerMask(LayerMask layerMask)
        {
            layerMask = _layerMask;
        }

        public void SetPlayer(Player.Player player)
        {
            _playerStats = player;
        }
    }
}