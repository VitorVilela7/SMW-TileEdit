using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SuperSMWTextEditor
{
    public partial class EditMusicText2 : Form
    {
        int[][] charLimit = new int[][] { // file select, delete, 1/2 players, 4 entries
            new int[] {
                16, 16, 16, 10,
                22, 22, 22, 3,
                13, 13, -1, -1,
            },
        };

        int[][] tileStart = new int[][] {
            new int[] {
                0x1EA, 0x22A, 0x26A, 0x2AA,
                0x1E7, 0x227, 0x267, 0x2A7,
                0x20A, 0x24A, -1, -1,
            },
        };

        string[] texts;
        bool lockChanges = true;
        int type;

        public EditMusicText2()
        {
            InitializeComponent();
            comboBox2.SelectedIndex = 0;
        }

        void UpdateStr(int slot, int startTile, int length)
        {
            if (startTile == -1)
            {
                texts[slot] = "";
                return;
            }

            length += startTile;

            int[] current;
            switch (this.current)
            {
                case 0: current = fileSelect; break;
                case 1: current = eraseMenu; break;
                case 2: current = playerSelect; break;

                default: throw new Exception("Internal Error!");
            }

            var output = new StringBuilder();

            do
            {
                output.Append(SMWtoChar(current[startTile]));
            } while (++startTile < length);

            texts[slot] = output.ToString();
        }

        void UpdateStrs()
        {
            var index = this.current * 4;
            texts = new string[4];

            textBox1.MaxLength = charLimit[type][0 + index];
            textBox2.MaxLength = charLimit[type][1 + index];

            if (charLimit[type][2 + index] != -1)
            {
                textBox3.Enabled = true;
                textBox3.MaxLength = charLimit[type][2 + index];
            }
            else
                textBox3.Enabled = false;

            if (charLimit[type][3 + index] != -1)
            {
                textBox4.Enabled = true;
                textBox4.MaxLength = charLimit[type][3 + index];
            }
            else
                textBox4.Enabled = false;

            for (int i = 0; i < 4; ++i)
            {
                UpdateStr(i, tileStart[type][i + index], charLimit[type][i + index]);
            }

            textBox1.Text = texts[0];
            textBox2.Text = texts[1];
            textBox3.Text = texts[2];
            textBox4.Text = texts[3];
        }

        int[] fileSelect;
        int[] eraseMenu;
        int[] playerSelect;
        int current;

        public delegate void TilemapUpdate(int[] fileSelect, int[] eraseMenu, int[] playerSelect);
        public event TilemapUpdate UpdateTilemapFrom;

        public void UpdateTilemapTo(int[] fileSelect,
            int[] eraseMenu, int[] playerSelect, int selected, int type)
        {
            lockChanges = true;

            this.playerSelect = playerSelect;
            this.fileSelect = fileSelect;
            this.eraseMenu = eraseMenu;
            this.current = selected;
            this.type = type;

            UpdateStrs();
            lockChanges = false;
        }

        char SMWtoChar(int x)
        {
            x &= 0x3FF;

            if (x >= 0 && x <= 9)
            {
                return (char)('0' + x);
            }
            else if (x >= 0xA && x <= ('Z' - 'A' + 0x0A))
            {
                return (char)('A' + x - 0x0A);
            }
            else if (x >= 0X122 && x <= (0X122 + 9))
            {
                return (char)('0' + x - 0x122);
            }
            else
            {
                switch (x)
                {
                    case 0xFC: return ' ';
                    case 0x24: return '.';
                    case 0x25: return ',';
                    case 0x26: return '*';
                    case 0x27: return '_';
                    case 0x28: return '!';
                    case 0x77: return '=';
                    case 0x78: return ':';

                    case 0x171: return 'A';
                    case 0x12C: return 'B';
                    case 0x12D: return 'C';
                    case 0x7C: return 'D';
                    case 0x173: return 'E';
                    case 0x175: return 'G';
                    case 0x84: return 'H';
                    case 0x82: return 'I';
                    case 0X170: return 'L';
                    case 0x176: return 'M';
                    case 0X79: return 'N';
                    case 0x83: return 'O';
                    case 0X16F: return 'P';
                    case 0x174: return 'R';
                    case 0X131: return 'S';
                    case 0X12F: return 'T';
                    case 0X7B: return 'U';
                    case 0X80: return 'V';
                    case 0X81: return 'W';
                    case 0X172: return 'Y';
                    case 0X121: return 'Z';


                    case 0X13E: return '0';

                    case 0x16D: return '1';
                    case 0X16E: return '2';
                    case 0X4E: return '3';
                    case 0X50: return '4';
                    case 0X51: return '5';
                    case 0X52: return '6';
                    case 0X53: return '7';

                    default: return '^';
                }
            }
        }

        int charToSMW(char c)
        {
            if (comboBox2.SelectedIndex == 0) // try with standard font
            {
                switch (Char.ToUpper(c))
                {
                    case 'A': return 0x171;
                    case 'B': return 0x12C;
                    case 'C': return 0x12D;
                    case 'D': return 0x7C;
                    case 'E': return 0x173;
                    case 'G': return 0x175;
                    case 'H': return 0x84;
                    case 'I': return 0x82;
                    case 'L': return 0X170;
                    case 'M': return 0x176;
                    case 'N': return 0X79;
                    case 'O': return 0x83;
                    case 'P': return 0X16F;
                    case 'R': return 0x174;
                    case 'S': return 0X131;
                    case 'T': return 0X12F;
                    case 'U': return 0X7B;
                    case 'V': return 0X80;
                    case 'W': return 0X81;
                    case 'Y': return 0X172;
                    case 'Z': return 0X121;
                    case '0': return 0X13E;

                    case '1': return 0x16D;
                    case '2': return 0X16E;
                    case '3': return 0X4E;
                    case '4': return 0X50;
                    case '5': return 0X51;
                    case '6': return 0X52;
                    case '7': return 0X53;

                    default:
                        if (c >= '0' && c <= '9')
                        {
                            return c - '0' + 0x122;
                        }
                        break;
                }
            }

            if (c >= '0' && c <= '9')
            {
                return c - '0'; // +0
            }
            else if (c >= 'a' && c <= 'z')
            {
                return c - 'a' + 0x0A;
            }
            else if (c >= 'A' && c <= 'Z')
            {
                return c - 'A' + 0x0A;
            }
            else
            {
                switch (c)
                {
                    case ' ': return 0xFC;
                    case '.': return 0x24;
                    case ',': return 0x25;
                    case '*': return 0x26;
                    case '_': return 0x27;
                    case '!': return 0x28;
                    case '=': return 0x77;
                    case ':': return 0x78;
                    case '^': return -1; // keep

                    default: return 0x00;
                }
            }
        }

        void textChanged(string newText, int slot)
        {
            if (lockChanges) return;

            int[] tilemap;
            switch (current)
            {
                case 0: tilemap = fileSelect; break;
                case 1: tilemap = eraseMenu; break;
                case 2: tilemap = playerSelect; break;

                default: throw new Exception("Internal Error!");
            }

            var tilePos = tileStart[type][slot + current * 4];

            for (int i = 0, j = newText.Length; i < j; ++i)
            {
                int palette = tilemap[tilePos + i] >> 10 & 7;
                if (comboBox1.SelectedIndex != 0)
                {
                    palette = comboBox1.SelectedIndex - 1;
                }

                int tile = charToSMW(newText[i]);
                if (tile != -1)
                {
                    tilemap[tilePos + i] = tile | (palette << 10);
                }

                texts[slot] = texts[slot].Remove(i, 1);
                texts[slot] = texts[slot].Insert(i, newText[i].ToString());
            }

            switch (current)
            {
                case 0: fileSelect = tilemap; break;
                case 1: eraseMenu = tilemap; break;
                case 2: playerSelect = tilemap; break;
            }

            UpdateTilemapFrom(fileSelect, eraseMenu, playerSelect);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textChanged(textBox1.Text, 0);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            textChanged(textBox2.Text, 1);
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            textChanged(textBox3.Text, 2);
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            textChanged(textBox4.Text, 3);
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = char.ToUpper(e.KeyChar);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == 0)
            {
                comboBox1.SelectedIndex = 4 + 1;
            }
            else
            {
                comboBox1.SelectedIndex = 6 + 1;
            }
        }
    }
}
