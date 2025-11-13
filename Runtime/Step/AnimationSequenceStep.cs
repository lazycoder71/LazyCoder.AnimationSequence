using System;
using UnityEngine;

namespace LazyCoder.AnimationSequencer
{
    public abstract class AnimationSequenceStep : MonoBehaviour
    {
        [Serializable]
        public enum AddType
        {
            Append = 0,
            Join = 1,
            Insert = 2,
        }
        
        // Friendly name for editor display (can be overridden)
        public virtual string DisplayName => GetType().Name;

        // Called from AnimationSequence.OnEnable to provide context/setup
        public virtual void Setup(AnimationSequence sequence) { }

        // Implement this to add tween to the provided sequence
        public abstract void AddToSequence(AnimationSequence sequence);
    }
}