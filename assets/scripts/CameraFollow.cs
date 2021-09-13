using Godot;
using System;

namespace LabyrinthDeck
{
    public class CameraFollow : Spatial
    {
        [Export]
        private Vector3 _offset;
        [Export]
        private Vector3 _focusedOffset;
        [Export]
        private float _focusFov;

        [Signal]
        delegate void CameraFocused();

        private MovingEntity _target;
        private Camera _cam;
        private Tween _tween;

        public override void _Ready()
        {
            _cam = GetNode<Camera> ("camera");
            _tween = GetNode<Tween> ("tween");
        }

        public override void _Process(float delta)
        {
            if (_target != null)
            {
                Translation = _target.Translation + _offset;
            }
        }

        public void Init(MovingEntity target)
        {
            _target = target;
        }

        public void SubscribeToGameEvents(Game game)
        {
            game.Connect("GameOver", this, nameof(OnGameOver));
            game.Connect("WinGame", this, nameof(OnWinGame));
        }

        private void FocusOnPlayer()
        {
            _tween.InterpolateProperty(_cam, "fov", _cam.Fov, _focusFov, 1f,
                                       Tween.TransitionType.Expo, Tween.EaseType.Out);
            _tween.InterpolateProperty(this, "translation", Translation, _target.Translation + _focusedOffset, 1f,
                                       Tween.TransitionType.Expo, Tween.EaseType.Out);
            _tween.InterpolateCallback(this, _tween.GetRuntime(), nameof(OnCameraFocused));
            _tween.Start();
        }

        private void OnCameraFocused()
        {
            EmitSignal(nameof(CameraFocused));
        }

        private void OnGameOver()
        {
            SetProcess(false);
            FocusOnPlayer();
        }

        private void OnWinGame()
        {
            SetProcess(false);
            FocusOnPlayer();
        }
    }
}
