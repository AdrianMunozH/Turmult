using DG.Tweening;
using ExtensionMethods;
namespace Ui
{
    public class ScaleXOnBoolAnimation : AnimateAbstract
    {
        protected override void SetToDefault()
        {
            if (boolObject.RuntimeValue == false)
            {
                rectTransform.localScale = rectTransform.localScale.With(x: 0);
            }
        }
         
        protected override void OpenAnimation()
        {
            rectTransform.localScale = rectTransform.localScale.With(x: 0);
            tween = rectTransform.DOScaleX(1, inDuration).SetEase(easeIn);
        }
     		    
        protected override void CloseAnimation()
        {
            tween = rectTransform.DOScaleX(0, outDuration).SetEase(easeOut);
        }
    }
}
