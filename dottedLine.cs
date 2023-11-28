using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class dottedLine : MonoBehaviour
{
    // Apply these values in the editor
    // private LineRenderer lineRenderer;
    private GameObject[] gameObjects;
    //[SerializeField] private Transform[] waypointTransforms; // Create array of transforms
    private Transform[] waypointTransforms;
    int numWaypoints;
    private LineRenderer dottedLines;
    private Vector3 hapticColliderPosition;
    // Start is called before the first frame update
    void Start()
    {
        // Create lineRenderer object
        dottedLines = GetComponent<LineRenderer>();
        dottedLines.useWorldSpace = true;
    }

    // Update is called once per frame
    void Update()
    {
        // if there are waypoints then execute code, this does not run unless there are waypoints, was getting errors before so I had to implement this
        if (GameObject.FindGameObjectsWithTag("Waypoint").Length >0){
            // Get GameObjects with Waypoint tag
            gameObjects = GameObject.FindGameObjectsWithTag("Waypoint");

            numWaypoints = gameObjects.Length;
            // Debug.Log(numWaypoints);
            // Sort waypoints in ascending order
            var sortedGameObjects = gameObjects.OrderBy(go => go.name, new AlphanumericComparer()).ToArray();
            gameObjects = sortedGameObjects;

            waypointTransforms = gameObjects.Select(go => go.transform).ToArray();

            // get location of the end effector
            GameObject touchActorObject = GameObject.Find("TouchActor");
            Transform hapticColliderTransform = touchActorObject.transform.Find("HapticCollider");
            hapticColliderPosition = hapticColliderTransform.position;
            
            // location of closest waypoint
            int nearestIndex = GetNearestWaypointIndex(hapticColliderPosition);

            // draw line from end effector to the closest waypoint
            dottedLines.positionCount = 2;
            // Debug.Log(hapticColliderPosition);
            // Debug.Log(waypointTransforms[nearestIndex].position);
            dottedLines.SetPosition(0,hapticColliderPosition);
            dottedLines.SetPosition(1,waypointTransforms[nearestIndex].position);
        
        }
    }

    // Custom comparer to sort strings in a numerical order
    public class AlphanumericComparer: IComparer<string>
    {
        public int Compare(string x, string y)
        {
            // Parse strings into integers if they represent numbers
            if(int.TryParse(x, out int a) && int.TryParse(y, out int b))
            {
                return a.CompareTo(b);
            }

            // If not numbers, use default string comparison
            return string.Compare(x, y, System.StringComparison.Ordinal);
        }
    }
    // Function to find the index of the nearest waypoint to a given position
    private int GetNearestWaypointIndex(Vector3 position)
    {
        int nearestIndex = 0;
        float minDistance = Mathf.Infinity;

        for (int i = 0; i < waypointTransforms.Length; i++)
        {
            float distance = Vector3.Distance(position, waypointTransforms[i].position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestIndex = i;
            }
        }

        return nearestIndex;
    }
    private void DrawDottedLine(Vector3 startPos, Vector3 endPos)
    {
        // Adjust this value to set the length of each segment of the dotted line
        float segmentLength = 0.1f;

        // Calculate the number of segments needed
        int segments = Mathf.CeilToInt(Vector3.Distance(startPos, endPos) / segmentLength);

        dottedLines.positionCount = segments * 2;

        for (int i = 0; i < segments; i++)
        {
            Vector3 pointAlongLine = Vector3.Lerp(startPos, endPos, (float)i / segments);
            dottedLines.SetPosition(i * 2, pointAlongLine);
            dottedLines.SetPosition(i * 2 + 1, pointAlongLine);
        }
    }
}
