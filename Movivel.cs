using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public abstract class Movivel : MovingObject
{
	public Vector3 destino; 
	protected bool ativar = false;
	public bool dentro = false;
	protected bool movendo = false;

	public bool vivo = true;

	private bool ativo = true;

	public Vector3 retorno;

	#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
		private Vector2 touchOrigin = -Vector2.one;	//Used to store location of screen touch origin for mobile controls.
	#endif

    public override void Iniciar (bool infinito = false)
	{
		//Call the Start function of the MovingObject base class.
        base.Iniciar (infinito);

		Camera.main.transform.SetParent(this.transform) ;
		Camera.main.transform.SetPositionAndRotation (new Vector3(this.transform.localPosition.x,this.transform.localPosition.y,-10),  Quaternion.identity);
	}


	protected virtual void Update ()
	{

		int horizontal = 0;  	//Used to store the horizontal move direction.
		int vertical = 0;		//Used to store the vertical move direction.
		ativar = false;


		//Check if we are running either in the Unity editor or in a standalone build.
		#if UNITY_STANDALONE || UNITY_WEBGL || UNITY_EDITOR || UNITY_STANDALONE_OSX

		if(Input.GetKeyDown(KeyCode.Escape) && ativo)
		{
			if(GameManager.instance.editor)
				GameManager.instance.PauseEditor();
			else
				GameManager.instance.Pause(!GameManager.instance.pause);
		}

		if (!podeMover && !ativo)
			return;

		if (Input.GetKeyDown (KeyCode.P) && GameManager.instance.editor && EditorManager.instance.seletor.podeMover)
		{
            EditorManager.instance.seletor.AdicionarObjeto (posicaoAtual);
		}

		if (Input.GetKeyDown (KeyCode.C) && GameManager.instance.editor && EditorManager.instance.seletor.podeMover)
		{
            EditorManager.instance.seletor.RemoverBloco (posicaoAtual);
		}

		if(!GameManager.instance.pause)
		{
			//Get input from the input manager, round it to an integer and store in horizontal to set x axis move direction
			horizontal = (int) (Input.GetAxisRaw ("Horizontal"));

			//Get input from the input manager, round it to an integer and store in vertical to set y axis move direction
			vertical = (int) (Input.GetAxisRaw ("Vertical"));

			ativar = Input.GetKey(KeyCode.LeftShift);

			//Check if moving horizontally, if so set vertical to zero.
			if(horizontal != 0)
			{
				vertical = 0;
			}
		}

        //Check if we are running on iOS, Android, Windows Phone 8 or Unity iPhone
        #elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE

        if (!podeMover && !ativo)
            return;

		if(!GameManager.instance.pause)
		{
			//Check if Input has registered more than zero touches
			if (Input.touchCount > 0)
			{
				Touch myTouch;
				//Store the first touch detected.
				if (Input.touchCount == 1)
					myTouch = Input.touches[0];
				else
					myTouch = Input.touches[1];

				//Check if the phase of that touch equals Began
				if (myTouch.phase == TouchPhase.Began)
				{
					//If so, set touchOrigin to the position of that touch
					touchOrigin = myTouch.position;
				}

				//If the touch phase is not Began, and instead is equal to Ended and the x of touchOrigin is greater or equal to zero:
				else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
				{
					//Set touchEnd to equal the position of this touch
					Vector2 touchEnd = myTouch.position;

					float distancia = Vector2.Distance(touchOrigin, touchEnd);


					if(distancia < 40)
						return;

					//Calculate the difference between the beginning and end of the touch on the x axis.
					float x = touchEnd.x - touchOrigin.x;

					//Calculate the difference between the beginning and end of the touch on the y axis.
					float y = touchEnd.y - touchOrigin.y;

					//Set touchOrigin.x to -1 so that our else if statement will evaluate false and not repeat immediately.
					touchOrigin.x = -1;

					float angle = Mathf.Atan2 (y, x) * Mathf.Rad2Deg;
					if(angle < 0)
						angle += 360;	

					//Check if the difference along the x axis is greater than the difference along the y axis.
					if (angle > 10 && angle <= 100)
					{
						horizontal = 0;
						vertical = 1;
					}else if (angle > 100 && angle <= 190)
					{
						horizontal = -1;
						vertical = 0;
					}else if (angle > 190 && angle <= 280)
					{
						horizontal = 0;
						vertical = -1;
					}else if (angle > 280 || angle <= 10)
					{
						horizontal = 1;
						vertical = 0;
					}

					if(Input.touchCount > 1)
					{
						ativar = true;
					}
				}

			}
		}

#endif

        if ((horizontal != 0 || vertical != 0) && !movendo && podeMover)
		{
			AttemptMove (horizontal, vertical);
		}
	}

	public override void Remover(){

		this.transform.DetachChildren();
		base.Remover ();

	}

	public override void Ativar ()
	{
		podeMover = true;
		ativo = true;
		Camera.main.transform.SetParent(this.transform) ;
		Camera.main.transform.SetPositionAndRotation (new Vector3(this.transform.localPosition.x,this.transform.localPosition.y,-10),  Quaternion.identity);
	}

	public override void Desativar ()
	{
		Debug.Log (Camera.main.transform.parent);
		this.transform.DetachChildren();

		ativo = false;
		podeMover = false;
	}

	public override void DefinirPosicao (Vector3 posicao)
	{
		base.DefinirPosicao (posicao);
		GameManager.instance.AtualizarHUD (posicaoAtual, posicao, true);
	}


	protected abstract void AttemptMove (int xDir, int zDir);
}

