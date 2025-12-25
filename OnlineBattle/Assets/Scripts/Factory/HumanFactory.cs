using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class HumanFactory : GameObjectFactory
{
    [SerializeField]
    public BaseHuman prefab = default;

    [SerializeField, Range(10f, 100f)]
    public float health;

    [SerializeField, Range(1f, 100f)]
    public float speed;

    [SerializeField]
    public Vector3 bornPosition;

    public BaseHuman Get(string desc)
    {
        BaseHuman instance = CreateGameObjectInstance(prefab);
        instance.OriginFactory = this;
        instance.Initialize(speed, health, bornPosition,desc);
        return instance;
    }

    public void Reclaim(BaseHuman human)
    {
        Destroy(human.gameObject);
    }
}
