using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class HumanFactory : GameObjectFactory
{
    [SerializeField]
    public BaseHuman prefabSelfPlayer = default;

    [SerializeField]
    public BaseHuman prefabOtherPlayer = default;

    [SerializeField, Range(10f, 100f)]
    public float health;

    [SerializeField, Range(1f, 100f)]
    public float speed;

    [SerializeField]
    public Vector3 bornPosition;

    public BaseHuman GetSelfPlayer()
    {
        BaseHuman instance = CreateGameObjectInstance(prefabSelfPlayer);
        instance.OriginFactory = this;
        string ipStr = NetManager.Instance.GetSelfIP();
        instance.Initialize(speed, health, bornPosition,ipStr);
        return instance;
    }

    public SyncHuman GetOtherPlayer(Vector3 bornPos,string desc)
    {

        SyncHuman instance = CreateGameObjectInstance(prefabOtherPlayer) as SyncHuman;
        instance.OriginFactory = this;
        instance.Initialize(speed, health, bornPos, desc);
        return instance;
    }
    

    public void Reclaim(BaseHuman human)
    {
        Destroy(human.gameObject);
    }
}
