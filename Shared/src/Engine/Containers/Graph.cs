//
// Graph.cs
// Midnight Blue
//
// ---------------------------------------------------
//
// Create by Jacob Milligan on 01/09/2016.
// Copyright (c) Jacob Milligan 2016. All rights reserved.
//


using System.Collections.Generic;

namespace MidnightBlue.Engine.Containers
{
  public class Graph<T> where T : Node
  {
    protected List<T> _nodes;
    protected List<Edge<T>> _edges;

    public Graph()
    {
      _nodes = new List<T>();
      _edges = new List<Edge<T>>();
    }

    public List<T> Nodes
    {
      get { return _nodes; }
    }

    public List<Edge<T>> Edges
    {
      get { return _edges; }
    }
  }
}

