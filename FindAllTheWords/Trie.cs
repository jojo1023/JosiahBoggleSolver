using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindAllTheWords
{
    public class Trie
    {
        public Node<char> BaseNode { get; }
        public string[] TrieDictonary { get; }
        public char StartLetter { get; }

        public Trie(char startLetter, string[] dictonaryOfWordsThatStartWithStartLetter)
        {
            BaseNode = new Node<char>(startLetter);

            TrieDictonary = dictonaryOfWordsThatStartWithStartLetter;
            StartLetter = startLetter;

            //FillTrie(1, TrieDictonary, BaseNode);
        }
        public void AddWord(string word)
        {
            if (word[0] == StartLetter)
            {
                AddWordRecursive(word, 1, BaseNode);
            }
        }
        void AddWordRecursive(string word, int level, Node<char> currentNode)
        {
            if (word.Length > level)
            {
                Node<char> newNode = currentNode.GetChildByValue(word[level]);
                if (newNode == null)
                {
                    newNode = new Node<char>(word[level], currentNode);
                    currentNode.Children.Add(newNode);
                }
                AddWordRecursive(word, level + 1, newNode);
            }
            else
            {
                currentNode.IsWord = true;
            }
        }
        public string getWord(Node<char> bottomNode)
        {
            return getWordRecusive(bottomNode, bottomNode.Value.ToString());
        }
        string getWordRecusive(Node<char> currentNode, string word)
        {
            if(currentNode.Parent != null)
            {
                return getWordRecusive(currentNode.Parent, currentNode.Parent.Value + word);
            }
            return word;
        }
    }
}
