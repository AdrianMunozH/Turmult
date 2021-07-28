using System;
using DG.Tweening;
using UnityEngine;
using ValueObjects;

namespace Ui
{
    public abstract class AnimateAbstract : MonoBehaviour
    {
    
        public BoolObject boolObject;
		
        public float inDuration = 0.5f;
        public float outDuration = 0.5f;
        //DOTween
        public Ease easeIn = Ease.OutElastic;
        public Ease easeOut = Ease.InOutCubic;

        protected RectTransform rectTransform;
        protected Tween tween;

        public event Action Opened;
        public event Action Closed;

        protected virtual void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        private void Start()
        {
            if (boolObject != null)
            {
                SetToDefault();
            }
        }

        private void OnEnable()
        {
            if (boolObject != null)
            {
                boolObject.ValueChangedTo += OnValueChangedTo;
            }
        }
		
        private void OnDisable()
        {
            if (boolObject != null)
            {
                boolObject.ValueChangedTo -= OnValueChangedTo;
            }
        }

        private void OnValueChangedTo(bool value)
        {
            if (value)
            {
                Open();
                tween.OnComplete(() => Opened?.Invoke());
            }
            else
            {
                Close();
                tween.OnComplete(() => Closed?.Invoke());
            }
        }

        protected abstract void SetToDefault();

        protected abstract void OpenAnimation();
		
        protected abstract void CloseAnimation();

        public void Open()
        {
            tween.Kill();
            OpenAnimation();
        }
		
        public void Close()
        {
            tween.Kill();
            CloseAnimation();
        }
    }
}
