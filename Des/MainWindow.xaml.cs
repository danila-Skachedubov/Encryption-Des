using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Des
{

    public partial class MainWindow : Window
    {
private const int sizeOfBlock = 128; 
private const int sizeOfChar = 16; 
private const int shiftKey = 2; 
private const int quantityOfRounds = 16; 
string[] Blocks;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Go(object sender, RoutedEventArgs e)
        {
            if (Choice1.IsChecked == true)
            {
                if (Key.Text.Length > 0)
                {
                    string s = "";

                    string key = Key.Text;
                    s = Input.Text;
                    s = InputRedact(s);
                    CutStringIntoBlocks(s);
                    key = CorrectKeyWord(key, s.Length / (2 * Blocks.Length));
                    Key.Text = key;
                    key = StringToBinaryFormat(key);

                    for (int j = 0; j < quantityOfRounds; j++)
                    {
                        for (int i = 0; i < Blocks.Length; i++)
                            Blocks[i] = EncodeDES_One_Round(Blocks[i], key);

                        key = KeyToNextRound(key);
                    }

                    key = KeyToPrevRound(key);

                    Key.Text = StringFromBinaryToNormalFormat(key);

                    string result = "";

                    for (int i = 0; i < Blocks.Length; i++)
                        result += Blocks[i];

                    Output.Text = StringFromBinaryToNormalFormat(result);

                }
                else
                    MessageBox.Show("Введите ключ!");
            }

            else if (Choice2.IsChecked == true)
            {

                if (Key.Text.Length > 0)
                {
                    string s = "";

                    string key = StringToBinaryFormat(Key.Text);

                    s = Input.Text;
                    s = StringToBinaryFormat(s);

                    CutBinaryStringIntoBlocks(s);

                    for (int j = 0; j < quantityOfRounds; j++)
                    {
                        for (int i = 0; i < Blocks.Length; i++)
                            Blocks[i] = DecodeDES_One_Round(Blocks[i], key);

                        key = KeyToPrevRound(key);
                    }

                    key = KeyToNextRound(key);

                    Key.Text = StringFromBinaryToNormalFormat(key);

                    string result = "";

                    for (int i = 0; i < Blocks.Length; i++)
                        result += Blocks[i];

                    Output.Text = StringFromBinaryToNormalFormat(result);
                }
                else
                    MessageBox.Show("Введите ключевое слово!");

            }

        }
        public string InputRedact(string input)//добавляем до 64(128) бит на блок
        {
            
            if (input.Length == 0) { 
                MessageBox.Show("Пустое сообщение");
            Input.ToolTip = "Введите значение больше 0";}
            else
                while (((input.Length * sizeOfChar) % sizeOfBlock) != 0)
                    input += "#";
            return input;
        }


        private void CutStringIntoBlocks(string input)//разбиваем на блоки
        {
            Blocks = new string[(input.Length * sizeOfChar) / sizeOfBlock];

            int lengthOfBlock = input.Length / Blocks.Length;

            for (int i = 0; i < Blocks.Length; i++)
            {
                Blocks[i] = input.Substring(i * lengthOfBlock, lengthOfBlock);
                Blocks[i] = StringToBinaryFormat(Blocks[i]);
            }
        }
        private void CutBinaryStringIntoBlocks(string input)//разбивает строку в двоичном формате на блоки
        {
            Blocks = new string[input.Length / sizeOfBlock];

            int lengthOfBlock = input.Length / Blocks.Length;

            for (int i = 0; i < Blocks.Length; i++)
                Blocks[i] = input.Substring(i * lengthOfBlock, lengthOfBlock);
        }

 
        private string StringToBinaryFormat(string input)
        {
            string output = "";

            for (int i = 0; i < input.Length; i++)
            {
                string char_binary = Convert.ToString(input[i], 2);

                while (char_binary.Length < sizeOfChar)
                    char_binary = "0" + char_binary;

                output += char_binary;
            }

            return output;
        }

        private string CorrectKeyWord(string input, int lengthKey)
        {
            if (input.Length > lengthKey)
                input = input.Substring(0, lengthKey);
            else
                while (input.Length < lengthKey)
                    input = "0" + input;

            return input;
        }
        private string EncodeDES_One_Round(string input, string key)
        {
            string L = input.Substring(0, input.Length / 2);
            string R = input.Substring(input.Length / 2, input.Length / 2);

            return (R + XOR(L, f(R, key)));
        }
      
        private string DecodeDES_One_Round(string input, string key)
        {
            string L = input.Substring(0, input.Length / 2);
            string R = input.Substring(input.Length / 2, input.Length / 2);

            return (XOR(f(L, key), R) + L);
        }

        private string XOR(string s1, string s2)
        {
            string result = "";

            for (int i = 0; i < s1.Length; i++)
            {
                bool a = Convert.ToBoolean(Convert.ToInt32(s1[i].ToString()));
                bool b = Convert.ToBoolean(Convert.ToInt32(s2[i].ToString()));

                if (a ^ b)
                    result += "1";
                else
                    result += "0";
            }
            return result;
        }

        private string f(string s1, string s2)
        {
            return XOR(s1, s2);
        }

        private string KeyToNextRound(string key)
        {
            for (int i = 0; i < shiftKey; i++)
            {
                key = key[key.Length - 1] + key;
                key = key.Remove(key.Length - 1);
            }

            return key;
        }


        private string KeyToPrevRound(string key)
        {
            for (int i = 0; i < shiftKey; i++)
            {
                key = key + key[0];
                key = key.Remove(0, 1);
            }

            return key;
        }

        private string StringFromBinaryToNormalFormat(string input)
        {
            string output = "";

            while (input.Length > 0)
            {
                string char_binary = input.Substring(0, sizeOfChar);
                input = input.Remove(0, sizeOfChar);

                int a = 0;
                int degree = char_binary.Length - 1;

                foreach (char c in char_binary)
                    a += Convert.ToInt32(c.ToString()) * (int)Math.Pow(2, degree--);

                output += ((char)a).ToString();
            }

            return output;
        }

    }

}
