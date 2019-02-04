using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class BlocoInfo: Bloco
{
	public Sprite blocoInfo;
	//private Info info;
	private int tamanho_x = 30;
	private int tamanho_y = 65;
	private int tamanho_font = 50;
	private Canvas canvas;
	private Image imagem_info;
	private Text texto_info;
	private int LETRAS_MAX = 28;
	private bool mostrar = false;
	private int TAMANHO_Y_MAX = 200;

	public string texto = "texto padrao";
	public Sprite imagem_fundo;
	private Animator animator;

	public override void Iniciar (bool infinito = false)
	{
		tipo = '4';

        base.Iniciar(infinito);
        animator = GetComponent<Animator>();
		sprite = GetComponent<SpriteRenderer> (); // we are accessing the SpriteRenderer that is attached to the Gameobject
		if (sprite.sprite == null)
		{
			sprite.sprite = blocoInfo;
		}
		sprite.sortingOrder = sortingOrder;
		sprite.sortingLayerName = LAYER_NAME;
		canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
		foreach (Image imagem in canvas.GetComponentsInChildren <Image>(true))
		{
			if (imagem.name == "Fundo_Info")
			{
				imagem_info = imagem;
			}
		}

		foreach (Text texto in canvas.GetComponentsInChildren<Text>(true))
		{
			if (texto.name == "Texto_Info")
			{
				texto_info = texto;
				texto_info.fontSize = tamanho_font;
			}
		}
		//info = new Info ("Texto padrao aqui.");
	}

	public void OnDestroy()
	{
		mostrar = false;
		if(imagem_info != null)
			imagem_info.gameObject.SetActive (false);
		if(texto_info != null)
			texto_info.gameObject.SetActive (false);
	}

	void Update ()
	{
        if (mostrar)
            MostrarInfo();
        else if (GameManager.instance.pause)
            MostrarInfo(false);

	}

	public override void avisar_bloco_abaixo(){
		mostrar = true;

	}

	public override void Desativar ()
	{
		animator.enabled = false;
		base.Desativar ();
	}

    public override void ModoEditor()
    {
        animator.enabled = false;
        base.ModoEditor();
    }

    public override void Reiniciar ()
	{
		animator.enabled = true;
		base.Reiniciar ();
	}


	private void MostrarInfo(bool mostrar = true)
	{
		if (GameManager.instance.editor)
			return;
		
		Vector3 teste = posicaoAtual + new Vector3 (0, 1, 0);

        if (kevin == null)
            BuscarKevin();

        if ((kevin.posicaoAtual == teste || kevin.destino == teste) && mostrar)
		{
			float vetor_tamanho_x = LETRAS_MAX * tamanho_x;
			if(texto.Length / LETRAS_MAX == 0)
				vetor_tamanho_x = texto.Length % LETRAS_MAX * tamanho_x;
			float vetor_tamanho_y = (texto.Length / LETRAS_MAX + 1) * tamanho_y;
			if (vetor_tamanho_y > TAMANHO_Y_MAX)
				vetor_tamanho_y = TAMANHO_Y_MAX;
			Vector2 tamanho = new Vector2 (vetor_tamanho_x, vetor_tamanho_y);
			imagem_info.sprite = imagem_fundo;
			imagem_info.rectTransform.sizeDelta = tamanho;
			imagem_info.gameObject.SetActive (true);

			texto_info.text = texto;
			texto_info.rectTransform.sizeDelta = tamanho;
			texto_info.gameObject.SetActive (true);
		} else
		{
			mostrar = false;
			imagem_info.gameObject.SetActive (false);
			texto_info.gameObject.SetActive (false);
		}
	}

	public void ReceberInfo(string info)
	{
		texto = info;
	}

	public void LerInfo(int numInfo)
	{
		string nomeArquivo;
		string dados = "";

		nomeArquivo = "fase" + fase.ToString() + "_texto.txt";

		string path = System.IO.Path.Combine(Application.streamingAssetsPath, nomeArquivo);

		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WebGLPlayer)
		{
			// Android only use WWW to read file
			WWW readerwww = new WWW(path);
			while ( ! readerwww.isDone) {}

			string realPath = Application.persistentDataPath + "/textos";
			System.IO.File.WriteAllBytes(realPath, readerwww.bytes);

			path = realPath;
		}


		//Read the text from directly from the test.txt file
		if (!File.Exists (path))
			return;
		StreamReader reader = new StreamReader(path);
		for (int i = 0; i <= numInfo; i++)
		{
			dados = reader.ReadLine();
		}
		texto = dados;

		reader.Close ();
	}
}