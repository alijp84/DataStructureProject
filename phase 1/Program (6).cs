namespace pattern_matching_tool
{
    using System;
    using System.Collections.Generic;

    public class Node
    {
        public Dictionary<char, Node> ChildNodes = new Dictionary<char, Node>();
        public List<int> Indices = new List<int>();

        public void InsertSuffix(string text, int start)
        {
            Node current = this;
            for (int i = start; i < text.Length; i++)
            {
                if (!current.ChildNodes.ContainsKey(text[i]))
                    current.ChildNodes[text[i]] = new Node();

                current = current.ChildNodes[text[i]];
                current.Indices.Add(start);
            }
        }
    }

    public class SuffixTree
    {
        private Node root = new Node();

        public SuffixTree(string text)
        {
            for (int i = 0; i < text.Length; i++)
                root.InsertSuffix(text, i);
        }

        public List<int> Search(string pattern)
        {
            Node current = root;

            foreach (char ch in pattern)
            {
                if (!current.ChildNodes.ContainsKey(ch))
                    return new List<int>();

                current = current.ChildNodes[ch];
            }

            return current.Indices;
        }
    }

    public class App
    {
        public static void Main()
        {
            string inputText = Console.ReadLine();

            var tree = new SuffixTree(inputText);
            int patternCount = int.Parse(Console.ReadLine());

            var patterns = new List<string>();
            for (int i = 0; i < patternCount; i++)
            {
                patterns.Add(Console.ReadLine());
            }

            foreach (string pattern in patterns)
            {
                var result = tree.Search(pattern);

                if (result.Count > 0)
                    Console.Write(string.Join("", result));
            }
        }
    }

}
