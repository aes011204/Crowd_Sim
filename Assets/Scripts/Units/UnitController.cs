using UnityEngine;

[RequireComponent(typeof(NPCMovement))]
public class UnitController : MonoBehaviour
{
    [SerializeField] private Team team;

    [SerializeField] private GameObject selectionMarker;

    private UnitMovement movement;

    public Team Team => team;

    private void Awake()
    {
        movement = GetComponent<UnitMovement>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveTo(Vector3 destination)
    {
        movement.MoveTo(destination);
    }

    public void SetSelected(bool selected)
    {
        if (selectionMarker != null)
        {
            selectionMarker.SetActive(selected);
        }
    }
}
