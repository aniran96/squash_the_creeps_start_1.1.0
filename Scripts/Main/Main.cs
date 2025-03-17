using Godot;
using System;

public partial class Main : Node
{
    [Export] public PackedScene mobScene { get; private set; }
    [Export] public PathFollow3D mobSpawnLocation { get; private set; }


    private Mob mobInstance;

    public override void _Ready()
    {
        GetNode<ColorRect>("UI/GameOverBackGround").Hide();
    }

    public void OnMobSpawnTimerTimeout()
    {
        var player = GetNode<Player>("Player");
        var spawnPoint = mobSpawnLocation;
        spawnPoint.ProgressRatio = GD.Randf();
        mobInstance = mobScene.Instantiate<Mob>();
        mobInstance.InitializeMob(spawnPoint.Position, player.Position);
        AddChild(mobInstance);
        
        //connecting the squash signal
        mobInstance.MobSquashed += GetNode<ScoreLabel>("UI/ScoreLabel").OnMobSquashed;

    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_accept") && GetNode<ColorRect>("UI/GameOverBackGround").Visible == true)
        {
            GetTree().ReloadCurrentScene();
        }
    }

    public void OnPlayerHit()
    {
        GetNode<ColorRect>("UI/GameOverBackGround").Show();
        Timer mobSpawnTimer = GetNode<Timer>("MobSpawnTimer");
        mobSpawnTimer.Stop();
    }
}
