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

    //玩家list
    public List<SyncHuman> GetOtherPlayers(string msg)
    {
        List<SyncHuman> res = new();
        string[] split = msg.Split(',');
        int count = split.Length / 6;
        for(int i = 0; i < count; i++)
        {
            string ip = split[i * 6 + 0];
            float x = float.Parse(split[i * 6 + 1]);
            float y = float.Parse(split[i * 6 + 2]);
            float z = float.Parse(split[i * 6 + 3]);
            float eulY = float.Parse(split[i * 6 + 4]);
            int hp = int.Parse(split[i * 6 + 5]);
            if (ip == NetManager.Instance.GetSelfIP()) continue;

            Vector3 bornPos = new Vector3(x, y, z);
            SyncHuman instance = CreateGameObjectInstance(prefabOtherPlayer) as SyncHuman;
            instance.OriginFactory = this;
            instance.Initialize(speed, health, bornPos, ip);
            res.Add(instance);
        }
        return res;
    }
    

    public void Reclaim(BaseHuman human)
    {
        Destroy(human.gameObject);
    }
}
