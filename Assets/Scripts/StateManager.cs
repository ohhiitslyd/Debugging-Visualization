using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    public static StateManager Instance;
    public Dictionary<int, GameObject> sceneBlockDict;
    public int selectedBlock = -1;

    public Material normal;
    public Material unfocused;


    public enum CameraState { Default, Focus, Analyze }
    public CameraState currentCameraState = CameraState.Default;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateMaterials()
    {
        foreach (int currKey in StateManager.Instance.sceneBlockDict.Keys)
        {
            GameObject currNode = StateManager.Instance.sceneBlockDict[currKey];
            GameObject cube = currNode.transform.GetChild(2).GetChild(0).gameObject;
            if (selectedBlock == -1)
            {
                cube.GetComponent<Renderer>().material = normal;
            }
            else
            {
                if (currKey == selectedBlock)
                { // Change me right now
                    cube.GetComponent<Renderer>().material = normal;
                }
                else
                {
                    cube.GetComponent<Renderer>().material = unfocused;
                }
            }
        }
    }

    public void SetCameraState(CameraState newState)
    {
        currentCameraState = newState;
    }

    public GameObject GetBlockCubeInFocus()
    {
        if (selectedBlock == -1)
        {
            return null;
        }

        foreach (int currKey in StateManager.Instance.sceneBlockDict.Keys)
        {
            if (currKey == selectedBlock)
            {
                GameObject currNode = StateManager.Instance.sceneBlockDict[currKey];
                GameObject cube = currNode.transform.GetChild(2).GetChild(0).gameObject;
                return cube;
            }
        }
        return null;
    }

    public GameObject GetBlockCanvasInFocus()
    {
        if (selectedBlock == -1)
        {
            return null;
        }

        foreach (int currKey in StateManager.Instance.sceneBlockDict.Keys)
        {
            if (currKey == selectedBlock)
            {
                GameObject currNode = StateManager.Instance.sceneBlockDict[currKey];
                GameObject cube = currNode.transform.GetChild(0).GetChild(0).gameObject;
                return cube;
            }
        }
        return null;
    }

}
