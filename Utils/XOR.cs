using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public static class XOR
    {
        public static byte[] DoXOROperation(byte[] word, byte[] key)
        {
            if (word.Length == key.Length)
            {
                byte[] result = new byte[word.Length];
                for (int i = 0; i < word.Length; i++)
                {
                    result[i] = (byte)(word[i] ^ key[i]);
                }
                return result;
            }
            else
            {
                throw new ArgumentException();
            }
        }
    }
}
