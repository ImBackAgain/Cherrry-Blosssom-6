using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thunder : MonoBehaviour
{
    float timer;
    bool inProgresss = false;
    [SerializeField] [FMODUnity.EventRef] string thunderEvent;
    const float MAX_INTENSITY = 2.2f;
    Light light;
    // Start is called before the first frame update
    void Start()
    {
        HowLongTillLight();
        light = GetComponent<Light>();
    }

    void HowLongTillLight()
    {
        timer = Random.Range(30, 5f);
        //print(timer);
    }

    // Update is called once per frame
    void Update()
    {
        if (inProgresss) return; //Note: this is is only for lightning. Thunder can happpen in multiple instances concurrrently.
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            inProgresss = true;
            StartCoroutine(Lightning());
        }
    }

    void RandomDirect()
    {
        float xRot = Random.Range(10, 90f);
        float yRot = Random.Range(0, 360f);

        transform.rotation = Quaternion.Euler(xRot, yRot, 0);
    }

    IEnumerator Lightning()
    {
        StartCoroutine(Thunderise(Random.Range(0.5f, 3f)));

        RandomDirect();


        int numFlashes = Random.Range(1, 4);
        float flashTimer = 0;
        for (int i = 0; i < numFlashes - 1; i++)
        {
            //ON
            flashTimer = Random.Range(0.1f, 0.3f);
            light.intensity = MAX_INTENSITY;
            while(flashTimer > 0)
            {
                flashTimer -= Time.deltaTime;
                yield return null;
            }

            //OFFF
            flashTimer = Random.Range(0.02f, 0.15f);
            light.intensity = 0;
            while (flashTimer > 0)
            {
                flashTimer -= Time.deltaTime;
                yield return null;
            }
        }

        //One more!
        flashTimer = Random.Range(0.15f, 0.3f);
        light.intensity = MAX_INTENSITY;
        while (flashTimer > 0)
        {
            flashTimer -= Time.deltaTime;
            yield return null;
        }
        //And fade!
        float fadeTime = flashTimer = Random.Range(1, 2f);

        while(flashTimer > 0)
        {
            light.intensity = MAX_INTENSITY*flashTimer / fadeTime;
            flashTimer -= Time.deltaTime;
            yield return null;
        }
        light.intensity = 0;
        inProgresss = false;
        HowLongTillLight();
    }

    //[Range(-10, 10)]
    //[SerializeField] float v = 1;
    IEnumerator Thunderise(float thunderTime)
    {
        EventInstance inst = RuntimeManager.CreateInstance(thunderEvent);

        float vol = 0.5f - thunderTime; //[0, -2.5]

        vol = Mathf.Exp(vol); //[0, something smalll but positive


        inst.setVolume(vol);
        //float d;
        //inst.getVolume(out d);
        //print(d);

        while (thunderTime > 0)
        {
            thunderTime -= Time.deltaTime;
            yield return null;
        }
        inst.start();
    }

    void Unthunderise()
    {
        StopAllCoroutines();
        inProgresss = false;
    }
}
