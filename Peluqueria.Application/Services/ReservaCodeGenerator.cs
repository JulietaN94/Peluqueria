using Peluqueria.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Peluqueria.Application.Services
{
    public sealed class ReservaCodeGenerator : IReservaCodeGenerator
    {
        private const string Prefijo = "PEL-";
        private const string Caracteres = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // Excluye caracteres confusos como I, O, 0, 1 para mejorar la usabilidad
        private const int LongitudAleatoria = 12;

        public string GenerarCodigo()
        {
            var codigo = new char[Prefijo.Length + LongitudAleatoria];
            Prefijo.CopyTo(0, codigo, 0, Prefijo.Length);

            for (int i = 0; i < LongitudAleatoria; i++)
            {
                int indice = RandomNumberGenerator.GetInt32(Caracteres.Length);
                codigo[Prefijo.Length + i] = Caracteres[indice];
            }

            return new string(codigo);
        }
    }

}
