using DG.Tweening;
using UnityEngine;

namespace LazyCoder.AnimationSequence
{
    public class AnimationSequenceStepInterval : AnimationSequenceStep
    {
        [SerializeField] private float _duration;

        [SerializeField] private bool _isPrepend;

        public override string DisplayName => "Interval";

        public override void AddToSequence(AnimationSequence animationSequence)
        {
            if (_isPrepend)
                animationSequence.Sequence.PrependInterval(_duration);
            else
                animationSequence.Sequence.AppendInterval(_duration);
        }
    }
}