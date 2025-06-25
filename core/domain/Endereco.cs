using System;

namespace API.PDV.Domain
{
    public class Endereco
    {
        public string Rua { get; }
        public string Numero { get; }
        public string Cidade { get; }
        public string Estado { get; }
        public string Cep { get; }

        public Endereco(string rua, string numero, string cidade, string estado, string cep)
        {
            if (string.IsNullOrWhiteSpace(rua)) throw new ArgumentException("Rua obrigatória.");
            if (string.IsNullOrWhiteSpace(numero)) throw new ArgumentException("Número obrigatório.");
            if (string.IsNullOrWhiteSpace(cidade)) throw new ArgumentException("Cidade obrigatória.");
            if (string.IsNullOrWhiteSpace(estado)) throw new ArgumentException("Estado obrigatório.");
            if (string.IsNullOrWhiteSpace(cep)) throw new ArgumentException("CEP obrigatório.");

            Rua = rua;
            Numero = numero;
            Cidade = cidade;
            Estado = estado;
            Cep = cep;
        }

        public override string ToString() => $"{Rua}, {Numero} - {Cidade}/{Estado}, {Cep}";
    }
}
