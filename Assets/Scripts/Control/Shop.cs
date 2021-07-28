using System;
using System.Collections;
using System.Collections.Generic;
using Field;
using UnityEngine;

namespace Control
{
    public class Shop : MonoBehaviour
    {
        private BuildManager buildManager;

        private void Start()
        {
            buildManager = BuildManager.instance;
        }

        //Mountain Turrets
        public void PurchaseMountainTurretOne()
        {
            Debug.Log("Mountain Turret 1 Purchased");
            buildManager.SetTurretToBuild(buildManager.mountainTurrets[0]);
        }
        public void PurchaseMountainTurretTwo()
        {
            Debug.Log("Mountain Turret 2 Purchased");
            buildManager.SetTurretToBuild(buildManager.mountainTurrets[1]);
        }
        public void PurchaseMountainTurretThree()
        {
            Debug.Log("Mountain Turret 3 Purchased");
            buildManager.SetTurretToBuild(buildManager.mountainTurrets[2]);
        }
        
        //Forest Turrets
        public void PurchaseForestTurretOne()
        {
            Debug.Log("Forest Turret 1 Purchased");
            buildManager.SetTurretToBuild(buildManager.forestTurrets[0]);
        }
        public void PurchaseForestTurretTwo()
        {
            Debug.Log("Forest Turret 2 Purchased");
            buildManager.SetTurretToBuild(buildManager.forestTurrets[1]);
        }
        public void PurchaseForestTurretThree()
        {
            Debug.Log("Forest Turret 3 Purchased");
            buildManager.SetTurretToBuild(buildManager.forestTurrets[2]);
        }
        
        //Swamp Turrets
        public void PurchaseSwampTurretOne()
        {
            Debug.Log("Swamp Turret 1 Purchased");
            buildManager.SetTurretToBuild(buildManager.swampTurrets[0]);
        }
        public void PurchaseSwampTurretTwo()
        {
            Debug.Log("Swamp Turret 2 Purchased");
            buildManager.SetTurretToBuild(buildManager.swampTurrets[1]);
        }
        public void PurchaseSwampTurretThree()
        {
            Debug.Log("Swamp Turret 3 Purchased");
            buildManager.SetTurretToBuild(buildManager.swampTurrets[2]);
        }
    }
}

