using DG.Tweening;
using UnityEngine;

namespace LazyCoder.AnimationSequencer
{
    public class AnimationSequenceStepTransformScale : AnimationSequenceStepTransform
    {
        public override string DisplayName => base.DisplayName + "Local Scale";

        protected override Tween GetTween(AnimationSequence animationSequence)
        {
            Transform owner = _isSelf ? animationSequence.Transform : _owner;

            float duration = _isSpeedBased ? Vector3.Distance(_value, owner.localScale) / _duration : _duration;

            Tweener tween = owner.DOScale(_relative ? owner.localScale + _value : _value, duration);

            if (_changeStartValue)
                tween.ChangeStartValue(_relative ? owner.localScale + _valueStart : _valueStart);
            else
                tween.ChangeStartValue(owner.localScale);
            
            owner.localScale = _relative ? owner.localScale + _value : _value;

            return tween;
        }

        protected override Tween GetResetTween(AnimationSequence animationSequence)
        {
            Transform owner = _isSelf ? animationSequence.Transform : _owner;

            return owner.DOScale(owner.localScale, 0.0f);
        }

        public override void Setup(AnimationSequence animationSequence)
        {
            Transform owner = _isSelf ? animationSequence.Transform : _owner;

            if (_changeStartValue)
                owner.localScale = _valueStart;
        }
    }
}