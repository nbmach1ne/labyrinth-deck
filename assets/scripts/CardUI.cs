using Godot;

namespace LabyrinthDeck
{
    public class CardUI : TextureButton
    {
        [Export]
        private Vector2 _mouseOverScale = new Vector2(1.1f, 1.1f);
        private Card _card;

        public void SetCard(Card card, Texture sprite)
        {     
            _card = card;
            TextureNormal = sprite;
        }

        public Card GetCard()
        {
            return _card;
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