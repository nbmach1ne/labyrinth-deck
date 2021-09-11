using Godot;
using System;

public class Fade : ColorRect
{
    [Signal]
    delegate void FadeInFinished();
    [Signal]
    delegate void FadeOutFinished();

    private AnimationPlayer _anim;

    public override void _Ready()
    {
        _anim = GetNode<AnimationPlayer> ("anim");
    }

    public void FadeIn()
    {
        _anim.Play("fade_in");
    }

    public void FadeOut()
    {
        _anim.Play("fade_out");
    }

    public void OnFadeInFinished()
    {
        EmitSignal(nameof(FadeInFinished));
    }

    public void OnFadeOutFinished()
    {
        EmitSignal(nameof(FadeOutFinished));
    }
}
