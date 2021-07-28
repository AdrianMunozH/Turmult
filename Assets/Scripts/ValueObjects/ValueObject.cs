using System;
using System.Collections.Generic;
using UnityEngine;

namespace ValueObjects
{
	public abstract class ValueObjectBase : ScriptableObject
	{
		public event Action ValueChanged;
		
		protected void CallValueChanged()
		{
			ValueChanged?.Invoke();
		}
	}
	
	public abstract class ValueObject<T> : ValueObjectBase, ISerializationCallbackReceiver
	{
		[SerializeField] private T defaultValue;
		[NonSerialized] private T runtimeValue;
		[SerializeField] [TextArea] private string description;
		
		public event Action<T> ValueChangedTo;
		
		public T RuntimeValue
		{
			get => runtimeValue;
			set
			{
				if (EqualityComparer<T>.Default.Equals(value, runtimeValue))
					return;

				runtimeValue = value;
				ValueChangedTo?.Invoke(runtimeValue);
				CallValueChanged();
			}
		}
	
		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			ResetValue();
		}

		public void ResetValue()
		{
			RuntimeValue = defaultValue;
		}
	}
}