using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;
using SnesGFX;
using SnesGFX.SNES;

namespace SuperSMWTextEditor
{
    static class Program
    {
        #region Contants
        /// <summary>
        /// Makes your window double buffered, killing flickering.
        /// </summary>
        public const ControlStyles DoubleBuffered = ControlStyles.AllPaintingInWmPaint
            | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer;
        #endregion

        #region Overused Functions
        public static byte[] LoadL3Graphics()
        {
            byte[] VRAM = new byte[0x4000]; // 0x4000 = 1/(8/2) 0x10000

            byte decompType = Program.ReadByte(0x0FFFEB);

            if (decompType != 2) // LZ3
            {
                decompType = 1; // LZ2
            }

            byte[][] file = new byte[4][];

            if (!File.Exists("./GFX28.bin"))
                file[0] = Decomp(0x28, 2048, decompType);
            else
                file[0] = File.ReadAllBytes("./GFX28.bin");

            if (!File.Exists("./GFX29.bin"))
                file[1] = Decomp(0x29, 2048, decompType);
            else
                file[1] = File.ReadAllBytes("./GFX29.bin");

            if (!File.Exists("./GFX2A.bin"))
                file[2] = Decomp(0x2A, 2048, decompType);
            else
                file[2] = File.ReadAllBytes("./GFX2A.bin");

            if (!File.Exists("./GFX2B.bin"))
                file[3] = Decomp(0x2B, 2048, decompType);
            else
                file[3] = File.ReadAllBytes("./GFX2B.bin");

            for (int i = 0, x = 0; i < 4; ++i, x += 0x0800)
            {
                file[i].CopyTo(VRAM, x);
                file[i] = null;
            }

            VRAM = Program.GetBitmap(VRAM, 2);

            for (int y = 512 - 8; y < 512; ++y) // tile 0x3FF is special.
            {
                for (int x = 128 - 8; x < 128; ++x)
                {
                    VRAM[(y << 7) + x] = 15;
                }
            }

            return VRAM;
        }
        #endregion

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            if (args.Length == 1)
            {
                if (File.Exists(args[0]))
                {
                    try
                    {
                        LoadROM(args[0]);
                    }
                    catch(Exception e)
                    {
                        MessageBox.Show(e.Message, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }

            Application.Run(new MDIParent1());
        }

        #region Tilemap Handling
        /// <summary>
        /// Reads a Stripe Image and writes to a tilemap.
        /// </summary>
        /// <param name="buffer">Contents of the stripe image.</param>
        /// <param name="tilemap">The tilemap to write.</param>
        /// <param name="allow512x">Allows writing to 64x64 tilemap area?</param>
        /// <param name="reverse">True to WRITE to stripe image instead of reverse.</param>
        /// <param name="startIndex">Start index of buffer.</param>
        /// <returns>the last stripe index.</returns>
        public static int StripeImageRt(ref byte[] buffer, ref int[] tilemap,
            int startIndex, bool allow512x = true, bool reverse = false)
        {
            //|Byte 1| |Byte 2| |Byte 3| |Byte 4|
            //EHHHYXyy yyyxxxxx DRLLLLLL llllllll
            //E              = End of data
            //HHH            = Data destination (VRAM address, see below)
            //Xxxxxx         = X coordinate
            //Yyyyyy         = Y coordinate
            //D              = Direction (0 = Horizontal, 1 = Vertical)
            //R              = RLE (see below)
            //LLLLLLllllllll = Length (amount of bytes to upload - 1)
            try
            {
                int index = startIndex;

                while ((buffer[index] & 0x80) == 0)
                {
                    int X = buffer[index + 1] & 0x1F;
                    int Y = (buffer[index] & 3) << 3;
                    Y |= buffer[index + 1] >> 5 & 7;

                    // if !XX && !YY, pos = 0x0000;
                    // if XX, pos = 0x0400;
                    // if YY, pos = 0x0800;
                    // if XX && YY, pos = 0x1800;

                    int basePosition = 0;

                    switch ((buffer[index] >> 2 & 3))
                    {
                        case 1: basePosition = 0x0400; break;
                        case 2: basePosition = 0x0800; break;
                        case 3: basePosition = 0x1800; break;
                    }

                    if (!allow512x)
                    {
                        basePosition = 0;
                    }

                    bool ToDown = buffer[index + 2] >> 7 != 0;
                    bool RLE = (buffer[index + 2] >> 6 & 1) != 0;

                    int LENGTH = buffer[index + 3];
                    LENGTH |= (buffer[index + 2] & 0x3F) << 8;
                    LENGTH++;

                    index += 4;

                    for (int i = 0; i < LENGTH; i += 2)
                    {
                        int position = basePosition + X + Y * 0x20;

                        if (RLE)
                        {
                            if (reverse)
                            {
                                if (i == 0)
                                {
                                    buffer[index] = (byte)(tilemap[position] & 0xFF);
                                    buffer[index + 1] = (byte)(tilemap[position] >> 8 & 0xFF);
                                }
                            }
                            else
                            {
                                tilemap[position] = buffer[index] | (buffer[index + 1] << 8);
                            }
                        }
                        else
                        {
                            if (reverse)
                            {
                                buffer[index + i] = (byte)(tilemap[position] & 0xFF);
                                buffer[index + i + 1] = (byte)(tilemap[position] >> 8 & 0xFF);
                            }
                            else
                            {
                                tilemap[position] = buffer[index + i] | (buffer[index + 1 + i] << 8);
                            }
                        }

                        if (ToDown)
                        {
                            Y++;
                            Y &= 0x1F;
                        }
                        else
                        {
                            X++;
                            X &= 0x1F;
                        }
                    }

                    if (RLE)
                    {
                        index += 2;
                    }
                    else
                    {
                        index += LENGTH;
                    }
                }

                return index + 1;
            }
            catch
            {
                throw new Exception(string.Format("Couldn't {0} stripe image!",
                    reverse ? "write" : "read"));
            }
        }

        #endregion

        #region Pixel Handling
        /// <summary>
        /// Decompress a ExGFX file from a SMW ROM.
        /// Note that only GFXs 0x00-0x31 is supported right now.
        /// </summary>
        /// <param name="number">The GFX number. Only 0x00-0x31 is valid.</param>
        /// <param name="maxSize">Maximium size for ExGFX file (i.e 2048 for 2bpp,
        /// 4096 for 4bpp, etc.)</param>
        /// <param name="format">The format to decompress. 1 is LZ2 and 2 is LZ3</param>
        /// <returns>The decompressed data</returns>
        public static unsafe byte[] Decomp(int number, uint maxSize, uint format)
        {
            byte low = Program.ReadByte(0x00B992 + number);
            byte middle = Program.ReadByte(0x00B9C4 + number);
            byte bank = Program.ReadByte(0x00B9F6 + number);

            int offset = low | (middle << 8) | (bank << 16);

            Program.CloseHandle(); // close the handle since it'll get handled by LC.

            byte[] destination = new byte[maxSize];
            uint size = 0;

            fixed (byte* ptr = destination)
            {
                if (!LC.LunarOpenFile(Program.Current, 0))
                {
                    throw new Exception("Couldn't open ROM.");
                }
                size = LC.LunarDecompress(ptr, (uint)Program.snes2pc(offset), maxSize, format, 0, null);
            }

            LC.LunarCloseFile();

            if (size == 0)
            {
                throw new Exception("Couldn't decompress the GFX");
            }
            else if (size > maxSize)
            {
                throw new Exception("The GFX file is larger than should be.");
            }

            return destination;
        }


        static IBitformat[] formats = new IBitformat[] {
            new _2BPP(), new _3BPP(),
            new _4BPP(), new _8BPP(),
        };

        static int[] bppTable = new int[] {
            -1, -1, 0, 1, 2, -1, -1, -1, 3
        };

        public static byte[] GetBitmap(byte[] raw, int bpp = 4)
        {
            return formats[bppTable[bpp]].Decode(raw);
        }

        /// <summary>
        /// Draws a tilemap on screen on a specific bitmap.
        /// </summary>
        /// <param name="bitmap">The bitmap to be drawn. For better results, use a pixel format of 32-bits.</param>
        /// <param name="destRect">The area of bitmap to be drawn. Width and Height should be a multiple of 8.</param>
        /// <param name="tiles">The number of tiles to draw.</param>
        /// <param name="palette">A pointer for a 32-bit palette (ARGB) of 256 entries.</param>
        /// <param name="tilemap">A pointer for the tilemap (SNES format: -------- -------- YXPCCCTT TTTTTTTT). The P flag is not interpreted though.</param>
        /// <param name="graphics">A pointer for the graphics, in bitmap format, which could be up to 8-bit, indexing the palette. The width should be 128px and the maximium height is 512px.</param>
        /// <param name="bitsPerPixel">The number of bits per pixel in graphics.</param>
        /// <param name="width">Log2 of area.Width</param>
        /// <remarks>Sending any invalid argument may CRASH.</remarks>
        public static unsafe void DrawTilemap(Bitmap bitmap, Rectangle destRect, int tiles,
            int* palette, int* tilemap, byte* graphics, int bitsPerPixel, int width)
        {
            BitmapData lockArea = bitmap.LockBits(destRect, ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb);

            int* ptr = (int*)lockArea.Scan0;
            int theWidth = destRect.Width;
            int x = 0, y = 0, posBase, palnum;
            int flipX, flipY;
            int yInc = 8 << width;
            int baseRender;
            int yBaseRender;
            int yBaseRender2;

            for (int i = 0; i < tiles; ++i, ++tilemap)
            {
                baseRender = (*tilemap << 3 & 0x78) + (*tilemap << 6 & 0xFC00);

                flipX = (*tilemap & 0x4000) != 0 ? 7 : 0;
                flipY = (*tilemap & 0x8000) != 0 ? 7 : 0;
                palnum = (*tilemap >> 10 & 7) << bitsPerPixel;
                posBase = y + x;

                for (int yy = 0; yy < 8; ++yy)
                {
                    yBaseRender = baseRender + ((yy ^ flipY) << 7);
                    yBaseRender2 = posBase + (yy << width);

                    for (int xx = 0; xx < 8; ++xx)
                    {
                        ptr[yBaseRender2 + xx] =
                            palette[graphics[xx ^ flipX + yBaseRender] + palnum];
                    }
                }

                if ((x += 8) == theWidth)
                {
                    x = 0;
                    y += yInc;
                }
            }

            bitmap.UnlockBits(lockArea);
        }
        #endregion

        #region ROM Handling
        static RomType DetectType(FileStream fs, bool header)
        {
            string romName = @"SUPER MARIOWORLD     ";

            byte[] rawName = new byte[romName.Length];

            fs.Position = 0x7FC0 + (header ? 512 : 0);
            fs.Read(rawName, 0, romName.Length);

            string cmpName = new ASCIIEncoding().GetString(rawName);

            byte romType, romKart, romSize;

            fs.Position = 0x7FD5 + (header ? 512 : 0);
            romType = (byte)fs.ReadByte();
            romKart = (byte)fs.ReadByte();
            romSize = (byte)fs.ReadByte();

            if (romName != cmpName)
                throw new Exception("This ROM have a wrong title or format!");

            switch (romType)
            {
                case 0x20:
                    if (romSize == 0x0D)
                    {
                        if (fs.Length < (4 * 1024 * 1024 + (header ? 512 : 0)))
                        {
                            throw new Exception("This ROM is too small for type ExLoROM!");
                        }
                        else if (fs.Length != (8 * 1024 * 1024 + (header ? 512 : 0)))
                        {
                            throw new Exception("Invalid ExLoROM size.");
                        }

                        rawName = new byte[romName.Length];
                        fs.Position = 0x407FC0 + (header ? 512 : 0);
                        fs.Read(rawName, 0, romName.Length);
                        cmpName = new ASCIIEncoding().GetString(rawName);

                        if (romName != cmpName)
                            throw new Exception("This ROM have a wrong title or format!");

                        return RomType.ExLoROM;
                    }
                    else
                    {
                        return RomType.LoROM;
                    }

                case 0x23:
                    if (romKart == 0x35 || romKart == 0x34)
                    {
                        return RomType.SA1ROM;
                    }
                    else
                    {
                        throw new Exception("Invalid SA-1 ROM header!");
                    }

                case 0x30:
                    if (romKart >= 0x02)
                    {
                        if (fs.Length > (4 * 1024 * 1024 + (header ? 512 : 0)))
                        {
                            throw new Exception("This ROM is too larger for type FastROM!");
                        }
                        return RomType.FastROM;
                    }
                    else
                    {
                        throw new Exception("Invalid FastROM header!");
                    }

                default:
                    throw new Exception("Invalid ROM type!");
            }
        }

        static bool ExamineLength(long length)
        {
            var header = false;

            if (length % 0x8000 == 512)
            {
                length -= 512;
                header = true;
            }
            else if (length % 0x800 != 0)
            {
                throw new Exception("Invalid ROM size.");
            }

            if (length > 8 * 1024 * 1024)
            {
                throw new Exception("The ROM is too larger.");
            }
            else if (length < 512 * 1024)
            {
                throw new Exception("This ROM is too small to be a valid Super Mario World ROM.");
            }

            return header;
        }

        public static void LoadROM(string filename)
        {
            bool header;
            RomType type;

            using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                header = ExamineLength(fs.Length);
                type = DetectType(fs, header);
            }

            Program.type = type;
            Program.header = header;
            Program.Current = filename;
            Program.Loaded = true;
        }

        public static int pc2snes(int pc, bool allowHeaderFix = true)
        {
            int returns;

            if (header && allowHeaderFix)
            {
                pc -= 0x200;
            }

            switch (type)
            {
                case RomType.FastROM:
                    returns = returns = (pc & 0x7fff) | (pc << 1 & 0x7f0000) | 0x808000;
                    break;

                case RomType.SA1ROM:
                    returns = (pc & 0x7fff) | (pc << 1 & 0x7f0000) | 0x8000;
                    if (returns > 0x3FFFFF) returns |= 0x800000;

                    if (pc >= 0x400000) returns |= 0x1000000; // bank switch required.
                    break;

                case RomType.ExLoROM:
                    returns = (pc & 0x7fff) | (pc << 1 & 0x7f0000) | 0x8000;

                    if (pc < 0x400000)
                    {
                        returns |= 0x800000;
                    }
                    break;

                default: // LoROM
                    returns = (pc & 0x7fff) | (pc << 1 & 0x7f0000) | 0x8000;
                    if (returns > 0x6FFFFF) returns |= 0x800000;
                    break;
            }

            return returns;
        }

        public static int snes2pc(int snes)
        {
            int returns;

            switch (type)
            {
                case RomType.SA1ROM:
                    if (snes >= 0x808000)
                    {
                        snes -= 0x400000;
                        // $80:8000-$BF:FFFF->$40:8000-$7F:FFFF
                    }
                    goto default;

                case RomType.ExLoROM:
                    returns = (snes & 0x7fff) | ((snes & 0x7f0000) >> 1) | (~snes >> 1 & 0x400000);
                    break;

                default: // FastROM, LoROM
                    returns = (snes & 0x7fff) | ((snes & 0x7f0000) >> 1);
                    break;
            }

            if (header)
            {
                returns += 512;
            }

            return returns;
        }

        public static byte ReadByte(int snes)
        {
            OpenHandle();
            fs.Position = snes2pc(snes);
            return (byte)fs.ReadByte();
        }

        public static byte[] ReadROM(int snes, int length)
        {
            OpenHandle();
            byte[] output = new byte[length];
            fs.Position = snes2pc(snes);
            fs.Read(output, 0, length);
            return output;
        }

        public static void WriteROM(int snes, byte[] data)
        {
            byte[] old = new byte[data.Length];

            OpenHandle();
            fs.Position = snes2pc(snes);
            fs.Read(old, 0, old.Length);
            fs.Position -= old.Length;
            fs.Write(data, 0, data.Length);

            // this is a fast method of computing checksum
            // just subtract the difference.
            // of course, if the checksum is invalid, it'll keep invalid :V
            CheckSum -= SumDiff(old, data);
        }

        public static void CloseHandle()
        {
            if (openHandle)
            {
                fs.Close();
                fs.Dispose();
                fs = null;
                openHandle = false;
            }
        }

        static int SumDiff(byte[] one, byte[] two)
        {
            int sum = 0;

            for (int i = 0, j = one.Length; i < j; ++i)
            {
                sum += one[i] - two[i];
            }

            return sum;
        }

        static void OpenHandle()
        {
            if (!openHandle)
            {
                fs = new FileStream(Current, FileMode.Open, FileAccess.ReadWrite);
                openHandle = true;
            }
        }

        static bool openHandle;
        static FileStream fs;
        static bool header;
        static RomType type;

        public static string Current { get; private set; }
        public static bool Loaded { get; private set; }
        public static int CheckSum
        {
            get
            {
                int baseAddress = 0;

                if (type == RomType.ExLoROM)
                {
                    baseAddress = 0x407FDE;
                }
                else
                {
                    baseAddress = 0x7FDE;
                }

                if (header)
                {
                    baseAddress += 512;
                }

                OpenHandle();
                fs.Position = baseAddress;
                int checksum;
                checksum = fs.ReadByte();
                checksum |= fs.ReadByte() << 8;

                return checksum & 0xFFFF;
            }
            set
            {
                int baseAddress = 0;

                if (type == RomType.ExLoROM)
                {
                    baseAddress = 0x407FDC;
                }
                else
                {
                    baseAddress = 0x7FDC;
                }

                if (header)
                {
                    baseAddress += 512;
                }

                int XOR = value ^ 0xFFFF;

                OpenHandle();
                fs.Position = baseAddress;
                fs.WriteByte((byte)(XOR & 0xFF));
                fs.WriteByte((byte)(XOR >> 8 & 0xFF));
                fs.WriteByte((byte)(value & 0xFF));
                fs.WriteByte((byte)(value >> 8 & 0xFF));

                if (type == RomType.ExLoROM)
                {
                    fs.Position = baseAddress - 0x400000;
                    fs.WriteByte((byte)(XOR & 0xFF));
                    fs.WriteByte((byte)(XOR >> 8 & 0xFF));
                    fs.WriteByte((byte)(value & 0xFF));
                    fs.WriteByte((byte)(value >> 8 & 0xFF));
                }
            }
        }
        #endregion
    }
}
