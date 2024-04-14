using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    void Update()
    {
        if(Player.Instance != null)
        {

            var position = transform.position;
            position = Vector3.Lerp(position, Player.Instance.transform.position, Time.deltaTime * 5f);
            position.z = transform.position.z;
            transform.position = position;
        }
    }
}
