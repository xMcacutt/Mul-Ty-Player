using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayer
{
    internal class RotationMatrix
    {
        public float M11;
        public float M12;
        public float M13;
        public float M14;
        public float M21;
        public float M22;
        public float M23;
        public float M24;
        public float M31;
        public float M32;
        public float M33;
        public float M34;

        public RotationMatrix(float pitch, float yaw, float roll)
        {
            M11 = (float)(Math.Cos(yaw) * Math.Cos(pitch));
            M12 = (float)((Math.Cos(yaw) * Math.Sin(pitch) * Math.Sin(roll)) - (Math.Sin(yaw) * Math.Cos(roll)));
            M13 = (float)((Math.Cos(yaw) * Math.Sin(pitch) * Math.Cos(roll)) + (Math.Sin(yaw) * Math.Sin(roll)));
            M14 = 0.0f;
            M21 = (float)(Math.Sin(yaw) * Math.Cos(pitch));
            M22 = (float)((Math.Sin(yaw) * Math.Sin(pitch) * Math.Sin(roll)) + (Math.Cos(yaw) * Math.Cos(roll)));
            M23 = (float)((Math.Sin(yaw) * Math.Sin(pitch) * Math.Cos(roll)) - (Math.Cos(yaw) * Math.Sin(roll)));
            M24 = 0.0f;
            M31 = (float)-Math.Sin(pitch);
            M32 = (float)(Math.Cos(pitch) * Math.Sin(roll));
            M33 = (float)(Math.Cos(pitch) * Math.Cos(roll));
            M34 = 0.0f;
        }

        public RotationMatrix(float yaw)
        {
            M11 = (float)Math.Cos(yaw);
            M12 = 0.0f;
            M13 = (float)Math.Sin(yaw);
            M14 = 0.0f;
            M21 = 0.0f;
            M22 = 1.0f;
            M23 = 0.0f;
            M24 = 0.0f;
            M31 = (float)-Math.Sin(yaw);
            M32 = 0.0f;
            M33 = (float)Math.Cos(yaw);
            M34 = 0.0f;
        }

        public byte[] GetBytes()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(BitConverter.GetBytes(M11));
                ms.Write(BitConverter.GetBytes(M12));
                ms.Write(BitConverter.GetBytes(M13));
                ms.Write(BitConverter.GetBytes(M14));
                ms.Write(BitConverter.GetBytes(M21));
                ms.Write(BitConverter.GetBytes(M22));
                ms.Write(BitConverter.GetBytes(M23));
                ms.Write(BitConverter.GetBytes(M24));
                ms.Write(BitConverter.GetBytes(M31));
                ms.Write(BitConverter.GetBytes(M32));
                ms.Write(BitConverter.GetBytes(M33));
                ms.Write(BitConverter.GetBytes(M34));
                return ms.ToArray();
            }
        }
    }
}