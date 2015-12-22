using UnityEngine;
using System.Collections;

public class RangedWeapon : MonoBehaviour
{
    public byte maxProjAmount;
    public byte magazineCount;
    public float reloadTime;
    public float fireDelay;

    private GUIText ammoUI;
    private GameObject projectile;
    private short currProjAmount;
    private float lastTimeFired = 0;
    private bool recoiling = false;
    private string gui;

    void Start()
    {
        currProjAmount = maxProjAmount;
        ammoUI = GameObject.Find("CurrAmmo").GetComponent<GUIText>();
    }

    public void Fire()
    {
        if(recoiling || (currProjAmount <= 0 && magazineCount <= 0))
            return;

        if(Time.time - lastTimeFired >= fireDelay)
        {
            SpawnProjectile();
            currProjAmount--;
            lastTimeFired = Time.time;
            SoundManager.Instance.PlayGun();
            UpdateGUI();
            if(currProjAmount <= 0 && magazineCount > 0)
            {
                StartCoroutine(IReload());
            }
        }
    }

    void SpawnProjectile()
    {
        Vector3 vv = Input.mousePosition + Camera.main.transform.position - transform.position;
        vv.x -= Camera.main.pixelWidth * 0.5f;
        vv.y -= Camera.main.pixelHeight * 0.5f;

        BulletManager.Instance.SpawnBullet(false,
           transform.position, Player.LookAt(vv));
    }

    IEnumerator IReload()
    {
        UpdateGUI();
        recoiling = true;
        gui = "Ammo: Recoilling..." + "\t\t\tMagazine: " + magazineCount.ToString();
        ammoUI.text = gui;
        yield return new WaitForSeconds(reloadTime);
        currProjAmount = maxProjAmount;
        magazineCount--;
        recoiling = false;
        UpdateGUI();
    }

    public void Reload()
    {
        StartCoroutine(IReload());
    }

    public void AddMagazine()
    {
        magazineCount++;

        if(magazineCount == 1 && currProjAmount == 0)
        {
            StartCoroutine(IReload());
        }
        else
            UpdateGUI();
    }

    public void UpdateGUI()
    {
        gui = "Ammo: " + currProjAmount.ToString() + "\t\t\tMagazine: " + magazineCount.ToString();
        ammoUI.text = gui;
    }
}
