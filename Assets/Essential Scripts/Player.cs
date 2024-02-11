using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour //Este es la clase del jugador.
{

    public InventoryObject inventory; //Con esto, el jugador ya tiene un inventario.



    private CharacterController controller; //Controller sencillo pa mover al jugador.
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float playerSpeed = 5.0f;
    private float jumpHeight = 1.0f;
    private float gravityValue = -9.81f;

    private void Start()
    {
        controller = gameObject.AddComponent<CharacterController>();
    }

    public void OnTriggerEnter(Collider other)  //esto nos dejara activar el sistema cuando el jugador toque un objeto.
    {
        var item = other.GetComponent<GroundItem>();
        if (item)
        {
            inventory.AddItem(new Item(item.item), 1); //esto nos dejara añadir al objeto al inventario
            Destroy(other.gameObject); //esto destruira al objeto fisico al momento de añadirlo al inventario.
            //Con esto ya tenemos un sistema que funciona, pero no limpia la lista.
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            inventory.Save();
        }
        if (Input.GetKeyDown(KeyCode.KeypadEnter)) //Por alguna razon que solo Dios entiende, esto es el Enter del NumPad, no del teclado. Dejare esto por posteridad. esto tomo una hora de mi vida.
        {
            inventory.Load(); //Magia, funciona. 
        }

        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        controller.Move(move * Time.deltaTime * playerSpeed);

        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }

        // Changes the height position of the player..
        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private void OnApplicationQuit()
    {   
        inventory.Container.Items = new InventorySlot[24]; //Con esto, la lista se limpiara cuando salgamos de la aplicacion. Originalmente, esto era un .Clear para la lista. Ya no es necesario.
        //Ahora cuando salgamos, la lista se borrara... haciendo una nueva. Magia.
    }


   
}
