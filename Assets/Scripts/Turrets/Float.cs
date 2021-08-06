using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Turrets
{
    public class Float : MonoBehaviour
    {
        // User Inputs
        [SerializeField] float degreesPerSecond;
        [SerializeField] float amplitude;
        [SerializeField] float frequency;

        private float randomizer;
 
        // Position Storage Variables
        Vector3 posOffset = new Vector3 ();
        Vector3 tempPos = new Vector3 ();
        
        void Start () {
            // Store the starting position & rotation of the object
            posOffset = transform.localPosition;
            randomizer = Random.Range(0.5f, 2f);

        }
        
        void Update () {
            // Spin object around Y-Axis
            transform.Rotate(new Vector3(0f, Time.deltaTime * degreesPerSecond, 0f), Space.World);
 
            // Float up/down with a Sin()
            tempPos = posOffset;
            tempPos.y += Mathf.Sin (Time.fixedTime * Mathf.PI * (frequency * randomizer)) * amplitude;
 
            transform.localPosition = tempPos;
        }
    }

}
