using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsOs.Utilities
{
    class AlphabetCounter
    {
        // This class is used to find the "distance" between to given letters in the alphabet
        // We use it to calculate the amount of product locations to create and what those letters should be

        // klasse bruges til at finde "afstanden" mellem to givne bogstaver i alfabetet
        // Vi bruger den til at udregne hvor mange produkt lokationer der skal oprettes og hvilke bogstaver der skal bruges


        // The alphabet is put into a Char Array
//        private static readonly char[] Alphabet = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
        private static readonly char[] Alphabet = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

        // LocationLetterIterations returns the "distance" between the two input. This is used so we know how many location we should create
        // If the inputs are reversed e.g. a-e is 5, but e-a would give -3. We handle this so e-a will return 5.
        //public static int LocationLetterIterations(string start, string end)
        //{
        //    int[] StartEndValues = FindStartEndValue(start, end);

        //    int resultVal = StartEndValues[1] - StartEndValues[0] + 1;
        //    if (resultVal <= 0)
        //    {
        //        resultVal = resultVal * -1 + 2;
        //    }
        //    return resultVal;
        //}
        // LettersToCreate returns an array of the characters that needs to be created
        public static char[] LettersToCreate(Char start, Char end)
        {
            int[] StartEndValues = FindStartEndValue(start, end);
            char[] letterArray = new char[StartEndValues[1] - StartEndValues[0] + 1];
            int j = 0;
            for (int i = StartEndValues[0]; i <= StartEndValues[1]; i++)
            {
                letterArray[j] = Alphabet[i];
                j++;
            }
            return letterArray;
        }
        // FindStartEndValue is used in both LocationLetterIterations and LettersToCreate, so we decided to make it its own method
        // To cut down the total amount of lines in the class
        public static int[] FindStartEndValue(Char start, Char end)
        {
            int startVal = 0;
            int endVal = 0;
            for (int i = 0; i < Alphabet.Length; i++)
            {
                if (Alphabet[i] == start)
                    startVal = i;

                if (Alphabet[i] == end)
                    endVal = i;
            }
            return new int[2] { startVal, endVal };
        }
    }
}
