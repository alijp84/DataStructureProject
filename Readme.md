# Data Structures Project - Phase 1 & 2
**Authors:** Kia & Saloos

## Project Overview
This repository contains the implementation of high-performance string matching algorithms developed in two phases. The project progresses from basic rolling hashes and Tries to memory-optimized Compressed Suffix Trees.

---

## Phase 1: Pattern Matching & Tries

### 1.1 Rabin-Karp Algorithm
Used for identifying multiple ID patterns within a text string.
* **Mechanism:** Rolling hash with base 256 and prime 101.
* **Efficiency:** Reduces full string comparisons by matching hash values first.
* **Complexity:** Average $O(n + m)$.



### 1.2 Suffix Trie
Implemented for the treasury password system to allow high-speed queries.
* **Mechanism:** A tree structure storing every suffix of the input text.
* **Search:** $O(m)$ time complexity per pattern.
* **Constraint:** $O(n^2)$ space complexity.

---

## Phase 2: Optimized Suffix Trees

### Transition
Due to increased data constraints ($10^5$ characters) in Phase 2, the standard Trie was replaced with a Suffix Array-based Compressed Suffix Tree to optimize memory.

### Technical Components
1. **Suffix Array:** Constructed in $O(n \log n)$ using the prefix doubling algorithm.
2. **LCP Array:** Constructed in $O(n)$ using Kasai’s algorithm to find the Longest Common Prefix between sorted suffixes.
3. **Compressed Suffix Tree:** Built by combining the Suffix Array and LCP data. This collapses non-branching internal nodes to achieve $O(n)$ space complexity.



---

## Performance Comparison

| Metric | Phase 1 (Suffix Trie) | Phase 2 (Suffix Tree) |
| :--- | :--- | :--- |
| **Preprocessing** | $O(n^2)$ | $O(n \log n)$ |
| **Space Complexity** | $O(n^2)$ | $O(n)$ |
| **Search Speed** | $O(m)$ | $O(m + K)$ |
| **Max Text Length** | $10^3$ characters | $10^5$ characters |

---

## Structure and Usage
The project is organized into two main directories:
* `/Phase1`: Rabin-Karp and Suffix Trie implementations.
* `/Phase2`: Suffix Array, LCP, and Compressed Suffix Tree implementations.

### Running the Code
1. Navigate to the desired phase directory.
2. Compile the C# files:
   ```bash
   dotnet build
   ```
3. Execute the program:
    ```bash
    dotnet run
    ```
