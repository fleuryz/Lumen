using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	public AudioSource[] efxSource;
	public AudioSource musicSource;
	public static SoundManager instance = null;

	public float lowPitchRange = .95f;
	public float highPitchRange = 1.05f;

	private int numFontesAtivas = 0;
	private int[] alturas = new int[8];
	private bool[] fontesAtivas = new bool[8];
	private int ALTURA = 10;
	private float volume = 0.8f;

    Player kevin;

	// Use this for initialization
	void Awake () {
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);

		DontDestroyOnLoad (gameObject);

	}

	public void PlayMusic(AudioClip music){
		musicSource.clip = music;
		musicSource.loop = true;
		musicSource.Play ();
	}

	public void StopMusic()
	{
		musicSource.Stop ();
	}

	public void PlayEmCena(AudioClip clip, int altura)
	{
		for (int i = 0; i < 8; i++)
		{
			AudioSource fonte = efxSource [i];
			if (!fonte.isPlaying)
			{
				int distancia = 0;
				if (GameManager.instance.boardScript.kevin != null)
					distancia = (int)Mathf.Abs(GameManager.instance.boardScript.kevin.posicaoAtual.y - altura);
				fonte.volume = volume - Mathf.Pow(distancia,2)/100;

				fonte.clip = clip;
				fonte.loop = true;
				fonte.Play ();
				alturas [i] = altura;
				fontesAtivas [i] = true;
		
				numFontesAtivas++;
				return;
			}
		}
	}

	public void PlaySingle(AudioClip clip, int distancia = 0)
		{
		for (int i = 0; i < 8; i++)
		{
			AudioSource fonte = efxSource [i];
			if (!fonte.isPlaying)
			{
				fonte.clip = clip;
				fonte.volume = volume - Mathf.Pow(distancia,2)/100;
				fonte.loop = false;
				fonte.Play ();
				return;
			}
		}
	}

	public void RandomSfxs (int distancia = 0, params AudioClip [] clips){
		int randomIndex = Random.Range (0, clips.Length);
		float randomPitch = Random.Range (lowPitchRange, highPitchRange);

		for (int i = 0; i < 8; i++)
		{
			AudioSource fonte = efxSource [i];
			if (!fonte.isPlaying)
			{
				fonte.pitch = randomPitch;
				fonte.clip = clips [randomIndex];
				fonte.loop = false;
				fonte.Play ();
				return;
			}
		}
		MatarFonte ();
		RandomSfxs (distancia, clips);
	}

	public void VolumeMusica(float valor){
		musicSource.volume = valor;
	}

	public void VolumeEfeitos(float valor){
		foreach(AudioSource fonte in efxSource){
			if (!fonte.isPlaying)
			{
				fonte.volume = valor;
				volume = valor;
			}
		}
	}

    public void StopSounds()
    {
        foreach (AudioSource fonte in efxSource)
        {
            if (fonte.isPlaying)
            {
                fonte.Stop();
                fonte.loop = false;
            }
        }
    }

	private void MatarFonte()
	{
		for (int i = 0; i < 8; i++)
		{
			AudioSource fonte = efxSource [i];
			if (fontesAtivas[i])
			{
				fonte.Stop ();
				fontesAtivas [i] = false;
				return;
			}
		}
		efxSource [0].Stop ();
	}
	
	// Update is called once per frame
	void Update () {
        kevin = GameManager.instance.infinito ? GameManager.instance.infiniteBoard.kevin : GameManager.instance.boardScript.kevin;
        if (kevin != null && numFontesAtivas > 0)
		{
			for (int i = 0; i < 8; i++)
			{
                int posicaoKevin = (int)kevin.posicaoAtual.y;
				if (posicaoKevin + ALTURA < alturas [i] || posicaoKevin - ALTURA > alturas [i] && fontesAtivas[i])
				{
					efxSource [i].Stop ();
					fontesAtivas [i] = false;
					numFontesAtivas--;
				}
			}
			
		}
	}

	public bool TocandoAltura(int altura){
		for (int i = 0; i < 8; i++)
		{
			int posicaoKevin = (int)kevin.posicaoAtual.y;
			if (alturas[i] == altura && posicaoKevin + ALTURA >= altura && posicaoKevin - ALTURA <= altura && fontesAtivas[i])
			{
				return true;
			}
		}
		return false;
	}
}
