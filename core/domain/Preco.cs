using System;
using System.Globalization;

namespace API.PDV.Domain
{
    public class Preco
    {
        public decimal Valor { get; }

        public Preco(decimal valor)
        {
            if (valor < 0)
                throw new ArgumentException("Preço não pode ser negativo.");

            Valor = Math.Round(valor, 2, MidpointRounding.AwayFromZero);
        }

        public override string ToString() => Valor.ToString("C", CultureInfo.CurrentCulture);
    }
}
