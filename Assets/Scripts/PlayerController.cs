using UnityEngine;

public enum State
{
    Idle,
    Walk,
    Run
}
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    private void Awake()
    {
        instance = this;
    }

    [Header("Var_Movement")]
    private Vector2 InputVector;
    private Vector3 MousePos;
    public bool Mouse;
    [SerializeField] private int Speed;
    private int SaveWalkSpeed;
    [SerializeField] private int RunSpeed;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private State CurrentState;

    [Header("Component")]
    [SerializeField] private Camera PlayerCamera;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator PlayerAnimator;


    // Start is called before the first frame update
    void Start()
    {
        SaveWalkSpeed = Speed;
        CurrentState = State.Idle;
        PlayerAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        var H = Input.GetAxis("Horizontal");
        var V = Input.GetAxis("Vertical");
        InputVector = new Vector2(H, V);
        MousePos = Input.mousePosition;


        var targetVector = new Vector3(InputVector.x, 0, InputVector.y);
        var lookDirection = Movement(targetVector);
        if(Mouse)
            RotateMouse();
        else       
            RotateDirection(lookDirection);

        ChangeAnimation();

        if (Input.GetButtonDown("Run"))
        {
            Speed = RunSpeed;
            CurrentState = State.Run;
        }
        else if (Input.GetButtonUp("Run"))
        {
            Speed = SaveWalkSpeed;
            CurrentState = State.Walk;
        }
        else if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
            CurrentState = State.Idle;
        else if ((Input.GetAxis("Horizontal") != 0 && Input.GetAxis("Vertical") != 0) && !Input.GetButton("Run"))
            CurrentState = State.Walk;

    }

    private void ChangeAnimation()
    {
        if (CurrentState == State.Idle)
        {
            PlayerAnimator.SetBool("Idle", true);
            PlayerAnimator.SetBool("Walk", false);
            PlayerAnimator.SetBool("Run", false);
        }
        if (CurrentState == State.Walk)
        {
            PlayerAnimator.SetBool("Idle", false);
            PlayerAnimator.SetBool("Walk", true);
            PlayerAnimator.SetBool("Run", false);
        }
        if (CurrentState == State.Run)
        {
            PlayerAnimator.SetBool("Idle", false);
            PlayerAnimator.SetBool("Walk", false);
            PlayerAnimator.SetBool("Run", true);
        }
    }

    private void RotateMouse()
    {
        Ray ray = PlayerCamera.ScreenPointToRay(MousePos);
        if(Physics.Raycast(ray, out RaycastHit hitinfo, maxDistance: 300f))
        {
            var target = hitinfo.point;
            transform.LookAt(target);
        }
    }

    private void RotateDirection(Vector3 lookDirection)
    {
        var rotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotateSpeed);
    }

    private Vector3 Movement(Vector3 targetVector)
    {
        var speed = Speed * Time.deltaTime;

        targetVector = Quaternion.Euler(0, PlayerCamera.gameObject.transform.eulerAngles.y, 0) * targetVector;
        var position = transform.position + targetVector * speed;

        transform.position = position;
        transform.position.Normalize();
        return targetVector;
    }

}
