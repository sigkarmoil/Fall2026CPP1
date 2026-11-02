using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    #region Camera State
    public enum CameraState
    {
        None,
        Follow,
        Bound,
        Transition
    }
    #endregion

    #region public variables
    // state of camera states
    public CameraState currentState;

    // Set the leftmost Y border of a level
    public float minYPos;

    //Set the rightmost Y border of a level
    public float maxYPos;

    // Create a variable called target that refers to its own TRANSFORM Inspector setting
    public Transform target;

    public float cameraSpeed = 5; //self explanatory

    #endregion


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
     if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogError("No Target assigned as player. Please include valid reference in the camera follow script");
                return;
            }
            target = player.transform;
        }   
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        //we need to capture current position in order to modify the x position for this update
        Vector3 currentPos = transform.position; // returns vector in general


        // what is the difference between transform.position and currentPos?
        // currentPOS.x just COPIES it because it is a struct. it does not immediately change the value of camera
        //modify to a clamp of x min and max values.
        currentPos.y = Mathf.Clamp(target.position.y, minYPos, maxYPos); // take the value of x, but limit it between minXPos and maxXpos
        currentPos.x = target.position.x;


        //currentPos.y = Mathf.Clamp
        // by default Unity does not allow transform.position.x
        //However it accepts an entire vector of it.

        transform.position = Vector3.MoveTowards(transform.position, currentPos, cameraSpeed * Time.deltaTime);




    }
}
