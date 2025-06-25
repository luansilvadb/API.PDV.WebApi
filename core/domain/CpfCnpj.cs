using System;
using System.Text.RegularExpressions;

namespace API.PDV.Domain
{
    public class CpfCnpj
    {
        private string _encryptedValue;

        public string Valor
        {
            get => SecurityUtils.Decrypt(_encryptedValue);
        }

        public CpfCnpj(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new ArgumentException("CPF/CNPJ não pode ser vazio.");

            valor = valor.Replace(".", "").Replace("-", "").Replace("/", "");

            if (valor.Length == 11 && Regex.IsMatch(valor, @"^\d{11}$"))
            {
                _encryptedValue = SecurityUtils.Encrypt(valor);
            }
            else if (valor.Length == 14 && Regex.IsMatch(valor, @"^\d{14}$"))
            {
                _encryptedValue = SecurityUtils.Encrypt(valor);
            }
            else
            {
                throw new ArgumentException("CPF/CNPJ inválido.");
            }
        }

        // Construtor para ORM/deserialização
        public CpfCnpj(string encryptedValue, bool isEncrypted)
        {
            _encryptedValue = isEncrypted ? encryptedValue : SecurityUtils.Encrypt(encryptedValue);
        }

        public string GetEncrypted() => _encryptedValue;

        public override string ToString() => Valor;
    }
}
