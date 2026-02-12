namespace ds_project_phase1_1
{
    using System;
    using System.Collections.Generic;


    class Program
    {
        const int Prime = 101; // A prime number for hashing
        const int Base = 256; // Any positive intiger (best if is bigger than size of ASCII character set)

        static void Main()
        {
            string mainString = Console.ReadLine();

            int substringConut = int.Parse(Console.ReadLine());

            List<string> substrings = new List<string>(); // Make a list and add all the substrings
            for (int i = 0; i < substringConut; i++)
                substrings.Add(Console.ReadLine());


            foreach (var substring in substrings)
            {
                List<int> indices = RabinKarp(mainString, substring); // run the algorithm for each substring

                if (indices.Count > 0) // If a substring is found in the original text print the indices of where it was found
                    Console.WriteLine(string.Join(" ", indices));

                else
                    Console.WriteLine(-1); // If the substring wasnt found print -1
            }
        }


        static List<int> RabinKarp(string text, string pattern)
        {
            List<int> indices = new List<int>();
            int textLength = text.Length;
            int patternLength = pattern.Length;

            if (patternLength > textLength) // If the length of pattern is longer than the length of the text return empty list
                return indices;


            int patternHash = Hash(pattern, patternLength);
            int currentHash = Hash(text, patternLength); // Hash the first n chars of the original string (n = patternLength)


            for (int i = 0; i <= textLength - patternLength; i++)
            // Loop for checking all possible substrings in text to know if they match the wanted substring
            {
                if (patternHash == currentHash && text.Substring(i, patternLength) == pattern)
                    // If the pattern and the substring have the same hash check if their string are equal to make sure
                    indices.Add(i);

                if (i != textLength - patternLength) // If possible go one char forward and hash again
                    currentHash = Rehash(text, currentHash, i, patternLength);
            }

            return indices;
        }


        static int Hash(string str, int length) // Function for hashing
        {
            int hash = 0;
            for (int i = 0; i < length; i++) // Loop that hashes one char at a time and sums them
                hash = (hash * Base + str[i]) % Prime; // Hash fourmula (turns the char to its Ascii code)

            return hash;
        }


        static int Rehash(string text, int hash, int index, int patternLength)
        // Function for rehashing by removing first char and adding a new char
        {
            int power = 1;
            // Power is the value of base to power of pattern length -1
            // It is used to undo the hash of left char by finding the multiplication that was done on it
            for (int i = 0; i < patternLength - 1; i++)
                power = power * Base % Prime;

            hash = (hash - text[index] * power) % Prime; // Remove the hash of the left most charechter
            hash = (hash * Base + text[index + patternLength]) % Prime; // Add the hash of the right charechter

            if (hash < 0) // If mod is lower than zero add mod
                hash += Prime;

            return hash;
        }
    }
}
