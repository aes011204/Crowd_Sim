using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPCMovement))]
public class NPCController : MonoBehaviour
{

    [SerializeField]
    private Facility[] facilities;
    [SerializeField]
    private NPCState currentState;

    private NPCMovement movement;
    private Facility currentFacility;
    private float remainingUseTime;

    private void Awake()
    {
        movement = GetComponent<NPCMovement>();
    }

    void Start()
    {
        if (facilities == null || facilities.Length == 0)
        {
            Debug.LogError("NPC에 이용할 시설을 연결해야 합니다.", this);
            enabled = false;
            return;
        }

        foreach (Facility facility in facilities)
        {
            if (facility == null)
            {
                Debug.LogError("시설 목록에 비어 있는 항목이 있습니다.", this);
                enabled = false;
                return;
            }
        }

        MoveToNextFacility();
    }

    void Update()
    {
        switch (currentState)
        {
            case NPCState.Moving:
                UpdateMoving();
                break;

            case NPCState.UsingFacility:
                UpdateUsingFacility();
                break;
        }
    }
    private void UpdateMoving()
    {
        if (movement.HasArrived() == false) return;

        remainingUseTime = currentFacility.UseTime;
        currentState = NPCState.UsingFacility;

    }

    private void UpdateUsingFacility()
    {
        remainingUseTime -= Time.deltaTime;
        if (remainingUseTime > 0) return;

        MoveToNextFacility();
    }

    private void MoveToNextFacility()
    {
        Facility nextFacility = SelectNextFacility();

        currentFacility = nextFacility;
        currentState = NPCState.Moving;

        movement.MoveTo(currentFacility.UsePosition);
    }

    private Facility SelectNextFacility()
    {
        if (facilities.Length == 1)
            return facilities[0];

        List<Facility> candidates = new List<Facility>();

        foreach (Facility facility in facilities)
        {
            if (facility != currentFacility)
                candidates.Add(facility);
        }

        return candidates[Random.Range(0, candidates.Count)];


    }
}
