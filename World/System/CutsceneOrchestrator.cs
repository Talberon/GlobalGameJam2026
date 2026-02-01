using Godot;
using System;

public partial class CutsceneOrchestrator : Node3D
{
	[ExportGroup("Points of Interest")] [Export]
	public Npc Romeo;

	[Export] public Npc Juliet;
	
	[Export] public Node3D WestStairs;
	[Export] public Node3D EastStairs;
	
	[Export] public Node3D Upstairs;
	//TODO: Balcony

	public void PlayIdentifyTargetsCutscene()
	{
		//TODO Use Tweens to move camera and then release to character camera
	}

	public void PlaySunRisesInEastCutscene()
	{
		//TODO Use Tweens to move camera and then release to character camera
	}

	public void PlayMoonRisesInWestCutscene()
	{
		//TODO Use Tweens to move camera and then release to character camera
	}

	public void PlayEndingCutscene()
	{
		//TODO Use Tweens to move camera and end the game
	}
	
}
