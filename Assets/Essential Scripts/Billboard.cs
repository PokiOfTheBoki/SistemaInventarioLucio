using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour  //esta script es para que los objetos siempre vean a la camara
{
    public Camera _camera;
    private void LateUpdate()  //esto es para evitar bugs y glitches extraños. Hacemos que el update se haga mas adelante que los demas.
    {
        transform.forward = _camera.transform.forward;
    }

}
