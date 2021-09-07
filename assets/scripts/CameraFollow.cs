using Godot;
using System;

namespace LabyrinthDeck
{
    public class CameraFollow : Spatial
    {
        [Export]
        private Vector3 _offset;

        private MovingEntity _target;

        public void Init(MovingEntity target)
        {
            _target = target;
        }

        public override void _Process(float delta)
        {
            if (_target != null)
            {
                Translation = _target.Translation + _offset;
            }
        }
    }
}
