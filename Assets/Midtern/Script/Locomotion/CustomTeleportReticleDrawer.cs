using Oculus.Interaction;
using Oculus.Interaction.Input;
using Oculus.Interaction.Locomotion;
using Oculus.Interaction.DistanceReticles;
using UnityEngine;

namespace IDVR_Midterm
{
    public class CustomTeleportReticleDrawer : InteractorReticle<ReticleDataTeleport>
    {
        public Transform Player;
        private Quaternion PlayerRotate;
        [SerializeField]
        private TeleportInteractor _interactor;

        [SerializeField]
        private Renderer[] _validTargetRenderers;
        [SerializeField]
        private Renderer[] _invalidTargetRenderers;
        [SerializeField]
        private Transform _controller;

        [SerializeField, Interface(typeof(IAxis1D))]
        private UnityEngine.Object _progress;
        private IAxis1D Progress;

        [SerializeField, Interface(typeof(IActiveState))]
        private UnityEngine.Object _highlightState;
        private IActiveState HighlightState;

        protected override IInteractorView Interactor { get; set; }

        protected override Component InteractableComponent => _interactor.Interactable;

        private static readonly int _progressKey = Shader.PropertyToID("_Progress");
        private static readonly int _highlightKey = Shader.PropertyToID("_Highlight");

        protected virtual void Awake()
        {
            Progress = _progress as IAxis1D;
            HighlightState = _highlightState as IActiveState;
            Interactor = _interactor;
        }

        protected override void Start()
        {
            this.BeginStart(ref _started, () => base.Start());
            this.AssertField(_interactor, nameof(_interactor));

            SetRenderersEnabled(_validTargetRenderers, false);
            SetRenderersEnabled(_invalidTargetRenderers, false);

            this.EndStart(ref _started);
        }

        protected override void Align(ReticleDataTeleport data)
        {
            bool highlight = HighlightState != null && HighlightState.Active;
            data.Highlight(highlight);

            if (data.HideReticle)
            {
                return;
            }

            Vector3 position = data.ProcessHitPoint(_interactor.ArcEnd.Point);
            Quaternion rotation = Quaternion.LookRotation(_interactor.ArcEnd.Normal);
            // 鎖定 X 軸的旋轉
            Vector3 eulerRotation = rotation.eulerAngles;
            eulerRotation.x = -_controller.rotation.z; // 鎖定 X 軸旋轉
            eulerRotation.y = -OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch).eulerAngles.z;
            rotation = Player.rotation * Quaternion.Euler(eulerRotation);
            this.transform.SetPositionAndRotation(position, rotation);
            // 根據玩家看向終點的角度旋轉
            PlayerRotate = rotation;

            float progress = Progress != null ? Progress.Value() : 0f;
            bool validDestination = _interactor.HasValidDestination();

            SetRenderersEnabled(_validTargetRenderers, validDestination);
            SetRenderersEnabled(_invalidTargetRenderers, !validDestination);
            Renderer[] reticles = validDestination ? _validTargetRenderers : _invalidTargetRenderers;
            SetReticleProgress(reticles, progress);
            if (HighlightState != null)
            {
                SetReticleHighlight(reticles, highlight);
            }
        }

        protected override void Draw(ReticleDataTeleport data)
        {
        }

        protected override void Hide()
        {
            SetRenderersEnabled(_validTargetRenderers, false);
            SetRenderersEnabled(_invalidTargetRenderers, false);
            Player.rotation = PlayerRotate;
            if (_targetData != null)
            {
                _targetData.Highlight(false);
            }
        }

        private void SetRenderersEnabled(Renderer[] renderers, bool enabled)
        {
            if (renderers != null)
            {
                foreach (var renderer in renderers)
                {
                    renderer.enabled = enabled;
                }
            }
        }

        private void SetReticleProgress(Renderer[] renderers, float progress)
        {
            if (renderers != null)
            {
                foreach (var renderer in renderers)
                {
                    renderer.material.SetFloat(_progressKey, progress);
                }
            }
        }

        private void SetReticleHighlight(Renderer[] renderers, bool highlight)
        {
            if (renderers != null)
            {
                foreach (var renderer in renderers)
                {
                    renderer.material.SetFloat(_highlightKey, highlight ? 1f : 0f);
                }
            }
        }

        #region Inject

        public void InjectAllCustomTeleportReticleDrawer(TeleportInteractor interactor)
        {
            InjectInteractor(interactor);
        }

        public void InjectInteractor(TeleportInteractor interactor)
        {
            _interactor = interactor;
        }

        public void InjectOptionalValidTargetRenderers(Renderer[] validTargetRenderers)
        {
            _validTargetRenderers = validTargetRenderers;
        }
        public void InjectOptionalInvalidTargetRenderers(Renderer[] invalidTargetRenderers)
        {
            _invalidTargetRenderers = invalidTargetRenderers;
        }

        public void InjectOptionalProgress(IAxis1D progress)
        {
            _progress = progress as UnityEngine.Object;
            Progress = progress;
        }
        #endregion
    }
}