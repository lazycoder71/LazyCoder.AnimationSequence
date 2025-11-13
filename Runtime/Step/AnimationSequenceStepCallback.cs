using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace LazyCoder.AnimationSequencer
{
    public class AnimationSequenceStepCallback : AnimationSequenceStep
    {
        [HorizontalGroup("AddType")]
        [SerializeField] private AddType _addType = AddType.Append;

        [HorizontalGroup("AddType"), LabelWidth(75), SuffixLabel("Second(s)", true)]
        [ShowIf("@_addType == AnimationSequenceStep.AddType.Insert"), MinValue(0)]
        [SerializeField] private float _insertTime = 0.0f;

        [SerializeField] private UnityEvent _callback;

        public override string DisplayName => "Callback";

        public override void AddToSequence(AnimationSequence animationSequence)
        {
            switch (_addType)
            {
                case AddType.Append:
                    animationSequence.Sequence.AppendCallback(() => { _callback?.Invoke(); });
                    break;

                case AddType.Join:
                    animationSequence.Sequence.JoinCallback(() => { _callback?.Invoke(); });
                    break;

                case AddType.Insert:
                    animationSequence.Sequence.InsertCallback(_insertTime, () => { _callback?.Invoke(); });
                    break;
            }
        }
    }
}