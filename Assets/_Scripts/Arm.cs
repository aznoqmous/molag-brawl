using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arm : MonoBehaviour
{
    public static Arm Instance;
    private void Awake()
    {
        Instance = this;
        ResetTargetPosition();
    }

    Vector3 _targetPosition;
    void Update()
    {
        transform.position= Vector3.Lerp(transform.position, _targetPosition, Time.unscaledDeltaTime * 5f);
    }

    public void SetTargetPosition(Vector3 targetPosition)
    {
        _targetPosition = targetPosition;
    }
    public void ResetTargetPosition()
    {
        _targetPosition = new Vector3(Screen.width / 2f, 0f, 0f);
    }
}
