using System.Collections.Generic;

public class Function
{
    public int address;
    public string name;
    public List<GraphNode> nodes;

    public static Dictionary<int, Function> AddressToFunctionFromGraphStructure(GraphStructure graphStructure)
    {
        List<GraphNode> nodes = new List<GraphNode>(graphStructure.nodes.Values);
        nodes.Sort((x, y) => x.address - y.address);

        Dictionary<int, Function> addrToFunction = new Dictionary<int, Function>();
        foreach (GraphNode node in nodes)
        {
            List<GraphNode> funcNodes;
            if (!addrToFunction.ContainsKey(node.function_address))
            {
                funcNodes = new List<GraphNode>();
                addrToFunction[node.function_address] = new Function
                {
                    address = node.function_address,
                    name = node.name,
                    nodes = funcNodes
                };
            }
            else funcNodes = addrToFunction[node.function_address].nodes;
            funcNodes.Add(node);
        }
        return addrToFunction;
    }
}