using UnityEngine;

public class DayNightCycle : MonoBehaviour
{

    public int rotationScale = 10;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotationScale * Time.deltaTime, 0, 0);
    }
}
