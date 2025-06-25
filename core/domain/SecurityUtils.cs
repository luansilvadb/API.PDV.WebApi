using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace API.PDV.Domain
{
    public static class SecurityUtils
    {
        // Chave e IV fixos para exemplo. Em produção, use gerenciamento seguro!
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("pdvkey1234567890"); // 16 bytes
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("pdvinitvector123"); // 16 bytes

        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return plainText;
            using var aes = Aes.Create();
            aes.Key = Key;
            aes.IV = IV;
            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
                sw.Write(plainText);
            return Convert.ToBase64String(ms.ToArray());
        }

        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return cipherText;
            using var aes = Aes.Create();
            aes.Key = Key;
            aes.IV = IV;
            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream(Convert.FromBase64String(cipherText));
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }

        public static string MaskCpfCnpj(string valor)
        {
            if (string.IsNullOrEmpty(valor)) return valor;
            valor = Regex.Replace(valor, @"\D", "");
            if (valor.Length == 11)
                return valor.Substring(0, 3) + ".***.***-" + valor.Substring(9, 2);
            if (valor.Length == 14)
                return valor.Substring(0, 2) + ".***.***/****-" + valor.Substring(12, 2);
            return valor;
        }

        public static string MaskEndereco(string endereco)
        {
            if (string.IsNullOrEmpty(endereco)) return endereco;
            // Exibe só cidade/estado/cep
            var match = Regex.Match(endereco, @"- ([^,]+), (\w+)$");
            return match.Success ? "***, " + match.Groups[1].Value : "***";
        }

        public static string SanitizeLogMessage(string message)
        {
            if (string.IsNullOrEmpty(message)) return message;
            // Mascara padrões de CPF/CNPJ
            message = Regex.Replace(message, @"\b\d{3}\.?\d{3}\.?\d{3}-?\d{2}\b", m => MaskCpfCnpj(m.Value));
            message = Regex.Replace(message, @"\b\d{2}\.?\d{3}\.?\d{3}/?\d{4}-?\d{2}\b", m => MaskCpfCnpj(m.Value));
            // Mascara endereços (simples)
            message = Regex.Replace(message, @"\d{1,4}\s+\w+,\s*\w+/\w+,\s*\d{5}-?\d{3}", "***");
            return message;
        }
    }
}