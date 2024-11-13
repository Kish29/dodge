using Godot;

namespace dodge;

public partial class Main : Node
{
    [Export]
    public PackedScene MobScene { get; set; }

    private long _score;

    private Timer _mobTimer;
    private Timer _scoreTimer;
    private Timer _startTimer;
    private Player _player;
    private Marker2D _startPos;

    private PathFollow2D _mobSpawnLine;

    private void Initialize()
    {
        _mobTimer = GetNode<Timer>("MobTimer");
        _scoreTimer = GetNode<Timer>("ScoreTimer");
        _startTimer = GetNode<Timer>("StartTimer");
        _player = GetNode<Player>("Player");
        _startPos = GetNode<Marker2D>("StartPosition");

        _mobSpawnLine = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Initialize();
        NewGame();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    // player hit的回调
    public void GameOver()
    {
        _mobTimer.Stop();
        _scoreTimer.Stop();
    }

    public void NewGame()
    {
        _score = 0;
        _player.Start(_startPos.Position);
        _startTimer.Start();
    }

    private void OnStartTimerTimeout()
    {
        _mobTimer.Start();
        _scoreTimer.Start();
    }

    private void OnMobTimerTimeout()
    {
        // 生成敌人, 这里的node实际是Mob
        var mob = MobScene.Instantiate<Mob>();
        // 设置线条上(无论什么线条)的随机一个百分比点位
        _mobSpawnLine.ProgressRatio = GD.Randf();
        // 设置敌人的位置
        mob.Position = _mobSpawnLine.Position;
        // 设置敌人的朝向
        // godot用的是弧度制，所以要让敌人超中心走，要旋转90度
        var towardsCenter = _mobSpawnLine.Rotation + Mathf.Pi / 2;
        // 随机偏移一点度数
        var towards = towardsCenter + (float)GD.RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
        mob.SetRotation(towards);
        AddChild(mob);

        // 只沿着x轴走
        mob.SetLinearVelocity(new Vector2(GD.RandRange(150, 250), 0).Rotated(towards));
    }

    private void OnScoreTimerTimeout()
    {
        _score++;
    }
}