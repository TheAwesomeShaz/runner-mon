using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public bool isFirePickup;

    public GameObject waterCollectVFX;
    public GameObject fireCollectVFX;

    [Tooltip("Amount the scale of player will increase/decrease after taking this collectible")]
    float collectibleAmount = 0.01f;

    [SerializeField] Material fireMat;
    [SerializeField] Material waterMat;

    MeshRenderer mesh;

    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        fireCollectVFX.SetActive(true);
        waterCollectVFX.SetActive(true);

        if (isFirePickup)
        {
            mesh.material = fireMat;
        }
        else
        {
            mesh.material = waterMat;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            // add if player and collectible are of same type or else subtract?
            PlayerController.instance.HandleCollectible(isFirePickup, collectibleAmount);

            if (isFirePickup)
            {
                var fireFX = Instantiate(fireCollectVFX, transform.position, Quaternion.identity) as GameObject;
                Destroy(fireFX, 5f);
            }
            else
            {
                var waterFX = Instantiate(waterCollectVFX, transform.position, Quaternion.identity) as GameObject;
                Destroy(waterFX, 5f);
            }
            Destroy(gameObject);
        }
    }
}
