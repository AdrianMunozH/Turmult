using UnityEngine;

namespace _48_Particle_Effect_Pack.Script
{
	public class csParticleMove : MonoBehaviour
	{
		public float speed = 0.1f;

		void Update () {
			transform.Translate(Vector3.back * speed);
		}
	}
}
