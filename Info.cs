using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Info : MonoBehaviour
{
	private Text info;
	private Image fundo;
	public Sprite imagemFundo;
	public Font fonte;
	public float x, y;
	public int tamanhoFonte;

	public Info (string texto)
	{
		info = Instantiate (info);
		fundo = Instantiate (fundo);
		info.text = texto;
		info.fontSize = tamanhoFonte;
		info.font = fonte;
		fundo.overrideSprite = imagemFundo;

	}
}

