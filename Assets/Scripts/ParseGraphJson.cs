using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class ParseGraphJson : MonoBehaviour
{
    void Start()
    {
        LoadJsonData();
    }

    private void LoadJsonData()
    {
        TextAsset file = Resources.Load<TextAsset>("jsondata");
        if (file != null)
        {
            string modifiedJson = PreprocessJson(file.text);
            Debug.Log(modifiedJson);
            GraphObject data = JsonConvert.DeserializeObject<GraphObject>(modifiedJson);
            if (data != null)
            {
                Debug.Log("JSON Data Loaded Successfully!");
                //Debug.Log(data);
                //Debug.Log(data.all_nodes);
                //Debug.Log(data.all_nodes["4096"].name);
                //Debug.Log(data.all_nodes["4096"].instructions[0]);
                //Debug.Log(data.all_nodes["4096"].successors[0].target);
                //Debug.Log(data.call_graph_edges);
                //Debug.Log(data.call_graph_edges.Count);
                //Debug.Log(data.call_graph_edges[0].type);
            }
            else
            {
                Debug.Log("Failed to load JSON Data.");
            }
        }
        else
        {
            Debug.LogError("Failed to load JSON file.");
        }
    }

    private string PreprocessJson(string originalJson)
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

    private JArray ConvertConnections(JArray connections)
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