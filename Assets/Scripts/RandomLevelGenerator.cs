using UnityEngine;
using UnityEditor;
namespace Hanzo.Utils
{
    // [ExecuteInEditMode]  // Allows the script to run in Edit Mode
    public class RandomLevelGenerator : MonoBehaviour
    {
        public GameObject[] prefabs;  // Array of prefabs to place (buildings, trees, cars, etc.)
        public int numberOfObjects = 20;  // Number of objects to place
        public LayerMask groundLayer;  // Layer mask for the ground (plane)
        public GameObject groundPlane;
        public float minDistanceBetweenObjects = 5f;  // Minimum distance between objects

        // Bounds for object placement
        private Vector3 planeBoundsMin;
        private Vector3 planeBoundsMax;

        private void Start()
        {
            // GenerateLevelInEditor();
        }
        // Function to trigger level generation in Edit Mode
        public void GenerateLevelInEditor()
        {
            // Find the plane in the scene and calculate its bounds
          
            if (groundPlane != null)
            {
                Renderer planeRenderer = groundPlane.GetComponent<Renderer>();
                if (planeRenderer != null)
                {
                    planeBoundsMin = planeRenderer.bounds.min;
                    planeBoundsMax = planeRenderer.bounds.max;

                    GenerateLevel();
                }
                else
                {
                    Debug.LogError("No Renderer found on ground plane.");
                }
            }
            else
            {
                Debug.LogError("No object with 'Ground' tag found.");
            }
        }

        private void GenerateLevel()
        {
            // Clear previous objects in edit mode (if needed)
            ClearGeneratedObjects();

            for (int i = 0; i < numberOfObjects; i++)
            {
                // Choose a random position on the plane
                Vector3 randomPosition = GetRandomPositionOnPlane();

                // Check for a hit on the ground layer using Raycast
                Debug.DrawRay(randomPosition + Vector3.up * 10f, Vector3.down * 10f, Color.red, 2f);

                Ray ray = new Ray(randomPosition + Vector3.up * 10f, Vector3.down);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
                {
                    Debug.Log("Hit detected at: " + hit.point);
                    if (hit.collider != null)
                    {
                        // // Ensure objects are not placed too close to each other
                        if (!IsPositionTooClose(hit.point))
                        {
                            // Randomly select a prefab to instantiate
                            GameObject prefabToPlace = prefabs[Random.Range(0, prefabs.Length)];
                            Debug.Log("Instantiating prefab: " + prefabToPlace.name + " at position: " + hit.point); // Log the prefab name and position

                            // Place the prefab at the hit position
                            GameObject obj = Instantiate(prefabToPlace, hit.point, Quaternion.identity);
                            Debug.Log("Prefab instantiated: " + obj.name); // Log after instantiation
                            obj.transform.SetParent(this.transform);
                        }
                    }

                }

            }
        }

        private Vector3 GetRandomPositionOnPlane()
        {
            float x = Random.Range(planeBoundsMin.x, planeBoundsMax.x);
            float z = Random.Range(planeBoundsMin.z, planeBoundsMax.z);
            return new Vector3(x, 0f, z);
        }

        private bool IsPositionTooClose(Vector3 newPosition)
        {
            foreach (Transform child in transform)
            {
                Renderer childRenderer = child.GetComponent<Renderer>();
                if (childRenderer != null)
                {
                    Bounds childBounds = childRenderer.bounds;
                    float distance = Vector3.Distance(newPosition, child.position);

                    // Use bounds extents to calculate if the new object would overlap
                    if (distance < (childBounds.extents.magnitude + minDistanceBetweenObjects))
                    {
                        Debug.Log("Too close to object: " + child.name + ", distance: " + distance);
                        return true;
                    }
                }
            }

            return false;
        }




        // Optionally clear objects generated in the editor
        public void ClearGeneratedObjects()
        {
            int childrenCleared = 0;
            while (transform.childCount > 0)
            {
                Transform child = transform.GetChild(0);
                DestroyImmediate(child.gameObject);
                childrenCleared++;
            }

            Debug.Log("Cleared " + childrenCleared + " objects.");
        }

    }



    [CustomEditor(typeof(RandomLevelGenerator))]
    public class RandomLevelGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Draw the default inspector with all the variables
            DrawDefaultInspector();

            RandomLevelGenerator generator = (RandomLevelGenerator)target;

            // Add a button to the inspector for level generation
            if (GUILayout.Button("Generate Level"))
            {
                generator.GenerateLevelInEditor();
            }

            // Add a button to clear the generated objects
            if (GUILayout.Button("Clear Generated Objects"))
            {
                generator.ClearGeneratedObjects();
            }
        }
    }
}
