using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class GraphNode
{
    
    public GraphStructure graphStructure;
    public int address;
    public string name;
    public int function_address;
    public List<string> instructions;
    public List<Connection> successors;
    public List<Connection> predecessors;
}

public class GraphStructure
{
    public Dictionary<int, GraphNode> nodes;
    public Dictionary<int, List<Connection>> successors;
    public Dictionary<int, List<Connection>> predecessors;
    public List<Connection> getSuccessors(int address)
    {
        return successors[address];
    }
    public List<Connection> getPredecessors(int address)
    {
        return predecessors[address];
    }

    public GraphStructure(Dictionary<int, GraphNode> nodes, Dictionary<int, List<Connection>> successors, Dictionary<int, List<Connection>> predecessors)
    {
        this.nodes = nodes;
        this.successors = successors;
        this.predecessors = predecessors;
    }

    public static GraphStructure ParsedJSONToGraph(ParsedJsonData parsedJson)
    {
        Dictionary<int, GraphNode> nodes = parsedJson.all_nodes.ToDictionary(x => int.Parse(x.Key), x =>
            {
                Node jsonNode = x.Value;
                GraphNode graphNode = new GraphNode
                {
                    address = jsonNode.addr,
                    name = jsonNode.name,
                    function_address = jsonNode.function_address,
                    instructions = jsonNode.instructions,
                    successors = jsonNode.successors,
                    predecessors = jsonNode.predecessors
                };
                return graphNode;
            }
        );


        Dictionary<int, List<Connection>> successors = new Dictionary<int, List<Connection>>();
        Dictionary<int, List<Connection>> predecessors = new Dictionary<int, List<Connection>>();
        foreach (Node n in parsedJson.all_nodes.Values)
        {
            successors[n.addr] = n.successors;
            predecessors[n.addr] = n.predecessors;
        }

        GraphStructure graphStructure = new GraphStructure(nodes, successors, predecessors);

        return graphStructure;
    }
}

