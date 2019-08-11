//using System;
using System;
using System.Collections;
using Assets.Scripts;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class BlobController : MonoBehaviour
{

    [SerializeField]
    private BlobModel _blobModel;

    public Vector3 _target;

    public CharacterController CharacterController { get; private set; }

    public float Energy {
        get {return _blobModel.Energy;}
        set {_blobModel.Energy = value;}
    }

    public Renderer Renderer;

    public bool IsAlive {
        get { return _blobModel.IsAlive; }
        set { _blobModel.IsAlive = value; }
    }

    //private float _targetRotation = 0;
    private Quaternion _targetRotation;
    private Action _onDeath;
    private Action<BlobModel, Vector3> _onReplicate;
    private Action<FoodController> _onEat;
    
    private void Awake() {
        CharacterController = GetComponent<CharacterController>();
        Renderer = GetComponent<Renderer>();
    }

    public void Initialize(BlobModel blobModel, WorldModel worldModel, Action onDeath, Action<BlobModel, Vector3> onReplicate, Action<FoodController> onEat) {

        //transform.localScale = Vector3.zero;

        _blobModel = blobModel;
        _onDeath = onDeath;
        _onReplicate = onReplicate;
        _onEat = onEat;

        gameObject.name = $"Blob #{_blobModel.Id}";

        Renderer.material.color = _blobModel.Color;

        transform.Translate(0, 0.5f + CharacterController.height * 0.5f,0);

        StartCoroutine(ChangeTargetRotation());
        StartCoroutine(SensorRoutine());

        StartCoroutine(ScaleAnimationRoutine(Vector3.zero, new Vector3(_blobModel.Size, _blobModel.Size, _blobModel.Size), null));

    }

   

    // Update is called once per frame
    void Update()
    {


    }

    private void SetDead() {

        _onDeath();
        IsAlive = false;

        StartCoroutine(ScaleAnimationRoutine(transform.localScale, Vector3.zero, () => gameObject.SetActive(false)));

    }

    private void OnDrawGizmos() {

        if (!IsAlive)
            return;

        Gizmos.DrawWireSphere(transform.position, _blobModel.SensorRadius);
    }

    public void FixedUpdate() {

        if (!IsAlive)
            return;

        UpdateRotation();
        UpdateMovement();
        
    }

    private void UpdateMovement() {

        // Blob is always moving forward
        CharacterController.Move(transform.forward * _blobModel.MovementSpeed * Time.fixedDeltaTime);

    }

    private void UpdateRotation() {


        //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.AngleAxis(_targetRotation, transform.up), _blobModel.RotationSpeed * Time.fixedDeltaTime);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, _blobModel.RotationSpeed * Time.fixedDeltaTime);


    }


    private IEnumerator ChangeTargetRotation() {

        var randomAngle = UnityEngine.Random.Range(0, 360f);
        _targetRotation = Quaternion.AngleAxis(randomAngle, transform.up);

        yield return new WaitForSeconds(_blobModel.TimeToChangeDirection);


        while (IsAlive) {

            randomAngle = UnityEngine.Random.Range(-_blobModel.ChangeDirectionRange, _blobModel.ChangeDirectionRange);
            _targetRotation *= Quaternion.AngleAxis(randomAngle, transform.up);

            yield return new WaitForSeconds(_blobModel.TimeToChangeDirection);
        }


    }


    private IEnumerator AcquireTarget(Vector3 worldSize) {

        var xSizeMin = -worldSize.x / 2;
        var xSizeMax = worldSize.x / 2;

        var zSizeMin = -worldSize.z / 2;
        var zSizeMax = worldSize.z / 2;

        while (IsAlive) {

            //_target = new Vector3(UnityEngine.Random.Range(xSizeMin, xSizeMax), 0, UnityEngine.Random.Range(zSizeMin, zSizeMax));

            yield return new WaitForSeconds(_blobModel.TimeToAcquireNewTarget);

        }

    }

    private IEnumerator SensorRoutine() {

        while (IsAlive) {

            var hitColliders = Physics.OverlapSphere(transform.position, _blobModel.SensorRadius + _blobModel.Size / 2, LayerMaskUtilities.IGNORE_LAYER);

            var closestObjectDistance = _blobModel.SensorRadius;
            var closestPoint = Vector3.zero;

            foreach (var collider in hitColliders) {

                if (collider.name == name || collider.tag == "Floor")
                    continue;

                var detectedPoint = collider.ClosestPointOnBounds(transform.position);

                var distance = Vector3.Distance(transform.position, detectedPoint);


                if (distance < closestObjectDistance) {
                    closestPoint = detectedPoint;
                    closestObjectDistance = distance;
                }
                
            }

            if (closestPoint != Vector3.zero) {

                var adjustedPosition = new Vector3(closestPoint.x, transform.position.y, closestPoint.z);

                var oppositeDirection = Vector3.Normalize(transform.position - adjustedPosition);

                if (oppositeDirection != Vector3.zero)
                    _targetRotation = Quaternion.LookRotation(oppositeDirection);

            }

            yield return new WaitForSeconds(_blobModel.TimeToFireSensor);

        }

    }

    public void Tick() {

        TickEnergy();
        TickDeath();
        TickReplication();
    }

    private void TickEnergy() {
        if (!IsAlive)
            return;

        _blobModel.Energy -= (Mathf.Pow(_blobModel.Size, 3) + Mathf.Pow(_blobModel.MovementSpeed, 2) + _blobModel.SensorRadius);

    }

    private void TickDeath() {
        if (!IsAlive)
            return;

        if (_blobModel.Energy <= 0){// || UnityEngine.Random.Range(0, 1f) < _blobModel.DyingChance) {

            SetDead();
        }
    }

    private void TickReplication() {
        if (!IsAlive)
            return;

        if (_blobModel.Energy >= BlobModel.REPLICATION_ENERGY_MINIMUM) {

            _onReplicate(Factory.CopyBlobModel(_blobModel), transform.position);

            _blobModel.Energy -= BlobModel.REPLICATION_ENERGY_COST;

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

    public void OnTriggerEnter(Collider other) {
        
        if (other.tag == "Food") {
            _onEat(other.GetComponent<FoodController>());
        }

    }
}
