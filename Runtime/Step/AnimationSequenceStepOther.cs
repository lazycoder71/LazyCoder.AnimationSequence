using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LazyCoder.AnimationSequencer
{
    public class AnimationSequenceStepOther : AnimationSequenceStep
    {
        [HorizontalGroup("AddType")]
        [SerializeField] private AddType _addType = AddType.Append;

        [HorizontalGroup("AddType"), LabelWidth(75), SuffixLabel("Second(s)", true)]
        [ShowIf("@_addType == AnimationSequenceStep.AddType.Insert"), MinValue(0)]
        [SerializeField] private float _insertTime = 0.0f;

        [SerializeField] private AnimationSequence _other;

        public override string DisplayName => "Other Animation Sequence";

        public override void AddToSequence(AnimationSequence animationSequence)
        {
            _other.Stop();

            switch (_addType)
            {
                case AddType.Append:
                    animationSequence.Sequence.Append(_other.Sequence);
                    break;
                case AddType.Join:
                    animationSequence.Sequence.Join(_other.Sequence);
                    break;
                case AddType.Insert:
                    animationSequence.Sequence.Insert(_insertTime, _other.Sequence);
                    break;
            }
        }
    }
}