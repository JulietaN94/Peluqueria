using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peluqueria.Application.DTOs.Comun
{
    public class PaginaResultadoDto<T>
    {
        public List<T> Items { get; set; } = new();

        public int TotalRegistros { get; set; }

        public int PaginaActual { get; set; } = 1;

        public int TamanoPagina { get; set; } = 10;

        public int TotalPaginas
        {
            get
            {
                if (TamanoPagina <= 0)
                    return 0;

                return (int)Math.Ceiling(
                    (double)TotalRegistros / TamanoPagina);
            }
        }
    }

}
