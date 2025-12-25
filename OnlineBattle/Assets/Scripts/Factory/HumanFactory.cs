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

    public BaseHuman GetOtherPlayer(string msg)
    {
        if (NetManager.Instance.IsSelf(msg)) return null;
        string[] split = msg.Split(',');
        string ip = split[0];
        float x = float.Parse(split[1]);
        float y = float.Parse(split[2]);
        float z = float.Parse(split[3]);
        float eulY = float.Parse(split[4]);
       
        Vector3 bornPos = new Vector3(x, y, z);
        BaseHuman instance = CreateGameObjectInstance(prefab);
        instance.OriginFactory = this;
        instance.Initialize(speed, health, bornPos, ip);
        return instance;
    }

    public void Reclaim(BaseHuman human)
    {
        Destroy(human.gameObject);
    }
}
