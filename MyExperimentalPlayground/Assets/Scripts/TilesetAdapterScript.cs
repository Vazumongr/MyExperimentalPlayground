using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesetAdapterScript : MonoBehaviour
{

    public GameObject positiveXWall,negativeXWall,positiveZWall,negativeZWall;
    private List<Vector3> _takenLocations = new List<Vector3>();
    [SerializeField]
    private GameObject _labyrinthGenerator;
    private LabyrinthGenerationScript _labyrinthGenerationScript;

    public int neighbors = 0;
    // Start is called before the first frame update
    void Start()
    {

        _labyrinthGenerator = GameObject.Find("LabyrinthGenerator");
        _labyrinthGenerationScript = _labyrinthGenerator.GetComponent<LabyrinthGenerationScript>();

        if (_labyrinthGenerationScript == null)
            Debug.LogError("ITS NULL");

        CheckForNeighbors();

    }

    // Update is called once per frame
    void Update()
    {
        //CheckForNeighbors();

    }

    void CheckForNeighbors()
    {
        if (!_takenLocations.Equals(_labyrinthGenerationScript.GetTakenLocations()))
        {
            _takenLocations = _labyrinthGenerationScript.GetTakenLocations();

            //Vector3 comparison = transform.position + new Vector3(10, 0, 0);
            //Debug.Log(comparison);
            //if there's a platform at my x+1 an no where else, get rid of wall.
            if (_takenLocations.Contains(transform.position + new Vector3(10, 0, 0)))
            {
                positiveXWall.SetActive(false);
                neighbors++;

            }
            if (_takenLocations.Contains(transform.position + new Vector3(-10, 0, 0)))
            {
                negativeXWall.SetActive(false);
                neighbors++;

            }
            if (_takenLocations.Contains(transform.position + new Vector3(0, 0, 10)))
            {
                positiveZWall.SetActive(false);
                neighbors++;

            }
            if (_takenLocations.Contains(transform.position + new Vector3(0, 0, -10)))
            {
                negativeZWall.SetActive(false);
                neighbors++;

            }
        }
        if(neighbors==4)
        {
            Debug.Log("I got removed because I have friends");
            _labyrinthGenerationScript.RemoveFromUsableTaken(transform.position);
        }
    }
}
