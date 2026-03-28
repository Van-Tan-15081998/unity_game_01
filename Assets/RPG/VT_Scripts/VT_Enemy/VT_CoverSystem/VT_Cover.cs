using System.Collections.Generic;
using UnityEngine;

public class VT_Cover : MonoBehaviour
{
    private Transform playerTransform;

    [Header("Cover Points")]
    [SerializeField] private GameObject coverPointPrefab;
    [SerializeField] private List<VT_CoverPoint> coverPoints = new List<VT_CoverPoint>();
    [SerializeField] private float xOffset = 1.25f;
    [SerializeField] private float yOffset = .2f;
    [SerializeField] private float zOffset = 1;


    private void Start()
    {
        GenerateCoverPoints();
        playerTransform = FindAnyObjectByType<VT_Player>().transform;
    }

    private void GenerateCoverPoints()
    {
        Vector3[] localCoverPoints =
        {
            new Vector3 (0, yOffset, zOffset), /// Front
            new Vector3 (0, yOffset, -zOffset), /// Back
            new Vector3 (xOffset, yOffset, 0), /// Right
            new Vector3 (-xOffset, yOffset, 0), /// Left
        };

        foreach (Vector3 localCoverPoint in localCoverPoints)
        {
            Vector3 worldPoint = transform.TransformPoint(localCoverPoint);

            VT_CoverPoint coverPoint =
                Instantiate(
                    coverPointPrefab,
                    worldPoint,
                    Quaternion.identity,
                    transform
                ).GetComponent<VT_CoverPoint>();

            coverPoints.Add(coverPoint);
        }
    }

    public List<VT_CoverPoint> GetValidCoverPoints(Transform enemyTransform)
    {
        List<VT_CoverPoint> validCoverPoints = new List<VT_CoverPoint>();

        foreach (VT_CoverPoint coverPoint in coverPoints)
        {
            if (IsValidCoverPoint(coverPoint, enemyTransform))
            {
                validCoverPoints.Add(coverPoint);
            }

        }

        return validCoverPoints;
    }

    private bool IsValidCoverPoint(VT_CoverPoint coverPoint, Transform enemyTransform)
    {
        if (coverPoint.occupied)
        {
            return false;
        }

        if (IsFutherestFromPlayer(coverPoint) == false)
        {
            return false;
        }

        if (IsCoverCloseToPlayer(coverPoint))
        {
            return false;
        }

        if (IsCoverBehindPlayer(coverPoint, enemyTransform))
        {
            return false;
        }

        if (IsCoverCloseToLastCover(coverPoint, enemyTransform))
        {
            return false;
        }

        return true;
    }

    private bool IsFutherestFromPlayer(VT_CoverPoint coverPoint)
    {
        VT_CoverPoint futherestCoverPoint = null;
        float futherestDistance = 0;

        foreach (VT_CoverPoint point in coverPoints)
        {
            float distance = Vector3.Distance(point.transform.position, playerTransform.transform.position);

            if (distance > futherestDistance)
            {
                futherestDistance = distance;
                futherestCoverPoint = point;
            }
        }

        return futherestCoverPoint == coverPoint;
    }

    private bool IsCoverBehindPlayer(VT_CoverPoint coverPoint, Transform enemyTransform)
    {
        float distanceToPlayer = Vector3.Distance(coverPoint.transform.position, playerTransform.position);
        float distanceToEnemy = Vector3.Distance(coverPoint.transform.position, enemyTransform.position);

        return distanceToPlayer < distanceToEnemy;
    }

    private bool IsCoverCloseToPlayer(VT_CoverPoint coverPoint)
    {
        return Vector3.Distance(coverPoint.transform.position, playerTransform.transform.position) < 2;
    }

    private bool IsCoverCloseToLastCover(VT_CoverPoint coverPoint, Transform enemyTransform)
    {
        VT_CoverPoint lastCover = enemyTransform.GetComponent<VT_Enemy_Range>().currentCover;

        return lastCover != null &&
            Vector3.Distance(coverPoint.transform.position, lastCover.transform.position) < 3;
    }
}
