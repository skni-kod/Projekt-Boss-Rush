using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScentController : MonoBehaviour
{
    /// <summary>
    /// Number of scent markers being created on scene start.
    /// </summary>
    [Header("Generating Parameters")]
    [SerializeField] private int initialChainLength = 15;
    /// <summary>
    /// Number of currently created scent markers.
    /// </summary>
    [ReadOnly][SerializeField] private int chainLength = 15;

    /// <summary>
    /// Number of currently enabled scent markers.
    /// </summary>
    [ReadOnly][SerializeField] private int activeChainLength = 0;

    /// <summary>
    /// How long will scent markers exist until going back to initial location.
    /// </summary>
    [SerializeField] private float markerLifeTime = 15f;

    /// <summary>
    /// Range for ConnectTwoNearbyMarkers(), how wide area should it seek markers for merge.
    /// </summary>
    [SerializeField] private float mergeRange = 5f;

    /// <summary>
    /// Reference to smell Gameobject prefab.
    /// </summary>
    [SerializeField] private GameObject scentMarkerPrefab;

    /// <summary>
    /// List storing scent markers transforms.
    /// </summary>
    private List<Transform> scentMarkers = new List<Transform>();

    /// <summary>
    /// Parent object that contains scent markers as children.
    /// </summary>
    [SerializeField] private GameObject scentMarkersContainer;

    /// <summary>
    /// Controls if scent markers should be spawned or not. True means it spawns.
    /// </summary>
    [Header("Debugging properties")]
    [SerializeField] bool enableAutoSpawn = true;

    /// <summary>
    /// Controls if lines connecting markers are visible or not. True means it draws.
    /// </summary>
    [SerializeField] bool enableDrawGizmos = true;
    
    private void Start()
    {
        if(scentMarkersContainer == null)
        {
            scentMarkersContainer = new GameObject();
            scentMarkersContainer.name = transform.name + " Scent Marker Container";
            scentMarkersContainer.transform.parent = null;
        }

        for(int i = 0; i < initialChainLength; i++)
        {
            var newMarker = Instantiate(scentMarkerPrefab.transform, scentMarkersContainer.transform);
            newMarker.name = "Scent Marker " + i;
            newMarker.gameObject.SetActive(false);
            scentMarkers.Add(newMarker);
        }
        if(enableAutoSpawn)
            InvokeRepeating("TeleportNextMarker", 0f, 0.5f);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            DetectObjectBetweenMarkers();
        if(Input.GetKeyDown(KeyCode.LeftControl))
            TeleportNextMarker();
    }

    private void FixedUpdate()
    {
        DetectObjectBetweenMarkers();
    }

    private void TeleportNextMarker()
    {
        if (activeChainLength == chainLength)
        {
            scentMarkers.Add(scentMarkers[0]);
            scentMarkers.RemoveAt(0);
            scentMarkers[chainLength - 1].position = transform.position;
        }
        else
        {
            scentMarkers[activeChainLength].position = transform.position;
            scentMarkers[activeChainLength].gameObject.SetActive(true);

            activeChainLength++;
        }

        DetectTwoMarkersNearby();
    }

    private void DetectTwoMarkersNearby()
    {
        LayerMask mask = LayerMask.GetMask("Scent Marker");
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, mergeRange, mask);
        if (hitColliders.Length > 1)
        {
            int[] markersToMergeIndex = new int[2];
            Vector3[] markersToMergePositions = new Vector3[2];
            for(int i = 0; i < 2; i++)
            {
                for(int j = activeChainLength - 1; j >= 0; j--)
                {
                    if (hitColliders[i].gameObject == scentMarkers[j].gameObject)
                    {
                        markersToMergeIndex[i] = j;
                        markersToMergePositions[i] = scentMarkers[j].position;
                        break;
                    }
                }
            }
            MergeBetweenMarkers(markersToMergeIndex[0], markersToMergeIndex[1], false);
        }
    }

    private void MergeBetweenMarkers(int firstIndex, int secondIndex, bool isPlayerPosition)
    {
        Vector3 mergedMarkersPosition;
        if (isPlayerPosition)
            mergedMarkersPosition = transform.position;
        else
            mergedMarkersPosition = ((transform.position + scentMarkers[secondIndex].position) / 2);

        scentMarkers[secondIndex].position = mergedMarkersPosition;

        int numberToCut = Mathf.Abs(firstIndex - secondIndex);
        int minIdex = Mathf.Min(firstIndex, secondIndex);

        for(int i = 0; i < numberToCut; i++)
        {
            var markerToMove = scentMarkers[minIdex];
            markerToMove.gameObject.SetActive(false);

            scentMarkers.RemoveAt(minIdex);
            scentMarkers.Add(markerToMove);
            activeChainLength--;
        }
    }

    private void DetectObjectBetweenMarkers()
    {
        LayerMask mask = LayerMask.GetMask("Default");
        Vector3 direction;
        RaycastHit hit;
        float distance;

        for (int i = 0; i < activeChainLength - 2; i++)
        {
            direction = -(scentMarkers[i].position - scentMarkers[i + 1].position).normalized;
            distance = Vector3.Distance(scentMarkers[i + 1].position, scentMarkers[i].position);
            if (Physics.Raycast(scentMarkers[i].position, direction, out hit, distance, mask))
            {
                MergeBetweenMarkers(i, activeChainLength - 1, true);
            }
        }
        
    }

    private void OnDrawGizmos()
    {
        if (enableDrawGizmos && (activeChainLength == 0 || scentMarkers.Count == 0))
            return;

        Gizmos.color = Color.green;
        Vector3 direction;
        float distance;
        for (int i = 0; i < activeChainLength - 1; i++)
        {
            direction = -(scentMarkers[i].position - scentMarkers[i + 1].position).normalized;
            distance = Vector3.Distance(scentMarkers[i + 1].position, scentMarkers[i].position);
            Gizmos.DrawLine(scentMarkers[i].position, scentMarkers[i].position + direction * distance);
        }
    }

    private void OnValidate()
    {
        if (!Application.isPlaying)
            return;

        if (enableAutoSpawn)
            InvokeRepeating("TeleportNextMarker", 0f, 0.5f);
        else
            CancelInvoke("TeleportNextMarker");
        Debug.Log("dupa");

    }
}
