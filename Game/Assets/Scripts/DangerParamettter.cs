using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
public class Bleh
{
    public static PARAMETER_ID GetIdFromEvent(string eventRef, string paramName)
    {
        EventDescription e = RuntimeManager.GetEventDescription(eventRef);

        PARAMETER_DESCRIPTION p;

        e.getParameterDescriptionByName(paramName, out p);

        return p.id;
    }
}
public class DangerParamettter : MonoBehaviour
{
    [SerializeField] StudioEventEmitter tensegrity;
    EventInstance inst;

    HashSet<Transform> nearby = new HashSet<Transform>();
    PARAMETER_ID  dangerId;
    // Start is called before the first frame update
    void Start()
    {
        inst = tensegrity.EventInstance;

        dangerId = Bleh.GetIdFromEvent(tensegrity.Event, "Danger");
    }

    [SerializeField] float danger = 0;
    /* 0 when one enemy is at about 351.
     * 10 when four enemies are lesss than about 66.
     * Delta is 351 - 66 = 285
     */
    // Update is called once per frame
    void Update()
    {
        danger = 0;
        try
        {
            foreach (Transform t in nearby)
            {
                if (!t || !t.gameObject.activeInHierarchy) nearby.Remove(t);
                else
                {
                    Vector3 thing = (t.position - transform.position);
                    thing.y = 0;
                    float ding = thing.sqrMagnitude;
                    //print(ding);
                    //do some math
                    ding -= 66; ding /= 285; ding *= 10 / 4f; ding = 10 / 4f - ding;

                    ding = Mathf.Clamp(0, ding, 10 / 4f);

                    danger += ding;
                }
            }
        }
        catch(System.InvalidOperationException x)
        {
            //I don't have time for this, pal.
        }
        inst.setParameterByID(dangerId, danger);
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            danger = 0;
            print("−");
        }
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            danger = 10;
            print("+");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        nearby.Add(other.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        nearby.Remove(other.transform);
    }
}
