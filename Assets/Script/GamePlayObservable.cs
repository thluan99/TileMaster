using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class GamePlayObservable : MonoBehaviour
{
    public Subject<Unit> WinHandleRequest {get; private set;} 
    public Subject<Unit> LoseHandleRequest {get; private set;}

    private void Awake() 
    {
        WinHandleRequest = new Subject<Unit>();
        LoseHandleRequest = new Subject<Unit>();
    }
}
