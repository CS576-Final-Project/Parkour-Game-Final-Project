using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelState : FSMState
{
    private FSMRifleman manager;
    private EnemyRiflemanParameter parameter;

    public ModelState(FSMRifleman manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {

    }

    public void OnUpdate()
    {

    }

    public void OnExit()
    {

    }

    public void OnFixedUpdate()
    {

    }
}
