using Godot;

namespace dodge;

public partial class Mob : RigidBody2D
{
    private const string ChildAnimation = "EnemyAM";
    private const string ChildCollision = "EnemyCL";
    private const string ChildVisibleEnabler = "EnemyVEnabler";

    private CollisionShape2D _collision;
    private AnimatedSprite2D _animate;
    private VisibleOnScreenEnabler2D _venabler;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _collision = GetNode<CollisionShape2D>(ChildCollision);
        _animate = GetNode<AnimatedSprite2D>(ChildAnimation);
        _venabler = GetNode<VisibleOnScreenEnabler2D>(ChildVisibleEnabler);

        // 随机设置动画
        var mobTypes = _animate?.GetSpriteFrames().GetAnimationNames();
        if (mobTypes != null)
        {
            _animate?.Play(mobTypes[GD.Randi() % mobTypes.Length]);
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    // 注意函数签名的正确性
    private void OnScreenExited()
    {
        ClearChildren();
        QueueFree();
    }

    // 在移动到屏幕外面时，移除子类能够让消失更平滑
    private void ClearChildren()
    {
        _animate.QueueFree();
        _collision.QueueFree();
        RemoveChild(_animate);
        RemoveChild(_collision);
    }
}