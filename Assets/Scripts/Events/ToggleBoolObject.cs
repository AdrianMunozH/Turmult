using UnityEngine;
using ValueObjects;

namespace Events
{
	public class ToggleBoolObject : MonoBehaviour
	{
		public static void Toggle(BoolObject boolObject)
		{
			boolObject.RuntimeValue = !boolObject.RuntimeValue;
		}
	}
}