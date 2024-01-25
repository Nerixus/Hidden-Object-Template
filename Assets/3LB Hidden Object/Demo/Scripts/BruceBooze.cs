using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThreeLittleBerkana;

public class BruceBooze : MonoBehaviour
{
    public Animation animationComponent;
    private void OnEnable()
    {
        GameplayManager.OnGameVictory += PlayFaintAnimation;
    }

    private void OnDisable()
    {
        GameplayManager.OnGameVictory -= PlayFaintAnimation;
    }

    void PlayFaintAnimation()
    {
        animationComponent.CrossFade("Faint");
    }
}
