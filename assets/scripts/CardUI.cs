using Godot;

namespace LabyrinthDeck
{
    public class CardUI : TextureButton
    {
        [Export]
        private Vector2 _mouseOverScale = new Vector2(1.1f, 1.1f);

        [Export]
        private float _showDelay = 0.1f;

        [Signal]
        delegate void CardChoosen(int cardIndex);

        // Animation variables
        private AnimationPlayer _anim;
        private Tween _tween;
        private Control _hiddenPos;
        private Control _shownPos;
        private Control _choosenPos;

        private Card _card;

        public override void _Ready()
        {
            _anim = GetNode<AnimationPlayer> ("anim");
            _tween = GetNode<Tween>("tween");
        }

        public void SetTweenValues(Control hiddenPos, Control shownPos, Control choosenPos)
        {
            _hiddenPos = hiddenPos;
            _shownPos = shownPos;
            _choosenPos = choosenPos;

            RectGlobalPosition = _hiddenPos.RectPosition;
        }

        public void SetCard(Card card, Texture sprite)
        {     
            _card = card;
            TextureNormal = sprite;
            ShowCard();
        }

        public Card GetCard()
        {
            return _card;
        }

        public void ShowCard()
        {
            Disabled = false;
            // From _hiddenPos to _shownPos
            _tween.InterpolateProperty(this, "rect_position", _hiddenPos.RectPosition, _shownPos.RectPosition, 0.5f,
                                       Tween.TransitionType.Expo, Tween.EaseType.Out,
                                       _showDelay);
            _tween.InterpolateCallback(this, _tween.GetRuntime(), nameof(CheckIfHovered));
            _tween.Start();            
        }

        private void CheckIfHovered()
        {
            if (IsHovered())
            {
                _on_card_mouse_entered();
            }
        }

        public void HideCard()
        {
            // From _shownPos to _hiddenPos
            _tween.InterpolateProperty(this, "rect_position", _shownPos.RectPosition, _hiddenPos.RectPosition, 0.5f);
            _tween.Start();
        }

        public void ChooseCard(Control callbackNode, string callbackName, int callbackArg)
        {
            Disabled = true;
            // From _shownPos to _choosenPos
            _tween.InterpolateProperty(this, "rect_position", _shownPos.RectPosition, _choosenPos.RectPosition, 0.25f,
                                       Tween.TransitionType.Expo, Tween.EaseType.Out);
            // From current scale to _choosenPos scale
            _tween.InterpolateProperty(this, "rect_scale", RectScale, _choosenPos.RectScale, 0.25f,
                                       Tween.TransitionType.Expo, Tween.EaseType.Out);
            // Flip card
            _tween.InterpolateCallback(this, _tween.GetRuntime(), nameof(FlipCard));
            // After the flip animation go back to _hiddenPos
            _tween.InterpolateProperty(this, "rect_scale", _choosenPos.RectScale, Vector2.One, 0.25f,
                                       Tween.TransitionType.Expo, Tween.EaseType.In,
                                       0.5f);
            _tween.InterpolateProperty(this, "rect_position", _choosenPos.RectPosition, _hiddenPos.RectPosition, 0.5f,
                                       Tween.TransitionType.Expo, Tween.EaseType.In,
                                       0.5f);
            _tween.InterpolateCallback(callbackNode, _tween.GetRuntime(), callbackName, callbackArg);
            _tween.Start();
        }

        private void FlipCard()
        {
            _anim.Play("flip");
        }

        public void _on_card_mouse_entered()
        {
            GD.Print("CARD_ENTERED");
            RectScale = _mouseOverScale;
            Material.Set("shader_param/noise_scale", 2f);
        }

        public void _on_card_mouse_exited()
        {
            GD.Print("CARD_EXITED");
            RectScale = Vector2.One;
            Material.Set("shader_param/noise_scale", 0f);
        }
    }
}