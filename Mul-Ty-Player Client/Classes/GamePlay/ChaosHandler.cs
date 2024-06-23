using System;
using System.Collections.Generic;
using System.Linq;
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
                        new PositionData(3342.59f, -61.304f, 8369.632f),
                        new PositionData(634.759f, -320.606f, 6359.147f),
                        new PositionData(-1073.892f, -104.979f, 4570.949f),
                        new PositionData(-4314.604f, 112.289f, 5107.654f),
                        new PositionData(-5052.86f, -730.101f, 2973.001f),
                        new PositionData(-583.541f, -570.599f, 2029.097f),
                        new PositionData(2956.548f, -355.495f, -352.471f),
                        new PositionData(1218.19f, -369.834f, -2380.862f),
                        new PositionData(5007.117f, 125.83f, -1467.677f),
                        new PositionData(3677.655f, 402.567f, -4996.91f),
                        new PositionData(2793.771f, 518.073f, -5456.067f),
                        new PositionData(5284.447f, 242.541f, -6032.078f),
                        new PositionData(2574.417f, -410.909f, -8570.458f),
                        new PositionData(-3767.526f, -637.551f, -7725.371f),
                        new PositionData(-4638.155f, -452.385f, -8743.168f),
                        new PositionData(-1208.736f, -422.729f, -5257.798f),
                        new PositionData(-2207.78f, -227.478f, -5179.264f),
                        new PositionData(336.411f, 9.134f, -3411.567f),
                        new PositionData(-4610.678f, 1077.669f, 298.17f),
                        new PositionData(-871.605f, 386.192f, 1028.722f),
                        new PositionData(-3531.13f, 162.842f, -3141.545f),
                        new PositionData(-5210.82f, -495.519f, -6355.628f),
                        new PositionData(-6548.943f, 527.825f, -2636.603f),
                        new PositionData(-8043.196f, 453.76f, -1778.842f),
                        new PositionData(-8680.219f, 471.282f, -352.16f),
                        new PositionData(-7867.592f, 469.886f, 18.273f),
                        new PositionData(-3674.774f, 401.245f, 7987.534f),
                        new PositionData(-3864.134f, 278.066f, 6736.796f),
                        new PositionData(-3051.992f, 335.863f, 9372.519f),
                        new PositionData(-1175.614f, 89.484f, 8406.682f),
                        new PositionData(2283.863f, -19.092f, 6929.222f),
                        new PositionData(1122.667f, -749.944f, 4437.642f),
                        new PositionData(312.202f, 84.015f, 4663.469f),
                        new PositionData(-3035.064f, 19.155f, 4096.939f),
                        new PositionData(-4337.075f, -443.139f, 4046.199f),
                        new PositionData(-627.791f, -25.668f, 2054.855f),
                        new PositionData(1385.622f, -298.697f, 2444.575f),
                        new PositionData(3410.647f, 120.715f, 2422.194f),
                        new PositionData(3191.592f, 377.944f, -3730.238f),
                        new PositionData(-1538.473f, -276.508f, -5937.341f),
                        new PositionData(2449.0f, 407.0f, 1376.0f),
                        new PositionData(3949.0f, 90.0f, -1787.0f),
                        new PositionData(-3289.0f, -389.0f, 2113.0f),
                        new PositionData(1232.0f, -41.0f, -2366.0f),
                        new PositionData(880.0f, 403.0f, -289.0f),
                        new PositionData(551.0f, 552.0f, -5201.0f),
                        new PositionData(-642.0f, -554.0f, 2359.0f),
                        new PositionData(-1894.0f, -333.0f, -7561.0f),
                        new PositionData(3402.0f, -553.0f, 6994.0f),
                        new PositionData(2649.0f, -268.0f, 6605.0f),
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
                        new PositionData(-8002.256f, -1091.655f, 7452.015f),
                        new PositionData(-8401.575f, -940.686f, 3955.345f),
                        new PositionData(-6013.314f, -1537.794f, 2670.495f),
                        new PositionData(-1415.395f, -1375.697f, 4011.795f),
                        new PositionData(-8151.76f, -1193.078f, 2027.67f),
                        new PositionData(-7415.405f, -1789.988f, 135.425f),
                        new PositionData(-2729.538f, -1009.154f, -6025.927f),
                        new PositionData(-1125.03f, -723.637f, -4584.981f),
                        new PositionData(-9843.771f, -1697.877f, -91.482f),
                        new PositionData(6544.381f, -3156.209f, -4678.972f),
                        new PositionData(2248.438f, -3018.295f, -613.147f),
                        new PositionData(2642.038f, -2481.875f, 3817.315f),
                        new PositionData(7018.573f, 273.944f, 6202.543f),
                        new PositionData(-1943.036f, -215.063f, 8747.775f),
                        new PositionData(1139.067f, -643.878f, 6184.324f),
                        new PositionData(-7364.093f, 354.218f, 6526.23f),
                        new PositionData(-10696.016f, 515.025f, -2665.92f),
                        new PositionData(-9232.119f, -686.088f, -5722.018f),
                        new PositionData(-4603.799f, -1005.817f, 6471.176f),
                        new PositionData(-4251.567f, -1795.573f, 776.413f),
                        new PositionData(-6856.0f, -1542.95f, 1968.0f),
                        new PositionData(-9461.527f, 660.963f, -2934.197f),
                        new PositionData(-7537.725f, 117.107f, -1517.788f),
                        new PositionData(-7694.977f, 145.849f, -2052.101f),
                        new PositionData(3462.644f, -1375.523f, -5223.588f),
                        new PositionData(4470.788f, -1722.424f, -6249.802f),
                        new PositionData(7582.867f, -208.74f, 7289.067f),
                        new PositionData(6195.532f, -2573.032f, 1654.408f),
                        new PositionData(5043.38f, -2651.574f, 3809.845f),
                        new PositionData(-8917.301f, 503.584f, 2093.883f),
                        new PositionData(-10637.238f, -888.804f, 5879.569f),
                        new PositionData(-10233.628f, 1401.789f, -3617.762f),
                        new PositionData(-4355.667f, -875.195f, 4534.458f),
                        new PositionData(-10490.591f, -2384.761f, -1403.549f),
                        new PositionData(-9527.718f, -1168.345f, 4917.166f),
                        new PositionData(-8527.374f, 1050.437f, -4267.23f),
                        new PositionData(-5587.728f, -944.827f, -4740.177f),
                        new PositionData(815.301f, -2821.226f, 3272.989f),
                        new PositionData(-5140.034f, -1133.006f, 4333.368f),
                        new PositionData(150.666f, -2909.257f, -2375.51f),
                        new PositionData(-9616.0f, -1158.0f, 6888.0f),
                        new PositionData(-1907.0f, -331.0f, 7552.0f),
                        new PositionData(186.0f, -403.0f, 5660.0f),
                        new PositionData(-7260.0f, 286.0f, 2851.0f),
                        new PositionData(-2328.0f, -1967.0f, 1044.0f),
                        new PositionData(-10251.0f, 1115.0f, -4260.0f),
                        new PositionData(-10935.0f, 717.0f, 723.0f),
                        new PositionData(66.0f, -2878.0f, -537.0f),
                        new PositionData(-4608.0f, -683.0f, -2357.0f),
                        new PositionData(-5919.0f, -941.0f, 5595.0f),
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
                        new PositionData(-12009.588f, 438.86f, 13882.164f),
                        new PositionData(-19895.498f, -641.363f, 17067.21f),
                        new PositionData(-10802.247f, -421.007f, 15657.317f),
                        new PositionData(-5653.209f, 9.289f, 18381.887f),
                        new PositionData(7246.08f, 158.936f, 25461.887f),
                        new PositionData(4100.974f, 962.062f, 15643.022f),
                        new PositionData(9291.538f, 968.821f, 12737.572f),
                        new PositionData(-14600.439f, -475.481f, -5168.593f),
                        new PositionData(-12826.298f, -440.349f, -15844.348f),
                        new PositionData(9957.795f, -259.114f, -3403.093f),
                        new PositionData(-17454.693f, -806.596f, -7368.758f),
                        new PositionData(-5309.77f, -78.552f, -6580.016f),
                        new PositionData(4080.184f, 292.231f, -12121.324f),
                        new PositionData(3472.956f, 2531.314f, -10358.145f),
                        new PositionData(2328.411f, 4288.037f, -13743.409f),
                        new PositionData(1406.233f, 5274.568f, -10830.843f),
                        new PositionData(-3586.08f, 343.544f, 22841.795f),
                        new PositionData(-2910.458f, -241.707f, 14513.239f),
                        new PositionData(197.524f, 502.622f, 24474.795f),
                        new PositionData(-10998.728f, 210.92f, -7885.27f),
                        new PositionData(888.374f, 2102.885f, 13787.025f),
                        new PositionData(10056.124f, 868.067f, 23581.07f),
                        new PositionData(12162.438f, -951.505f, 24266.584f),
                        new PositionData(10675.925f, -775.12f, 5171.091f),
                        new PositionData(-9807.953f, 353.02f, 23640.182f),
                        new PositionData(-2584.869f, 62.245f, -5118.238f),
                        new PositionData(3202.475f, -1455.566f, 9735.476f),
                        new PositionData(-11565.813f, 570.876f, 23091.02f),
                        new PositionData(-152.385f, 689.911f, -14950.092f),
                        new PositionData(-9628.58f, -91.65f, -16463.701f),
                        new PositionData(-14471.941f, -345.811f, 14823.504f),
                        new PositionData(7727.657f, 605.071f, 21596.352f),
                        new PositionData(-11302.086f, -392.455f, 19475.91f),
                        new PositionData(2304.547f, -919.756f, 24906.824f),
                        new PositionData(-13458.734f, 210.92f, 12714.017f),
                        new PositionData(-15929.078f, 185.744f, 9567.188f),
                        new PositionData(-14148.234f, 511.4f, 23133.479f),
                        new PositionData(-14014.021f, 1144.167f, 23007.123f),
                        new PositionData(-9397.36f, 33.191f, 14774.649f),
                        new PositionData(-9055.0f, 842.0f, 22298.0f),
                        new PositionData(-11911.0f, -593.0f, 14419.0f),
                        new PositionData(4677.0f, 104.0f, 3959.0f),
                        new PositionData(-18109.0f, 590.0f, 12209.0f),
                        new PositionData(-892.0f, -596.0f, 10465.0f),
                        new PositionData(955.0f, 7445.0f, -12265.0f),
                        new PositionData(10273.0f, 711.0f, 16585.0f),
                        new PositionData(8607.0f, 458.0f, -11152.0f),
                        new PositionData(-9520.0f, 814.0f, -14829.0f),
                        new PositionData(-1494.0f, 676.0f, 19589.0f),
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
                        new PositionData(524.91f, -700.291f, 6535.549f),
                        new PositionData(1713.885f, 98.243f, 5052.506f),
                        new PositionData(-1575.069f, 515.513f, 372.548f),
                        new PositionData(-2189.966f, 620.962f, -300.501f),
                        new PositionData(-3554.621f, 698.409f, -6383.568f),
                        new PositionData(-6243.728f, 760.233f, -3056.865f),
                        new PositionData(-1884.711f, 1485.381f, -3719.418f),
                        new PositionData(602.864f, -921.509f, 5300.294f),
                        new PositionData(-4443.678f, -878.245f, 3682.552f),
                        new PositionData(-1355.787f, 418.836f, -8529.073f),
                        new PositionData(-102.928f, 115.383f, -8182.199f),
                        new PositionData(1418.023f, -271.384f, -10784.745f),
                        new PositionData(1283.387f, -67.131f, -11952.093f),
                        new PositionData(7051.821f, -137.807f, -11389.133f),
                        new PositionData(6135.512f, -480.722f, -14016.171f),
                        new PositionData(7599.232f, 208.688f, -8432.084f),
                        new PositionData(9875.733f, -102.666f, -6519.271f),
                        new PositionData(12668.574f, 574.205f, -3112.948f),
                        new PositionData(7330.7f, 960.91f, -2547.216f),
                        new PositionData(1354.634f, 343.808f, -4491.096f),
                        new PositionData(4602.705f, 427.189f, -4744.4f),
                        new PositionData(4247.574f, -187.016f, -841.65f),
                        new PositionData(9079.281f, 654.626f, 2604.11f),
                        new PositionData(5671.643f, 657.092f, 2420.126f),
                        new PositionData(8657.997f, 828.922f, 620.091f),
                        new PositionData(7641.276f, 540.196f, -468.483f),
                        new PositionData(-5210.556f, 1011.193f, -7815.388f),
                        new PositionData(-7348.14f, 999.451f, -6387.415f),
                        new PositionData(-6543.346f, 302.447f, -6530.028f),
                        new PositionData(-2237.262f, 923.141f, -15452.874f),
                        new PositionData(874.917f, -136.15f, -14149.349f),
                        new PositionData(3015.877f, 397.039f, -12764.968f),
                        new PositionData(7755.535f, 827.111f, -6437.452f),
                        new PositionData(1109.707f, -157.673f, -7096.263f),
                        new PositionData(408.538f, 579.645f, -7067.392f),
                        new PositionData(-2588.557f, 1185.208f, -3578.802f),
                        new PositionData(-2299.796f, 947.942f, -2494.458f),
                        new PositionData(-790.13f, -633.416f, 6422.868f),
                        new PositionData(2035.057f, -148.314f, 4684.46f),
                        new PositionData(-1451.265f, -696.254f, 601.891f),
                        new PositionData(947.0f, -304.0f, -640.0f),
                        new PositionData(-2539.0f, 53.0f, 1949.0f),
                        new PositionData(2819.0f, -661.0f, 2886.0f),
                        new PositionData(4583.0f, 226.0f, -8566.0f),
                        new PositionData(4151.0f, -92.0f, -11144.0f),
                        new PositionData(-1478.0f, 196.0f, -6790.0f),
                        new PositionData(8496.0f, 271.0f, -15472.0f),
                        new PositionData(-213.0f, 578.0f, -9853.0f),
                        new PositionData(-6414.0f, 842.0f, -7971.0f),
                        new PositionData(-5448.0f, 792.0f, -3871.0f),
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
                        new PositionData(-1010.315f, -3043.464f, -3787.269f),
                        new PositionData(3007.629f, -2854.616f, 1453.288f),
                        new PositionData(8235.445f, -3911.312f, 1274.007f),
                        new PositionData(13038.142f, -5637.26f, 2610.779f),
                        new PositionData(27856.354f, -8396.811f, 207.466f),
                        new PositionData(24323.275f, -8675.43f, -4539.396f),
                        new PositionData(19397.113f, -8497.921f, -19251.55f),
                        new PositionData(41291.52f, -1915.912f, -12611.469f),
                        new PositionData(37999.746f, -448.912f, -5066.627f),
                        new PositionData(41073.05f, -2351.136f, -9815.728f),
                        new PositionData(33900.77f, -5725.321f, 742.021f),
                        new PositionData(42627.375f, -116.283f, 826.073f),
                        new PositionData(21491.348f, -9577.59f, -9534.755f),
                        new PositionData(7499.659f, -5944.27f, -13988.626f),
                        new PositionData(15814.11f, -7127.051f, -9676.521f),
                        new PositionData(15091.364f, -7265.148f, -10379.997f),
                        new PositionData(38226.535f, -1584.368f, -153.279f),
                        new PositionData(14479.974f, -6543.91f, -1678.871f),
                        new PositionData(40482.43f, -2849.046f, -12904.928f),
                        new PositionData(35744.66f, -5333.81f, -18263.9f),
                        new PositionData(3656.477f, -7813.59f, -19099.424f),
                        new PositionData(41431.73f, -2364.272f, -19348.578f),
                        new PositionData(42704.574f, -2068.76f, -10152.374f),
                        new PositionData(21788.709f, -9375.846f, 3645.784f),
                        new PositionData(31391.479f, -6971.374f, -17268.367f),
                        new PositionData(21378.82f, -9996.149f, -19976.77f),
                        new PositionData(26062.848f, -8942.609f, -20708.396f),
                        new PositionData(29184.098f, -10508.501f, -17385.826f),
                        new PositionData(39955.11f, -3565.447f, -6750.923f),
                        new PositionData(37368.535f, -3983.056f, -20250.771f),
                        new PositionData(38094.723f, -5124.443f, -7752.262f),
                        new PositionData(18049.88f, -7794.517f, 696.136f),
                        new PositionData(34127.867f, -5998.631f, -9240.475f),
                        new PositionData(39518.98f, -1938.056f, -3627.478f),
                        new PositionData(43392.312f, 141.944f, 238.844f),
                        new PositionData(43447.19f, -2932.045f, -24006.248f),
                        new PositionData(-428.247f, -6838.036f, -17099.762f),
                        new PositionData(2797.034f, -3979.112f, -7808.716f),
                        new PositionData(4534.936f, -4568.821f, -10109.335f),
                        new PositionData(359.569f, -2660.067f, 1109.951f),
                        new PositionData(-999.0f, -2049.0f, 2343.0f),
                        new PositionData(38101.0f, -10947.0f, -5018.0f),
                        new PositionData(-1447.0f, -3013.0f, -8221.0f),
                        new PositionData(49700.0f, -3051.0f, -19733.0f),
                        new PositionData(30894.0f, -10897.0f, -3926.0f),
                        new PositionData(37996.0f, -2257.0f, -3695.0f),
                        new PositionData(24179.0f, -8270.0f, -9965.0f),
                        new PositionData(29879.0f, -7411.0f, -17045.0f),
                        new PositionData(24305.0f, -8700.0f, -17198.0f),
                        new PositionData(13878.0f, -7616.0f, -17169.0f),
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
                        new PositionData(-16706.686f, 4704.143f, 21516.244f),
                        new PositionData(-12181.925f, 4853.874f, 16836.572f),
                        new PositionData(-10265.825f, 3549.57f, -3891.976f),
                        new PositionData(762.083f, 3469.194f, -14038.974f),
                        new PositionData(16434.008f, 3757.063f, -12410.312f),
                        new PositionData(21436.303f, 4193.972f, -9308.101f),
                        new PositionData(16896.537f, 4280.988f, 16796.902f),
                        new PositionData(23135.414f, 4219.994f, 18382.58f),
                        new PositionData(35847.906f, 3849.727f, -10859.831f),
                        new PositionData(57903.93f, 3862.508f, -28574.541f),
                        new PositionData(41478.96f, 3759.095f, -28605.78f),
                        new PositionData(51270.066f, 5520.823f, -21256.365f),
                        new PositionData(19214.336f, 3220.695f, -23465.951f),
                        new PositionData(7070.854f, 3384.272f, -10622.472f),
                        new PositionData(40761.066f, 12129.716f, -29377.56f),
                        new PositionData(54941.105f, 8681.389f, -40252.55f),
                        new PositionData(48782.293f, 5632.946f, -21762.441f),
                        new PositionData(4860.995f, 3755.601f, 3526.692f),
                        new PositionData(5919.644f, 3619.263f, -14664.417f),
                        new PositionData(-5849.525f, 3577.75f, -3298.027f),
                        new PositionData(-13172.997f, 2403.968f, -14923.625f),
                        new PositionData(42314.34f, 4117.102f, -26335.064f),
                        new PositionData(-14431.704f, 3908.352f, 9861.34f),
                        new PositionData(-10801.41f, 4094.834f, 11200.675f),
                        new PositionData(54883.633f, 3758.108f, -25485.162f),
                        new PositionData(9031.136f, 3842.484f, 1608.268f),
                        new PositionData(9206.699f, 3785.78f, -9489.461f),
                        new PositionData(-11587.504f, 4846.443f, 19019.273f),
                        new PositionData(19755.889f, 3887.231f, 3865.777f),
                        new PositionData(17035.799f, 4266.285f, 21897.207f),
                        new PositionData(18518.959f, 4240.229f, 23226.98f),
                        new PositionData(23203.326f, 4257.181f, 26161.928f),
                        new PositionData(25062.227f, 4202.985f, 26425.88f),
                        new PositionData(27941.764f, 4198.559f, 23474.617f),
                        new PositionData(29884.836f, 4264.73f, 23836.984f),
                        new PositionData(31187.676f, 4340.013f, 3016.754f),
                        new PositionData(5836.976f, 3800.599f, 1130.748f),
                        new PositionData(-2248.225f, 3894.876f, 12578.184f),
                        new PositionData(-12182.686f, 4557.22f, 14162.512f),
                        new PositionData(-15861.762f, 4903.401f, 15100.186f),
                        new PositionData(-17665.0f, 3452.0f, -2723.0f),
                        new PositionData(42157.0f, 10985.0f, -30196.0f),
                        new PositionData(27100.0f, 8653.0f, -32410.0f),
                        new PositionData(48427.0f, 3838.0f, -17440.0f),
                        new PositionData(27694.0f, 3768.0f, 5624.0f),
                        new PositionData(24796.0f, 4174.0f, 22341.0f),
                        new PositionData(50363.0f, 3791.0f, -38433.0f),
                        new PositionData(19687.0f, 3834.0f, -7449.0f),
                        new PositionData(-7453.0f, 3586.0f, -15340.0f),
                        new PositionData(9649.0f, 3477.0f, -10704.0f),
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
                        new PositionData(-3000.962f, -48.674f, 4166.261f),
                        new PositionData(-4124.389f, -89.358f, 6671.349f),
                        new PositionData(-3740.196f, -513.617f, 311.138f),
                        new PositionData(-4795.743f, 1222.681f, 535.995f),
                        new PositionData(-10656.148f, 40.279f, 960.743f),
                        new PositionData(-6865.329f, 570.932f, 372.55f),
                        new PositionData(-6473.55f, 1082.51f, -830.619f),
                        new PositionData(-4446.308f, 394.008f, -826.342f),
                        new PositionData(-8477.49f, 1129.944f, -5355.018f),
                        new PositionData(-7020.403f, 433.505f, -3697.182f),
                        new PositionData(343.322f, -1325.585f, -5147.916f),
                        new PositionData(3504.97f, -1418.737f, -4828.855f),
                        new PositionData(2819.054f, -2395.2f, -2255.486f),
                        new PositionData(-3513.475f, -2566.835f, -90.747f),
                        new PositionData(3122.701f, -2316.052f, 2401.534f),
                        new PositionData(4086.682f, -1789.59f, 2683.65f),
                        new PositionData(3496.234f, -1704.012f, 4128.582f),
                        new PositionData(5090.766f, -2300.075f, 4523.075f),
                        new PositionData(-823.77f, -1868.559f, 3682.659f),
                        new PositionData(-511.783f, -1112.611f, 3325.688f),
                        new PositionData(-712.808f, -2548.258f, 7692.674f),
                        new PositionData(-1710.058f, -3986.606f, 8363.467f),
                        new PositionData(-3582.443f, -1815.138f, -766.527f),
                        new PositionData(-1510.201f, -2989.005f, -1876.295f),
                        new PositionData(-1013.668f, -1015.749f, 19.179f),
                        new PositionData(-2822.411f, -84.42f, 1593.245f),
                        new PositionData(-6137.338f, 3081.718f, -471.577f),
                        new PositionData(-8797.986f, 444.745f, 5289.278f),
                        new PositionData(-11029.582f, 812.787f, 275.984f),
                        new PositionData(-8232.83f, 590.869f, 2887.319f),
                        new PositionData(-6210.969f, 301.658f, 5179.545f),
                        new PositionData(-3763.569f, 238.562f, 6616.375f),
                        new PositionData(-9729.173f, 638.501f, -3363.058f),
                        new PositionData(-3598.566f, 102.026f, 1912.889f),
                        new PositionData(-4382.528f, 91.129f, -3263.447f),
                        new PositionData(-7206.017f, -766.553f, 9916.889f),
                        new PositionData(-10787.909f, -767.502f, 9922.212f),
                        new PositionData(-5448.63f, -903.151f, 9011.454f),
                        new PositionData(-6204.589f, -881.494f, 9387.128f),
                        new PositionData(-6051.855f, 42.284f, 9318.031f),
                        new PositionData(-650.0f, -2921.0f, -286.0f),
                        new PositionData(-1707.0f, -2264.0f, -1219.0f),
                        new PositionData(-5600.0f, -22.0f, 568.0f),
                        new PositionData(-5872.0f, 3181.0f, -624.0f),
                        new PositionData(-8098.0f, 260.0f, -3766.0f),
                        new PositionData(-8732.0f, -108.0f, 3971.0f),
                        new PositionData(-11073.0f, 505.0f, 5531.0f),
                        new PositionData(2006.0f, -2722.0f, 4816.0f),
                        new PositionData(4418.0f, -1791.0f, 4082.0f),
                        new PositionData(-4563.0f, 469.0f, 4962.0f),
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
                        new PositionData(-5422.37f, -439.292f, -8258.129f),
                        new PositionData(-801.102f, -75.972f, -6048.705f),
                        new PositionData(-2926.148f, -144.279f, -3773.856f),
                        new PositionData(5925.451f, 30.519f, -10857.948f),
                        new PositionData(6956.838f, -805.187f, -1781.407f),
                        new PositionData(-1941.0f, 38.05f, -5328.0f),
                        new PositionData(4195.388f, 4360.157f, 8152.485f),
                        new PositionData(-4300.223f, -419.43f, -1569.544f),
                        new PositionData(-1717.863f, 1518.222f, 1341.145f),
                        new PositionData(8697.516f, 1568.034f, 1152.273f),
                        new PositionData(8671.316f, 2387.059f, 4720.235f),
                        new PositionData(6153.008f, 1538.853f, 7414.664f),
                        new PositionData(1073.055f, 6553.167f, 10642.601f),
                        new PositionData(-2834.189f, 8838.2f, 13511.788f),
                        new PositionData(-13517.0f, -1317.453f, 19298.0f),
                        new PositionData(-10153.205f, 10607.175f, 17830.098f),
                        new PositionData(-20903.443f, -5086.674f, 19864.475f),
                        new PositionData(1470.559f, 153.027f, -8459.858f),
                        new PositionData(5798.667f, -439.49f, -9049.346f),
                        new PositionData(-20215.408f, -5471.216f, 21837.006f),
                        new PositionData(10022.551f, -437.345f, -11202.665f),
                        new PositionData(-23176.248f, -5522.46f, 21245.715f),
                        new PositionData(-22859.312f, -5524.778f, 19678.158f),
                        new PositionData(-17895.271f, -4782.056f, 21177.496f),
                        new PositionData(-6097.221f, 9703.689f, 17767.389f),
                        new PositionData(-4444.314f, 7237.529f, 11619.61f),
                        new PositionData(-6538.679f, 263.748f, -5148.935f),
                        new PositionData(6415.47f, 1990.944f, 2705.081f),
                        new PositionData(2589.873f, -483.415f, -5920.058f),
                        new PositionData(2589.873f, -483.415f, -5920.058f),
                        new PositionData(-3186.101f, 1518.222f, 4442.388f),
                        new PositionData(-3379.045f, 36.944f, -1919.799f),
                        new PositionData(-4677.277f, 973.646f, 302.227f),
                        new PositionData(6281.906f, -49.574f, -889.334f),
                        new PositionData(-680.129f, 1558.661f, 2920.697f),
                        new PositionData(6511.241f, -31.763f, -10536.228f),
                        new PositionData(-3704.234f, -442.623f, 1169.268f),
                        new PositionData(8385.075f, 2150.252f, 3597.872f),
                        new PositionData(-1752.095f, -735.311f, -6888.657f),
                        new PositionData(-4256.139f, 959.95f, 1233.426f),
                        new PositionData(-8127.0f, 10604.0f, 18303.0f),
                        new PositionData(-1897.0f, 1277.0f, -1391.0f),
                        new PositionData(-12757.0f, 1774.0f, 16543.0f),
                        new PositionData(2853.0f, 675.0f, -5351.0f),
                        new PositionData(-1566.0f, -118.0f, -8844.0f),
                        new PositionData(9231.0f, 91.0f, -6155.0f),
                        new PositionData(-5164.0f, 344.0f, -752.0f),
                        new PositionData(-14945.0f, -5562.0f, 24578.0f),
                        new PositionData(-14988.0f, -5565.0f, 22050.0f),
                        new PositionData(9575.0f, 46.0f, -10906.0f),
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
                        new PositionData(-5725.568f, -1960.727f, -1522.521f),
                        new PositionData(-15374.289f, -247.19f, 3428.477f),
                        new PositionData(-10347.546f, -1374.085f, 6763.552f),
                        new PositionData(-9417.81f, -1122.427f, 1685.693f),
                        new PositionData(-5792.65f, -2564.42f, 6568.514f),
                        new PositionData(8862.069f, -1115.878f, 2598.497f),
                        new PositionData(4727.105f, -2776.641f, -4131.424f),
                        new PositionData(4238.892f, -1236.588f, -16238.836f),
                        new PositionData(18083.691f, -316.541f, -14098.088f),
                        new PositionData(23191.469f, -1033.927f, -4409.087f),
                        new PositionData(23586.027f, -2024.318f, -14993.88f),
                        new PositionData(30742.459f, -1410.339f, -1003.987f),
                        new PositionData(28009.648f, -866.056f, 17.183f),
                        new PositionData(31188.582f, -360.455f, -2276.454f),
                        new PositionData(33203.625f, -931.627f, -822.873f),
                        new PositionData(25514.133f, -461.046f, 6940.465f),
                        new PositionData(16359.186f, -1465.798f, -15806.1f),
                        new PositionData(12772.348f, -3222.42f, -10957.551f),
                        new PositionData(-343.064f, -2201.117f, -23185.008f),
                        new PositionData(30491.469f, -2832.561f, -36417.285f),
                        new PositionData(37359.49f, -2107.139f, -29243.21f),
                        new PositionData(33502.07f, -1150.79f, -28668.307f),
                        new PositionData(32738.3f, -2476.938f, -22742.158f),
                        new PositionData(29093.576f, -2524.175f, -8754.486f),
                        new PositionData(18907.574f, -2297.902f, 2009.703f),
                        new PositionData(22643.57f, -1428.202f, -15130.665f),
                        new PositionData(26351.807f, -136.767f, -16625.537f),
                        new PositionData(19276.49f, -2123.13f, 7430.383f),
                        new PositionData(16005.339f, -2842.156f, 18104.553f),
                        new PositionData(-3547.285f, -2530.719f, -12861.225f),
                        new PositionData(-19827.283f, -2423.22f, -5877.53f),
                        new PositionData(-16067.822f, -2026.213f, -14005.335f),
                        new PositionData(-15494.705f, -386.468f, -3024.843f),
                        new PositionData(-11684.233f, -1716.743f, 2293.355f),
                        new PositionData(2107.423f, -1361.71f, -4726.938f),
                        new PositionData(-4344.043f, -1716.787f, -7234.633f),
                        new PositionData(24892.95f, -1493.477f, 12860.215f),
                        new PositionData(25276.863f, -1485.632f, 5165.148f),
                        new PositionData(19665.697f, -1393.835f, 2853.431f),
                        new PositionData(26199.914f, -1702.69f, -7270.132f),
                        new PositionData(-8669.0f, -477.0f, 7403.0f),
                        new PositionData(-3313.0f, -3137.0f, -17574.0f),
                        new PositionData(11189.0f, -2726.0f, -28697.0f),
                        new PositionData(32131.0f, -2882.0f, 8285.0f),
                        new PositionData(6627.0f, -1373.0f, -4994.0f),
                        new PositionData(20609.0f, -604.0f, -32807.0f),
                        new PositionData(26633.0f, -286.0f, -2347.0f),
                        new PositionData(7093.0f, -1461.0f, -14376.0f),
                        new PositionData(32715.0f, -674.0f, -29387.0f),
                        new PositionData(27586.0f, -879.0f, 4942.0f),
                    }
                }
            }
        }
    };

    public Dictionary<int, byte[]> CurrentPositionIndices = new();
    private Random _random;
    private int _chaosSeed;
    public int ChaosSeed
    {
        get => _chaosSeed;
        set
        {
            _chaosSeed = value;
            ShuffleIndices(value);
        }
    }

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
    }

    public void MoveCollectibles(int level)
    {
        if (!_positions.ContainsKey(level) || !CurrentPositionIndices.ContainsKey(level))
            return;
        //Logger.Write($"Moving collectibles for level {level}");
        for (var cogIndex = 0; cogIndex < 10; cogIndex++)
        {
            // THIS IS THE UGLIEST CODE I HAVE EVER AND WILL EVER WRITE
            var transform = _positions[level]["Cog"][CurrentPositionIndices[level][cogIndex]];
            var cogPosAddr = PointerCalculations.GetPointerAddress(0x270310, new[] { 0x144 * cogIndex + 0x8, 0x74 });
            var bytesToWrite = 
                BitConverter.GetBytes(transform.X)
                .Concat(BitConverter.GetBytes(transform.Y))
                .Concat(BitConverter.GetBytes(transform.Z)).ToArray();
            //Logger.Write($"Writing {transform.X}, {transform.Y}, {transform.Z} for cog at index {CurrentPositionIndices[level][cogIndex]}");
            ProcessHandler.WriteData(cogPosAddr, bytesToWrite);
        }
        for (var bilbyIndex = 0; bilbyIndex < 5; bilbyIndex++)
        {
            // THIS IS THE UGLIEST CODE I HAVE EVER AND WILL EVER WRITE
            var transform = _positions[level]["Bilby"][CurrentPositionIndices[level][bilbyIndex + 10]];
            var cagePosAddr = PointerCalculations.GetPointerAddress(0x27D608, new[] { 0x8, 0x134 * bilbyIndex + 0x8, 0x74 });
            var bilbyPosAddr = PointerCalculations.GetPointerAddress(0x27D608, new[] { 0x8, 0x134 * bilbyIndex + 0x4, 0x74 });
            var bytesToWrite = 
                BitConverter.GetBytes(transform.X)
                    .Concat(BitConverter.GetBytes(transform.Y))
                    .Concat(BitConverter.GetBytes(transform.Z)).ToArray();
            ProcessHandler.WriteData(cagePosAddr, bytesToWrite);
            ProcessHandler.WriteData(bilbyPosAddr, bytesToWrite);
            // WRITE ROTATION
            // THIS IS GOING TO BE MORE PAINFUL THAN I THOUGHT LINEAR ALGEBRA TIME AGAIN
        }
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
}