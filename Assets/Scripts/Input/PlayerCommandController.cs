using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using UnityEngine.AI;
public class PlayerCommandController : MonoBehaviour
{
    [SerializeField] private Camera worldCamera;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float dragThreshold = 5f;

    private List<UnitController> selectedUnits = new List<UnitController>();
    
    private Vector2 selectionStartPosition;
    private bool isSelecting;
    private bool isDragging;
    private void Awake()
    {
        if (worldCamera == null)
            worldCamera = Camera.main;

        if (worldCamera == null)
        {
            Debug.LogError("World Camera를 연결해야 합니다.", this);
            enabled = false;
        }
    }
    void Update()
    {
        if (Mouse.current == null || Keyboard.current == null)
            return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();

        bool shiftHeld = Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            selectionStartPosition = mousePosition;
            isSelecting = true;
            isDragging = false;
        }

        if (isSelecting && !isDragging)
        {
            float distance = Vector2.Distance(selectionStartPosition,mousePosition);

            if (distance >= dragThreshold)
                isDragging = true;
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame && isSelecting)
        {
            if (isDragging) // 드레그 
                SelectUnitsInRectangle(selectionStartPosition,mousePosition,shiftHeld);
            else // 진자 조금 움직임 = 클릭
                SelectSingleUnit(mousePosition, shiftHeld);

            isSelecting = false;
            isDragging = false;
        }


        // 이동 명령
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            MoveSelectedUnits(mousePosition);
        }
    }

    private void SelectUnitsInRectangle(Vector2 start, Vector2 end, bool shiftHeld)
    {
        if (!shiftHeld)
            ClearSelection();

        Rect selectionRect = CreateRectangle(start, end);

        // 나중에 비용등이 문제면 콜라이더 , 프러스텀 등 방법 사용가능 
        UnitController[] units =
            Object.FindObjectsByType<UnitController>(
                FindObjectsSortMode.None
            );

        foreach (UnitController unit in units)
        {
            if (unit.Team != Team.Ally)
                continue;

            Vector3 screenPosition = worldCamera.WorldToScreenPoint(unit.transform.position);

            Vector2 point = new Vector2(screenPosition.x, screenPosition.y);

            if (selectionRect.Contains(point))
                AddSelection(unit);
        }
    }
    private void SelectSingleUnit(Vector2 mousePosition, bool shiftHeld)
    {

        Ray ray = worldCamera.ScreenPointToRay(mousePosition);
        UnitController clickedUnit = null;

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000f))
        {
            clickedUnit = hit.collider.GetComponentInParent<UnitController>();

            if (clickedUnit != null && clickedUnit.Team != Team.Ally)
            {
                clickedUnit = null;
            }
        }

        if (shiftHeld)
        {
            if (clickedUnit == null)
                return;

            if (selectedUnits.Contains(clickedUnit))
            {
                clickedUnit.SetSelected(false);
                selectedUnits.Remove(clickedUnit);
            }
            else
            {
                AddSelection(clickedUnit);
            }
        }
        else
        {
            ClearSelection();

            if (clickedUnit != null)
                AddSelection(clickedUnit);
        }
    }
    private void AddSelection(UnitController unit)
    {
        if (selectedUnits.Contains(unit))
            return;

        selectedUnits.Add(unit);
        unit.SetSelected(true);
    }

    private void ClearSelection()
    {
        foreach (UnitController unit in selectedUnits)
        {
            if (unit != null)
                unit.SetSelected(false);
        }

        selectedUnits.Clear();
    }

    private void MoveSelectedUnits(Vector2 mousePosition)
    {
        if (selectedUnits.Count == 0)
            return;

        Ray ray = worldCamera.ScreenPointToRay(mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit hit, 1000f, groundMask))
            return;

        foreach (UnitController unit in selectedUnits)
        {
            if (unit == null)
                continue;

            NavMeshAgent agent = unit.GetComponent<NavMeshAgent>();

            if (agent == null || !agent.isActiveAndEnabled || !agent.isOnNavMesh)
            {
                continue;
            }

            if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 1f, agent.areaMask))
            {
                unit.MoveTo(navHit.position);
            }
        }


    }

    private Rect CreateRectangle(Vector2 start, Vector2 end)
    {
        return Rect.MinMaxRect(
            Mathf.Min(start.x, end.x),
            Mathf.Min(start.y, end.y),
            Mathf.Max(start.x, end.x),
            Mathf.Max(start.y, end.y)
        );
    }

    private void OnGUI()
    {
        if (!isSelecting || !isDragging || Mouse.current == null)
            return;

        Vector2 currentPosition = Mouse.current.position.ReadValue();

        // 마우스 좌표는 왼쪽 아래가 원점.
        // OnGUI는 왼쪽 위가 원점이라 Y를 변환.
        Vector2 start = new Vector2(selectionStartPosition.x, Screen.height - selectionStartPosition.y);

        Vector2 end = new Vector2(
            currentPosition.x,
            Screen.height - currentPosition.y
        );

        GUI.Box(CreateRectangle(start, end), GUIContent.none);
    }

    private void OnDisable()
    {
        isSelecting = false;
        isDragging = false;
        ClearSelection();
    }
}
