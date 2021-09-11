using Godot;

public class Start : Node
{
    private string _sceneToLoad = "res://assets/scenes/level_1.tscn";

    private Fade _fade;

    public override void _Ready()
    {
        _fade = GetNode<Fade>("menu/fade");
        _fade.Connect("FadeInFinished", this, nameof(OnFadeInFinished));
    }

    public void _on_start_button_button_down()
    {
        _fade.FadeIn();
    }

    private void OnFadeInFinished()
    {
        GetTree().ChangeScene(_sceneToLoad);
    }
}
