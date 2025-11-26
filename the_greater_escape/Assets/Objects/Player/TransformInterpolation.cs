using UnityEngine;

public abstract class TransformInterpolation
{
    private float time = 0.0f;

    public virtual Vector3 InterpolatedPosition(float time)
    {
        return Vector3.zero;
    }

    public virtual Matrix4x4 InterpolatedRotation(float time)
    {
        return Matrix4x4.identity;
    }

    public abstract void Finish(Player player);

    public bool Step(Player player, float deltaTime)
    {
        time += deltaTime;

        bool finished = false;
        if (time >= 1.0f)
        {
            Finish(player);
            time = 0.0f;
            finished = true;
        }
        var rotation = player.player_transform.Rotation() * InterpolatedRotation(time);
        var position = player.player_transform.Position() + InterpolatedPosition(time);

        player.transform.localRotation = rotation.rotation;
        player.transform.localPosition = position;

        return finished;
    }
}

public class RotateLeftInterpolation : TransformInterpolation
{
    public override void Finish(Player player)
    {
        player.player_transform.forward = -player.player_transform.calc_right();
    }
}

public class RotateRightInterpolation : TransformInterpolation
{
    public override void Finish(Player player)
    {
        player.player_transform.forward = player.player_transform.calc_right();
    }
}

public class RotateDownInterpolation : TransformInterpolation
{
    public override void Finish(Player player)
    {
        var down = -player.player_transform.up;
        player.player_transform.up = player.player_transform.forward;
        player.player_transform.forward = down;
    }
}

public class RotateUpInterpolation : TransformInterpolation
{
    public override void Finish(Player player)
    {
        var backward = -player.player_transform.forward;
        player.player_transform.forward = player.player_transform.up;
        player.player_transform.up = backward;
    }
}

public class MoveForwardInterpolation : TransformInterpolation
{
    public override void Finish(Player player)
    {
        player.player_transform.grid_position += player.player_transform.forward;
    }
}

public class BumpWallInterpolation : TransformInterpolation
{
    public override void Finish(Player player)
    {
    }
}