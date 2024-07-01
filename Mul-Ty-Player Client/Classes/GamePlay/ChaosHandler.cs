using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MulTyPlayer;
using Riptide;

namespace MulTyPlayerClient;

public struct PositionData
{
    public float X;
    public float Y;
    public float Z;
    public float Yaw;

    public PositionData(float x, float y, float z, float yaw = 0)
    {
        X = x;
        Y = y;
        Z = z;
        Yaw = yaw;
    }
}

public class ChaosHandler
{
    private Dictionary<int, Dictionary<string, PositionData[]>> _positions = new()
    {
        {
            4, new Dictionary<string, PositionData[]>()
            {
                {
                    "Bilby", new PositionData[]
                    {
                        new PositionData(-3822.803f, 308.366f, 9116.404f, 0.271f),
                        new PositionData(-2041.474f, 127.447f, 8776.881f, 1.42f),
                        new PositionData(3965.84f, -720.751f, 6277.515f, 5.345f),
                        new PositionData(-1399.128f, -746.911f, 4797.485f, 5.435f),
                        new PositionData(-2243.23f, 281.237f, 5182.763f, 4.845f),
                        new PositionData(-5100.461f, -230.457f, 4854.048f, 6.066f),
                        new PositionData(2213.4f, -83.611f, 1346.88f, 4.745f),
                        new PositionData(-202.77f, -197.27f, -1607.163f, 0.56f),
                        new PositionData(4767.584f, -122.078f, -4117.393f, 4.179f),
                        new PositionData(-137.779f, 413.918f, -5278.394f, 1.517f),
                        new PositionData(2096.096f, 456.717f, -6355.519f, 2.812f),
                        new PositionData(3671.294f, 202.835f, -6686.886f, 1.846f),
                        new PositionData(-341.36f, -400.935f, -8601.748f, 3.124f),
                        new PositionData(-2893.603f, 190.135f, -2475.652f, 3.104f),
                        new PositionData(-2498.504f, 523.03f, -390.781f, 4.646f),
                        new PositionData(1470.596f, 137.234f, -349.768f, 4.621f),
                        new PositionData(1087.491f, 101.983f, 1053.854f, 4.654f),
                        new PositionData(-4881.107f, 239.09f, -2702.864f, 2.157f),
                        new PositionData(-4130.989f, -457.802f, -4515.076f, 5.115f),
                        new PositionData(-6654.193f, 209.032f, -2406.927f, 3.642f),
                        new PositionData(-5848.0f, 304.0f, -420.0f, 1.853f),
                        new PositionData(4145.0f, -325.0f, -1943.0f, 4.339f),
                        new PositionData(-1755.0f, -343.0f, 2683.0f, 0.0f),
                        new PositionData(4432.0f, -546.0f, 5122.0f, -3.131f),
                        new PositionData(-6126.0f, -441.0f, 3604.0f, 1.841f),
                    }
                },
                {
                    "Cog", new PositionData[]
                    {
                        new PositionData(3292.59f, -61.304f, 8319.632f),
                        new PositionData(584.759f, -320.606f, 6309.147f),
                        new PositionData(-1123.892f, -104.979f, 4520.949f),
                        new PositionData(-4364.604f, 112.289f, 5057.654f),
                        new PositionData(-5102.86f, -730.101f, 2923.001f),
                        new PositionData(-633.541f, -570.599f, 1979.097f),
                        new PositionData(2906.548f, -355.495f, -402.471f),
                        new PositionData(1168.19f, -369.834f, -2430.862f),
                        new PositionData(4957.117f, 125.83f, -1517.677f),
                        new PositionData(3627.655f, 402.567f, -5046.91f),
                        new PositionData(2743.771f, 518.073f, -5506.067f),
                        new PositionData(5234.447f, 242.541f, -6082.078f),
                        new PositionData(2524.417f, -410.909f, -8620.458f),
                        new PositionData(-3817.526f, -637.551f, -7775.371f),
                        new PositionData(-4688.155f, -452.385f, -8793.168f),
                        new PositionData(-1258.736f, -422.729f, -5307.798f),
                        new PositionData(-2257.78f, -227.478f, -5229.264f),
                        new PositionData(286.411f, 9.134f, -3461.567f),
                        new PositionData(-4660.678f, 1077.669f, 248.17000000000002f),
                        new PositionData(-921.605f, 386.192f, 978.722f),
                        new PositionData(-3581.13f, 162.842f, -3191.545f),
                        new PositionData(-5260.82f, -495.519f, -6405.628f),
                        new PositionData(-6598.943f, 527.825f, -2686.603f),
                        new PositionData(-8093.196f, 453.76f, -1828.842f),
                        new PositionData(-8730.219f, 471.282f, -402.16f),
                        new PositionData(-7917.592f, 469.886f, -31.727f),
                        new PositionData(-3724.774f, 401.245f, 7937.534f),
                        new PositionData(-3914.134f, 278.066f, 6686.796f),
                        new PositionData(-3101.992f, 335.863f, 9322.519f),
                        new PositionData(-1225.614f, 89.484f, 8356.682f),
                        new PositionData(2233.863f, -19.092f, 6879.222f),
                        new PositionData(1072.667f, -749.944f, 4387.642f),
                        new PositionData(262.202f, 84.015f, 4613.469f),
                        new PositionData(-3085.064f, 19.155f, 4046.9390000000003f),
                        new PositionData(-4387.075f, -443.139f, 3996.199f),
                        new PositionData(-677.791f, -25.668f, 2004.855f),
                        new PositionData(1335.622f, -298.697f, 2394.575f),
                        new PositionData(3360.647f, 120.715f, 2372.194f),
                        new PositionData(3141.592f, 377.944f, -3780.238f),
                        new PositionData(-1588.473f, -276.508f, -5987.341f),
                        new PositionData(2399.0f, 407.0f, 1326.0f),
                        new PositionData(3899.0f, 90.0f, -1837.0f),
                        new PositionData(-3339.0f, -389.0f, 2063.0f),
                        new PositionData(1182.0f, -41.0f, -2416.0f),
                        new PositionData(830.0f, 403.0f, -339.0f),
                        new PositionData(501.0f, 552.0f, -5251.0f),
                        new PositionData(-692.0f, -554.0f, 2309.0f),
                        new PositionData(-1944.0f, -333.0f, -7611.0f),
                        new PositionData(3352.0f, -553.0f, 6944.0f),
                        new PositionData(2599.0f, -268.0f, 6555.0f),
                    }
                }
            }
        },
        {
            5, new Dictionary<string, PositionData[]>()
            {
                {
                    "Bilby", new PositionData[]
                    {
                        new PositionData(-9956.699f, -1552.092f, 6924.409f, 1.63f),
                        new PositionData(-9207.075f, -1221.275f, 5635.146f, 3.422f),
                        new PositionData(-7468.986f, -1405.712f, 5743.225f, 4.5f),
                        new PositionData(-7143.794f, -1315.932f, 5645.113f, 1.296f),
                        new PositionData(-1460.342f, -2616.124f, -4034.063f, 4.503f),
                        new PositionData(-8533.444f, -1862.765f, -426.065f, 1.629f),
                        new PositionData(-5676.547f, 66.905f, -95.308f, 1.629f),
                        new PositionData(5796.938f, -3384.073f, -4376.573f, 4.888f),
                        new PositionData(-9973.629f, -1349.276f, 3455.284f, 5.304f),
                        new PositionData(-137.275f, -2686.874f, 1296.892f, 0.087f),
                        new PositionData(7850.211f, -2228.991f, 1552.121f, 1.639f),
                        new PositionData(-2025.659f, -389.882f, 9564.432f, 0.0f),
                        new PositionData(2765.938f, -3046.89f, 200.862f, 3.065f),
                        new PositionData(-2058.466f, -1282.954f, -7444.292f, 0.0f),
                        new PositionData(-7793.682f, -1762.52f, 1129.029f, 3.366f),
                        new PositionData(-8085.177f, -1457.458f, 3615.445f, 1.294f),
                        new PositionData(-9044.725f, 1039.854f, -4817.408f, 3.423f),
                        new PositionData(-6706.301f, 21.776f, -617.547f, 5.692f),
                        new PositionData(-4487.0f, -294.918f, 2015.169f, 4.833f),
                        new PositionData(-10962.784f, 762.491f, -605.935f, 3.37f),
                        new PositionData(-10339.0f, -334.0f, -2644.0f, 6.475f),
                        new PositionData(-2309.0f, -2031.0f, 5678.0f, -0.018f),
                        new PositionData(-4467.0f, -1254.0f, 978.0f, -0.976f),
                        new PositionData(562.0f, -2875.0f, 2953.0f, 0.0f),
                        new PositionData(-10701.0f, -2510.0f, -1913.0f, 3.131f),
                    }
                },
                {
                    "Cog", new PositionData[]
                    {
                        new PositionData(-8052.256f, -1091.655f, 7402.015f),
                        new PositionData(-8451.575f, -940.686f, 3905.345f),
                        new PositionData(-6063.314f, -1537.794f, 2620.495f),
                        new PositionData(-1465.395f, -1375.697f, 3961.795f),
                        new PositionData(-8201.76f, -1193.078f, 1977.67f),
                        new PositionData(-7465.405f, -1789.988f, 85.42500000000001f),
                        new PositionData(-2779.538f, -1009.154f, -6075.927f),
                        new PositionData(-1175.03f, -723.637f, -4634.981f),
                        new PositionData(-9893.771f, -1697.877f, -141.482f),
                        new PositionData(6494.381f, -3156.209f, -4728.972f),
                        new PositionData(2198.438f, -3018.295f, -663.147f),
                        new PositionData(2592.038f, -2481.875f, 3767.315f),
                        new PositionData(6968.573f, 273.944f, 6152.543f),
                        new PositionData(-1993.036f, -215.063f, 8697.775f),
                        new PositionData(1089.067f, -643.878f, 6134.324f),
                        new PositionData(-7414.093f, 354.218f, 6476.23f),
                        new PositionData(-10746.016f, 515.025f, -2715.92f),
                        new PositionData(-9282.119f, -686.088f, -5772.018f),
                        new PositionData(-4653.799f, -1005.817f, 6421.176f),
                        new PositionData(-4301.567f, -1795.573f, 726.413f),
                        new PositionData(-6906.0f, -1542.95f, 1918.0f),
                        new PositionData(-9511.527f, 660.963f, -2984.197f),
                        new PositionData(-7587.725f, 117.107f, -1567.788f),
                        new PositionData(-7744.977f, 145.849f, -2102.101f),
                        new PositionData(3412.644f, -1375.523f, -5273.588f),
                        new PositionData(5076.538f, -1631.1735f, -7353.204f),
                        new PositionData(7532.867f, -208.74f, 7239.067f),
                        new PositionData(6145.532f, -2573.032f, 1604.408f),
                        new PositionData(4993.38f, -2651.574f, 3759.845f),
                        new PositionData(-8967.301f, 503.584f, 2043.8829999999998f),
                        new PositionData(-10687.238f, -888.804f, 5829.569f),
                        new PositionData(-10283.628f, 1401.789f, -3667.762f),
                        new PositionData(-4405.667f, -875.195f, 4484.458f),
                        new PositionData(-10540.591f, -2384.761f, -1453.549f),
                        new PositionData(-9577.718f, -1168.345f, 4867.166f),
                        new PositionData(-8577.374f, 1050.437f, -4317.23f),
                        new PositionData(-5637.728f, -944.827f, -4790.177f),
                        new PositionData(765.301f, -2821.226f, 3222.989f),
                        new PositionData(-5190.034f, -1133.006f, 4283.368f),
                        new PositionData(100.666f, -2909.257f, -2425.51f),
                        new PositionData(-9666.0f, -1158.0f, 6838.0f),
                        new PositionData(-1957.0f, -331.0f, 7502.0f),
                        new PositionData(136.0f, -403.0f, 5610.0f),
                        new PositionData(-7310.0f, 286.0f, 2801.0f),
                        new PositionData(-2378.0f, -1967.0f, 994.0f),
                        new PositionData(-10301.0f, 1115.0f, -4310.0f),
                        new PositionData(-10985.0f, 717.0f, 673.0f),
                        new PositionData(16.0f, -2878.0f, -587.0f),
                        new PositionData(-4658.0f, -683.0f, -2407.0f),
                        new PositionData(-5969.0f, -941.0f, 5545.0f),
                    }
                }
            }
        },
        {
            6, new Dictionary<string, PositionData[]>()
            {
                {
                    "Bilby", new PositionData[]
                    {
                        new PositionData(-11441.138f, 275.632f, 24266.705f, 0.387f),
                        new PositionData(-5905.033f, -12.576f, 15642.216f, 0.0f),
                        new PositionData(-14179.623f, 2.492f, 21155.705f, 0.344f),
                        new PositionData(-13325.058f, 275.707f, 24581.329f, 1.099f),
                        new PositionData(7585.274f, 395.65f, 15118.996f, 4.95f),
                        new PositionData(278.799f, 48.618f, 25929.23f, 5.43f),
                        new PositionData(11239.742f, -16.829f, 11082.507f, 0.27f),
                        new PositionData(6909.334f, 42.943f, 6189.175f, 5.412f),
                        new PositionData(-14511.787f, 93.916f, 16232.41f, 0.833f),
                        new PositionData(-12451.105f, 78.125f, -9702.298f, 0.0f),
                        new PositionData(7358.027f, -18.527f, -18043.523f, 3.413f),
                        new PositionData(-9191.168f, -7.318f, 17899.751f, 3.122f),
                        new PositionData(566.244f, 5791.441f, -12336.197f, 5.173f),
                        new PositionData(4772.742f, 35.584f, 19000.227f, 4.574f),
                        new PositionData(6738.203f, -6.848f, 12800.188f, 5.57f),
                        new PositionData(-18164.832f, -16.839f, 12971.375f, 4.497f),
                        new PositionData(-9633.459f, 163.57f, 6147.973f, 4.689f),
                        new PositionData(2919.209f, 45.588f, 19078.831f, 4.932f),
                        new PositionData(-4319.688f, 123.159f, 11245.484f, 1.73f),
                        new PositionData(9056.991f, 10.439f, -9417.215f, 4.681f),
                        new PositionData(7912.0f, 86.0f, -3765.0f, -2.826f),
                        new PositionData(2884.0f, 123.0f, -8990.0f, 2.571f),
                        new PositionData(2772.0f, 678.0f, 21903.0f, 0.0f),
                        new PositionData(12477.0f, 482.0f, 9308.0f, -1.215f),
                        new PositionData(-9441.0f, 578.0f, 14725.0f, 0.0f),
                    }
                },
                {
                    "Cog", new PositionData[]
                    {
                        new PositionData(-12059.588f, 438.86f, 13832.164f),
                        new PositionData(-19945.498f, -641.363f, 17017.21f),
                        new PositionData(-10852.247f, -421.007f, 15607.317f),
                        new PositionData(-5703.209f, 9.289f, 18331.887f),
                        new PositionData(7196.08f, 158.936f, 25411.887f),
                        new PositionData(4050.974f, 962.062f, 15593.022f),
                        new PositionData(9241.538f, 968.821f, 12687.572f),
                        new PositionData(-14650.439f, -475.481f, -5218.593f),
                        new PositionData(-12876.298f, -440.349f, -15894.348f),
                        new PositionData(9907.795f, -259.114f, -3453.093f),
                        new PositionData(-17504.693f, -806.596f, -7418.758f),
                        new PositionData(-5359.77f, -78.552f, -6630.016f),
                        new PositionData(4030.184f, 292.231f, -12171.324f),
                        new PositionData(3422.956f, 2531.314f, -10408.145f),
                        new PositionData(2278.411f, 4288.037f, -13793.409f),
                        new PositionData(1356.233f, 5274.568f, -10880.843f),
                        new PositionData(-3636.08f, 343.544f, 22791.795f),
                        new PositionData(-2960.458f, -241.707f, 14463.239f),
                        new PositionData(147.524f, 502.622f, 24424.795f),
                        new PositionData(-11048.728f, 210.92f, -7935.27f),
                        new PositionData(838.374f, 2102.885f, 13737.025f),
                        new PositionData(10006.124f, 868.067f, 23531.07f),
                        new PositionData(12112.438f, -951.505f, 24216.584f),
                        new PositionData(10625.925f, -775.12f, 5121.091f),
                        new PositionData(-9857.953f, 353.02f, 23590.182f),
                        new PositionData(-2634.869f, 62.245f, -5168.238f),
                        new PositionData(3152.475f, -1455.566f, 9685.476f),
                        new PositionData(-11615.813f, 570.876f, 23041.02f),
                        new PositionData(-202.385f, 689.911f, -15000.092f),
                        new PositionData(-9678.58f, -91.65f, -16513.701f),
                        new PositionData(-14521.941f, -345.811f, 14773.504f),
                        new PositionData(7677.657f, 605.071f, 21546.352f),
                        new PositionData(-11352.086f, -392.455f, 19425.91f),
                        new PositionData(2254.547f, -919.756f, 24856.824f),
                        new PositionData(-13508.734f, 210.92f, 12664.017f),
                        new PositionData(-15979.078f, 185.744f, 9517.188f),
                        new PositionData(-14198.234f, 511.4f, 23083.479f),
                        new PositionData(-14064.021f, 1144.167f, 22957.123f),
                        new PositionData(-9447.36f, 33.191f, 14724.649f),
                        new PositionData(-9105.0f, 842.0f, 22248.0f),
                        new PositionData(-11961.0f, -593.0f, 14369.0f),
                        new PositionData(4627.0f, 104.0f, 3909.0f),
                        new PositionData(-18159.0f, 590.0f, 12159.0f),
                        new PositionData(-942.0f, -596.0f, 10415.0f),
                        new PositionData(905.0f, 7445.0f, -12315.0f),
                        new PositionData(10223.0f, 711.0f, 16535.0f),
                        new PositionData(8557.0f, 458.0f, -11202.0f),
                        new PositionData(-9570.0f, 814.0f, -14879.0f),
                        new PositionData(-1544.0f, 676.0f, 19539.0f),
                    }
                }
            }
        },
        {
            8, new Dictionary<string, PositionData[]>()
            {
                {
                    "Bilby", new PositionData[]
                    {
                        new PositionData(1383.987f, -719.713f, 5683.788f, 1.497f),
                        new PositionData(2101.085f, -364.967f, 2447.264f, 2.393f),
                        new PositionData(-357.118f, -338.784f, 1482.581f, 0.849f),
                        new PositionData(-4845.4f, -115.128f, -523.695f, 2.22f),
                        new PositionData(-2109.27f, 398.652f, -307.237f, 2.111f),
                        new PositionData(-2172.279f, 424.576f, -2556.774f, 3.153f),
                        new PositionData(-5877.959f, 564.352f, -3301.755f, 0.818f),
                        new PositionData(-2488.827f, 980.332f, -3602.856f, 1.735f),
                        new PositionData(6312.99f, 189.094f, -13622.432f, 5.992f),
                        new PositionData(9208.809f, 124.089f, -9900.374f, 0.841f),
                        new PositionData(12930.533f, 524.204f, -4265.194f, 5.043f),
                        new PositionData(3206.831f, 192.365f, -1989.7f, 0.999f),
                        new PositionData(4602.208f, 533.46f, 1140.811f, 3.256f),
                        new PositionData(7885.76f, 610.535f, 5943.435f, 5.941f),
                        new PositionData(-6313.873f, 1041.09f, -8252.097f, 3.743f),
                        new PositionData(-7181.493f, 185.499f, -7254.178f, 1.546f),
                        new PositionData(-2592.965f, 124.204f, -9611.747f, 0.117f),
                        new PositionData(-654.343f, 106.648f, -16182.571f, 2.884f),
                        new PositionData(9208.491f, 179.319f, -11969.719f, 2.571f),
                        new PositionData(-3386.291f, 403.332f, 614.949f, 1.351f),
                        new PositionData(-4300.0f, 426.0f, -2654.0f, 1.289f),
                        new PositionData(-3479.0f, 1074.0f, -3179.0f, 2.394f),
                        new PositionData(2531.0f, 64.0f, -6472.0f, 2.957f),
                        new PositionData(-6273.0f, -678.0f, 4732.0f, 1.418f),
                        new PositionData(8578.0f, 509.0f, 583.0f, -1.841f),
                    }
                },
                {
                    "Cog", new PositionData[]
                    {
                        new PositionData(474.90999999999997f, -700.291f, 6485.549f),
                        new PositionData(1663.885f, 98.243f, 5002.506f),
                        new PositionData(-1625.069f, 515.513f, 322.548f),
                        new PositionData(-2239.966f, 620.962f, -350.501f),
                        new PositionData(-3604.621f, 698.409f, -6433.568f),
                        new PositionData(-6293.728f, 760.233f, -3106.865f),
                        new PositionData(-1934.711f, 1485.381f, -3769.418f),
                        new PositionData(552.864f, -921.509f, 5250.294f),
                        new PositionData(-4493.678f, -878.245f, 3632.552f),
                        new PositionData(-1405.787f, 418.836f, -8579.073f),
                        new PositionData(-152.928f, 115.383f, -8232.199f),
                        new PositionData(1368.023f, -271.384f, -10834.745f),
                        new PositionData(1233.387f, -67.131f, -12002.093f),
                        new PositionData(7001.821f, -137.807f, -11439.133f),
                        new PositionData(6085.512f, -480.722f, -14066.171f),
                        new PositionData(7549.232f, 208.688f, -8482.084f),
                        new PositionData(9825.733f, -102.666f, -6569.271f),
                        new PositionData(12618.574f, 574.205f, -3162.948f),
                        new PositionData(7280.7f, 960.91f, -2597.216f),
                        new PositionData(1304.634f, 343.808f, -4541.096f),
                        new PositionData(4552.705f, 427.189f, -4794.4f),
                        new PositionData(4197.574f, -187.016f, -891.65f),
                        new PositionData(9029.281f, 654.626f, 2554.11f),
                        new PositionData(5621.643f, 657.092f, 2370.126f),
                        new PositionData(8607.997f, 828.922f, 570.091f),
                        new PositionData(7591.276f, 540.196f, -518.483f),
                        new PositionData(-5260.556f, 1011.193f, -7865.388f),
                        new PositionData(-7398.14f, 999.451f, -6437.415f),
                        new PositionData(-6593.346f, 302.447f, -6580.028f),
                        new PositionData(-2287.262f, 923.141f, -15502.874f),
                        new PositionData(824.917f, -136.15f, -14199.349f),
                        new PositionData(2965.877f, 397.039f, -12814.968f),
                        new PositionData(7705.535f, 827.111f, -6487.452f),
                        new PositionData(1059.707f, -157.673f, -7146.263f),
                        new PositionData(358.538f, 579.645f, -7117.392f),
                        new PositionData(-2638.557f, 1185.208f, -3628.802f),
                        new PositionData(-2349.796f, 947.942f, -2544.458f),
                        new PositionData(-840.13f, -633.416f, 6372.868f),
                        new PositionData(1985.057f, -148.314f, 4634.46f),
                        new PositionData(-1501.265f, -696.254f, 551.891f),
                        new PositionData(897.0f, -304.0f, -690.0f),
                        new PositionData(-2589.0f, 53.0f, 1899.0f),
                        new PositionData(2769.0f, -661.0f, 2836.0f),
                        new PositionData(4533.0f, 226.0f, -8616.0f),
                        new PositionData(4101.0f, -92.0f, -11194.0f),
                        new PositionData(-1528.0f, 196.0f, -6840.0f),
                        new PositionData(8446.0f, 271.0f, -15522.0f),
                        new PositionData(-263.0f, 578.0f, -9903.0f),
                        new PositionData(-6464.0f, 842.0f, -8021.0f),
                        new PositionData(-5498.0f, 792.0f, -3921.0f),
                    }
                }
            }
        },
        {
            9, new Dictionary<string, PositionData[]>()
            {
                {
                    "Bilby", new PositionData[]
                    {
                        new PositionData(-642.649f, -2603.134f, 1283.686f, 3.142f),
                        new PositionData(5416.325f, -3255.931f, 1345.119f, 4.935f),
                        new PositionData(14324.464f, -6367.995f, 1804.223f, 5.685f),
                        new PositionData(16856.305f, -7779.646f, -6944.114f, 1.258f),
                        new PositionData(2631.398f, -5541.176f, -13252.725f, 1.514f),
                        new PositionData(34455.301f, -5868.991f, -18084.238f, 3.261f),
                        new PositionData(-1682.196f, -3647.031f, -7422.692f, 2.051f),
                        new PositionData(-1987.968f, -3260.601f, -5551.578f, 2.552f),
                        new PositionData(13864.963f, -6603.062f, -6285.938f, 1.277f),
                        new PositionData(8649.903f, -4614.118f, -4657.603f, 1.439f),
                        new PositionData(9515.83f, -5108.783f, -7075.345f, 2.921f),
                        new PositionData(-2468.825f, -5741.663f, -14532.724f, 2.011f),
                        new PositionData(36931.958f, -4358.087f, -13204.597f, 4.748f),
                        new PositionData(42646.222f, -381.637f, 3167.294f, 5.433f),
                        new PositionData(38049.026f, -5118.895f, -6499.84f, 5.919f),
                        new PositionData(37682.651f, -4506.253f, -19907.268f, 0.122f),
                        new PositionData(25992.233f, -8749.711f, -9774.074f, 6.11f),
                        new PositionData(13261.674f, -8076.424f, -18469.562f, 0.0f),
                        new PositionData(28777.029f, -8522.737f, -5987.118f, 3.966f),
                        new PositionData(38788.437f, -1579.344f, -1890.829f, 6.193f),
                        new PositionData(-1287.0f, -6607.0f, -17443.0f, 1.749f),
                        new PositionData(45445.0f, -2166.0f, -16590.0f, -1.473f),
                        new PositionData(35324.0f, -5775.0f, -18503.0f, -0.773f),
                        new PositionData(4928.0f, -7569.0f, -18504.0f, 1.639f),
                        new PositionData(18544.0f, -8354.0f, -13332.0f, 1.692f),
                    }
                },
                {
                    "Cog", new PositionData[]
                    {
                        new PositionData(-1060.315f, -3043.464f, -3837.269f),
                        new PositionData(2957.629f, -2854.616f, 1403.288f),
                        new PositionData(8185.445f, -3911.312f, 1224.007f),
                        new PositionData(12988.142f, -5637.26f, 2560.779f),
                        new PositionData(27806.354f, -8396.811f, 157.466f),
                        new PositionData(24273.275f, -8675.43f, -4589.396f),
                        new PositionData(19347.113f, -8497.921f, -19301.55f),
                        new PositionData(41241.52f, -1915.912f, -12661.469f),
                        new PositionData(37949.746f, -448.912f, -5116.627f),
                        new PositionData(41023.05f, -2351.136f, -9865.728f),
                        new PositionData(33850.77f, -5725.321f, 692.021f),
                        new PositionData(42577.375f, -116.283f, 776.073f),
                        new PositionData(21441.348f, -9577.59f, -9584.755f),
                        new PositionData(7449.659f, -5944.27f, -14038.626f),
                        new PositionData(15764.11f, -7127.051f, -9726.521f),
                        new PositionData(15041.364f, -7265.148f, -10429.997f),
                        new PositionData(38176.535f, -1584.368f, -203.279f),
                        new PositionData(14429.974f, -6543.91f, -1728.871f),
                        new PositionData(40432.43f, -2849.046f, -12954.928f),
                        new PositionData(35694.66f, -5333.81f, -18313.9f),
                        new PositionData(3606.477f, -7813.59f, -19149.424f),
                        new PositionData(41381.73f, -2364.272f, -19398.578f),
                        new PositionData(42654.574f, -2068.76f, -10202.374f),
                        new PositionData(21738.709f, -9375.846f, 3595.784f),
                        new PositionData(31341.479f, -6971.374f, -17318.367f),
                        new PositionData(21328.82f, -9996.149f, -20026.77f),
                        new PositionData(26012.848f, -8942.609f, -20758.396f),
                        new PositionData(29134.098f, -10508.501f, -17435.826f),
                        new PositionData(39905.11f, -3565.447f, -6800.923f),
                        new PositionData(37318.535f, -3983.056f, -20300.771f),
                        new PositionData(38044.723f, -5124.443f, -7802.262f),
                        new PositionData(17999.88f, -7794.517f, 646.136f),
                        new PositionData(34077.867f, -5998.631f, -9290.475f),
                        new PositionData(39468.98f, -1938.056f, -3677.478f),
                        new PositionData(43342.312f, 141.944f, 188.844f),
                        new PositionData(43397.19f, -2932.045f, -24056.248f),
                        new PositionData(-478.247f, -6838.036f, -17149.762f),
                        new PositionData(2747.034f, -3979.112f, -7858.716f),
                        new PositionData(4484.936f, -4568.821f, -10159.335f),
                        new PositionData(309.569f, -2660.067f, 1059.951f),
                        new PositionData(-1049.0f, -2049.0f, 2293.0f),
                        new PositionData(38051.0f, -10947.0f, -5068.0f),
                        new PositionData(-1497.0f, -3013.0f, -8271.0f),
                        new PositionData(49650.0f, -3051.0f, -19783.0f),
                        new PositionData(30844.0f, -10897.0f, -3976.0f),
                        new PositionData(37946.0f, -2257.0f, -3745.0f),
                        new PositionData(24129.0f, -8270.0f, -10015.0f),
                        new PositionData(29829.0f, -7411.0f, -17095.0f),
                        new PositionData(24255.0f, -8700.0f, -17248.0f),
                        new PositionData(13828.0f, -7616.0f, -17219.0f),
                    }
                }
            }
        },
        {
            10, new Dictionary<string, PositionData[]>()
            {
                {
                    "Bilby", new PositionData[]
                    {
                        new PositionData(-17851.842f, 4654.143f, 19982.277f, 1.275f),
                        new PositionData(-3122.043f, 3286.206f, -14639.581f, 3.195f),
                        new PositionData(16955.121f, 3659.025f, -13137.63f, 4.362f),
                        new PositionData(32509.851f, 4137.162f, 2948.162f, 4.988f),
                        new PositionData(27845.593f, 4137.722f, 20061.04f, 3.138f),
                        new PositionData(20822.916f, 4217.555f, 26977.557f, 0.53f),
                        new PositionData(36320.233f, 3728.021f, -22167.691f, 1.841f),
                        new PositionData(18629.466f, 3492.344f, -17837.816f, 5.251f),
                        new PositionData(60536.932f, 5017.184f, -31992.368f, 4.948f),
                        new PositionData(45309.665f, 10693.005f, -29898.34f, 5.575f),
                        new PositionData(-17548.16f, 3106.025f, -5511.625f, 0.092f),
                        new PositionData(8767.489f, 3720.652f, 8909.115f, 4.774f),
                        new PositionData(43237.991f, 3707.748f, -15918.854f, 6.164f),
                        new PositionData(-17284.607f, 3169.316f, -12185.963f, 1.445f),
                        new PositionData(-10936.409f, 3600.452f, 4507.692f, 4.96f),
                        new PositionData(-16960.716f, 4682.119f, 16904.646f, 1.629f),
                        new PositionData(22334.399f, 4242.846f, 18712.816f, 1.263f),
                        new PositionData(7064.413f, 3335.992f, -10686.806f, 1.893f),
                        new PositionData(-18060.893f, 4724.723f, 18533.672f, 0.584f),
                        new PositionData(25803.201f, 4333.992f, -447.464f, 4.837f),
                        new PositionData(32026.0f, 4257.0f, 25326.0f, 0.0f),
                        new PositionData(56008.0f, 3727.0f, -35631.0f, 0.0f),
                        new PositionData(28636.0f, 4447.0f, -2721.0f, 0.0f),
                        new PositionData(-3112.0f, 2834.0f, -9055.0f, 0.0f),
                        new PositionData(34420.0f, 3964.0f, -14061.0f, 0.0f),
                    }
                },
                {
                    "Cog", new PositionData[]
                    {
                        new PositionData(-16756.686f, 4704.143f, 21466.244f),
                        new PositionData(-12231.925f, 4853.874f, 16786.572f),
                        new PositionData(-10315.825f, 3549.57f, -3941.976f),
                        new PositionData(712.083f, 3469.194f, -14088.974f),
                        new PositionData(16384.008f, 3757.063f, -12460.312f),
                        new PositionData(21386.303f, 4193.972f, -9358.101f),
                        new PositionData(16846.537f, 4280.988f, 16746.902f),
                        new PositionData(23085.414f, 4219.994f, 18332.58f),
                        new PositionData(35797.906f, 3849.727f, -10909.831f),
                        new PositionData(57853.93f, 3862.508f, -28624.541f),
                        new PositionData(41428.96f, 3759.095f, -28655.78f),
                        new PositionData(51220.066f, 5520.823f, -21306.365f),
                        new PositionData(19164.336f, 3220.695f, -23515.951f),
                        new PositionData(7020.854f, 3384.272f, -10672.472f),
                        new PositionData(40711.066f, 12129.716f, -29427.56f),
                        new PositionData(54891.105f, 8681.389f, -40302.55f),
                        new PositionData(48732.293f, 5632.946f, -21812.441f),
                        new PositionData(4810.995f, 3755.601f, 3476.692f),
                        new PositionData(5869.644f, 3619.263f, -14714.417f),
                        new PositionData(-5899.525f, 3577.75f, -3348.027f),
                        new PositionData(-13222.997f, 2403.968f, -14973.625f),
                        new PositionData(42264.34f, 4117.102f, -26385.064f),
                        new PositionData(-14481.704f, 3908.352f, 9811.34f),
                        new PositionData(-10851.41f, 4094.834f, 11150.675f),
                        new PositionData(54833.633f, 3758.108f, -25535.162f),
                        new PositionData(8981.136f, 3842.484f, 1558.268f),
                        new PositionData(9156.699f, 3785.78f, -9539.461f),
                        new PositionData(-11637.504f, 4846.443f, 18969.273f),
                        new PositionData(19705.889f, 3887.231f, 3815.777f),
                        new PositionData(16985.799f, 4266.285f, 21847.207f),
                        new PositionData(18468.959f, 4240.229f, 23176.98f),
                        new PositionData(23153.326f, 4257.181f, 26111.928f),
                        new PositionData(25012.227f, 4202.985f, 26375.88f),
                        new PositionData(27891.764f, 4198.559f, 23424.617f),
                        new PositionData(29834.836f, 4264.73f, 23786.984f),
                        new PositionData(31137.676f, 4340.013f, 2966.754f),
                        new PositionData(5786.976f, 3800.599f, 1080.748f),
                        new PositionData(-2298.225f, 3894.876f, 12528.184f),
                        new PositionData(-12232.686f, 4557.22f, 14112.512f),
                        new PositionData(-15911.762f, 4903.401f, 15050.186f),
                        new PositionData(-17715.0f, 3452.0f, -2773.0f),
                        new PositionData(42107.0f, 10985.0f, -30246.0f),
                        new PositionData(27050.0f, 8653.0f, -32460.0f),
                        new PositionData(48377.0f, 3838.0f, -17490.0f),
                        new PositionData(27644.0f, 3768.0f, 5574.0f),
                        new PositionData(24746.0f, 4174.0f, 22291.0f),
                        new PositionData(50313.0f, 3791.0f, -38483.0f),
                        new PositionData(19637.0f, 3834.0f, -7499.0f),
                        new PositionData(-7503.0f, 3586.0f, -15390.0f),
                        new PositionData(9599.0f, 3477.0f, -10754.0f),
                    }
                }
            }
        },
        {
            12, new Dictionary<string, PositionData[]>()
            {
                {
                    "Bilby", new PositionData[]
                    {
                        new PositionData(-4469.313f, -121.087f, 1599.222f, 0.714f),
                        new PositionData(-3840.998f, 5.844f, 4190.916f, 5.667f),
                        new PositionData(-8787.372f, -563.261f, 5523.847f, 4.815f),
                        new PositionData(-7225.993f, 146.724f, 3182.057f, 5.411f),
                        new PositionData(-10919.292f, -92.444f, 1684.453f, 0.556f),
                        new PositionData(-10294.062f, 677.994f, -1075.797f, 0.696f),
                        new PositionData(-8620.32f, -593.584f, -4378.58f, 5.897f),
                        new PositionData(-1745.524f, -3022.347f, -1119.453f, 3.324f),
                        new PositionData(-2033.949f, -3000.536f, 656.147f, 0.173f),
                        new PositionData(-2851.515f, -2744.95f, -1113.564f, 4.519f),
                        new PositionData(2145.716f, -2281.954f, 2134.239f, 2.007f),
                        new PositionData(4667.796f, -2510.818f, 4081.244f, 1.704f),
                        new PositionData(-3023.22f, -2598.258f, 7661.016f, 1.571f),
                        new PositionData(-3158.627f, -4036.667f, 8280.918f, 0.0f),
                        new PositionData(-6114.992f, 1058.116f, -305.027f, 6.134f),
                        new PositionData(-7354.163f, -557.153f, 2711.841f, 6.0f),
                        new PositionData(-5965.322f, 68.82f, 4484.624f, 4.859f),
                        new PositionData(-4492.226f, -5.507f, 5063.18f, 2.405f),
                        new PositionData(-6136.049f, -564.087f, 545.34f, 2.246f),
                        new PositionData(-11465.634f, -979.018f, 7865.622f, 3.49f),
                        new PositionData(-1716.0f, -2068.0f, 2394.0f, -4.199f),
                        new PositionData(-4491.0f, -575.0f, 741.0f, -0.589f),
                        new PositionData(-5630.0f, 1032.0f, -465.0f, 1.242f),
                        new PositionData(663.0f, -2854.0f, 4703.0f, 0.0f),
                        new PositionData(-9902.0f, -576.0f, -4261.0f, 1.547f),
                    }
                },
                {
                    "Cog", new PositionData[]
                    {
                        new PositionData(-3050.962f, -48.674f, 4116.261f),
                        new PositionData(-4174.389f, -89.358f, 6621.349f),
                        new PositionData(-3790.196f, -513.617f, 261.138f),
                        new PositionData(-4845.743f, 1222.681f, 485.995f),
                        new PositionData(-10706.148f, 40.279f, 910.743f),
                        new PositionData(-6915.329f, 570.932f, 322.55f),
                        new PositionData(-6523.55f, 1082.51f, -880.619f),
                        new PositionData(-4496.308f, 394.008f, -876.342f),
                        new PositionData(-8527.49f, 1129.944f, -5405.018f),
                        new PositionData(-7070.403f, 433.505f, -3747.182f),
                        new PositionData(293.322f, -1325.585f, -5197.916f),
                        new PositionData(3454.97f, -1418.737f, -4878.855f),
                        new PositionData(2769.054f, -2395.2f, -2305.486f),
                        new PositionData(-3563.475f, -2566.835f, -140.747f),
                        new PositionData(3072.701f, -2316.052f, 2351.534f),
                        new PositionData(4036.682f, -1789.59f, 2633.65f),
                        new PositionData(3446.234f, -1704.012f, 4078.5820000000003f),
                        new PositionData(5040.766f, -2300.075f, 4473.075f),
                        new PositionData(-873.77f, -1868.559f, 3632.659f),
                        new PositionData(-561.783f, -1112.611f, 3275.688f),
                        new PositionData(-762.808f, -2548.258f, 7642.674f),
                        new PositionData(-1760.058f, -3986.606f, 8313.467f),
                        new PositionData(-3632.443f, -1815.138f, -816.527f),
                        new PositionData(-1560.201f, -2989.005f, -1926.295f),
                        new PositionData(-1063.6680000000001f, -1015.749f, -30.821f),
                        new PositionData(-2872.411f, -84.42f, 1543.245f),
                        new PositionData(-6187.338f, 3081.718f, -521.577f),
                        new PositionData(-8847.986f, 444.745f, 5239.278f),
                        new PositionData(-11079.582f, 812.787f, 225.98399999999998f),
                        new PositionData(-8282.83f, 590.869f, 2837.319f),
                        new PositionData(-6260.969f, 301.658f, 5129.545f),
                        new PositionData(-3813.569f, 238.562f, 6566.375f),
                        new PositionData(-9779.173f, 638.501f, -3413.058f),
                        new PositionData(-3648.566f, 102.026f, 1862.889f),
                        new PositionData(-4432.528f, 91.129f, -3313.447f),
                        new PositionData(-7256.017f, -766.553f, 9866.889f),
                        new PositionData(-10837.909f, -767.502f, 9872.212f),
                        new PositionData(-5498.63f, -903.151f, 8961.454f),
                        new PositionData(-6254.589f, -881.494f, 9337.128f),
                        new PositionData(-6101.855f, 42.284f, 9268.031f),
                        new PositionData(-700.0f, -2921.0f, -336.0f),
                        new PositionData(-1757.0f, -2264.0f, -1269.0f),
                        new PositionData(-5650.0f, -22.0f, 518.0f),
                        new PositionData(-5922.0f, 3181.0f, -674.0f),
                        new PositionData(-8148.0f, 260.0f, -3816.0f),
                        new PositionData(-8782.0f, -108.0f, 3921.0f),
                        new PositionData(-11123.0f, 505.0f, 5481.0f),
                        new PositionData(1956.0f, -2722.0f, 4766.0f),
                        new PositionData(4368.0f, -1791.0f, 4032.0f),
                        new PositionData(-4613.0f, 469.0f, 4912.0f),
                    }
                }
            }
        },
        {
            13, new Dictionary<string, PositionData[]>()
            {
                {
                    "Bilby", new PositionData[]
                    {
                        new PositionData(-6538.992f, -501.167f, -6703.954f, 1.519f),
                        new PositionData(9974.571f, -487.345f, -11260.666f, 0.0f),
                        new PositionData(8042.02f, -470.438f, -3297.411f, 1.595f),
                        new PositionData(2762.977f, -778.244f, -7224.633f, 0.986f),
                        new PositionData(-3528.0f, 907.253f, -128.255f, 1.306f),
                        new PositionData(-5374.662f, -488.438f, -1330.801f, 2.261f),
                        new PositionData(-6883.318f, 1468.222f, 2723.359f, 2.144f),
                        new PositionData(-498.704f, 4768.78f, 7964.632f, 2.371f),
                        new PositionData(-537.471f, 1468.222f, 5395.647f, 2.791f),
                        new PositionData(-13617.734f, -5651.208f, 14309.943f, 4.371f),
                        new PositionData(-12567.678f, 983.052f, 20515.762f, 5.305f),
                        new PositionData(8684.843f, 1468.174f, 3667.725f, 2.144f),
                        new PositionData(-2146.814f, 6866.956f, 11677.653f, 3.992f),
                        new PositionData(-21611.49f, -5567.132f, 22281.393f, 2.677f),
                        new PositionData(7268.401f, 1468.239f, 6232.839f, 0.12f),
                        new PositionData(1304.265f, -489.101f, -10153.332f, 4.713f),
                        new PositionData(-3211.165f, -484.199f, -1979.635f, 4.624f),
                        new PositionData(254.276f, 1468.222f, 4548.862f, 0.692f),
                        new PositionData(-1532.589f, 1468.222f, 54.138f, 1.66f),
                        new PositionData(9373.522f, -442.7f, -6659.131f, 3.852f),
                        new PositionData(-817.0f, 823.0f, -1794.0f, -1.98f),
                        new PositionData(1278.0f, -437.0f, -5337.0f, 1.473f),
                        new PositionData(5210.0f, -6.0f, -9636.0f, -2.505f),
                        new PositionData(-3272.0f, -487.0f, 442.0f, -1.584f),
                        new PositionData(3922.0f, 1494.0f, -1607.0f, 3.204f),
                    }
                },
                {
                    "Cog", new PositionData[]
                    {
                        new PositionData(-5472.37f, -439.292f, -8308.129f),
                        new PositionData(-851.102f, -75.972f, -6098.705f),
                        new PositionData(-2976.148f, -144.279f, -3823.856f),
                        new PositionData(5875.451f, 30.519f, -10907.948f),
                        new PositionData(6906.838f, -805.187f, -1831.407f),
                        new PositionData(-1991.0f, 38.05f, -5378.0f),
                        new PositionData(4145.388f, 4360.157f, 8102.485f),
                        new PositionData(-4350.223f, -419.43f, -1619.544f),
                        new PositionData(-1767.863f, 1518.222f, 1291.145f),
                        new PositionData(8647.516f, 1568.034f, 1102.273f),
                        new PositionData(8621.316f, 2387.059f, 4670.235f),
                        new PositionData(6103.008f, 1538.853f, 7364.664f),
                        new PositionData(1023.0550000000001f, 6553.167f, 10592.601f),
                        new PositionData(-2884.189f, 8838.2f, 13461.788f),
                        new PositionData(-13567.0f, -1317.453f, 19248.0f),
                        new PositionData(-10203.205f, 10607.175f, 17780.098f),
                        new PositionData(-20953.443f, -5086.674f, 19814.475f),
                        new PositionData(1420.559f, 153.027f, -8509.858f),
                        new PositionData(5748.667f, -439.49f, -9099.346f),
                        new PositionData(-20265.408f, -5471.216f, 21787.006f),
                        new PositionData(9972.551f, -437.345f, -11252.665f),
                        new PositionData(-23226.248f, -5522.46f, 21195.715f),
                        new PositionData(-22909.312f, -5524.778f, 19628.158f),
                        new PositionData(-17945.271f, -4782.056f, 21127.496f),
                        new PositionData(-6147.221f, 9703.689f, 17717.389f),
                        new PositionData(-4494.314f, 7237.529f, 11569.61f),
                        new PositionData(-6588.679f, 263.748f, -5198.935f),
                        new PositionData(6365.47f, 1990.944f, 2655.081f),
                        new PositionData(2539.873f, -483.415f, -5970.058f),
                        new PositionData(2539.873f, -483.415f, -5970.058f),
                        new PositionData(-3236.101f, 1518.222f, 4392.388f),
                        new PositionData(-3429.045f, 36.944f, -1969.799f),
                        new PositionData(-4727.277f, 973.646f, 252.22699999999998f),
                        new PositionData(6231.906f, -49.574f, -939.334f),
                        new PositionData(-730.129f, 1558.661f, 2870.697f),
                        new PositionData(6461.241f, -31.763f, -10586.228f),
                        new PositionData(-3754.234f, -442.623f, 1119.268f),
                        new PositionData(8335.075f, 2150.252f, 3547.872f),
                        new PositionData(-1802.095f, -735.311f, -6938.657f),
                        new PositionData(-4306.139f, 959.95f, 1183.426f),
                        new PositionData(-8177.0f, 10604.0f, 18253.0f),
                        new PositionData(-1947.0f, 1277.0f, -1441.0f),
                        new PositionData(-12807.0f, 1774.0f, 16493.0f),
                        new PositionData(2803.0f, 675.0f, -5401.0f),
                        new PositionData(-1616.0f, -118.0f, -8894.0f),
                        new PositionData(9181.0f, 91.0f, -6205.0f),
                        new PositionData(-5214.0f, 344.0f, -802.0f),
                        new PositionData(-14995.0f, -5562.0f, 24528.0f),
                        new PositionData(-15038.0f, -5565.0f, 22000.0f),
                        new PositionData(9525.0f, 46.0f, -10956.0f),
                    }
                }
            }
        },
        {
            14, new Dictionary<string, PositionData[]>()
            {
                {
                    "Bilby", new PositionData[]
                    {
                        new PositionData(-15110.902f, -1438.521f, 1225.359f, 1.552f),
                        new PositionData(-14481.358f, -1427.812f, 4200.94f, 1.884f),
                        new PositionData(12808.463f, -1497.171f, 9602.831f, 3.121f),
                        new PositionData(11446.785f, -1540.213f, -5483.665f, 5.882f),
                        new PositionData(21538.525f, -1490.331f, -20191.469f, 2.975f),
                        new PositionData(27761.603f, -1458.593f, -3934.944f, 2.903f),
                        new PositionData(26710.031f, -1261.807f, -2464.766f, 5.85f),
                        new PositionData(20687.856f, -1563.734f, 15831.236f, 2.384f),
                        new PositionData(14640.469f, -1507.541f, 4287.658f, 1.585f),
                        new PositionData(24080.62f, -1596.778f, -9235.065f, 4.088f),
                        new PositionData(23568.62f, -1499.25f, -24453.403f, 0.393f),
                        new PositionData(13935.051f, -1505.314f, -24422.328f, 0.0f),
                        new PositionData(30705.966f, -1465.768f, -31200.278f, 0.24f),
                        new PositionData(34034.79f, -1205.902f, -31462.354f, 3.142f),
                        new PositionData(26739.676f, -1482.959f, -4756.506f, 5.849f),
                        new PositionData(22233.293f, -1486.274f, -439.099f, 3.816f),
                        new PositionData(21825.662f, -1553.873f, 5981.611f, 4.525f),
                        new PositionData(5862.78f, -1331.382f, -20210.175f, 3.577f),
                        new PositionData(-14395.734f, -1736.885f, -12159.346f, 4.958f),
                        new PositionData(904.537f, -1496.854f, -10005.908f, 4.501f),
                        new PositionData(20476.0f, -1192.0f, -9637.0f, 6.364f),
                        new PositionData(11742.0f, -1499.0f, -19244.0f, 3.131f),
                        new PositionData(-10974.0f, -719.0f, -7756.0f, 2.589f),
                        new PositionData(11755.0f, -1496.0f, -3477.0f, 1.565f),
                        new PositionData(23990.0f, -1505.0f, 16246.0f, 2.405f),
                    }
                },
                {
                    "Cog", new PositionData[]
                    {
                        new PositionData(-5775.568f, -1960.727f, -1572.521f),
                        new PositionData(-15424.289f, -247.19f, 3378.477f),
                        new PositionData(-10397.546f, -1374.085f, 6713.552f),
                        new PositionData(-9467.81f, -1122.427f, 1635.693f),
                        new PositionData(-5842.65f, -2564.42f, 6518.514f),
                        new PositionData(8812.069f, -1115.878f, 2548.497f),
                        new PositionData(4677.105f, -2776.641f, -4181.424f),
                        new PositionData(4188.892f, -1236.588f, -16288.836f),
                        new PositionData(18033.691f, -316.541f, -14148.088f),
                        new PositionData(23141.469f, -1033.927f, -4459.087f),
                        new PositionData(23536.027f, -2024.318f, -15043.88f),
                        new PositionData(30692.459f, -1410.339f, -1053.987f),
                        new PositionData(27959.648f, -866.056f, -32.817f),
                        new PositionData(31138.582f, -360.455f, -2326.454f),
                        new PositionData(33153.625f, -931.627f, -872.873f),
                        new PositionData(25464.133f, -461.046f, 6890.465f),
                        new PositionData(16309.186f, -1465.798f, -15856.1f),
                        new PositionData(12722.348f, -3222.42f, -11007.551f),
                        new PositionData(-393.064f, -2201.117f, -23235.008f),
                        new PositionData(30441.469f, -2832.561f, -36467.285f),
                        new PositionData(37309.49f, -2107.139f, -29293.21f),
                        new PositionData(33452.07f, -1150.79f, -28718.307f),
                        new PositionData(32688.3f, -2476.938f, -22792.158f),
                        new PositionData(29043.576f, -2524.175f, -8804.486f),
                        new PositionData(18857.574f, -2297.902f, 1959.703f),
                        new PositionData(22593.57f, -1428.202f, -15180.665f),
                        new PositionData(26301.807f, -136.767f, -16675.537f),
                        new PositionData(19226.49f, -2123.13f, 7380.383f),
                        new PositionData(15955.339f, -2842.156f, 18054.553f),
                        new PositionData(-3597.285f, -2530.719f, -12911.225f),
                        new PositionData(-19877.283f, -2423.22f, -5927.53f),
                        new PositionData(-16117.822f, -2026.213f, -14055.335f),
                        new PositionData(-15544.705f, -386.468f, -3074.843f),
                        new PositionData(-11734.233f, -1716.743f, 2243.355f),
                        new PositionData(2057.423f, -1361.71f, -4776.938f),
                        new PositionData(-4394.043f, -1716.787f, -7284.633f),
                        new PositionData(24842.95f, -1493.477f, 12810.215f),
                        new PositionData(25226.863f, -1485.632f, 5115.148f),
                        new PositionData(19615.697f, -1393.835f, 2803.431f),
                        new PositionData(26149.914f, -1702.69f, -7320.132f),
                        new PositionData(-8719.0f, -477.0f, 7353.0f),
                        new PositionData(-3363.0f, -3137.0f, -17624.0f),
                        new PositionData(11139.0f, -2726.0f, -28747.0f),
                        new PositionData(32081.0f, -2882.0f, 8235.0f),
                        new PositionData(6577.0f, -1373.0f, -5044.0f),
                        new PositionData(20559.0f, -604.0f, -32857.0f),
                        new PositionData(26583.0f, -286.0f, -2397.0f),
                        new PositionData(7043.0f, -1461.0f, -14426.0f),
                        new PositionData(32665.0f, -674.0f, -29437.0f),
                        new PositionData(27536.0f, -879.0f, 4892.0f),
                    }
                }
            }
        }
    };

    private Dictionary<int, int> _originalPortals = new()
        { { 1, 5 }, { 2, 4 }, { 3, 13 }, { 4, 10 }, { 8, 9 }, { 11, 12 }, { 12, 8 }, { 13, 6 }, { 14, 14 } };
    private Dictionary<int, int> _newPortals = new();

    public Dictionary<int, byte[]> CurrentPositionIndices = new();
    private Random _random;
    private int _chaosSeed;
    public int ChaosSeed
    {
        get => _chaosSeed;
        set
        {
            _chaosSeed = value;
            Logger.Write($"The Chaos Mode seed is now {Client.HChaos.ChaosSeed}");
            ShuffleIndices(value);
            MoveCollectibles(Client.HLevel.CurrentLevelId);
        }
    }

    private bool _shuffleOnStart;

    public bool ShuffleOnStart
    {
        get => _shuffleOnStart;
        set
        {
            _shuffleOnStart = value;
            Logger.Write($"Chaos Mode shuffle on start set to {value}.");
            OnShuffleOnStartChanged?.Invoke(value);
        }
    }
    
    public delegate void ShuffleOnStartChangedEventHandler(bool shuffleOnStart);
    public static event ShuffleOnStartChangedEventHandler OnShuffleOnStartChanged;

    public ChaosHandler()
    {
        ShuffleIndices(0);
    }

    public void ShuffleIndices(int seed)
    {
        _random = new Random(seed);
        CurrentPositionIndices.Clear();
        foreach (var level in Levels.MainStages)
        {
            var cogs = new HashSet<byte>();
            while (cogs.Count < 10)
                cogs.Add((byte)_random.Next(0, 49));
            var bilbies = new HashSet<byte>();
            while (bilbies.Count < 5)
                bilbies.Add((byte)_random.Next(0, 24));
            var randomisedCollectibleIndices = cogs.Concat(bilbies).ToArray();
            CurrentPositionIndices.Add(level.Id, randomisedCollectibleIndices);
        }
        _newPortals = _originalPortals.Keys.Zip(_originalPortals.Values
            .OrderBy(x => _random.Next()), (k, v) => new { k, v })
            .ToDictionary(x => x.k, x => x.v);
    }

    public void SwapPortals()
    {
        if (Client.Relaunching || Client.HGameState.IsOnMainMenuOrLoading)
            return;
        var basePortalAddr = PointerCalculations.GetPointerAddress(0x267408, new[] { 0x0 });
        foreach (var portal in _newPortals)
            ProcessHandler.WriteData(basePortalAddr + 0xAC + 0xB0 * portal.Key, BitConverter.GetBytes(portal.Value));
    }
    
    public void MoveCollectibles(int level)
    {
        if (!_positions.ContainsKey(level) || !CurrentPositionIndices.ContainsKey(level) || Client.HGameState.IsOnMainMenuOrLoading)
            return;
        //Logger.Write($"Moving collectibles for level {level}");
        for (var cogIndex = 0; cogIndex < 10; cogIndex++)
        {
            // THIS IS THE UGLIEST CODE I HAVE EVER AND WILL EVER WRITE
            var transform = _positions[level]["Cog"][CurrentPositionIndices[level][cogIndex]];
            var cogPosAddr =
                PointerCalculations.GetPointerAddress(0x270310, new[] { 0x144 * cogIndex + 0x8, 0x74 });
            var cogPlatformAddr =
                PointerCalculations.GetPointerAddress(0x270310, new[] { 0x144 * cogIndex + 0x138, 0});
            if (cogPlatformAddr != 0)
            {
                var platformAttachmentsAddr =
                    PointerCalculations.GetPointerAddress(cogPlatformAddr, new[] { 0x94, 0x0 });
                ProcessHandler.WriteData(cogPlatformAddr + 0x98, new byte[4]);
                ProcessHandler.WriteData(platformAttachmentsAddr, new byte[4]);
            }
            var bytesToWrite =
                BitConverter.GetBytes(transform.X)
                    .Concat(BitConverter.GetBytes(transform.Y))
                    .Concat(BitConverter.GetBytes(transform.Z)).ToArray();
            //Logger.Write(
               // $"Writing {transform.X}, {transform.Y}, {transform.Z} for cog at index {CurrentPositionIndices[level][cogIndex]}");
            ProcessHandler.WriteData(cogPosAddr, bytesToWrite);
        }

        for (var bilbyIndex = 0; bilbyIndex < 5; bilbyIndex++)
        {
            // THIS IS THE UGLIEST CODE I HAVE EVER AND WILL EVER WRITE
            var transform = _positions[level]["Bilby"][CurrentPositionIndices[level][bilbyIndex + 10]];
            var cagePosAddr =
                PointerCalculations.GetPointerAddress(0x27D608, new[] { 0x0, 0x134 * bilbyIndex + 0x8, 0x74 });
            var bilbyPosAddr =
                PointerCalculations.GetPointerAddress(0x27D608, new[] { 0x0, 0x134 * bilbyIndex + 0x4, 0x74 });
            var bilbyTEPosAddr =
                PointerCalculations.GetPointerAddress(0x27D608, new[] { 0x0, 0x134 * bilbyIndex + 0xC });
            var bytesToWrite =
                BitConverter.GetBytes(transform.X)
                    .Concat(BitConverter.GetBytes(transform.Y))
                    .Concat(BitConverter.GetBytes(transform.Z)).ToArray();
            //Logger.Write($"Writing {transform.X}, {transform.Y}, {transform.Z} for bilby at index {CurrentPositionIndices[level][bilbyIndex + 10]}");
            ProcessHandler.WriteData(cagePosAddr, bytesToWrite);
            ProcessHandler.WriteData(bilbyPosAddr, bytesToWrite);
            ProcessHandler.WriteData(bilbyTEPosAddr, bytesToWrite);
            // WRITE ROTATION
            var mat = new RotationMatrix(transform.Yaw);
            var matBytes = mat.GetBytes();
            ProcessHandler.WriteData(cagePosAddr - 0x30, matBytes);
            ProcessHandler.WriteData(bilbyPosAddr - 0x30, matBytes);
        }
        
        if (Client.HLevel.CurrentLevelId == 0)
            SwapPortals();
    }
    
    [MessageHandler((ushort)MessageID.CH_Shuffle)]
    public static void ReceiveSeed(Message message)
    {
        Client.HChaos.ChaosSeed = message.GetInt();
    }

    public static void RequestShuffle()
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.CH_Shuffle);
        Client._client.Send(message);
    }

    public static void RequestSeed(int newSeed)
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.CH_SetSeed);
        message.AddInt(newSeed);
        Client._client.Send(message);
    }

    public static void RequestShuffleOnStartToggle()
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.CH_ShuffleOnStartToggle);
        Client._client.Send(message);
    }

    [MessageHandler((ushort)MessageID.CH_ShuffleOnStartToggle)]
    public static void HandleShuffleOnStartResponse(Message message)
    {
        Client.HChaos.ShuffleOnStart = message.GetBool();
    }
}