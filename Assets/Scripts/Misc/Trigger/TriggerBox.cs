using UnityEngine;

public class TriggerBox : MonoBehaviour
{
    public GameObject objectToReplace; // the object to replace the current object
    public KeyCode keyToPress; // the key to press to trigger the replacement
    public float newYPosition; // the new y position of the object to replace

    private bool hasBeenTriggered = false; // whether the replacement has been triggered

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hasBeenTriggered = false; // reset the trigger state when the player enters the trigger box
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Input.GetKeyDown(keyToPress) && !hasBeenTriggered)
            {
                Vector3 newPos = objectToReplace.transform.position;
                newPos.y = newYPosition; // set the new y position of the object to replace
                GameObject newObject = Instantiate(objectToReplace, newPos, objectToReplace.transform.rotation); // replace the current object with the new object at the specified position
                newObject.transform.position = new Vector3(transform.position.x, newObject.transform.position.y, transform.position.z); // retain the x and z position of the replaced object
                Destroy(gameObject); // destroy the current object
                hasBeenTriggered = true; // set the trigger state to true so that the replacement won't happen again
            }
        }
    }
}
