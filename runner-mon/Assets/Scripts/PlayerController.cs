using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("Floats")]
    [SerializeField] float runSpeed;
    [SerializeField] float leftRightSpeed;
    [SerializeField] float evolveSpeed;
    [SerializeField] float scaleSpeed;
    [SerializeField] float moveMultiplier;
    [SerializeField] float fireTypeEvolveLimit;
    [SerializeField] float fireTypeFinalEvolveLimit;
    [SerializeField] float waterTypeEvolveLimit;
    [SerializeField] float waterTypeFinalEvolveLimit;


    [SerializeField] float stuffCollected = 0f;
    float popLimit = 5000f;
    float initialScale = 0.1f;

    [Header("Booleans")]
    [SerializeField] bool isRunning;
    [SerializeField] bool canAttack;
    public bool hasReachedEnd;
    public bool isEvolving;
    public bool hasEvolved;
    public bool hasEvolvedFinal;
    [SerializeField] bool canFinalEvolve;

    public bool isFireType;
    public bool isWaterType;
    public bool canExplode;


    public FloatingJoystick joystick;

    [Header("Characters")]
    [SerializeField] GameObject pokeBall;
    [SerializeField] GameObject charmander;
    [SerializeField] GameObject charizard;
    [SerializeField] GameObject waterMon;
    [SerializeField] GameObject blastoise;
    [SerializeField] GameObject[] wartortleStuff;

    //UI Stuff
    [Header("UI Stuff")]
    [SerializeField] GameObject plusOneCanvas;
    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI text;

    [Header("VFX")]
    [SerializeField] GameObject fireTypeEvolveExplosion;
    [SerializeField] GameObject waterTypeEvolveExplosion;
    [SerializeField] GameObject smallTailFireVFX;
    [SerializeField] GameObject bigTailFireVFX;
    public GameObject fireBallFX;

    [Header("Materials")]
    [SerializeField] Material fireTypeEvolveEffectMaterial;
    [SerializeField] Material fireTypeEvolvedMaterial;
    [SerializeField] Material[] waterTypeEvolveEffectMaterials;
    [SerializeField] Material[] waterTypeEvolvedMaterials;

    [Header("Positions")]
    public Transform fireBallPos;


    Rigidbody rb;
    Animator anim;
    [SerializeField] Animator ballAnim;
    [SerializeField] SkinnedMeshRenderer fireTypeMeshRenderer;
    [SerializeField] SkinnedMeshRenderer waterTypeMeshRenderer;
    Material originalMaterial;

    float weight = 0;
    Vector3 scale = new Vector3(.1f, .1f, .1f);
    Vector3 targetPos;


    public static PlayerController instance;
    private float _xAxis;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {

        anim = GetComponent<Animator>();

        rb = GetComponent<Rigidbody>();
        originalMaterial = fireTypeMeshRenderer.material;

        pokeBall.SetActive(true);
        // charmander can transform into charmeleon
        charmander.SetActive(false);
        charizard.SetActive(false);
        waterMon.SetActive(false);
        blastoise.SetActive(false);

        slider.gameObject.SetActive(false);
        text.gameObject.SetActive(false);

        // set waterPokemon stuff inactive and blendshape weight to 0
        foreach (GameObject thing in wartortleStuff)
        {
            thing.SetActive(false);
            thing.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(0, 0);
        }

        if (isFireType)
        {
            smallTailFireVFX.SetActive(true);
            bigTailFireVFX.SetActive(false);
        }
    }

    public void IncreaseScale(float limit)
    {
        // print("before increasing " + scale.x);
        if (scale.x < limit)
        {
            anim.ResetTrigger("Pop");
            scale.x += scaleSpeed * Time.deltaTime;
            scale.y += scaleSpeed * Time.deltaTime;
            scale.z += scaleSpeed * Time.deltaTime;

            transform.localScale = scale;
        }
        // print("After increasing " + scale.x);
    }

    public void DecreaseScale(float amount)
    {
        if (scale.x > amount)
        {
            scale.x -= scaleSpeed * Time.deltaTime;
            scale.y -= scaleSpeed * Time.deltaTime;
            scale.z -= scaleSpeed * Time.deltaTime;

            transform.localScale = scale;
        }
    }

    private void Update()
    {
        HandleInput();
        Evolve();
        FinalEvolveIncrease();
        Attack();
    }

    private void Attack()
    {
        if (canAttack && Input.GetMouseButtonDown(0) && hasReachedEnd)
        {
            EnableAttackUI();
            if (isFireType)
            {
                charizard.GetComponent<Animator>().SetTrigger("Attack");
                // instantiate fire fx at some position
            }
            else if (isWaterType)
            {
                // blastoise attack or just stand idle
                // instantiate water cannon fx at some position
            }
        }
    }



    private void EnableAttackUI()
    {
        text.gameObject.SetActive(false);
        slider.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        HandleRunning();
    }

    void HandleRunning()
    {
        //Start running on Tap
        if (Input.GetMouseButtonDown(0) && GameController.instance.isPlaying && !isRunning && !hasReachedEnd)
        {
            anim.SetBool("Running", true);
            ballAnim.SetBool("Roll", true);
            isRunning = true;
            return;
        }

        //Apply Forward force
        if (isRunning)
        {
            PlayerMovement();
        }
    }

    void HandleInput()
    {
        float h = Input.GetAxis("Horizontal");

        _xAxis = h;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                _xAxis = Mathf.Clamp(touch.deltaPosition.x / 30f, -5f, 5f);
            }
            else
            {
                _xAxis = 0f;
            }
        }
    }

    private void PlayerMovement()
    {
        Vector3 movePosition = new Vector3(_xAxis * leftRightSpeed * Time.deltaTime, 0f, 1f * runSpeed * Time.deltaTime);

        if (transform.position.x <= -1.2f && movePosition.x < 0)
            movePosition.x = 0f;
        else if (transform.position.x >= 1.2f && movePosition.x > 0)
            movePosition.x = 0f;

        rb.velocity = movePosition * leftRightSpeed * Time.deltaTime;
    }

    #region Evolve Stuff
    void Evolve()
    {
        //FIRE TYPE EVOLVE
        if (isFireType && !isWaterType)
        {
            if (fireTypeMeshRenderer.GetBlendShapeWeight(0) < 100 && isEvolving)
            {
                weight += (evolveSpeed * Time.deltaTime);
                fireTypeMeshRenderer.SetBlendShapeWeight(0, weight);
                if (scale.x < fireTypeEvolveLimit)
                {
                    IncreaseScale(fireTypeEvolveLimit);
                    fireTypeMeshRenderer.material.Lerp(originalMaterial, fireTypeEvolveEffectMaterial, 500f);
                    // print("Changing material to whiteboi");
                }
                if (scale.x >= fireTypeEvolveLimit)
                {
                    FireExplosion();
                    canExplode = false;
                    smallTailFireVFX.SetActive(false);
                    bigTailFireVFX.SetActive(true);
                    fireTypeMeshRenderer.material.Lerp(fireTypeEvolveEffectMaterial, fireTypeEvolvedMaterial, 500f);
                    // print("Changing material to red boi");
                    hasEvolved = true;
                    Debug.Log("Evolve Set to true");

                    isEvolving = false;
                }
                // Debug.Log("Fire Evolving in process");
            }
        }
        //WATER TYPE EVOLVE
        if (!isFireType && isWaterType && isEvolving)
        {
            // TODO: fix ears and tail later after material stuff is working
            foreach (GameObject thing in wartortleStuff)
            {
                thing.SetActive(true);
                SkinnedMeshRenderer earTailRenderer = thing.GetComponent<SkinnedMeshRenderer>();

                if (earTailRenderer.GetBlendShapeWeight(0) < 100)
                {
                    weight += (evolveSpeed * Time.deltaTime);
                    earTailRenderer.SetBlendShapeWeight(0, weight);
                }
            }

            if (scale.x < waterTypeEvolveLimit)
            {
                Debug.Log("ChangeMaterials to evolve!! scale is less than limit");
                waterTypeMeshRenderer.sharedMaterial = waterTypeEvolveEffectMaterials[0];

                //lerp is just lightening the color so we are not doing that
                // waterTypeMeshRenderer.sharedMaterial.Lerp(waterTypeMeshRenderer.sharedMaterial, waterTypeEvolveEffectMaterials[0], 500f);

                IncreaseScale(waterTypeEvolveLimit);

                // print("Changing material to whiteboi");
            }
            if (scale.x >= waterTypeEvolveLimit)
            {
                // Debug.Log("ChangeMaterials to evolvedd!! scale is less than limit");

                WaterExplosion();
                canExplode = false;
                waterTypeMeshRenderer.sharedMaterial = waterTypeEvolvedMaterials[0];

                hasEvolved = true;
                // Debug.Log("Evolve Set to true");
                isEvolving = false;
            }
            // Debug.Log("Water Evolving in process");

        }

    }

    private void FinalEvolveIncrease()
    {
        // print("Final Evolve increase Called : " + (canFinalEvolve && isEvolving && hasEvolved && !hasEvolvedFinal));
        if (canFinalEvolve && !hasEvolvedFinal)
        {
            if (isFireType)
            {

                if (scale.x < fireTypeFinalEvolveLimit)
                {
                    IncreaseScale(fireTypeFinalEvolveLimit);

                    fireTypeMeshRenderer.material.Lerp(originalMaterial, fireTypeEvolveEffectMaterial, 500f);
                }
                if (scale.x >= fireTypeFinalEvolveLimit)
                {
                    canExplode = false;
                    smallTailFireVFX.SetActive(false);
                    bigTailFireVFX.SetActive(true);

                    EvolveFinal();
                    fireTypeMeshRenderer.material.Lerp(fireTypeEvolveEffectMaterial, fireTypeEvolvedMaterial, 500f);
                }
            }
            if (isWaterType)
            {
                if (scale.x < waterTypeFinalEvolveLimit)
                {
                    IncreaseScale(waterTypeFinalEvolveLimit);

                    //TODO: change to evolve effect material
                    waterTypeMeshRenderer.sharedMaterial = waterTypeEvolveEffectMaterials[0];
                    // fireTypeMeshRenderer.material.Lerp(originalMaterial, fireTypeEvolveEffectMaterial, 500f);
                }
                if (scale.x >= waterTypeFinalEvolveLimit)
                {
                    canExplode = false;

                    //TODO: fix evolve final for water type?
                    EvolveFinal();
                    //TODO: decide wether we need the below line or not
                    // fireTypeMeshRenderer.material.Lerp(fireTypeEvolveEffectMaterial, fireTypeEvolvedMaterial, 500f);
                }
            }
        }
    }

    private void EvolveFinal()
    {
        if (hasEvolved && !hasEvolvedFinal)
        {
            if (isFireType)
            {
                canExplode = true;
                BigFireExplosion(2f);
                canExplode = false;
                charizard.SetActive(true);
                charmander.SetActive(false);
                CamFollow.instance.offset = CamFollow.instance.offset + Vector3.up * 1f;
                hasEvolvedFinal = true;
                canFinalEvolve = false;
            }

            if (isWaterType)
            {
                canExplode = true;
                BigWaterExplosion(2f);
                canExplode = false;
                blastoise.SetActive(true);
                hasEvolvedFinal = true;
                waterMon.SetActive(false);
                CamFollow.instance.offset = CamFollow.instance.offset + Vector3.up * 1f;
                canFinalEvolve = false;
            }
        }
    }
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        // print(other.name);
        if (other.tag == "BlueTag")
        {
            isFireType = false;
            isWaterType = true;

            canExplode = true;
            WaterExplosion();

            pokeBall.SetActive(false);
            charmander.SetActive(false);
            waterMon.SetActive(true);
            UpdateUI();

        }
        else if (other.tag == "RedTag")
        {
            isFireType = true;
            isWaterType = false;

            canExplode = true;
            FireExplosion();

            pokeBall.SetActive(false);
            waterMon.SetActive(false);
            charmander.SetActive(true);
            UpdateUI();
        }
        else if (other.tag == "EndTag")
        {
            isRunning = false;
            canAttack = true;
            hasReachedEnd = true;
            if (isFireType)
            {
                // play charizard normal attack anim?
                // on tap attack will be true then a function in update will become enabled
            }

            if (isWaterType)
            {
                //play blastoise attack anim?
                // on tap attack will be true then a function in update will become enabled
            }
        }
    }

    #region Collectible Stuff

    public void HandleCollectible(bool isFirePickup, float collectibleAmount)
    {
        // they = player and pickup
        // if they are of the same type then show +1
        if ((isFirePickup && isFireType) || (!isFirePickup && !isFireType))
        {
            PlusOneVFX();
            CollectStuff();
        }
        // they are not of the same type then do nothing or prolly decrease?
        else if ((!isFirePickup && isFireType) || (isFirePickup && !isFireType))
        {
            plusOneCanvas.SetActive(false);
            // print("oof");
            UnCollectStuff();
        }
    }

    private void UnCollectStuff()
    {
        if (stuffCollected > 0)
        {
            stuffCollected--;
        }
        UpdateUI();
    }

    public void CollectStuff()
    {
        if (stuffCollected < 10)
        {
            stuffCollected++;
        }
        if (stuffCollected == 10)
        {
            isEvolving = true;
            if (hasEvolved && !hasEvolvedFinal)
            {
                isEvolving = true;
                canFinalEvolve = true;

                // print("conditions enabled so that finalEvolveIncrease() van be called");
                stuffCollected = 0;
                //this will enable the FinalEvolveIncrease() call in update
            }
            else
            {
                if (!hasEvolved)
                {
                    Evolve();
                }
                if (!hasEvolvedFinal)
                {
                    stuffCollected = 0;
                }
                // if(isFireType)
                hasEvolved = true;
                // Debug.Log("Evolve Set to true");

            }
        }
        UpdateUI();
    }
    #endregion

    #region VFX Stuff
    private void FireExplosion(float offset = .5f)
    {
        if (canExplode)
        {
            GameObject fx = Instantiate(fireTypeEvolveExplosion, transform.position + new Vector3(0f, offset, 0f), Quaternion.identity) as GameObject;
            Destroy(fx, 4f);
        }
    }

    private void WaterExplosion(float offset = .5f)
    {
        if (canExplode)
        {
            GameObject fx = Instantiate(waterTypeEvolveExplosion, transform.position + new Vector3(0f, offset, 0f), Quaternion.identity) as GameObject;
            Destroy(fx, 4f);
        }
    }

    private void BigFireExplosion(float offset = .5f)
    {
        if (canExplode)
        {
            GameObject fx = Instantiate(fireTypeEvolveExplosion, transform.position + new Vector3(0f, offset, 0f), Quaternion.identity) as GameObject;
            fx.transform.localScale = new Vector3(2f, 2f, 2f);
            Destroy(fx, 4f);
        }
    }

    private void BigWaterExplosion(float offset = .5f)
    {
        if (canExplode)
        {
            GameObject fx = Instantiate(waterTypeEvolveExplosion, transform.position + new Vector3(0f, offset, 0f), Quaternion.identity) as GameObject;
            fx.transform.localScale = new Vector3(2f, 2f, 2f);
            Destroy(fx, 4f);
        }
    }
    #endregion

    #region UI Stuff

    void UpdateUI()
    {
        slider.gameObject.SetActive(true);
        text.gameObject.SetActive(true);

        slider.value = stuffCollected;
        if (!hasEvolved) // small
        {
            if (isFireType)
            {
                text.text = "FIREMON";
            }
            else if (isWaterType)
            {
                text.text = "WATERMON";
            }
        }
        if (hasEvolved) //medium
        {
            if (isFireType)
            {
                text.text = "FIRESAUR";
            }
            else if (isWaterType)
            {
                text.text = "WATERSAUR";
            }
        }
        if (hasEvolvedFinal) // final large
        {
            if (isFireType)
            {
                text.text = "DRAGON";
            }
            else if (isWaterType)
            {
                text.text = "CANON";
            }
        }
    }

    public void PlusOneVFX()
    {
        plusOneCanvas.SetActive(true);
        var fx = Instantiate(plusOneCanvas, transform.position + Vector3.up * 2f, Quaternion.identity);
        Destroy(fx, 2f);
    }

    #endregion 


}
