using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xpog.Services
{
    public interface IHashingService
    {
        string Hash(string text);
        bool Verify(string plainString, string hash);
    }
    public class HashingService:IHashingService
    {
        private static byte OFFSET = 15; 
        private string Encode(string text)
        {
            var letters = Encoding.ASCII.GetBytes(text);

            byte[] asciiResult = new byte[letters.Length];
            int position = 0;

            foreach (byte letter in letters) {
                byte encodedLetter = letter;

                if(letter > 64 && letter <91) {
                    encodedLetter += OFFSET;
                    if (encodedLetter > 90 ) {
                        encodedLetter -= 25;
                    }
                }

                if (letter > 96 && letter < 123) {
                    encodedLetter += OFFSET;
                    if (encodedLetter > 122) {
                        encodedLetter -= 25;
                    }
                }

                asciiResult[position] = encodedLetter;
                position++;
            }

            return System.Text.Encoding.ASCII.GetString(asciiResult);

        }
        public string Hash(string text) => Encode(text);

        public bool Verify(string plainString, string hash)
        {
            string hashedString = Encode(plainString);
            return hash == hashedString;
        }
    }
}