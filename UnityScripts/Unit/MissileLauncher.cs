using UnityEngine;
using System.Collections;

public class MissileLauncher : MonoBehaviour {

    public GameObject Missile;

    public void Fire(GameObject target)
    {
        GameObject go = Instantiate(Missile, transform.position, Quaternion.identity) as GameObject;
        go.transform.localScale = GameManager.Instance.GlobalScale;
        go.GetComponent<Projectile>().target = target;
        go.GetComponent<Projectile>().shooter = this.gameObject;
    }

}
