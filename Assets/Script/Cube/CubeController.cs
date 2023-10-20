using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class CubeController : MonoBehaviour, IEntity, IUndo, IStored
{
    [SerializeField] private string _name;
    public int Id => (Guid.NewGuid()).GetHashCode();
    public string Name => _name;

    public bool IsStored { get; set; }

    private Vector3 _position;
    private Vector3 _rotation;
    private Transform _parent;

    private void Start() 
    {
        _position = transform.localPosition;
        _rotation = transform.rotation.eulerAngles;
        _parent = transform.parent;
    }

    public void Undo()
    {
        transform.SetParent(_parent);
        transform.DOLocalMove(_position, GameManager.TIME_TO_ANIMATE);
        transform.DORotate(_rotation, GameManager.TIME_TO_ANIMATE);

        IsStored = false;
    }

    public void SetProperty(Vector3 position, Vector3 rotation, Transform parent)
    {
        _position = position;
        _rotation = rotation;
        _parent = parent;
    }
}
