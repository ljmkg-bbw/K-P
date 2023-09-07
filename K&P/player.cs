using Godot;
using System;
using System.Numerics;
using System.Runtime.InteropServices;

public partial class player : CharacterBody2D
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;
	private AnimationTree animationTree;
	private Sprite2D Skin;
	public Godot.Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	//Wird 1 mal beim Start aufgerufen
	public override void _Ready()
	{
		GD.Print("SCRIPT START FUNKTIONIERT!");
		animationTree = GetNode<AnimationTree>("AnimationTree");
		Skin = GetNode<Sprite2D>("Skin");
		
		//animationTree.Active = true;
		//animationTree.Set("parameters/conditions/idle", true);
		animationTree.Set("parameters/conditions/start", true);
	}
	
	//updatet die Animationen
	public void UpdateAnimation()
	{
		GD.Print("DY=" + direction.Y + " : DX=" + direction.X + " | " + "VY=" + Velocity.Y + " : VX=" + Velocity.X);
		animationTree.Set("parameters/move/blend_position", direction.X);
		animationTree.Set("parameters/jump/blend_position", Velocity.Y);
		animationTree.Set("parameters/fall/blend_position", Velocity.Y);

		//prüft welche Animation gebraucht wird
		if (Velocity.Y < 0 && !IsOnFloor()) {
			animationTree.Set("parameters/conditions/is_running", false);
			animationTree.Set("parameters/conditions/fall", false);
			animationTree.Set("parameters/conditions/jump", true);
		}	else if (Velocity.Y > 0 && !IsOnFloor()) {
			animationTree.Set("parameters/conditions/is_running", false);
			animationTree.Set("parameters/conditions/jump", true);
			animationTree.Set("parameters/conditions/fall", true);
		} else {
			animationTree.Set("parameters/conditions/jump", false);
			animationTree.Set("parameters/conditions/fall", false);
			animationTree.Set("parameters/conditions/is_running", true);
		}
		
		//dreht den Charackter in die richtige Richtung
		if (direction.X == 0) {

		} else if (direction.X > 0) {
			Skin.FlipH = false;
			Skin.Position = new Godot.Vector2(7, 0);
		} else {
			Skin.FlipH = true;
			Skin.Position = new Godot.Vector2(-7, 0);
		}
	}

	//läuft die ganze Zeit
	public override void _PhysicsProcess(double delta)
	{
		Godot.Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y += gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			velocity.Y = JumpVelocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		//Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		if (direction != Godot.Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();

		UpdateAnimation();
	}
}
