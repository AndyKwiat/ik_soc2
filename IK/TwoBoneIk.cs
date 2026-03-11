using System;

namespace ik;

public struct TwoBoneIkResult
{
    public Vec3 Root;
    public Vec3 Mid;
    public Vec3 End;
    public bool TargetWasClamped;

    public TwoBoneIkResult(Vec3 root, Vec3 mid, Vec3 end, bool targetWasClamped)
    {
        Root = root;
        Mid = mid;
        End = end;
        TargetWasClamped = targetWasClamped;
    }
}

public static class TwoBoneIk
{
    /*
     Solves a simple 3D two-bone IK chain and returns the three joint positions:
       Root -> Mid -> End

     Inputs:
       - rootPosition: start joint position (shoulder / hip)
       - targetPosition: desired end position (hand / foot)
       - poleVector: preferred bend direction (where elbow / knee should point)
       - bone1Length: length from Root to Mid
       - bone2Length: length from Mid to End

     How it works:
       1. Build the direction from root to target.
       2. Clamp the target distance to the reachable range [|A-B|, A+B].
       3. Use the pole vector to define the bend plane.
       4. Use triangle math (law of cosines) to place the mid joint in that plane.
       5. Place the end joint on the clamped target position.

     Notes:
       - The pole vector is NOT the hinge axis; it just chooses the bend plane.
       - If the target is unreachable, the solver returns the closest reachable pose.
       - If the pole vector is parallel to the target direction, a fallback perpendicular is chosen.
    */
    public static TwoBoneIkResult Solve(
        Vec3 rootPosition,
        Vec3 targetPosition,
        Vec3 poleVector,
        float bone1Length,
        float bone2Length)
    {
        const float epsilon = 1e-6f;

        Vec3 toTarget = targetPosition - rootPosition;
        float targetDistance = toTarget.Length();

        // Handle degenerate case where target is at the root.
        Vec3 forward = targetDistance > epsilon
            ? toTarget / targetDistance
            : new Vec3(1, 0, 0);

        float minReach = MathF.Abs(bone1Length - bone2Length);
        float maxReach = bone1Length + bone2Length;

        float clampedDistance = Clamp(targetDistance, minReach, maxReach);
        bool targetWasClamped = MathF.Abs(clampedDistance - targetDistance) > 1e-5f;

        Vec3 clampedTargetPosition = rootPosition + forward * clampedDistance;

        // Project pole vector onto plane perpendicular to forward.
        Vec3 bendDirection = poleVector - forward * Vec3.Dot(poleVector, forward);

        if (bendDirection.LengthSquared() < epsilon)
        {
            bendDirection = FindAnyPerpendicular(forward);
        }
        else
        {
            bendDirection = bendDirection.Normalized();
        }

        // Triangle math:
        // x = distance along forward from root to the projection of the mid joint
        // y = perpendicular offset from that line toward bendDirection
        float x = (bone1Length * bone1Length - bone2Length * bone2Length + clampedDistance * clampedDistance)
                  / (2.0f * clampedDistance);

        float ySquared = bone1Length * bone1Length - x * x;
        float y = ySquared > 0.0f ? MathF.Sqrt(ySquared) : 0.0f;

        Vec3 midPosition = rootPosition + forward * x + bendDirection * y;
        Vec3 endPosition = clampedTargetPosition;

        return new TwoBoneIkResult(rootPosition, midPosition, endPosition, targetWasClamped);
    }

    private static float Clamp(float value, float min, float max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }

    private static Vec3 FindAnyPerpendicular(Vec3 v)
    {
        // Pick a helper axis that is not too parallel to v.
        Vec3 helper = MathF.Abs(v.X) < 0.9f ? new Vec3(1, 0, 0) : new Vec3(0, 1, 0);
        return Vec3.Cross(v, helper).Normalized();
    }
}
