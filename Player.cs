using Godot;
using System;

namespace dodge;

public partial class Player : Area2D
{
    private const string MoveUpKey = "move_up";
    private const string MoveDownKey = "move_down";
    private const string MoveLeftKey = "move_left";
    private const string MoveRightKey = "move_right";
    private const string ChildAnimation = "PlayerAM";
    private const string ChildCollision = "PlayerCL";
    private const string HorizontalAnimaKey = "walk";
    private const string VerticalAnimaKey = "up";

    [Export]
    public int Speed { get; set; } = 400;
    [Signal]
    public delegate void PlayerBodyHitEventHandler();

    public Vector2 ScreenSize;

    private CollisionShape2D _collision;
    private AnimatedSprite2D _animate;
    private float _width;
    private float _height;

    public void Start(Vector2 pos)
    {
        SetPosition(pos);
        Show();
        // 能够碰撞
        _collision.SetDisabled(false);
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ScreenSize = GetViewportRect().Size;
        Hide();
        InitializeWidthHeight();
    }

    private void InitializeWidthHeight()
    {
        // initialize width and height
        _collision = GetNode<CollisionShape2D>(ChildCollision);
        _animate = GetNode<AnimatedSprite2D>(ChildAnimation);
        if (_collision != null && _collision.Shape is CapsuleShape2D capsule)
        {
            _width = capsule.Radius * 2 * Transform.Scale.X;
            _height = capsule.Height * Transform.Scale.Y;
            Console.WriteLine("Width: " + _width + ", Height: " + _height);
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (_animate == null) return;

        var offset = GetProperVelocity(delta);
        var bias = offset.Length();
        if (bias <= 0f)
        {
            _animate.Stop();
            return;
        }

        PlayAnimation(offset);
        Movement(offset);
    }

    private void PlayAnimation(Vector2 offset)
    {
        if (offset.X != 0f)
        {
            _animate.Animation = HorizontalAnimaKey;
            // 这个是贴图翻转，如果向左移动，贴图就要翻转过来展示
            _animate.FlipH = offset.X < 0;
        }
        else
        {
            _animate.Animation = VerticalAnimaKey;
            _animate.FlipV = offset.Y > 0;
        }

        _animate.Play();
    }

    private void Movement(Vector2 offset)
    {
        // change position
        Position += offset;
        // clamp
        float xbias = _width / 2;
        float ybias = _height / 2;
        Position = new Vector2(
            x: Mathf.Clamp(Position.X, xbias, ScreenSize.X - xbias),
            y: Mathf.Clamp(Position.Y, ybias, ScreenSize.Y - ybias)
        );
    }

    private Vector2 GetProperVelocity(double delta)
    {
        var velocity = Vector2.Zero;
        var inputPressed = false;

        float actualOffset = (float)(1 * delta);
        if (Input.IsActionPressed(MoveUpKey))
        {
            inputPressed = true;
            velocity.Y -= actualOffset;
        }

        if (Input.IsActionPressed(MoveDownKey))
        {
            inputPressed = true;
            velocity.Y += actualOffset;
        }

        if (Input.IsActionPressed(MoveLeftKey))
        {
            inputPressed = true;
            velocity.X -= actualOffset;
        }

        if (Input.IsActionPressed(MoveRightKey))
        {
            inputPressed = true;
            velocity.X += actualOffset;
        }

        if (inputPressed)
        {
            velocity = velocity.Normalized() * Speed;
        }

        return velocity;
    }

    // 注意函数签名的正确性
    // 接收Player body entered的事件
    public void OnBodyEntered(Node2D _)
    {
        Hide();
        // 发出Player hit的信号, 以便其他地方能够接收
        EmitSignal(SignalName.PlayerBodyHit);
        // 由于我们无法在回调中修改物理属性，所以将属性推迟到帧结束时修改
        _collision.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
    }
}