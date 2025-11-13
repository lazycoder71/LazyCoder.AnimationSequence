using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace LazyCoder.AnimationSequencer
{
    [DisallowMultipleComponent]
    public class AnimationSequence : MonoBehaviour
    {
        [Serializable, Flags]
        public enum ActionOnEnable
        {
            Restart = 1 << 1,
            Complete = 1 << 2,
            PlayForward = 1 << 3,
            PlayBackwards = 1 << 4,
        }

        [Serializable, Flags]
        public enum ActionOnDisable
        {
            Kill = 1 << 1,
            Pause = 1 << 2,
        }

        [Title("Settings")]
        [SerializeField] private bool _isAutoKill = true;

        [SerializeField] private ActionOnEnable _actionOnEnable;

        [SerializeField] private ActionOnDisable _actionOnDisable;

        [MinValue(-1)]
        [HorizontalGroup("LoopSettings")]
        [SerializeField] private int _loopCount;

        [ShowIf("@_loopCount != 0")]
        [HorizontalGroup("LoopSettings")]
        [SerializeField] private LoopType _loopType;

        [InlineButton("@_isIndependentUpdate = !_isIndependentUpdate",
            Label = "@_isIndependentUpdate ? \"Independent\" : \"Timescale Based\"")]
        [SerializeField] private UpdateType _updateType = UpdateType.Normal;

        [HideInInspector]
        [SerializeField] private bool _isIndependentUpdate = false;

        [SerializeField] private float _delay;

        private Sequence _sequence;

        private RectTransform _rectTransform;

        private Graphic _graphic;

        private Transform _transform;

        private GameObject _gameObject;

        // Cache of discovered steps (not serialized)
        private AnimationSequenceStep[] _steps = Array.Empty<AnimationSequenceStep>();

        public Transform Transform
        {
            get
            {
                if (_transform == null)
                    _transform = transform;

                return _transform;
            }
        }

        public GameObject GameObject
        {
            get
            {
                if (_gameObject == null)
                    _gameObject = gameObject;

                return _gameObject;
            }
        }

        public RectTransform RectTransform
        {
            get
            {
                if (_rectTransform == null)
                    _rectTransform = GetComponent<RectTransform>();

                return _rectTransform;
            }
        }

        public Graphic Graphic
        {
            get
            {
                if (_graphic == null)
                    _graphic = GetComponent<Graphic>();

                return _graphic;
            }
        }

        public Sequence Sequence
        {
            get
            {
                InitSequence();

                return _sequence;
            }
        }

        public UpdateType UpdateType
        {
            get => _updateType;
            set => _updateType = value;
        }

        public ActionOnEnable OnEnableAction
        {
            get => _actionOnEnable;
            set => _actionOnEnable = value;
        }

        public ActionOnDisable OnDisableAction
        {
            get => _actionOnDisable;
            set => _actionOnDisable = value;
        }

        #region MonoBehaviour

        private void OnDestroy()
        {
            _sequence?.Kill();
        }

        private void OnEnable()
        {
            // Discover and setup step components
            _steps = DiscoverSteps();
            for (int i = 0; i < _steps.Length; i++)
                _steps[i].Setup(this);

            // If no action flagged on enable, we shouldn't initialize sequence
            if (_actionOnEnable != 0)
                InitSequence();

            if (_actionOnEnable.HasFlag(ActionOnEnable.Complete))
                _sequence?.Complete();

            if (_actionOnEnable.HasFlag(ActionOnEnable.Restart))
                _sequence?.Restart();

            if (_actionOnEnable.HasFlag(ActionOnEnable.PlayForward))
                _sequence?.PlayForward();

            if (_actionOnEnable.HasFlag(ActionOnEnable.PlayBackwards))
                _sequence?.PlayBackwards();
        }

        private void OnDisable()
        {
            if (_actionOnDisable.HasFlag(ActionOnDisable.Pause))
                _sequence?.Pause();

            if (_actionOnDisable.HasFlag(ActionOnDisable.Kill))
                _sequence?.Kill();
        }

        #endregion

        // Made public so the custom editor can query current components
        public AnimationSequenceStep[] DiscoverSteps()
        {
            return GetComponents<AnimationSequenceStep>();
        }

        private void InitSequence()
        {
            // fixed null-check to avoid NullReferenceException
            if (_sequence != null && _sequence.IsActive())
                return;

            _sequence?.Kill();
            _sequence = DOTween.Sequence();

            // Refresh discovered steps to ensure latest order/components
            _steps = DiscoverSteps();
            for (int i = 0; i < _steps.Length; i++)
            {
                _steps[i].AddToSequence(this);
            }

            _sequence.SetAutoKill(_isAutoKill);

            _sequence.SetLoops(_loopCount, _loopType);

            _sequence.SetDelay(_delay);

            _sequence.SetUpdate(_updateType, _isIndependentUpdate);
        }

        [ButtonGroup(Order = -1, ButtonHeight = 25)]
        [Button(Name = "", Icon = SdfIconType.PlayFill)]
        public void Play()
        {
            InitSequence();

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                _sequence?.Restart();

                DG.DOTweenEditor.DOTweenEditorPreview.PrepareTweenForPreview(_sequence);
                DG.DOTweenEditor.DOTweenEditorPreview.Start();

                return;
            }
#endif

            _sequence?.Play();
        }

        public void Restart()
        {
            InitSequence();

            _sequence?.Restart();
        }

        public void Kill()
        {
            _sequence?.Kill();
            _sequence = null;
        }

#if UNITY_EDITOR

        [ButtonGroup]
        [Button(Name = "", Icon = SdfIconType.SkipStartFill)]
        public void PlayBackward()
        {
            _sequence?.PlayBackwards();
        }

        [ButtonGroup]
        [Button(Name = "", Icon = SdfIconType.SkipEndFill)]
        public void PlayForward()
        {
            _sequence?.PlayForward();
        }

        [ButtonGroup]
        [Button(Name = "", Icon = SdfIconType.StopFill)]
        public void Stop()
        {
            DG.DOTweenEditor.DOTweenEditorPreview.Stop(true);

            _sequence?.Kill();
            _sequence = null;
        }

        [ButtonGroup]
        [Button(Name = "", Icon = SdfIconType.SkipBackwardFill)]
        private void Rewind()
        {
            _sequence?.Rewind();
        }

        [ButtonGroup]
        [Button(Name = "", Icon = SdfIconType.SkipForwardFill)]
        private void Complete()
        {
            _sequence?.Complete();
        }

        private void BeginDrawListElement(int index)
        {
            // kept for compatibility with any custom editor logic
            if (index >= 0 && index < _steps.Length)
                Sirenix.Utilities.Editor.SirenixEditorGUI.BeginBox(_steps[index].DisplayName);
        }

        private void EndDrawListElement(int index)
        {
            Sirenix.Utilities.Editor.SirenixEditorGUI.EndBox();
        }

#endif
    }
}