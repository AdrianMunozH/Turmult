using System;
using Field;
using UnityEngine;

namespace Turrets
{
    [CreateAssetMenu(fileName = "Turret", menuName = "SO/Turret/Turret", order = 0)]
    public class TurretScriptableObject : ScriptableObject
    {
        [Header("Stats")] public float cost;
        public float health;
        public float mana;
        public float range = 1f;
        public float fireRate = 1f;
        public float turnSpeed = 10f;
        private float fireCountdown = 0f;
        public TurretType turretType; 
        public string enemyTag = "Enemy";
        public Ressource.RessourceType ressourceType;

        [Header("Rotation und Firepoint")]
        public bool canRotate;

        [Header("UI / Description")] public String name;
        [Multiline] public String beschreibung;
        public Sprite logo;
    }
}