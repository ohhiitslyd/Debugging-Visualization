using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

[Serializable]
public class Connection
{
    public int target;
    public string type;

    public override string ToString()
    {
        return $"{type} Address: {target}";
    }
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


public class ParsedJsonData
{
    public Dictionary<string, Node> all_nodes;
    public List<CallGraphEdge> call_graph_edges;

    public static ParsedJsonData JSONToParsedData(string path)
    {
        TextAsset file = Resources.Load<TextAsset>(path);
        if (file == null)
        {
            return null;
        }
        string modifiedJson = PreprocessJson(file.text);
        ParsedJsonData parsedJsonData = JsonConvert.DeserializeObject<ParsedJsonData>(modifiedJson);
        if (parsedJsonData == null)
        {
            return null;
        }
        return parsedJsonData;
    }


    private static string PreprocessJson(string originalJson)
    {
        JObject json = JObject.Parse(originalJson);

        // convert successors and predecessors
        JObject nodes = (JObject)json["all_nodes"];
        foreach (var node in nodes)
        {
            JArray successors = (JArray)node.Value["successors"];
            JArray predecessors = (JArray)node.Value["predecessors"];
            node.Value["successors"] = ConvertConnections(successors);
            node.Value["predecessors"] = ConvertConnections(predecessors);
            
        }

        // convert call_graph_edges
        JArray edges = (JArray)json["call_graph_edges"];
        JArray modifiedEdges = new JArray();
        foreach (JArray edge in edges)
        {
            JObject newEdge = new JObject
            {
                ["source"] = edge[0],
                ["target"] = edge[1],
                ["type"] = edge[2]
            };
            modifiedEdges.Add(newEdge);
        }
        json["call_graph_edges"] = modifiedEdges;

        return json.ToString();
    }

    private static JArray ConvertConnections(JArray connections)
    {
        JArray newConnections = new JArray();
        foreach (JArray connection in connections)
        {
            JObject newConnection = new JObject
            {
                ["target"] = connection[0],
                ["type"] = connection[1]
            };
            newConnections.Add(newConnection);
        }
        return newConnections;
    }

}