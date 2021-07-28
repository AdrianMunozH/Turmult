
using System;
using DG.Tweening;
using ExtensionMethods;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using ValueObjects;

namespace Ui
{


    public class TweenButtonScaleOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public event Action Opened;
        public event Action Closed;
        public BoolObject boolObject;
        protected RectTransform rectTransform;
        protected Tween tween;

        public float inDuration = 0.5f;
        public float outDuration = 0.5f;
        public Ease easeIn = Ease.OutElastic;
        public Ease easeOut = Ease.InOutCubic;
        public Color32 standard = new Color32(88,243,168,255);
        public Color32 highlighted = new Color32(242,137,63,255);
        private TextMeshProUGUI text;


        
        protected virtual void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            text = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        }

        protected void OpenAnimation()
        {
            rectTransform.localScale = rectTransform.localScale.With(x: 1);
            tween = rectTransform.DOScaleX(1.05f, inDuration).SetEase(easeIn);
            
            text.fontStyle = FontStyles.Underline;
            text.color = highlighted;
        }

        protected void CloseAnimation()
        {
            rectTransform.localScale = rectTransform.localScale.With(x: 1);
            tween = rectTransform.DOScaleX(1, outDuration).SetEase(easeOut); 
            text.fontStyle = FontStyles.Normal;
            text.color = standard;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            boolObject.ValueChangedTo += OnValueChangedTo;
            boolObject.RuntimeValue = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            boolObject.RuntimeValue = false;
            boolObject.ValueChangedTo -= OnValueChangedTo;
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

        public void OnClickEndAnimation()
        {
            boolObject.RuntimeValue = false;
            boolObject.ValueChangedTo -= OnValueChangedTo;
        }

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