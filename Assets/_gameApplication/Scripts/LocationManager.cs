using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random= UnityEngine.Random;
using Utils;

[Serializable]
public struct LocationData {
    public string locationID;
    public BoxCollider boundsBoxCol;
}

public class LocationManager : Singleton<LocationManager>
{
    [SerializeField] private LocationData[] locationDatas;
    private Dictionary<string, LocationData> dictOfLocationData;

    public override void Awake()
    {
        base.Awake();
        CreateDictionary();
    }

    public bool RandomPointAtLocation(string locationID, out Vector3 point) { 
        point = Vector3.zero;
        if (!dictOfLocationData.ContainsKey(locationID)) {
            return false;
        }

        Bounds bounds = dictOfLocationData[locationID].boundsBoxCol.bounds;
        Vector3 center = RandomPointInBounds(bounds);
        return RandomPoint(center, 1.0f, out point);
    }

    public static Vector3 RandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    public void CreateDictionary() {
        dictOfLocationData= new Dictionary<string, LocationData>();
        for (int i = 0; i < locationDatas.Length; i++) {
            dictOfLocationData.Add(
                locationDatas[i].locationID,
                locationDatas[i]
                );
        }
    }

    public static bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }
}
