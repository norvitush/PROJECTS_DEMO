using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VOrb.Extensions;

namespace VOrb.SantaJam
{
    public class TrajectoryRenderer : MonoBehaviour
    {
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private GameObject _TargetObject;
        [SerializeField] private int stepCount = 30;
        [SerializeField] private float _lerpSpeed = 0.2f;
        [SerializeField] private float timeBeteenStep = 1;
        private int _targetPoint;
        private Vector3 _collisionPoint;
        private Vector3 _currentState;
        private Vector3  _targetState;
        private Coroutine LerpToTarget = null;
        public void ShowTrajectory(Vector3 origin, Vector3 force, Rigidbody rig)
        {
            
            if (rig == null)
            {
                return;
            }

            DrawTrajectory(force, rig);
            _TargetObject.SetActive(true);
            //if (!_inScreen)
            //{
            //    _inScreen = true;
            //    _TargetObject.SetActive(true);
            //    DrawTrajectory(force, rig);
            //}
            //else
            //{
            //    _targetState =  force;
            //    if (LerpToTarget != null)
            //    {
            //        StopCoroutine(LerpToTarget);
            //    }
            //    LerpToTarget = StartCoroutine(MoveToTarget(rig));
            //}


        }

        private IEnumerator MoveToTarget(Rigidbody rig)
        {
         
            float currentRotation = 0;
            Debug.Log("Lerp between : " + _currentState + " | target: " + _targetState + " | Angle: " + Vector3.Angle(_currentState, _targetState));
            while (Vector3.Angle(_currentState, _targetState) > 0.1f 
                && Mathf.Abs(_currentState.magnitude - _targetState.magnitude)>0.05f
                )
            {
                yield return null;
                currentRotation  += _lerpSpeed*Time.deltaTime;
                var _nextShowVector = Vector3.Lerp(_currentState, _targetState, currentRotation);
                DrawTrajectory(_nextShowVector, rig);
            }
            LerpToTarget = null;
        }
        private void DrawTrajectory(Vector3 force, Rigidbody rig)
        {
            _currentState = force;
            var points = rig.CalculateMovement(stepCount, timeBeteenStep, Vector3.zero, force);
            _lineRenderer.positionCount = stepCount + 1;
            _lineRenderer.SetPosition(0, rig.transform.position);
            _targetPoint = points.Length - 1;
            _collisionPoint = points[_targetPoint];
            bool finded = false;
            for (int i = 0; i < points.Length; ++i)
            {
                _lineRenderer.SetPosition(i + 1, points[i]);
                if (!finded && i < points.Length - 1)
                {
                    RaycastHit hit;
                    Ray ray = new Ray(points[i], (points[i + 1] - points[i]).normalized);
                    if (Physics.Raycast(ray, out hit, Vector3.Distance(points[i + 1], points[i])))
                    {
                        _collisionPoint = hit.point;
                        _targetPoint = i;
                        finded = true;
                    }
                }

            }

            _TargetObject.transform.position = _collisionPoint;
            _lineRenderer.positionCount = _targetPoint + 3;

            float iy = GameService.Instance.SantaController.Ground.transform.position.y;
            if (_TargetObject.transform.position.y < iy)
            {
                _TargetObject.transform.position = _TargetObject.transform.position.SetYTo(iy + 0.2f);
            }


        }
        public void DropState()
        {
            _lineRenderer.positionCount = 0;
            _TargetObject.SetActive(false);
            if (LerpToTarget!=null)
            {
                StopCoroutine(LerpToTarget);
                LerpToTarget = null;
            }
            
        }
    }
}