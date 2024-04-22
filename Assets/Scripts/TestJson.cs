using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class ParseGraphJson : MonoBehaviour
{
    public ParsedJsonData parsedJsonData;
    public GraphStructure graphStructure;
    void Start()
    {
        TestJSONData();
    }

    private void TestJSONData()
    {
        parsedJsonData = ParsedJsonData.JSONToParsedData("angr_jsons/simple_debug__angr");
        if (parsedJsonData == null)
        {
            Debug.Log("Failed to load JSON Data.");
            return;
        }
        graphStructure = GraphStructure.ParsedJSONToGraph(parsedJsonData);
        if (graphStructure == null)
        {
            Debug.Log("Failed to convert to Graph Structure.");
            return;
        }

        printSuccessors(graphStructure, 4096);
        printSuccessors(graphStructure, 4243);

    }

    private void printSuccessors(GraphStructure graphStructure, int address)
    {
        List<Connection> succ = graphStructure.getSuccessors(address);
        foreach (Connection c in succ)
        {
            Debug.Log($"Successor of {address}: {c.target}");
        }
    }

}
