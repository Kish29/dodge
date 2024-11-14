using Godot;
using System;

public partial class Hud : CanvasLayer
{
    [Signal]
    public delegate void StartGameEventHandler();

    private Label _scoreLabel;
    private Label _messageLabel;
    private Button _startButton;
    private Timer _messageTimer;

    private void Initialize()
    {
        _scoreLabel = GetNode<Label>("ScoreLabel");
        _messageLabel = GetNode<Label>("Message");
        _startButton = GetNode<Button>("StartButton");
        _messageTimer = GetNode<Timer>("MessageTimer");
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Initialize();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void SnapShotOneMessage(string text)
    {
        _messageLabel.Text = text;
        _messageLabel.Show();
        _messageTimer.Start();
    }

    public async void ShowGameOver()
    {
        SnapShotOneMessage("Game Over");
        // 等待超时
        await ToSignal(_messageTimer, Timer.SignalName.Timeout);

        SnapShotOneMessage("Dodge the creeps!");
        await ToSignal(GetTree().CreateTimer(1.0), SceneTreeTimer.SignalName.Timeout);
        _startButton.Show();
    }

    public void UpdateScore(long score)
    {
        _scoreLabel.Text = score.ToString();
    }

    private void OnStartPressed()
    {
        _startButton.Hide();
        _messageLabel.Hide();
        _scoreLabel.Text = "0";
        EmitSignal(SignalName.StartGame);
    }

    private void OnMessageTimeout()
    {
        _messageLabel.Hide();
    }
}