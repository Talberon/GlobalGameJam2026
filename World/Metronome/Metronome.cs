using Godot;
using System;

public partial class Metronome : Node3D
{
	[Export] private Color restColor = Colors.Red;
	[Export] private Color beatColor = Colors.Green;
	[Export] private int beatsPerMinute = 100;

	private float beatDelaySeconds;
	
	[Export] private MeshInstance3D mesh;

	[ExportGroup("Squash")] [Export] private float beatScaleY = 1f;
	[Export] private float restScaleY = 0.5f;

	private double nextBeat = 0;

	private ShaderMaterial shaderMat;

	private bool onBeat;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		beatDelaySeconds = 60f / beatsPerMinute;
		shaderMat = (ShaderMaterial)mesh.GetActiveMaterial(0);
		SetColorTones(shaderMat, restColor);
		nextBeat = beatDelaySeconds;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//TODO: Squash and change colour on the beat and off the beat
		nextBeat -= delta;
		if (nextBeat <= 0)
		{
			if (onBeat)
			{
				SetColorTones(shaderMat, beatColor);
				mesh.Scale = mesh.Scale with
				{
					Y = beatScaleY
				};
				GD.Print("BEAT!");
			}
			else
			{
				SetColorTones(shaderMat, restColor);
				mesh.Scale = mesh.Scale with
				{
					Y = restScaleY
				};
				GD.Print("OFF BEAT!");
			}

			nextBeat += beatDelaySeconds;
			onBeat = !onBeat;
		}
	}

	private void SetColorTones(ShaderMaterial mat, Color desiredColor, float tweenDuration = 0.1f)
	{
		//Set tones

		Color highlight = Color.FromHsv(desiredColor.H, desiredColor.S, 1.0f); // 100%
		Color mid1 = Color.FromHsv(desiredColor.H, desiredColor.S, 0.8f); // 80%
		Color mid2 = Color.FromHsv(desiredColor.H, desiredColor.S, 0.5f); // 50%
		Color shadow = Color.FromHsv(desiredColor.H, desiredColor.S, 0.3f); // 30%


		// Interpolate

		Tween tween = CreateTween();


		tween.TweenProperty(mat, "shader_parameter/highlight_color", highlight, tweenDuration)
			.SetEase(Tween.EaseType.Out)
			.SetTrans(Tween.TransitionType.Cubic);

		tween.Parallel()
			.TweenProperty(mat, "shader_parameter/mid1_color", mid1, tweenDuration)
			.SetEase(Tween.EaseType.Out)
			.SetTrans(Tween.TransitionType.Cubic);

		tween.Parallel()
			.TweenProperty(mat, "shader_parameter/mid2_color", mid2, tweenDuration)
			.SetEase(Tween.EaseType.Out)
			.SetTrans(Tween.TransitionType.Cubic);

		tween.Parallel()
			.TweenProperty(mat, "shader_parameter/shadow_color", shadow, tweenDuration)
			.SetEase(Tween.EaseType.Out)
			.SetTrans(Tween.TransitionType.Cubic);
	}
}
