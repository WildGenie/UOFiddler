using System;
using System.Collections.Generic;
using System.IO;

namespace Ultima
{
    public sealed class CalibrationInfo
    {
        public byte[] Mask { get; }
        public byte[] Vals { get; }
        public byte[] DetX { get; }
        public byte[] DetY { get; }
        public byte[] DetZ { get; }
        public byte[] DetF { get; }

        public CalibrationInfo(byte[] mask, byte[] vals, byte[] detx, byte[] dety, byte[] detz, byte[] detf)
        {
            Mask = mask;
            Vals = vals;
            DetX = detx;
            DetY = dety;
            DetZ = detz;
            DetF = detf;
        }

        private static byte[] ReadBytes(StreamReader ip)
        {
            string line = ip.ReadLine();

            if (line == null)
            {
                return null;
            }

            var buffer = new byte[(line.Length + 2) / 3];
            int index = 0;

            for (int i = 0; (i + 1) < line.Length; i += 3)
            {
                char ch = line[i + 0];
                char cl = line[i + 1];

                if (ch >= '0' && ch <= '9')
                {
                    ch -= '0';
                }
                else if (ch >= 'a' && ch <= 'f')
                {
                    ch -= (char)('a' - 10);
                }
                else if (ch >= 'A' && ch <= 'F')
                {
                    ch -= (char)('A' - 10);
                }
                else
                {
                    return null;
                }

                if (cl >= '0' && cl <= '9')
                {
                    cl -= '0';
                }
                else if (cl >= 'a' && cl <= 'f')
                {
                    cl -= (char)('a' - 10);
                }
                else if (cl >= 'A' && cl <= 'F')
                {
                    cl -= (char)('A' - 10);
                }
                else
                {
                    return null;
                }

                buffer[index++] = (byte)((ch << 4) | cl);
            }

            return buffer;
        }

        public static CalibrationInfo[] DefaultList { get; set; } = {
            new CalibrationInfo(
                //Post 7.0.4.0 (Andreew)
                new byte[]
                {
                    0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF,
                    0xFF, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF
                },
                new byte[]
                {
                    0xFF, 0xD0, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x8B, 0x0D, 0x00, 0x00, 0x00, 0x00, 0x8B, 0x11, 0x8B,
                    0x82, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xD0, 0x5B, 0x83, 0x00, 0x00, 0x00, 0x00, 0x00, 0xEC
                },
                new byte[]{ 0x22, 0x04, 0xFF, 0xFF, 0xFF, 0x04, 0x0C }, //x
                new byte[]{ 0x22, 0x04, 0xFF, 0xFF, 0xFF, 0x04, 0x08 }, //y
                new byte[]{ 0x22, 0x04, 0xFF, 0xFF, 0xFF, 0x04, 0x04 }, //z
                new byte[]{ 0x22, 0x04, 0xFF, 0xFF, 0xFF, 0x04, 0x10 }),//f
            new CalibrationInfo(
                /* (arul) 6.0.9.x+ : Calibrates both  */
                new byte[]
                {
                    0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF,
                    0xFF, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF
                },
                new byte[]
                {
                    0xFF, 0xD0, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x8B, 0x0D, 0x00, 0x00, 0x00, 0x00, 0x8B, 0x11, 0x8B,
                    0x82, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xD0, 0x5E, 0xE9, 0x00, 0x00, 0x00, 0x00, 0x8B, 0x0D
                },
                new byte[]{ 0x1F, 0x04, 0xFF, 0xFF, 0xFF, 0x04, 0x0C },
                new byte[]{ 0x1F, 0x04, 0xFF, 0xFF, 0xFF, 0x04, 0x08 },
                new byte[]{ 0x1F, 0x04, 0xFF, 0xFF, 0xFF, 0x04, 0x04 },
                new byte[]{ 0x1F, 0x04, 0xFF, 0xFF, 0xFF, 0x04, 0x10 }),
            new CalibrationInfo(
                /* Facet */
                new byte[]
                {
                    0xFF, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF
                },
                new byte[]
                {
                    0xA0, 0x00, 0x00, 0x00, 0x00, 0x84, 0xC0, 0x0F, 0x85, 0x00, 0x00, 0x00, 0x00, 0x8B, 0x0D
                },
                Array.Empty<byte>(),
                Array.Empty<byte>(),
                Array.Empty<byte>(),
                new byte[]{ 0x01, 0x04, 0xFF, 0xFF, 0xFF, 0x01 }
            ),
            new CalibrationInfo(
                /* Location */
                new byte[]
                {
                    0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0xFF, 0x00, 0x00,
                    0x00, 0x00, 0xFF, 0xFF, 0xFF, 0x00, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0x00
                },
                new byte[]
                {
                    0x8B, 0x15, 0x00, 0x00, 0x00, 0x00, 0x83, 0xC4, 0x10, 0x66, 0x89, 0x5A, 0x00, 0xA1, 0x00, 0x00,
                    0x00, 0x00, 0x66, 0x89, 0x78, 0x00, 0x8B, 0x0D, 0x00, 0x00, 0x00, 0x00, 0x66, 0x89, 0x71, 0x00
                },
                new byte[]{ 0x02, 0x04, 0x04, 0x0C, 0x01, 0x02 },
                new byte[]{ 0x0E, 0x04, 0x04, 0x15, 0x01, 0x02 },
                new byte[]{ 0x18, 0x04, 0x04, 0x1F, 0x01, 0x02 },
                Array.Empty<byte>()
            ),
            new CalibrationInfo(
                /* UO3D Only, calibrates both */
                new byte[]
                {
                    0xFF, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0xFF, 0xFF,
                    0xFF, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
                    0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00
                },
                new byte[]
                {
                    0xA1, 0x00, 0x00, 0x00, 0x00, 0x68, 0x40, 0x2E, 0x04, 0x01, 0x0F, 0xBF, 0x50, 0x00, 0x0F, 0xBF,
                    0x48, 0x00, 0x52, 0x51, 0x0F, 0xBF, 0x50, 0x00, 0x52, 0x8D, 0x85, 0xE4, 0xFD, 0xFF, 0xFF, 0x68,
                    0x00, 0x00, 0x00, 0x00, 0x50, 0xE8, 0x07, 0x44, 0x10, 0x00, 0x8A, 0x0D, 0x00, 0x00, 0x00, 0x00
                },
                new byte[] { 0x01, 0x04, 0x04, 0x17, 0x01, 0x02 },
                new byte[] { 0x01, 0x04, 0x04, 0x11, 0x01, 0x02 },
                new byte[] { 0x01, 0x04, 0x04, 0x0D, 0x01, 0x02 },
                new byte[] { 0x2C, 0x04, 0xFF, 0xFF, 0xFF, 0x01 }
            )
        };

        public static CalibrationInfo[] GetList()
        {
            var list = new List<CalibrationInfo>();

            string path = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
            path = Path.Combine(path, "calibration.cfg");

            if (File.Exists(path))
            {
                using (var ip = new StreamReader(path))
                {
                    string line;

                    while ((line = ip.ReadLine()) != null)
                    {
                        line = line.Trim();

                        if (!line.Equals("Begin", StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        byte[] mask, vals, detx, dety, detz, detf;

                        if ((mask = ReadBytes(ip)) == null)
                        {
                            continue;
                        }

                        if ((vals = ReadBytes(ip)) == null)
                        {
                            continue;
                        }

                        if ((detx = ReadBytes(ip)) == null)
                        {
                            continue;
                        }

                        if ((dety = ReadBytes(ip)) == null)
                        {
                            continue;
                        }

                        if ((detz = ReadBytes(ip)) == null)
                        {
                            continue;
                        }

                        if ((detf = ReadBytes(ip)) == null)
                        {
                            continue;
                        }

                        list.Add(new CalibrationInfo(mask, vals, detx, dety, detz, detf));
                    }
                }
            }

            list.AddRange(DefaultList);

            return list.ToArray();
        }
    }
}