using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LabyrinthGenerationScript : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _tilesets;     //THIS IS AN ARRAY TO HOLD THE PREFAB TILESETS I USE TO CREATE MY WORLD
    public GameObject experiment;       //THIS IS FOR MESSING AROUND WITH STUFF

    //THIS IS TO PARENT ALL THE SPAWNED PREFABS SO THEY DONT FILL MY HEIRARCHY
    [SerializeField]
    private GameObject _labyrinthContainer;  //This is needed to get the transform
    private Transform _labyrinthContainerTransform; //This is whats needed to make the spawned tiles a child of the container

    private List<Vector3> _takenLocations = new List<Vector3>();    //A list of all taken locations so that no objects can spawn in the same place
    private List<GameObject> _spawnedTiles = new List<GameObject>();    //A list of all the objects spawned
    private List<Vector3> _usableTakenLocations = new List<Vector3>();
    [SerializeField]
    private int mazeSize = 1000;    //The amount of tiles to be spawned
    public TextAsset timesText;     //Text file that gets written to (was used for time testing different methods of generation)

    public Canvas loadingScreen;

    void Awake()
    {
        _labyrinthContainerTransform = _labyrinthContainer.GetComponent<Transform>();   //Gets the transform
        StartCoroutine(GenerateMazeC());
    }

    // Start is called before the first frame update
    void Begin()
    {
        _labyrinthContainerTransform = _labyrinthContainer.GetComponent<Transform>();   //Gets the transform

        System.DateTime start = System.DateTime.Now;    //Gets the time before generating the maze

        //GenerateMaze2();
        StartCoroutine(GenerateMazeC());

        System.DateTime end = System.DateTime.Now;      //Gets the time after generating the maze

        System.TimeSpan timeDif = end - start;          //This finds how long it took to generate the maze

        string path = "Assets/Resources/Times.txt";     //Path to the text file I want to write too

        StreamWriter writer = new StreamWriter(path, true); //Writer which is what writes to the file

        writer.WriteLine(timeDif);  //Writes to the text file

        writer.Close(); //Closes the writer so there's no memory leaks
        
        Debug.Log(timeDif);
        /*        Debug.Log("Done generating the maze");
                Debug.Log(experiment.GetComponent<Transform>().localScale);
                Debug.Log(experiment.transform.localScale);
                Debug.Log(experiment.GetComponent<Renderer>().bounds);*/
        //FindFurthestTile();
    }


    // Update is called once per frame
    void Update()
    {
        
    }


    //THE SECOND MORE EFFECIENT METHOD OF GENERATING THE MAZE
    void GenerateMaze2()
    {
        //int mazeSize = 5000;
        Vector3 oldLocation = new Vector3(0, 0, 0); //This is so we start at the origin.
        Vector3 spawnLocation = new Vector3(0, 0, 0);   //This is so we start at the origin

        float spacing = _tilesets[0].GetComponent<Transform>().localScale.x;    //gets the dimensions of the tile so i know how far apart to spawn them

        for (int i = 0; i < mazeSize; i++)
        {

            
            GameObject selection = _tilesets[Random.Range(0, _tilesets.Length)];    //Picks a random tileset (they're just color coded blocks rn.
            int failsafe = 0;
            //If this is the first one, we spawn it at our origin
            if (i > 0)
            {
                List<Vector3> attempts = new List<Vector3>();   //This tracks all our attempts so I know if tile tries the same spot more than once.
                /* PART OF THE SECOND METHOD */
                List<Vector3> availableSpaces = new List<Vector3>();    //Track of all our available spaces that the tile can spawn in
                int[] difference = { (int)spacing * -1, (int)spacing }; //This is used to calculate the possible spawn spaces.
                //These are the possible spawn spaces
                availableSpaces.Add(new Vector3(difference[1], 0, 0) + oldLocation);
                availableSpaces.Add(new Vector3(0, 0, difference[1]) + oldLocation);
                availableSpaces.Add(new Vector3(difference[0], 0, 0) + oldLocation);
                availableSpaces.Add(new Vector3(0, 0, difference[0]) + oldLocation);
                /**/
                //Simply runs a check to make sure we aren't trying to spawn it inside of another object.
                do
                {


                    /* PART OF THE SECOND METHOD */
                    int index = Random.Range(0, availableSpaces.Count); //Picks a random index
                    spawnLocation = availableSpaces[index]; //Grabs the vector at that index. This becomes the new spawn location.
                    availableSpaces.RemoveAt(index);//Removes the vector at that index so we can't grab it again.
                    /**/

                    //this is used to keep track of how many times a tile tries a spawn in a taken spot.
                    if (attempts.Contains(spawnLocation))
                        Debug.Log("Tried a location more than once");
                    else
                        attempts.Add(spawnLocation);

                    //If we run out available spaces, we pick a random tile in the world and go to it.
                    /* PART OF THE SECOND METHOD */
                    if(availableSpaces.Count == 0)
                    {

                        //oldLocation = _takenLocations[Random.Range(0, _takenLocations.Count)]; // ORIGINAL
                        oldLocation = _usableTakenLocations[Random.Range(0, _usableTakenLocations.Count)];  //CHANGED
                        availableSpaces.Add(new Vector3(difference[1], 0, 0) + oldLocation);
                        availableSpaces.Add(new Vector3(0, 0, difference[1]) + oldLocation);
                        availableSpaces.Add(new Vector3(difference[0], 0, 0) + oldLocation);
                        availableSpaces.Add(new Vector3(0, 0, difference[0]) + oldLocation);

                    }
                    /**/
                    
                    //if the spot we ended up with is already taken, try it again. When we pick a spot that isn't taken, we will break out of the loop
                } while (_takenLocations.Contains(spawnLocation)); 
            }
            else
            {
                spawnLocation = new Vector3(0, 0, 0);
            }
            //Create a reference so we can store the position fo the newly spawned object
            GameObject spawn = Instantiate(selection, spawnLocation, new Quaternion(), _labyrinthContainerTransform) as GameObject;

            //Gets our old location
            oldLocation = spawn.transform.position;
            //Adds it to our list so we don't spawn blocks on top of eachother
            _takenLocations.Add(oldLocation);
            _usableTakenLocations.Add(oldLocation); //Potential Efficiency boost
            _spawnedTiles.Add(spawn);


        }
    }

    /*
     * I think using a couroutine is the way I'll have to do 
     * my maze generation so I can have a loading screen
     * during the maze generation. Right now it takes about 30 secs 
     * to generate the maze using the coroutine but that can be 
     * shortened by changed the mod value
     * used towards the end of the method.
     */
    //Coroutine of 2nd maze generation method
    IEnumerator GenerateMazeC()
    {
        //int mazeSize = 5000;
        Vector3 oldLocation = new Vector3(0, 0, 0); //This is so we start at the origin.
        Vector3 spawnLocation = new Vector3(0, 0, 0);   //This is so we start at the origin

        float spacing = _tilesets[0].GetComponent<Transform>().localScale.x;    //gets the dimensions of the tile so i know how far apart to spawn them

        for (int i = 0; i < mazeSize; i++)
        {


            GameObject selection = _tilesets[Random.Range(0, _tilesets.Length)];    //Picks a random tileset (they're just color coded blocks rn.
            int failsafe = 0;
            //If this is the first one, we spawn it at our origin
            if (i > 0)
            {
                List<Vector3> attempts = new List<Vector3>();   //This tracks all our attempts so I know if tile tries the same spot more than once.
                /* PART OF THE SECOND METHOD */
                List<Vector3> availableSpaces = new List<Vector3>();    //Track of all our available spaces that the tile can spawn in
                int[] difference = { (int)spacing * -1, (int)spacing }; //This is used to calculate the possible spawn spaces.
                //These are the possible spawn spaces
                availableSpaces.Add(new Vector3(difference[1], 0, 0) + oldLocation);
                availableSpaces.Add(new Vector3(0, 0, difference[1]) + oldLocation);
                availableSpaces.Add(new Vector3(difference[0], 0, 0) + oldLocation);
                availableSpaces.Add(new Vector3(0, 0, difference[0]) + oldLocation);
                /**/
                //Simply runs a check to make sure we aren't trying to spawn it inside of another object.
                do
                {


                    /* PART OF THE SECOND METHOD */
                    int index = Random.Range(0, availableSpaces.Count); //Picks a random index
                    spawnLocation = availableSpaces[index]; //Grabs the vector at that index. This becomes the new spawn location.
                    availableSpaces.RemoveAt(index);//Removes the vector at that index so we can't grab it again.
                    /**/

                    //this is used to keep track of how many times a tile tries a spawn in a taken spot.
                    if (attempts.Contains(spawnLocation))
                        Debug.Log("Tried a location more than once");
                    else
                        attempts.Add(spawnLocation);

                    //If we run out available spaces, we pick a random tile in the world and go to it.
                    /* PART OF THE SECOND METHOD */
                    if (availableSpaces.Count == 0)
                    {

                        //oldLocation = _takenLocations[Random.Range(0, _takenLocations.Count)]; // ORIGINAL
                        oldLocation = _usableTakenLocations[Random.Range(0, _usableTakenLocations.Count)];  //CHANGED
                        availableSpaces.Add(new Vector3(difference[1], 0, 0) + oldLocation);
                        availableSpaces.Add(new Vector3(0, 0, difference[1]) + oldLocation);
                        availableSpaces.Add(new Vector3(difference[0], 0, 0) + oldLocation);
                        availableSpaces.Add(new Vector3(0, 0, difference[0]) + oldLocation);

                    }
                    /**/

                    //if the spot we ended up with is already taken, try it again. When we pick a spot that isn't taken, we will break out of the loop
                } while (_takenLocations.Contains(spawnLocation));
            }
            else
            {
                spawnLocation = new Vector3(0, 0, 0);
            }
            //Create a reference so we can store the position fo the newly spawned object
            GameObject spawn = Instantiate(selection, spawnLocation, new Quaternion(), _labyrinthContainerTransform) as GameObject;

            //Gets our old location
            oldLocation = spawn.transform.position;
            //Adds it to our list so we don't spawn blocks on top of eachother
            _takenLocations.Add(oldLocation);
            _usableTakenLocations.Add(oldLocation); //Potential Efficiency boost
            _spawnedTiles.Add(spawn);
            /*
             * The greater the number I mod i by,
             * the shorter it takes to generate the 
             * maze. But will increase the time that
             * nothing else can be performed (such as a loading screen animation).
             */
            int modVal = 100;

            if (i % modVal == 0)
                yield return null;

            //Debug.Log(i);   //simply logs the tile count
        }
        foreach(Transform child in _labyrinthContainerTransform)
        {
            child.GetComponent<TilesetAdapterScript>().CheckForNeighbors();
        }
        FindFurthestTile();
        loadingScreen.enabled = false;
    }

    void GenerateMaze()
    {
        //int mazeSize = 1000;
        Vector3 oldLocation = new Vector3(0,0,0);
        Vector3 spawnLocation = new Vector3(0,0,0);

        float spacing = _tilesets[0].GetComponent<Transform>().localScale.x;

        for (int i = 0; i < mazeSize; i++)
        {

            //Picks a random tileset (they're just color coded blocks rn.
            GameObject selection = _tilesets[Random.Range(0,1)];
            int failsafe = 0;
            //If this is the first one, we spawn it at our origin
            if (i > 0)
            {
                List<Vector3> attempts = new List<Vector3>();
                //Simply runs a check to make sure we aren't trying to spawn it inside of another object.
                do
                {
                    /**/
                    //int difference = Random.Range(-1,2);
                    int[] difference = { (int)spacing * -1, (int)spacing };
                    int xorz = Random.Range(0, 2);
                    /**/


                    /* PART OF THE FIRST METHOD */
                    if (xorz == 1)
                        spawnLocation = new Vector3(difference[Random.Range(0, 2)], 0, 0) + oldLocation;
                    else
                        spawnLocation = new Vector3(0, 0, difference[Random.Range(0, 2)]) + oldLocation;
                    /**/



                    if (attempts.Contains(spawnLocation))
                        Debug.Log("Tried a location more than once");
                    else
                        attempts.Add(spawnLocation);

                    /* PART OF THE FIRST METHOD */
                    //This is the failsafe incase there are no more available blocks.
                    failsafe++;
                    if (failsafe > 50)
                    {
                        //Need to go back to step back
                        //Debug.LogError("////FAILSAFE HAS BEEN TRIGGERED\\\\\\\\////FAILSAFE HAS BEEN TRIGGERED\\\\\\\\");
                        oldLocation = _takenLocations[Random.Range(0, _takenLocations.Count)];

                    }
                    /**/
                }while (_takenLocations.Contains(spawnLocation));
            }
            else
            {
                spawnLocation = new Vector3(0, 0, 0);
            }
            //Create a reference so we can store the position fo the newly spawned object
            GameObject spawn = Instantiate(selection, spawnLocation, new Quaternion(),_labyrinthContainerTransform) as GameObject;

            //Gets our old location
            oldLocation = spawn.transform.position;
            //Adds it to our list so we don't spawn blocks on top of eachother
            _takenLocations.Add(oldLocation);
            _spawnedTiles.Add(spawn);


        }
    }

    private void PrintList(List<Vector3> list)
    {
        for(int i = 0; i < list.Count; i++)
        {
            Debug.Log(list[i]);
        }
    }

    public List<Vector3> GetTakenLocations()
    {
        //Debug.Log(_takenLocations.Count);
        return _takenLocations;
    }

    private void FindFurthestTile()
    {
        float temp = 0f;
        GameObject furthestTile = null;

        foreach(GameObject i in _spawnedTiles)
        {
            float x = Mathf.Abs(i.transform.position.x);
            float z = Mathf.Abs(i.transform.position.z);
            float distance = Mathf.Sqrt((x * x) + (z * z));
            if (distance > temp)
            {
                temp = distance;
                furthestTile = i;
            }
        }
        furthestTile.transform.position = furthestTile.transform.position + new Vector3(0, 10, 0);
        Debug.Log(furthestTile.transform.position);
    }

    public void RemoveFromUsableTaken(Vector3 vector)
    {
        _usableTakenLocations.Remove(vector);
    }
}
