using UnityEngine;
using DG.Tweening;  // IMPORTANT

public class DOTweenTest : MonoBehaviour
{
    void Start()
    {
        transform.DOMoveX(5, 1f); // move object 5 units in 1 second
    }
}
