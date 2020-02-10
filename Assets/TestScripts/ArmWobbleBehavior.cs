using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class ArmWobbleBehavior : MonoBehaviour
{
    public GameObject shoulder;
    public GameObject elbow;
    public GameObject hand;
    public GameObject brush;

    public SpriteShapeController spriteShapeController;

    public int elbowFramesBehindHand = 25;
    public float elbowWobbleMultiplier = 1.2f;
    public float elbowLoweringDistance = 1.2f;

    // each frame we pop the front off to move the elbow and add the hand position to the back - handles elbow wobble movement
    public Queue<float> yPositions; 

    private float zPos;
    // Start is called before the first frame update
    void Start()
    {
        yPositions = new Queue<float>();
        for(int i =0; i < elbowFramesBehindHand; i++){
            yPositions.Enqueue(hand.transform.position.y);
        }
        zPos = elbow.transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        //move elbow
        //deque & use
        float yChange = yPositions.Dequeue() - elbow.transform.position.y;
        float y =  elbow.transform.position.y + (yChange * elbowWobbleMultiplier) - elbowLoweringDistance;
        float x = (shoulder.transform.position.x + hand.transform.position.x) / 2f;
        elbow.transform.position = new Vector3(x, y, zPos);
        //enqueue hand pos
        yPositions.Enqueue(hand.transform.position.y);

        // update the spline
        var spline = spriteShapeController.spline;
        spline.SetPosition(1, elbow.transform.localPosition);
        var adjustedHandLocalPositionRelativeToShoulder = brush.transform.localPosition + hand.transform.localPosition;
        spline.SetPosition(2, adjustedHandLocalPositionRelativeToShoulder);
    }
}
