using FindAllTheWords;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FindAllTheWords
{
    public partial class Form1 : Form
    {
        List<Trie> tries;
        Graph<Point> textboxGraph;
        TextBox[,] textboxes;

        string[] dictionary;
        Stopwatch time;
        List<WordPath> words;

        Graphics gfx;
        public Form1()
        {
            InitializeComponent();
            gfx = this.CreateGraphics();
            time = new Stopwatch();

            dictionary = File.ReadAllLines("Dictionary.txt");
            
            addTextBoxes(4, 4, new Point(20, 20), 10);
            time.Restart();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            time.Restart();
            FillTries();
            FillGraph();
            timeText.Text += time.ElapsedTicks.ToString();
            time.Restart();
            FillTries();
            FillGraph();
            timeText.Text += ", " + time.ElapsedTicks.ToString();
            time.Stop();

            foreach (Trie trie in tries)
            {
                TreeNode rootNode = treeView1.Nodes.Add(trie.StartLetter.ToString() + " " + trie.BaseNode.IsWord.ToString());
                fillTreeView(rootNode, trie.BaseNode);
            }
        }

        public void FillTries()
        {
            tries = new List<Trie>();

            foreach (string word in dictionary)
            {
                if (word.Length > 0)
                {
                    Trie trie = TrieByStartLetter(word[0]);
                    if (trie == null)
                    {
                        trie = new Trie(word[0], WordsThatStartWithLetter(word[0], dictionary));
                        tries.Add(trie);
                    }
                    trie.AddWord(word);
                }
            }
        }

        public void FillGraph()
        {
            if (textboxes.GetLength(0) > 0 && textboxes.GetLength(1) > 0)
            {
                textboxGraph = new Graph<Point>(new Node<Point>(new Point(0, 0)));
                //Node<Point> currentTextBox = textboxGraph.BaseNode;
                //FillGraphRecursive(currentTextBox, new Point(0, 0));

                for (int x = 0; x < textboxes.GetLength(0); x++)
                {
                    for (int y = 0; y < textboxes.GetLength(1); y++)
                    {
                        Node<Point> currentNode = textboxGraph.getNode(new Point(x, y));
                        if (currentNode == null)
                        {
                            currentNode = new Node<Point>(new Point(0, 0));
                        }
                        FillGraphNode(currentNode);
                    }
                }
            }
        }
        public void FillGraphNode(Node<Point> node)
        {
            if (textboxes.GetLength(0) > node.Value.X + 1)
            {
                textboxGraph.AddNode(node, new Point(node.Value.X + 1, node.Value.Y));
            }
            if (textboxes.GetLength(1) > node.Value.Y + 1)
            {
                textboxGraph.AddNode(node, new Point(node.Value.X, node.Value.Y + 1));
            }
            if (textboxes.GetLength(0) > node.Value.X + 1 && textboxes.GetLength(1) > node.Value.Y + 1)
            {
                textboxGraph.AddNode(node, new Point(node.Value.X + 1, node.Value.Y + 1));
            }
            if (node.Value.X > 0 && textboxes.GetLength(1) > node.Value.Y + 1)
            {
                textboxGraph.AddNode(node, new Point(node.Value.X - 1, node.Value.Y + 1));
            }
        }

        private void goButton_Click(object sender, EventArgs e)
        {
            time.Restart();
            words = FindWordsAlgorithm();
            ShowWords(words);
            timeText.Text += ", " + time.ElapsedTicks.ToString();
            time.Restart();
            words = FindWordsAlgorithm();
            ShowWords(words);
            timeText.Text += ", " + time.ElapsedTicks.ToString();
            time.Stop();
        }

        public List<WordPath> FindWordsAlgorithm()
        {
            Node<Point> currentNode;
            Trie trie;
            List<WordPath> words = new List<WordPath>();
            for (int x = 0; x < textboxes.GetLength(0); x++)
            {
                for (int y = 0; y < textboxes.GetLength(1); y++)
                {
                    currentNode = textboxGraph.getNode(new Point(x, y));

                    if (currentNode != null && textboxes[currentNode.Value.X, currentNode.Value.Y].Text.Length > 0)
                    {

                        trie = TrieByStartLetter(textboxes[currentNode.Value.X, currentNode.Value.Y].Text[0]);
                        if (trie != null)
                        {
                            currentNode.IsVisited = true;
                            int NumberOfWordsBefore = words.Count;
                            words.AddRange(getWords(currentNode, trie, trie.BaseNode, new List<WordPath>()));
                            currentNode.IsVisited = false;
                            for (int i = NumberOfWordsBefore; i < words.Count; i++)
                            {
                                words[i].ParentNode = new Node<Point>(currentNode.Value, new List<Node<Point>>() { words[i].ParentNode });
                            }
                        }
                    }
                }
            }
            return words;
        }

        public List<WordPath> getWords(Node<Point> node, Trie trie, Node<char> trieNode, List<WordPath> currentWords)
        {
            List<WordPath> completeWords = currentWords;

            foreach (Node<Point> child in node.Children)
            {
                if (textboxes[child.Value.X, child.Value.Y].Text.Length > 0 && !child.IsVisited)
                {
                    Node<char> curentTrieNode = trieNode.GetChildByValue(textboxes[child.Value.X, child.Value.Y].Text[0]);
                    if (curentTrieNode != null)
                    {
                        if (curentTrieNode.IsWord)
                        {
                            completeWords.Add(new WordPath(trie.getWord(curentTrieNode), new Node<Point>(child.Value)));
                        }
                        child.IsVisited = true;
                        int NumberOfWordsBefore = completeWords.Count;
                        completeWords = getWords(child, trie, curentTrieNode, completeWords);
                        child.IsVisited = false;
                        for (int i = NumberOfWordsBefore; i < completeWords.Count; i++)
                        {
                            completeWords[i].ParentNode = new Node<Point>(child.Value, new List<Node<Point>>() { completeWords[i].ParentNode });
                        }
                    }
                }
            }
            return completeWords;
        }

        public void fillTreeView(TreeNode currentNode, Node<char> TrieNode)
        {
            foreach (Node<char> node in TrieNode.Children)
            {
                TreeNode newCurrentNode = currentNode.Nodes.Add(node.Value.ToString() + " " + node.IsWord.ToString());
                fillTreeView(newCurrentNode, node);
            }
        }

        List<WordAndAmount> wordAndAmounts;

        public void ShowWords(List<WordPath> words)
        {
            wordAndAmounts = new List<WordAndAmount>();
            bool wordExists;
            foreach (WordPath word in words)
            {
                wordExists = false;
                foreach (WordAndAmount wordAndAmount in wordAndAmounts)
                {
                    if (wordAndAmount.Words[0].Word == word.Word)
                    {
                        wordExists = true;
                        wordAndAmount.Words.Add(word);
                    }
                }
                if (!wordExists)
                {
                    wordAndAmounts.Add(new WordAndAmount(word));
                }
            }
            listBox1.Items.Clear();
            foreach (WordAndAmount wordAndAmount in wordAndAmounts)
            {
                listBox1.Items.Add(wordAndAmount.Words[0].Word + "(" + wordAndAmount.Words.Count.ToString() + ")");
            }
        }

        public void addTextBoxes(int amountOfCollums, int amountOfRows, Point size, int spaceing)
        {
            if (amountOfCollums > 0 && amountOfRows > 0)
            {
                textboxes = new TextBox[amountOfCollums, amountOfRows];
                TextBox newTextBox;
                Point position = new Point(spaceing, spaceing);
                for (int x = 0; x < amountOfCollums; x++)
                {
                    for (int y = 0; y < amountOfRows; y++)
                    {
                        newTextBox = new TextBox();
                        newTextBox.MaxLength = 1;
                        newTextBox.Location = position;
                        newTextBox.Size = new Size(size);

                        this.Controls.Add(newTextBox);
                        textboxes[x, y] = newTextBox;
                        //newTextBox.Text = x.ToString() + ", " + y.ToString();

                        position.Y += size.Y + spaceing;
                    }
                    position.X += size.X + spaceing;
                    position.Y = spaceing;
                }
            }
        }

        Trie TrieByStartLetter(char letter)
        {
            foreach (Trie trie in tries)
            {
                if (trie.StartLetter == letter)
                {
                    return trie;
                }
            }
            return null;
        }

        string[] WordsThatStartWithLetter(char letter, string[] dictonary)
        {
            List<string> returnList = new List<string>();
            foreach (string word in dictionary)
            {
                if (word.Length > 0 && word[0] == letter)
                {
                    returnList.Add(word);
                }
            }
            return returnList.ToArray();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int x = 0; x < textboxes.GetLength(0); x++)
            {
                for (int y = 0; y < textboxes.GetLength(1); y++)
                {
                    textboxes[x, y].BackColor = Color.White;
                }
            }
            listBox2.Items.Clear();
            listBox2Words = new List<WordPath>();
            gfx.Clear(Color.White);
            if (listBox1.SelectedIndex >= 0)
            {
                if (wordAndAmounts[listBox1.SelectedIndex].Words.Count == 1)
                {
                    ShowTextBoxPath(wordAndAmounts[listBox1.SelectedIndex].Words[0].ParentNode, Color.White, Pens.Black);
                }
                else
                {
                    foreach (WordPath wordPath in wordAndAmounts[listBox1.SelectedIndex].Words)
                    {
                        listBox2.Items.Add(wordPath.Word);
                        listBox2Words.Add(wordPath);
                    }
                }
            }
        }

        List<WordPath> listBox2Words = new List<WordPath>();

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int x = 0; x < textboxes.GetLength(0); x++)
            {
                for (int y = 0; y < textboxes.GetLength(1); y++)
                {
                    textboxes[x, y].BackColor = Color.White;
                }
            }
            gfx.Clear(Color.White);
            ShowTextBoxPath(listBox2Words[listBox2.SelectedIndex].ParentNode, Color.White, Pens.Black);
        }

        public void ShowTextBoxPath(Node<Point> textboxNode, Color color, Pen pen)
        {
            textboxes[textboxNode.Value.X, textboxNode.Value.Y].BackColor = color;

            if (textboxNode.Children.Count > 0)
            {
                TextBoxSides connectingSide = OnWhatSide(textboxNode.Value, textboxNode.Children[0].Value);
                TextBoxSides oppositeSide = OppositeSide(connectingSide);
                PointF thisSide = GetTextBoxSide(textboxNode.Value, connectingSide);
                PointF otherSide = GetTextBoxSide(textboxNode.Children[0].Value, oppositeSide);

                gfx.DrawLine(pen, thisSide, otherSide);
                //gfx.DrawLine(pen, otherSide, new PointF());
                ShowTextBoxPath(textboxNode.Children[0], color, pen);
            }
        }

        public PointF GetTextBoxSide(Point textboxIndex, TextBoxSides side)
        {
            PointF returnPoint = new PointF();

            if (side == TextBoxSides.TopLeft || side == TextBoxSides.TopMiddle || side == TextBoxSides.TopRight)
            {
                returnPoint.Y = textboxes[textboxIndex.X, textboxIndex.Y].Top;
            }
            else if (side == TextBoxSides.RightMiddle || side == TextBoxSides.LeftMiddle)
            {
                returnPoint.Y = textboxes[textboxIndex.X, textboxIndex.Y].Location.Y + textboxes[textboxIndex.X, textboxIndex.Y].Size.Height / 2;
            }
            else
            {
                returnPoint.Y = textboxes[textboxIndex.X, textboxIndex.Y].Bottom;
            }

            if (side == TextBoxSides.BottomRight || side == TextBoxSides.RightMiddle || side == TextBoxSides.TopRight)
            {
                returnPoint.X = textboxes[textboxIndex.X, textboxIndex.Y].Right;
            }
            else if (side == TextBoxSides.TopMiddle || side == TextBoxSides.BottomMiddle)
            {
                returnPoint.X = textboxes[textboxIndex.X, textboxIndex.Y].Location.X + textboxes[textboxIndex.X, textboxIndex.Y].Size.Width / 2;
            }
            else
            {
                returnPoint.X = textboxes[textboxIndex.X, textboxIndex.Y].Left;
            }

            return returnPoint;
        }

        public TextBoxSides OnWhatSide(Point textboxIndex, Point textboxOnSideIndex)
        {
            Point difference = new Point(textboxOnSideIndex.X - textboxIndex.X, textboxOnSideIndex.Y - textboxIndex.Y);
            if (difference == new Point(1, -1))
            {
                return TextBoxSides.TopRight;
            }
            if (difference == new Point(1, 0))
            {
                return TextBoxSides.RightMiddle;
            }
            if (difference == new Point(0, -1))
            {
                return TextBoxSides.TopMiddle;
            }
            if (difference == new Point(-1, -1))
            {
                return TextBoxSides.TopLeft;
            }
            if (difference == new Point(-1, 0))
            {
                return TextBoxSides.LeftMiddle;
            }
            if (difference == new Point(1, 1))
            {
                return TextBoxSides.BottomRight;
            }
            if (difference == new Point(-1, 1))
            {
                return TextBoxSides.BottomLeft;
            }
            return TextBoxSides.BottomMiddle;
        }

        public TextBoxSides OppositeSide(TextBoxSides side)
        {
            switch (side)
            {
                case (TextBoxSides.TopLeft):
                    return TextBoxSides.BottomRight;
                case (TextBoxSides.TopMiddle):
                    return TextBoxSides.BottomMiddle;
                case (TextBoxSides.TopRight):
                    return TextBoxSides.BottomLeft;
                case (TextBoxSides.RightMiddle):
                    return TextBoxSides.LeftMiddle;
                case (TextBoxSides.BottomRight):
                    return TextBoxSides.TopLeft;
                case (TextBoxSides.BottomMiddle):
                    return TextBoxSides.TopMiddle;
                case (TextBoxSides.BottomLeft):
                    return TextBoxSides.TopRight;
                default:
                    return TextBoxSides.RightMiddle;
            }
        }
        
    }
}


public class WordAndAmount
{
    public List<WordPath> Words { get; set; }
    public WordAndAmount(WordPath word)
    {
        Words = new List<WordPath>();
        Words.Add(word);
    }
}
public class WordPath
{
    public string Word { get; set; }
    public Node<Point> ParentNode { get; set; }
    public WordPath(string word, Node<Point> parentNode)
    {
        Word = word;
        ParentNode = parentNode;
    }
}

public enum TextBoxSides
{
    TopLeft,
    TopMiddle,
    TopRight,
    BottomLeft,
    BottomMiddle,
    BottomRight,
    LeftMiddle,
    RightMiddle
}