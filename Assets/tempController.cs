using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tempController : MonoBehaviour
{
    public GameObject fromObj;
    public GameObject toObj;
    public GameObject edgePrefab;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 fromCenter = fromObj.transform.position;
        Vector3 toCenter = toObj.transform.position;

        Vector3 fromEdgePoint = FindClosestIntersectionPoint(toCenter, fromCenter);
        Vector3 toEdgePoint = FindClosestIntersectionPoint(fromCenter, toCenter);

        GameObject edgeObj = Instantiate(edgePrefab, Vector3.zero, Quaternion.identity);
        Arrow.ArrowRenderer animatedArrowRenderer = edgeObj.GetComponent<Arrow.ArrowRenderer>();
        animatedArrowRenderer.SetPositions(fromEdgePoint, toEdgePoint);

        Debug.Log("==== add edge obj into edge");
    }

    private Vector3 FindClosestIntersectionPoint(Vector3 center, Vector3 target)
    {
        Vector3 direction = (target - center).normalized;
        float distance = Vector3.Distance(center, target);

        if (Physics.Raycast(center, direction, out RaycastHit hit, distance))
        {
            return hit.point;
        }
        else
        {
            return target;
        }
    }
}
