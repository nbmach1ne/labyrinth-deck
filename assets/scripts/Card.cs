using Godot;

namespace LabyrinthDeck
{
    public class Card : Object
    {
        public enum CardType
        {
            MOVE_UP,
            MOVE_DOWN,
            MOVE_LEFT,
            MOVE_RIGHT,
            DRAW_AGAIN,
            STUN_ENEMY
        }

        public bool IsMovementCard
        {
            get { return Type == CardType.MOVE_UP ||
                  Type == CardType.MOVE_DOWN ||
                  Type == CardType.MOVE_LEFT ||
                  Type == CardType.MOVE_RIGHT; }
        }

        public CardType Type;
        public int Param; 

        public Card(CardType type, int param)
        {
            Type = type;
            Param = param;
        }
    }
}