using UnityEngine;
using UnityEngine.Splines;

namespace EugeneC.Mono {

    [AddComponentMenu("Eugene/Simple Spline Mover")]
    public class MonoSplineMover : MonoBehaviour {

        [SerializeField] private SplineContainer container;
        [SerializeField] [Min(0f)] private float speed = 1f; // meters per second
        private float _distance; // traveled distance along the path
        private float _length; // total path length in meters

        private SplinePath<Spline> _path;

        private void Start() {
            if (_path == null) BuildPath();
        }

        private void Update() {
            if (_length <= 0f || speed <= 0f) return;

            _distance += speed * Time.deltaTime;
            // loop endlessly
            if (_distance >= _length) _distance %= _length;

            var t = _path.ConvertIndexUnit(_distance, PathIndexUnit.Distance, PathIndexUnit.Normalized);
            Vector3 pos = container.EvaluatePosition(_path, t);
            transform.position = pos;

            Vector3 forward = container.EvaluateTangent(_path, t).xyz;
            Vector3 up = container.EvaluateUpVector(_path, t);

            if (forward.sqrMagnitude > 0.0001f)
                transform.rotation = Quaternion.LookRotation(forward, up);
        }

        private void OnEnable() {
            BuildPath();
            Spline.Changed += OnSplineChanged;
        }

        private void OnDisable() {
            Spline.Changed -= OnSplineChanged;
        }

        private void OnSplineChanged(Spline spline, int knotIndex, SplineModification modification) {
            BuildPath();
            _distance = Mathf.Repeat(_distance, Mathf.Max(_length, 0.0001f));
        }

        private void BuildPath() {
            if (container != null && container.Splines is { Count: > 0 }) {
                _path = new SplinePath<Spline>(container.Splines);
                _length = _path.GetLength();
            }
            else {
                _path = null;
                _length = 0f;
            }
        }

    }

}