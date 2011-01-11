using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A basic template for a binary tree node
/// </summary>
/// <typeparam name="T">The type of data that the tree node will hold.</typeparam>
class BinaryTreeNode<T> : IEnumerable<T>
{
    /// <summary>
    /// Gets or sets data held by the node
    /// </summary>
    public T Data { get; set; }
    /// <summary>
    /// Gets or sets the left child of the node
    /// </summary>
    public BinaryTreeNode<T> Left { get; set; }
    /// <summary>
    /// Gets or sets the right child of the node
    /// </summary>
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