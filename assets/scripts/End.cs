using Godot;
using System;

public class End : Node
{
    private string _sceneToLoad = "res://assets/scenes/start.tscn";

    private Fade _fade;

    public override void _Ready()
    {
        _fade = GetNode<Fade>("ui/fade");
        _fade.Connect("FadeInFinished", this, nameof(OnFadeInFinished));

        DelayedChangeScene();
    }

    private async void DelayedChangeScene()
    {
        await ToSignal(GetTree().CreateTimer(5f), "timeout");

        _fade.FadeIn();
    }

    private void OnFadeInFinished()
    {
        GetTree().ChangeScene(_sceneToLoad);
    }
}
