using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindAllTheWords
{
    public class Graph<T>
    {
        public Node<T> BaseNode { get; }
        public Graph(Node<T> baseNode)
        {
            BaseNode = baseNode;
        }
        public Node<T> AddNode(Node<T> parent, T value)
        {
            Node<T> existingNode = getNode(value);
            if (existingNode == null)
            {
                Node<T> newNode = new Node<T>(value);
                newNode.Children.Add(parent);
                parent.Children.Add(newNode);
                return newNode;
            }
            else
            {
                if (existingNode.GetChildByValue(parent.Value) == null)
                {
                    existingNode.Children.Add(parent);
                }
                if (parent.GetChildByValue(existingNode.Value) == null)
                {
                    parent.Children.Add(existingNode);
                }
                return existingNode;
            }
            return null;
        }
        public Node<T> getNode(T node)
        {
            Node<T> foundNode = getNodeRecursive(node, BaseNode);
            unVisit(BaseNode);
            return foundNode;
        }
        Node<T> getNodeRecursive(T node, Node<T> currentNode)
        {
            if (!currentNode.IsVisited && currentNode.Value.Equals(node))
            {
                return currentNode;
            }
            currentNode.IsVisited = true;
            foreach (Node<T> child in currentNode.Children)
            {
                if (!child.IsVisited)
                {
                    child.IsVisited = true;
                    if (child.Value.Equals(node))
                    {
                        return child;
                    }
                    else
                    {
                        Node<T> possibleNode = getNodeRecursive(node, child);
                        if (possibleNode != null)
                        {
                            return possibleNode;
                        }
                    }
                }
            }
            return null;
        }

        void unVisit(Node<T> currentNode)
        {
            foreach (Node<T> child in currentNode.Children)
            {
                if (child.IsVisited)
                {
                    child.IsVisited = false;
                    unVisit(child);
                }
            }
        }
    }
}
