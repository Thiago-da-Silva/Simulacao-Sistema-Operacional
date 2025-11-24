using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_Operacional.SistemaArquivos
{
    public class Bloco
    {
        public int Id { get; set; }
        public string Dados { get; set; }
        public int ProximoBloco { get; set; } = -1; // Para alocação encadeada
        public bool Ocupado { get; set; } = false;
    }
}