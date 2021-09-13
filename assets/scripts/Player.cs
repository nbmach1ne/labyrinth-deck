using Godot;

namespace LabyrinthDeck
{
    public class Player : MovingEntity
    {
        [Signal]
        delegate void GameOver();
        
        public void OnAreaEntered(Area area)
        {
            GD.Print("AYAYAYAYAYAYAYA");
            EmitSignal(nameof(GameOver));
        }
    }
}