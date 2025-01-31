using UnityEngine;

public class spinning : MonoBehaviour
{
    public float rotationSpeed = 50f; // Adjust the rotation speed as needed

    private float reverseRotationDelay;
    private bool isReversing = false;

    // Start is called before the first frame update
    void Start()
    {
        rotationSpeed=Random.Range(10f,60f);
        // Set a random delay for reversing the rotation
        reverseRotationDelay = Random.Range(2f, 5f);
        Invoke("StartReverseRotation", reverseRotationDelay);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isReversing)
        {
            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
        }
        else
        {
            transform.Rotate(Vector3.back * rotationSpeed * Time.deltaTime);

           // if (transform.rotation.eulerAngles.y <= 0.1f)
           // {
                //isReversing = false;

                reverseRotationDelay = Random.Range(2f, 5f);
                Invoke("StartReverseRotation", reverseRotationDelay);
           // }
        }
    }

    // Method to start reversing the rotation
    void StartReverseRotation()
    {
        isReversing = true;
    }
}
