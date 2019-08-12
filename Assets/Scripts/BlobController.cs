//using System;
using System;
using System.Collections;
using Assets.Scripts;
using UnityEngine;

public class BlobController : MonoBehaviour, IEdible {

    public static int FOOD_ENERGY_MULTIPLIER = 20;


    [SerializeField]
    public BlobModel BlobModel;

    public Vector3 Target;
    public Quaternion TargetRotation;

    public Terrain Terrain;


    //public CharacterController CharacterController { get; private set; }
    public Rigidbody RigidBody;

    public Collider Collider;

    public float Energy {
        get { return BlobModel.Energy; }
        set { BlobModel.Energy = value; }
    }

    public Renderer Renderer;

    public bool IsAlive {
        get { return BlobModel.IsAlive; }
        set { BlobModel.IsAlive = value; }
    }
    public float MovementSpeed {
        get { return BlobModel.MovementSpeed; }
        set { BlobModel.MovementSpeed = value; }
    }
    public float Size {
        get { return BlobModel.Size; }
        set { BlobModel.Size = value; }
    }
    public float SensorRange {
        get { return BlobModel.SensorRadius; }
        set { BlobModel.SensorRadius = value; }
    }
    public int Age {
        get { return BlobModel.Age; }
        set { BlobModel.Age = value; }
    }



    //private float _targetRotation = 0;
    private Action _onDeath;
    private Action<BlobModel, Vector3> _onReplicate;
    private Action<IEdible> _onEat;

    float _lastHeight;


    private void Awake() {
          
    }

    public void Initialize(Vector3 startingPosition, BlobModel blobModel, WorldModel worldModel, Action onDeath, Action<BlobModel, Vector3> onReplicate, Action<IEdible> onEat) {

        //transform.localScale = Vector3.zero;

        _lastHeight = startingPosition.y;

        BlobModel = blobModel;
        _onDeath = onDeath;
        _onReplicate = onReplicate;
        _onEat = onEat;

        gameObject.name = $"Blob #{BlobModel.Id}";

        Renderer.material.color = BlobModel.Color;

        transform.Translate(0, 0.5f + blobModel.Size * 0.5f, 0);

        StartCoroutine(ChangeTargetRotation());
        StartCoroutine(SensorRoutine());

        StartCoroutine(ScaleAnimationRoutine(Vector3.zero, new Vector3(BlobModel.Size, BlobModel.Size, BlobModel.Size), null));

    }



    // Update is called once per frame
    void Update() {


    }

    private void SetDead() {

        _onDeath();
        IsAlive = false;

        StartCoroutine(ScaleAnimationRoutine(transform.localScale, Vector3.zero, () => gameObject.SetActive(false)));

    }

    private void OnDrawGizmos() {

        if (!IsAlive)
            return;

        Gizmos.DrawWireSphere(transform.position, BlobModel.AdjustedRadius);
    }

    public void FixedUpdate() {

        if (!IsAlive)
            return;

        UpdateRotation();
        UpdateMovement();

    }

    
    private void UpdateMovement() {

        var height = Terrain.SampleHeight(transform.position);
        var movement = transform.forward * BlobModel.MovementSpeed * Time.deltaTime;

        
        if (height > _lastHeight) {
            movement *= 0.8f;
        } else {
            movement *= 1.5f;
        }
        

        var newPosition = transform.position + movement;
        newPosition.y = height + Size / 2;

        transform.position = newPosition;

        _lastHeight = height;
                

        // Blob is always moving forward

        //var move = transform.forward * BlobModel.MovementSpeed * Time.deltaTime + transform.position;

       

        // if (CharacterController.isGrounded) {
        //move.y = -gravity;
        //}

        //move.y = CharacterController.isGrounded ? 0 : -gravity;

        //CharacterController.Move(move * Time.fixedDeltaTime);

        //CharacterController.Move(transform.forward * BlobModel.MovementSpeed * Time.fixedDeltaTime);
        //CharacterController.SimpleMove(transform.forward * BlobModel.MovementSpeed * Time.fixedDeltaTime * 50);
        //RigidBody.velocity = move;
        //RigidBody.AddForce(move, ForceMode.VelocityChange);

    }

    private void UpdateRotation() {


        //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.AngleAxis(_targetRotation, transform.up), _blobModel.RotationSpeed * Time.fixedDeltaTime);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, TargetRotation, BlobModel.RotationSpeed * Time.fixedDeltaTime);


    }


    private IEnumerator ChangeTargetRotation() {

        var randomAngle = UnityEngine.Random.Range(0, 360f);
        TargetRotation = Quaternion.AngleAxis(randomAngle, transform.up);

        yield return new WaitForSeconds(BlobModel.TimeToChangeDirection);


        while (IsAlive) {

            randomAngle = UnityEngine.Random.Range(-BlobModel.ChangeDirectionRange, BlobModel.ChangeDirectionRange);
            TargetRotation *= Quaternion.AngleAxis(randomAngle, transform.up);

            yield return new WaitForSeconds(BlobModel.TimeToChangeDirection);
        }


    }


    private IEnumerator AcquireTarget(Vector3 worldSize) {

        var xSizeMin = -worldSize.x / 2;
        var xSizeMax = worldSize.x / 2;

        var zSizeMin = -worldSize.z / 2;
        var zSizeMax = worldSize.z / 2;

        while (IsAlive) {

            //_target = new Vector3(UnityEngine.Random.Range(xSizeMin, xSizeMax), 0, UnityEngine.Random.Range(zSizeMin, zSizeMax));

            yield return new WaitForSeconds(BlobModel.TimeToAcquireNewTarget);

        }

    }

    private IEnumerator SensorRoutine() {

        while (IsAlive) {

            var closest = FindClosestObject();

            if (closest != null)
                BlobDecider.DecideOnSensedClosestObject(this, closest);

            yield return new WaitForSeconds(BlobModel.TimeToFireSensor);

        }

    }

    public SensedObjectModel FindClosestObject() {

        SensedObjectModel closestObject = null;

        var hitColliders = Physics.OverlapSphere(transform.position, BlobModel.AdjustedRadius, LayerMaskUtilities.LayerMaskIgnore());

        foreach (var collider in hitColliders) {

            if (collider.name == name || collider.tag == "Floor")
                continue;

            var sensedObject = SensedObjectModel.Build(transform.position, collider);

            if (closestObject == null) {
                closestObject = sensedObject;
                continue;
            }

            if (sensedObject.Distance < closestObject.Distance) {
                closestObject = sensedObject;
            }

        }

        return closestObject;

    }

 
    public void Tick() {

        TickEnergy();
        TickDeath();
        TickReplication();
    }

    private void TickEnergy() {
        if (!IsAlive)
            return;

        BlobModel.Energy -= (Mathf.Pow(BlobModel.Size, 3) + Mathf.Pow(BlobModel.MovementSpeed, 2) + BlobModel.SensorRadius);

    }

    private void TickDeath() {
        if (!IsAlive)
            return;

        BlobModel.Age++;

        if (BlobModel.Energy <= 0 || UnityEngine.Random.Range(0, 1f) < BlobModel.DyingChance * BlobModel.Age) {

            SetDead();
        }
    }

    private void TickReplication() {
        if (!IsAlive)
            return;

        if (BlobModel.Energy >= BlobModel.REPLICATION_ENERGY_MINIMUM) {

            _onReplicate(BlobFactory.MutateBlobModel(BlobModel), transform.position);

            BlobModel.Energy -= BlobModel.REPLICATION_ENERGY_COST;

        }
    }

    private IEnumerator ScaleAnimationRoutine(Vector3 startingSize, Vector3 endingSize, Action onAnimationEnd) {

        float t = 0;

        while (true) {

            transform.localScale = Vector3.Lerp(startingSize, endingSize, t);
            t += 0.1f;

            if (t >= 1) {
                onAnimationEnd?.Invoke();
            }
            
            yield return new WaitForEndOfFrame();
        }


    }

    public void MouthCollision(Collider other) {

        if (other.tag == "Food") {
            _onEat(other.GetComponent<FoodController>());
            return;
        }

        if (other.tag == "Blob") {

            var otherBlob = other.GetComponent<BlobController>();

            if (CanEat(otherBlob)) {

                //Debug.Log($"{name} has eaten {otherBlob.name}");

                _onEat(other.GetComponent<BlobController>());
            }
        }

    }

    public void SetEaten() {
        SetDead();
    }

    public float GetEnergyValue() {
        return Size * FOOD_ENERGY_MULTIPLIER;
    }

    public bool CanEat(BlobController other) {
        return (Size * BlobModel.SIZE_DIFFERENCE_TO_EAT > other.Size );
    }

    public void MoveTowards(Vector3 point) {
        var rotation = RotationHelper.GetTowardsLookRotation(transform.position, point);

        if (rotation == null)
            return;

        TargetRotation = (Quaternion)rotation;
    }

    public void RunAwayFrom(Vector3 point) {
        var rotation = RotationHelper.GetOppositeLookRotation(transform.position, point);

        if (rotation == null)
            return;

        TargetRotation = (Quaternion)rotation;
    }


    private void OnTriggerEnter(Collider collider) {
        if (collider.tag == "Wall") {
            SetDead();
        }

    }
}
