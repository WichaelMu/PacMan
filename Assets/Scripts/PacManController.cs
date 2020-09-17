using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PacManController : MonoBehaviour
{
    [Header("Ghosts")]
    public GameObject RedGhost;
    public GameObject PinkGhost;
    public GameObject OrangeGhost;
    public GameObject BlueGhost;

    [Header("")]
    public Transform GhostHolder;
    public Transform PortalL;
    public Transform PortalR;

    Rigidbody PacMan;
    GameObject switcher;
    Animator DeadStateAnim;

    public float moveSpeed;

    float DefaultMoveSpeed;
    bool LR = false, IsStill, IsOutOfPortal = true;
    string nextAvailableTurn, CurrentDirection;

    void Awake()
    {
        PacMan = GetComponent<Rigidbody>();
        DefaultMoveSpeed = moveSpeed;

        DeadStateAnim = GetComponent<Animator>();

        ResetGame();
        PlayerStats.score = 0;

        R();
    }

    void FixedUpdate()
    {
        Movement();
        KeyboardControl();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Switcher"))
        {
            this.switcher = other.gameObject;
            Switcher switcher = other.GetComponent<Switcher>();

            if (switcher.allowDirection(nextAvailableTurn))
                Invoke(nextAvailableTurn, 0f);

            if (!switcher.allowDirection(CurrentDirection))
                HoldPosition(true);
        }

        if (other.CompareTag("Pellet"))
            EatPellet(other.gameObject);
        if (other.CompareTag("BigPellet"))
            EatBigPellet(other.gameObject);
        if (other.CompareTag("Ghost"))
            OnHitGhost(other.gameObject);
        if (other.CompareTag("Portal"))
            PortalHandler(other.name);
        if (other.CompareTag("Fruit"))
            EatFruit(other.gameObject);

    }

    void EatPellet(GameObject Pellet)
    {
        Destroy(Pellet);
        PlayerStats.score += 10;
        PlaySound("EATPELLET");
    }
    void EatBigPellet(GameObject BigPellet)
    {
        Destroy(BigPellet);
        PlayerStats.score += 50;
        PlaySound("EATFRUIT");
        StopSound("AMBIENT");

        foreach (Transform t in GhostHolder)
        {
            GhostController gc = t.gameObject.GetComponent<GhostController>();

            if (gc.IsAlive)
                gc.SetScared(true);
        }
    }

    void EatFruit(GameObject Fruit)
    {
        //Destroy(Fruit);
        PlaySound("EATFRUIT");
        PlayerStats.score += 100;
    }

    void OnHitGhost(GameObject Ghost)
    {
        GhostController ghost = Ghost.GetComponent<GhostController>();

        if (ghost.ScaredState)
            OnHitScaredGhost(Ghost);
        else
            PacManIsDead();
    }

    void OnHitScaredGhost(GameObject Ghost)
    {
        Ghost.GetComponent<GhostController>().OnHitPacMan();
    }

    void PacManIsDead()
    {
        //TODO: Play the death animation for Pac Man.
        DeadStateAnim.SetTrigger("PacManIsDead");
        PlaySound("PACMANDEATH");

        Invoke("ResetGame", 3f);
    }

    void PortalHandler(string Portal)
    {
        if (Portal.Equals("PortalL(Clone)") && IsOutOfPortal)
            transform.position = new Vector3(8.4375f, 4.375f, 0f);
        if (Portal.Equals("PortalR(Clone)") && IsOutOfPortal)
            transform.position = new Vector3(0f, 4.375f, 0f);
        IsOutOfPortal = !IsOutOfPortal;
    }

    Vector3 SpawnPoint = new Vector3(4f, 1.555555555556f, -.05f);

    void ResetGame()
    {
        LR = false;
        IsOutOfPortal = true;

        transform.position = SpawnPoint;
    }

    void PlaySound(string name)
    {
        FindObjectOfType<AudioController>().PlaySound(name);
    }

    void StopSound(string name)
    {
        FindObjectOfType<AudioController>().StopSound(name);
    }

    #region The Movement of Pac Man

    void Movement()
    {
        PacMan.MovePosition(transform.position + (transform.right * moveSpeed * Time.deltaTime));
    }

    void KeyboardControl()
    {
        string keyboardIn = Input.inputString;

        #region WASD Controls
        switch (keyboardIn)
        {
            case "w":
                determineRotation("U");
                return;
            case "W":
                determineRotation("U");
                return;
            case "s":
                determineRotation("D");
                return;
            case "S":
                determineRotation("D");
                return;
            case "a":
                determineRotation("L");
                return;
            case "A":
                determineRotation("L");
                return;
            case "d":
                determineRotation("R");
                return;
            case "D":
                determineRotation("R");
                return;
        }

        #endregion

        #region ARROW KEY Controls
        if (Input.GetKey(KeyCode.UpArrow)) determineRotation("U");
        if (Input.GetKey(KeyCode.DownArrow)) determineRotation("D");
        if (Input.GetKey(KeyCode.LeftArrow)) determineRotation("L");
        if (Input.GetKey(KeyCode.RightArrow)) determineRotation("R");
        #endregion
    }

    void determineRotation(string whereTo)
    {

        if (IsStill)
        {
            Switcher s = switcher.GetComponent<Switcher>(); //  If Pac Man is still, you're probably on a corner.

            if (s.allowDirection(whereTo))
                Invoke(whereTo, 0f);
            return;
        }

        if (LR)
        {
            if (whereTo == "L") { L(); return; }
            if (whereTo == "R") { R(); return; }
        }
        else nextTurn(whereTo);

        if (!LR)
        {
            if (whereTo == "U") { U(); return; }
            if (whereTo == "D") { D(); return; }
        }
        else nextTurn(whereTo);
    }

    #region The Rotation of Pac Man

    void U()    //  Up turn.
    {
        rotateP(new Vector3(000f, 000f, 090f)); //  Rotates Pac Man to face up.

        //Debug.Log("Moving Up");
        PlayerStats.timesTurnedU++;

        NullifyNextAvTurn();
        CurrentDirection = "U";

        HoldPosition(false);
        LR = false;
    }

    void D()    //  Down turn.
    {
        rotateP(new Vector3(000f, 000f, 270f)); //  Rotates Pac Man to face down.

        //Debug.Log("Moving Down");
        PlayerStats.timesTurnedD++;

        NullifyNextAvTurn();
        CurrentDirection = "D";

        HoldPosition(false);
        LR = false;
    }

    void L()    //  Left turn.
    {
        rotateP(new Vector3(000f, 000f, 180f)); //  Rotates Pac Man to face left.

        //Debug.Log("Moving Left");
        PlayerStats.timesTurnedL++;

        //nextTurn("L");
        NullifyNextAvTurn();
        CurrentDirection = "L";

        HoldPosition(false);
        LR = true;
    }

    void R()    //  Right turn.
    {
        rotateP(new Vector3(000f, 000f, 000f)); //  Rotates Pac Man to face right.

        //Debug.Log("Moving Right");
        PlayerStats.timesTurnedR++;

        NullifyNextAvTurn();
        CurrentDirection = "R";

        HoldPosition(false);
        LR = true;
    }

    void rotateP(Vector3 faceAngle)
    {
        transform.localEulerAngles = faceAngle;
    }

    #endregion

    void nextTurn(string whereTo)
    {
        nextAvailableTurn = whereTo;
    }
    void NullifyNextAvTurn()
    {
        nextAvailableTurn = null;
    }

    void HoldPosition(bool instruction)
    {
        if (instruction)
        {
            moveSpeed = 0f;
            IsStill = true;
            return;
        }
        moveSpeed = DefaultMoveSpeed;
        IsStill = false;
    }

    #endregion

}
