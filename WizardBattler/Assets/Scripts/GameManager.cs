using UnityEngine;
using System.Collections;

//Manages turn control and camera panning
public class GameManager : MonoBehaviour
{
    //VARIABLES
    //Phases of gameplay, turns and summons
    public enum GamePhases {outSidePlay, p1Turn, p1Summon, p2Turn, p2Summon};
    public GamePhases gamePhase;

    //For positioning the camera
    private Transform cam;
    //0 - p1, 1 - p1summon, 2 - p2, 3 - p2summon
    public Vector3[] points;
    private Vector3 camPos;

    public PlayerController p1;
    public PlayerController p2;

    private int phaseIndex = 1;

    public void NextPhase()
    {
        phaseIndex++;
        if(phaseIndex > 4)
        {
            phaseIndex = 1;
        }
        gamePhase = (GamePhases)phaseIndex;
        UpdatePhase();
    }

    void Update()
    {
        if(gamePhase == GamePhases.outSidePlay)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                gamePhase = GamePhases.p1Turn;
                UpdatePhase();
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawIcon(points[0], "WizardIcon.png", true);
        Gizmos.DrawIcon(points[2], "WizardIcon.png", true);
        Gizmos.color = Color.red;
        Gizmos.DrawIcon(points[1], "SummonIcon.png", true);
        Gizmos.DrawIcon(points[3], "SummonIcon.png", true);
    }

	void Start ()
    {
        gamePhase = GamePhases.outSidePlay;
        cam = Camera.main.transform;
        camPos = cam.position;
	}

    //Updates the game phase according to its new state, basically a coroutine junction
    void UpdatePhase()
    {
        StopAllCoroutines();

        switch (gamePhase)
        {
            case GamePhases.p1Turn:
                StartCoroutine("P1Turn");
                break;
            case GamePhases.p1Summon:
                StartCoroutine("P1Summon");
                break;
            case GamePhases.p2Turn:
                StartCoroutine("P2Turn");
                break;
            case GamePhases.p2Summon:
                StartCoroutine("P2Summon");
                break;

            default:
            break;
        }
    }

    IEnumerator P1Turn()
    {
        //pan the camera to the player, then tell the player they're active
        Vector3 targPos = new Vector3(points[0].x, camPos.y, camPos.z);
        while(Vector3.Distance(cam.transform.position, targPos) > 1)
        {
            cam.position = Vector3.Lerp(cam.position, targPos, Time.smoothDeltaTime);
            yield return null;
        }
        //tell player one he's active
        p1.StartCoroutine("PlayerTurn");
    }

    IEnumerator P1Summon()
    {
        //pan the camera to the summon spot, tell the player to summon, then continue
        Vector3 targPos = new Vector3(points[1].x, camPos.y, camPos.z);
        while (Vector3.Distance(cam.transform.position, targPos) > 1)
        {
            cam.position = Vector3.Lerp(cam.position, targPos, Time.smoothDeltaTime);
            yield return null;
        }
        //tell player one to summon
        p1.StartCoroutine("Summon");
    }

    IEnumerator P2Turn()
    {
        //pan the camera to the player, then tell the player they're active
        Vector3 targPos = new Vector3(points[2].x, camPos.y, camPos.z);
        while (Vector3.Distance(cam.transform.position, targPos) > 1)
        {
            cam.position = Vector3.Lerp(cam.position, targPos, Time.smoothDeltaTime);
            yield return null;
        }
        //tell player two he's active
        p2.StartCoroutine("PlayerTurn");
    }

    IEnumerator P2Summon()
    {
        //pan the camera to the summon spot, tell the player to summon, then continue
        Vector3 targPos = new Vector3(points[3].x, camPos.y, camPos.z);
        while (Vector3.Distance(cam.transform.position, targPos) > 1)
        {
            cam.position = Vector3.Lerp(cam.position, targPos, Time.smoothDeltaTime);
            yield return null;
        }
        //tell player two to summon
        p2.StartCoroutine("Summon");
    }

    void WinGame(int player)
    {
        //Win state
        gamePhase = GamePhases.outSidePlay;
    }
}
