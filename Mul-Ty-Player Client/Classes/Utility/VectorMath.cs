using System;

namespace MulTyPlayerClient.Classes.Utility;

public static class VectorMath
{
    public static double CalculatePitch(double[] P1, double[] P2)
    {
        double vx = P2[0] - P1[0];
        double vy = P2[1] - P1[1];
        double vz = P2[2] - P1[2];
        return Math.Atan2(vz, Math.Sqrt(vx * vx + vy * vy));
    }

    public static double CalculateYaw(double[] P1, double[] P2)
    {
        double vx = P2[0] - P1[0];
        double vy = P2[1] - P1[1];
        return Math.Atan2(vy, vx);
    }
}