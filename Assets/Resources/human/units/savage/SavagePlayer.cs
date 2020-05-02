using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavagePlayer : MonoBehaviour
{
    public GameObject weapon1, bone;
    public Animator animator;

    bool isJump = false;

    // Start is called before the first frame update
    void Start()
    {
        Animation anim = GetComponent<Animation>();
        animator.Play("savage_idle");

        // weapon attachment test
        weapon1 = Instantiate(Resources.Load("human/units/savage/weapons/sword", typeof(GameObject))) as GameObject;
        //bone = GameObject.Find("_bone_swoosh_a_r");
        weapon1.transform.SetParent(bone.transform);
        weapon1.transform.localPosition = new Vector3(0, 0, -0.07f);
        weapon1.transform.localRotation = Quaternion.Euler(90, 0, 0);
    }


    public float speed = 3.0F;
    public float rotateSpeed = 3.0F;

    // Update is called once per frame
    /*
    Writen by Windexglow 11-13-10.  Use it, edit it, steal it I don't care.  
    Converted to C# 27-02-13 - no credit wanted.
    Simple flycam I made, since I couldn't find any others made public.  
    Made simple to use (drag and drop, done) for regular keyboard layout  
    wasd : basic movement
    shift : Makes camera accelerate
    space : Moves camera on X and Z axis only.  So camera doesn't gain any height*/
     
     
    float mainSpeed = 1.0f; //regular speed
    float shiftAdd = 2.50f; //multiplied by how long shift is held.  Basically running
    float maxShift = 10.000f; //Maximum speed when holdin gshift
    float camSens = 0.25f; //How sensitive it with mouse
    private Vector3 lastMouse = new Vector3(255, 255, 255); //kind of in the middle of the screen, rather than at the top (play)
    private float totalRun= 1.0f;

    bool isAnimAttack = false;
 
    void Update () {

        CharacterController controller = GetComponent<CharacterController>();

        // Rotate around y - axis
        transform.Rotate(0, Input.GetAxis("Horizontal") * rotateSpeed, 0);

        // Move forward / backward
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        float curSpeed = speed * Input.GetAxis("Vertical");
        controller.SimpleMove(forward * curSpeed);


        /*if(GetComponent<Rigidbody>().velocity.y > 0.1 || GetComponent<Rigidbody>().velocity.y < -0.1) {
            isJump = true;
            animator.SetBool("isLand" , false);
        }
        else {
            isJump = false;
            animator.SetBool("isLand" , true);
        }*/

        if (Input.GetMouseButtonDown(0) /*&& !isAnimAttack*/) {
            isAnimAttack = true;
            animator.SetBool("isAttack", true);
            Animation anim = GetComponent<Animation>();
            //anim.Play("savage_attack_0");
            StartCoroutine("OnCompleteAttackAnimation");
        }

	Animation myAnim = GetComponent<Animation>();

        lastMouse = Input.mousePosition - lastMouse ;
        lastMouse = new Vector3(-lastMouse.y * camSens, lastMouse.x * camSens, 0 );
        lastMouse = new Vector3(transform.eulerAngles.x + lastMouse.x , transform.eulerAngles.y + lastMouse.y, 0);

        // 3rd person melee
        lastMouse.x = 0;

        ////transform.eulerAngles = lastMouse;
        lastMouse =  Input.mousePosition;
        //Mouse  camera angle done.  
       
        //Keyboard commands
        float f = 0.0f;
        Vector3 p = GetBaseInput();
	if(p.Equals(new Vector3(0,0,0)) && !isAnimAttack) {
	    Animation anim = GetComponent<Animation>();
	    //anim.Play("savage_idle");
            return;
	}
        if (Input.GetKey (KeyCode.LeftShift)){
            totalRun += Time.deltaTime;
            p  = p * totalRun * shiftAdd;
            p.x = Mathf.Clamp(p.x, -maxShift, maxShift);
            //p.y = Mathf.Clamp(p.y, -maxShift, maxShift);
            p.z = Mathf.Clamp(p.z, -maxShift, maxShift);
        }
        else{
            totalRun = Mathf.Clamp(totalRun * 0.5f, 1f, 1000f);
            p = p * mainSpeed;
        }
       
        p = p * Time.deltaTime;
       Vector3 newPosition = transform.position;
            p.y = 0;
            ////transform.Translate(p);
            newPosition.x = transform.position.x;
            newPosition.z = transform.position.z;
            ////transform.position = newPosition;    
    }
     
    private Vector3 GetBaseInput() { //returns the basic values, if it's 0 than it's not active.
        Vector3 p_Velocity = new Vector3();
        animator.SetBool("isRunFwd", false);
        if (Input.GetKey (KeyCode.W)){
            p_Velocity += new Vector3(0, 0 , 1);
	    Animation anim = GetComponent<Animation>();
	    //anim.Play("savage_run_fwd");
            animator.SetBool("isRunFwd", true);
            animator.SetBool("isRunBack", false);
        }
        if (Input.GetKey (KeyCode.S)){
            p_Velocity += new Vector3(0, 0, -1);
            animator.SetBool("isRunBack", true);
            animator.SetBool("isRunFwd", false);
        } else {
            animator.SetBool("isRunBack", false);
        }
        if (Input.GetKey (KeyCode.A)){
            p_Velocity += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey (KeyCode.D)){
            p_Velocity += new Vector3(1, 0, 0);
        }
        if(Input.GetKey(KeyCode.Space) && isJump == false) {
            isJump = true;
            animator.SetBool("isJump", true);
            //transform.Translate(Vector3.up * 10 * Time.deltaTime, Space.World);
        }
        return p_Velocity;
 
   }

    // https://gamedev.stackexchange.com/questions/117423/unity-detect-animations-end/150281 20-04-20
    IEnumerator OnCompleteAttackAnimation()
    {
        Animator pc_anim = GetComponent<Animator>();
        yield return new WaitUntil(() => pc_anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f);
        isAnimAttack = false;
        //animator.SetBool("isAttack", false);
    }

}
