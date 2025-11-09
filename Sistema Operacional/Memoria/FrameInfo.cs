using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_Operacional.Memoria
{
    // Informação sobre uma Moldura de Página (Page Frame) na memória física
    public class FrameInfo
    {
        public bool Ocupado { get; set; } = false;
        public int ProcessoId { get; set; } = -1;
        public int PaginaLogicaId { get; set; } = -1; // A qual página lógica esse frame está mapeado

        public void Alocar(int processoId, int paginaLogicaId)
        {
            Ocupado = true;
            ProcessoId = processoId;
            PaginaLogicaId = paginaLogicaId;
        }

        public void Liberar()
        {
            Ocupado = false;
            ProcessoId = -1;
            PaginaLogicaId = -1;
        }
    }
}
