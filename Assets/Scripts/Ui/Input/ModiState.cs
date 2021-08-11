using Field;
using Player;
using UnityEngine;

namespace Ui.Input
{
    public abstract class ModiState
    {
        public string name;
        protected LayerMask _layerMask;
        protected PlayerStats _playerStats;
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

        public abstract void Input();

        public void SetLayerMask(LayerMask layerMask)
        {
            layerMask = _layerMask;
        }
    }
}