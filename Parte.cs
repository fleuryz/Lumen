using System;

public struct Parte
{
    readonly string nome;
    readonly int saida;

    public Parte(string nome, int saida){
        this.nome = nome;
        this.saida = saida;
    }

    public string GetNome()
    {
        return this.nome;
    }

    public int GetSaida()
    {
        return this.saida;
    }
}
