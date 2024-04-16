using System;
using System.Collections.Generic;

[Serializable]
public class Connection
{
    public int target;
    public string type;
}

[Serializable]
public class Node
{
    public int addr;
    public int size;
    public string name;
    public int function_address;
    public List<string> instructions;
    public List<Connection> successors; 
    public List<Connection> predecessors;
}

[Serializable]
public class CallGraphEdge
{
    public int source;
    public int target;
    public string type;
}


[Serializable]
public class GraphObject
{
    public Dictionary<string, Node> all_nodes;
    public List<CallGraphEdge> call_graph_edges;
}