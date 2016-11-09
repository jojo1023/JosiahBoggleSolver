using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindAllTheWords
{
    public class Node<T> 
    {
        public T Value { get; set; }
        public List<Node<T>> Children { get; set; }
        public Node<T> Parent;
        public bool IsWord { get; set; }
        public bool IsVisited { get; set; }
        public Node(T value)
        {
            IsWord = false;
            Value = value;
            Children = new List<Node<T>>();
            IsVisited = false;
        }
        public Node(T value, Node<T> parent)
        {
            IsWord = false;
            Value = value;
            Children = new List<Node<T>>();
            IsVisited = false;
            Parent = parent;
        }
        public Node(T value, List<Node<T>> children)
        {
            IsWord = false;
            Value = value;
            Children = children;
            IsVisited = false;
        }
        public Node<T> GetChildByValue(T value)
        {
            foreach(Node<T> child in Children)
            {
                if(child.Value.Equals(value))
                {
                    return child;
                }
            }
            return null;
        }
    }
}
