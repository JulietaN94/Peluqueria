using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peluqueria.Application.DTOs.Comun
{
    public class PaginaResultadoDto<T>
    {
        public IReadOnlyList<T> Elementos { get; set; } = Array.Empty<T>();
        public int TotalRegistros { get; set; }
        public int Pagina { get; set; }
        public int TamanoPagina { get; set; }
        public int TotalPaginas => TamanoPagina > 0
            ? (int)Math.Ceiling((double)TotalRegistros / TamanoPagina) : 0;
    }
}
