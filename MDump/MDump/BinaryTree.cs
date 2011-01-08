using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A basic template for a binary tree node
/// </summary>
/// <typeparam name="T">The type of data that the tree node will hold.</typeparam>
class BinaryTreeNode<T> : IEnumerable<T>
{
    public T Data { get; set; }
    public BinaryTreeNode<T> Left { get; set; }
    public BinaryTreeNode<T> Right { get; set; }
    public BinaryTreeNode() : this(default(T), null, null) { }
    public BinaryTreeNode(T data) : this(data, null, null) { }
    public BinaryTreeNode(T data, BinaryTreeNode<T> l, BinaryTreeNode<T> r)
    {
        Data = data;
        Left = l;
        Right = r;
    }

    #region IEnumerator
    /// <summary>
    /// Iterates through the current node and all its children
    /// </summary>
    public IEnumerator<T> GetEnumerator()
    {
        if (Left != null)
        {
            foreach (T item in Left)
            {
                yield return item;
            }
        }

        yield return Data;

        if (Right != null)
        {
            foreach (T item in Right)
            {
                yield return item;
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
    #endregion
}