namespace SuffixTreeApp
{
    using System;
    using System.Collections.Generic;

    class Node
    {
        public Node Parent { get; set; }
        public Dictionary<char, Node> Children { get; set; } = new Dictionary<char, Node>();
        public int Depth { get; set; }
        public int Start { get; set; }
        public int End { get; set; }

        public Node(Node parent, int depth, int start, int end)
        {
            Parent = parent;
            Depth = depth;
            Start = start;
            End = end;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string input = Console.ReadLine() + '$'; //$ character used to show that this is the end of our string

            var suffixArr = GenerateSuffixArray(input);
            var lcpArr = ComputeLCP(input, suffixArr);
            var treeRoot = BuildTree(input, suffixArr, lcpArr);

            int patternCount = int.Parse(Console.ReadLine());
            var patterns = new List<string>();

            for (int i = 0; i < patternCount; i++)
            {
                patterns.Add(Console.ReadLine());
            }

            foreach (var pattern in patterns)
            {
                var matchIndices = SearchPattern(treeRoot, input, pattern); // search the pattern in the suffix tree
                Console.Write(matchIndices.Count > 0 ? string.Join(" ", matchIndices) + " " : ""); // write the indices where the pattern is found or empty string
            }
        }

        static List<int> GenerateSuffixArray(string text) // generates the suffix array using the "sort and classify" approach
        {
            int n = text.Length;
            var order = SortChars(text); // sort single characters in the string
            var classArray = AssignClasses(text, order); // assign classes for each char based on sorting
            int length = 1; // initial length of substring for sorting is 1

            while (length < n)
            {
                order = SortDoubled(text, length, order, classArray); // sort double-length substrings
                classArray = UpdateClasses(order, classArray, length); // update classes based on new order
                length *= 2; // double the length
            }
            return order;
        }

        static List<int> SortChars(string text) // sorts single characters in the string
        {
            int n = text.Length;
            var order = new List<int>(new int[n]);
            var charCount = new int[256]; // array to count every character and store in its ascii index

            foreach (var c in text)
            {
                charCount[c]++; // count characters in the input text
            }
            for (int i = 1; i < 256; i++)
            {
                charCount[i] += charCount[i - 1]; // cumulative count to determine position of chars
            }
            for (int i = n - 1; i >= 0; i--)
            {
                char c = text[i];
                charCount[c]--;
                order[charCount[c]] = i; // assign position in order array
            }
            return order;
        }

        static List<int> AssignClasses(string text, List<int> order) // assigns classes to characters based on their sorted order
        {
            int n = text.Length;
            var classes = new List<int>(new int[n]);
            classes[order[0]] = 0; // first char gets class 0

            for (int i = 1; i < n; i++)
            {
                if (text[order[i]] != text[order[i - 1]])
                {
                    classes[order[i]] = classes[order[i - 1]] + 1; // new class if different char
                }
                else
                {
                    classes[order[i]] = classes[order[i - 1]]; // same class for same char
                }
            }
            return classes;
        }

        static List<int> SortDoubled(string text, int length, List<int> order, List<int> classes) // sorts doubled substrings (length 2, 4, 8, ...)
        {
            int n = text.Length;
            var newOrder = new List<int>(new int[n]);
            var count = new int[n];

            for (int i = 0; i < n; i++)
            {
                count[classes[i]]++; // count occurrences of each class
            }
            for (int j = 1; j < n; j++)
            {
                count[j] += count[j - 1]; // cumulative count to find positions
            }
            for (int i = n - 1; i >= 0; i--)
            {
                int start = (order[i] - length + n) % n; // starting position of doubled substring
                int cl = classes[start];
                count[cl]--;
                newOrder[count[cl]] = start; // assign position in new order
            }
            return newOrder;
        }

        static List<int> UpdateClasses(List<int> newOrder, List<int> classes, int length) // updates classes after sorting doubled substrings
        {
            int n = newOrder.Count;
            var newClasses = new List<int>(new int[n]);
            newClasses[newOrder[0]] = 0; // first suffix always gets class 0

            for (int i = 1; i < n; i++)
            {
                int cur = newOrder[i], prev = newOrder[i - 1];
                int mid = (cur + length) % n, midPrev = (prev + length) % n;

                // if either pair of substrings differ, assign new class
                if (classes[cur] != classes[prev] || classes[mid] != classes[midPrev])
                {
                    newClasses[cur] = newClasses[prev] + 1;
                }
                else
                {
                    newClasses[cur] = newClasses[prev]; // same class for same substrings
                }
            }
            return newClasses;
        }

        static List<int> ComputeLCP(string text, List<int> suffixArr) // computes the LCP (Longest Common Prefix) array
        {
            int n = text.Length;
            var lcpArr = new List<int>(new int[n - 1]);
            var posInOrder = InvertSuffixArray(suffixArr); // invert suffix array to find positions quickly
            int lcp = 0;

            for (int i = 0; i < n; i++)
            {
                int orderIdx = posInOrder[i];
                if (orderIdx == n - 1)
                {
                    lcp = 0; // no lcp for last suffix
                    continue;
                }
                int nextSuffix = suffixArr[orderIdx + 1]; // compare with next suffix
                lcp = ComputeLCPForSuffixes(text, i, nextSuffix, lcp - 1); // calculate lcp for these suffixes
                lcpArr[orderIdx] = lcp;
                lcp = Math.Max(lcp - 1, 0); // reduce lcp for next iteration
            }
            return lcpArr;
        }

        static int ComputeLCPForSuffixes(string text, int i, int j, int equal) // helper to compute LCP for two suffixes
        {
            int lcp = Math.Max(0, equal); // start with known equal part
            while (i + lcp < text.Length && j + lcp < text.Length)
            {
                if (text[i + lcp] == text[j + lcp]) // keep comparing while chars match
                {
                    lcp++;
                }
                else
                {
                    break;
                }
            }
            return lcp; // return length of longest common prefix
        }

        static List<int> InvertSuffixArray(List<int> order) // inverts the suffix array
        {
            int n = order.Count;
            var pos = new List<int>(new int[n]);
            for (int i = 0; i < n; i++)
            {
                pos[order[i]] = i; // store positions of suffixes
            }
            return pos;
        }

        static Node BuildTree(string text, List<int> order, List<int> lcpArr) // builds the suffix tree using the suffix and LCP arrays
        {
            var root = new Node(null, 0, -1, -1); // create root node for suffix tree
            int lcpPrev = 0;
            var curNode = root;

            for (int i = 0; i < text.Length; i++)
            {
                int suffix = order[i];
                while (curNode.Depth > lcpPrev)
                {
                    curNode = curNode.Parent; // move up if current depth is greater than lcp
                }

                if (curNode.Depth == lcpPrev)
                {
                    curNode = CreateLeaf(curNode, text, suffix); // create leaf if depth matches lcp
                }
                else
                {
                    int edgeStart = order[i - 1] + curNode.Depth; // get start of edge
                    int offset = lcpPrev - curNode.Depth; // calculate offset
                    var midNode = SplitEdge(curNode, text, edgeStart, offset); // split the edge
                    curNode = CreateLeaf(midNode, text, suffix); // create leaf for remaining suffix
                }

                if (i < text.Length - 1)
                {
                    lcpPrev = lcpArr[i]; // update lcpPrev for next iteration
                }
            }

            return root; // return root of suffix tree
        }

        static Node CreateLeaf(Node node, string text, int suffix) // creates a leaf node
        {
            var leaf = new Node(node, text.Length - suffix, suffix + node.Depth, text.Length - 1);
            node.Children[text[leaf.Start]] = leaf; // add leaf to parent's children
            return leaf;
        }

        static Node SplitEdge(Node node, string text, int start, int offset) // splits an edge and creates a mid-node
        {
            char startChar = text[start];
            var child = node.Children[startChar];
            var mid = new Node(node, node.Depth + offset, child.Start, child.Start + offset - 1);
            child.Start += offset;
            child.Parent = mid;
            mid.Children[text[child.Start]] = child; // reassign child to new mid node
            node.Children[startChar] = mid; // update parent's child to mid
            return mid;
        }

        static List<int> SearchPattern(Node root, string text, string pattern)  // searches for a pattern in the suffix tree
        {
            var results = new List<int>();
            if (Search(root, text, pattern, out var matches)) // try to find the pattern in the tree
            {
                foreach (var index in matches)
                {
                    results.Add(text.Length - index); // convert leaf indices to text starting positions
                }
            }
            return results; // return all match positions.
        }

        static bool Search(Node node, string text, string pattern, out List<int> results) // this recursive func navigates the tree to find matches for the given pattern
        {
            results = new List<int>();

            if (string.IsNullOrEmpty(pattern)) // if the pattern is empty, collect all leaves
            {
                CollectLeaves(node, results); // gather all indices from this node's subtree
                return true;
            }


            if (node.Children.TryGetValue(pattern[0], out var child)) // check if child node is the first character of the pattern
            {
                int edgeLength = child.End - child.Start + 1;
                int matchLength = 0; // pattern matches so far

                // try to match characters along the edge
                while (matchLength < edgeLength && matchLength < pattern.Length && text[child.Start + matchLength] == pattern[matchLength])
                {
                    matchLength++; // match characters as long as they align
                }

                if (matchLength == pattern.Length) // entire pattern matches along this edge
                {
                    CollectLeaves(child, results); // collect all matches from this subtree
                    return true;
                }

                if (matchLength == edgeLength) // edge matched, so move to the next node
                {
                    return Search(child, text, pattern.Substring(matchLength), out results);
                }
            }

            return false; // no match found for the current pattern
        }

        static void CollectLeaves(Node node, List<int> indices) // collects indices from all leaves under a node
        {
            if (node.Children.Count == 0)
            {
                indices.Add(node.Depth); // add start index of suffix
            }
            else
            {
                foreach (var child in node.Children.Values)
                {
                    CollectLeaves(child, indices); // recursively collect indices
                }
            }
        }
    }
}
