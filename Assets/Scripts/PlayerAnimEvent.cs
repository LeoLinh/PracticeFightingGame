using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimEvent : MonoBehaviour
{
    private PlayerController player;

    void Start()
    {
        player = GetComponentInParent<PlayerController>();
    }

    private void AnimationTrigger()
    {
        player.AttackOver();
        player.CrouchOver();
        player.BlockOver();
        player.CastOver();
        player.DieOver();
        player.DizzyOver();
        player.HurtOver();
        player.WinOver();
        player.StrikeOver();
    }

}
