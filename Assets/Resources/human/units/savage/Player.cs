using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject weapon1, bone;
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        Animation anim = GetComponent<Animation>();
	//animator = GetComponent<Animator>();

	/*
        foreach (AnimationState state in anim)
        {
            state.speed = 0.5F;
        }
	*/
	//anim.Play("savage_idle");
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
        if (Input.GetMouseButtonDown(0) /*&& !isAnimAttack*/) {
            isAnimAttack = true;
            animator.SetBool("isAttack", true);
            Animation anim = GetComponent<Animation>();
            //anim.Play("savage_attack_0");
            StartCoroutine("OnCompleteAttackAnimation");
        }

	Animation myAnim = GetComponent<Animation>();
        // get animations currently playing
        // https://answers.unity.com/questions/1206196/retrieve-the-animation-clip-that-animator-is-playi.html (2020-04-20)
/*        
int w = myAnim.GetCurrentAnimatorClipInfo(0).Length;
        string[] clipName = new string[w];
        for (int i = 0; i < w; i += 1)
        {
            clipName[i] = myAnim.GetCurrentAnimatorClipInfo(0)[i].clip.name;
            
        }
*/
        lastMouse = Input.mousePosition - lastMouse ;
        lastMouse = new Vector3(-lastMouse.y * camSens, lastMouse.x * camSens, 0 );
        lastMouse = new Vector3(transform.eulerAngles.x + lastMouse.x , transform.eulerAngles.y + lastMouse.y, 0);

        // 3rd person melee
        lastMouse.x = 0;

        transform.eulerAngles = lastMouse;
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
        //if (Input.GetKey(KeyCode.Space)){ //If player wants to move on X and Z axis only
            p.y = 0;
            transform.Translate(p);
            newPosition.x = transform.position.x;
            newPosition.z = transform.position.z;
            transform.position = newPosition;
        //}
        //else{
            //transform.Translate(p);
        //}      
    }
     
    private Vector3 GetBaseInput() { //returns the basic values, if it's 0 than it's not active.
        Vector3 p_Velocity = new Vector3();
        animator.SetBool("isRunFwd", false);
        if (Input.GetKey (KeyCode.W)){
            p_Velocity += new Vector3(0, 0 , 1);
	    Animation anim = GetComponent<Animation>();
	    //anim.Play("savage_run_fwd");
            animator.SetBool("isRunFwd", true);
        }
        if (Input.GetKey (KeyCode.S)){
            p_Velocity += new Vector3(0, 0, -1);
        }
        if (Input.GetKey (KeyCode.A)){
            p_Velocity += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey (KeyCode.D)){
            p_Velocity += new Vector3(1, 0, 0);
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
