using UnityEngine;

public abstract class TransformInterpolation
{
    private float time = 0.0f;

    public float Time() { return time; }

    public virtual float Factor() { return 1.0f; }

    public virtual Vector3 InterpolatedPosition(PlayerTransform player_transform, float time)
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
        time += deltaTime * Factor();

        bool finished = false;
        if (time >= 1.0f)
        {
            Finish(player);
            time = 0.0f;
            finished = true;
        }
        var rotation = player.player_transform.Rotation() * InterpolatedRotation(time);
        var position = player.player_transform.Position() + InterpolatedPosition(player.player_transform, time);

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

    public override Matrix4x4 InterpolatedRotation(float time)
    {
        return Matrix4x4.Rotate(Quaternion.AngleAxis(-time * 90, Vector3.up));
    }
}

public class RotateRightInterpolation : TransformInterpolation
{
    public override void Finish(Player player)
    {
        player.player_transform.forward = player.player_transform.calc_right();
    }

    public override Matrix4x4 InterpolatedRotation(float time)
    {
        return Matrix4x4.Rotate(Quaternion.AngleAxis(time * 90, Vector3.up));
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

    public override Matrix4x4 InterpolatedRotation(float time)
    {
        return Matrix4x4.Rotate(Quaternion.AngleAxis(time * 90, Vector3.right));
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

    public override Matrix4x4 InterpolatedRotation(float time)
    {
        return Matrix4x4.Rotate(Quaternion.AngleAxis(-time * 90, Vector3.right));
    }
}

public class MoveForwardInterpolation : TransformInterpolation
{
    public override void Finish(Player player)
    {
        player.player_transform.grid_position += player.player_transform.forward;
    }

    public override Vector3 InterpolatedPosition(PlayerTransform player_transform, float time)
    {
        return (Vector3)player_transform.forward * time;
    }
}

public class BumpWallInterpolation : TransformInterpolation
{

    public override float Factor() { return 2.0f; }

    public override void Finish(Player player) { }

    public override Vector3 InterpolatedPosition(PlayerTransform player_transform, float time)
    {
        return (Vector3)player_transform.forward * Mathf.Min(time, 1.0f - time) * 0.5f;
    }
}