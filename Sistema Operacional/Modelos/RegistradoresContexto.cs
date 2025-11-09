using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_Operacional.Modelos
{
    // Simula o contexto de hardware da CPU, como registradores e o Program Counter (PC).
    public class RegistradoresContexto
    {
        // Registradores de propósito geral simulados
        public int AX { get; set; }
        public int BX { get; set; }
        public int CX { get; set; }
        public int DX { get; set; }

        // Contador de Programa (Program Counter) Lógico. Simula qual "linha" do programa o processo está executando.
        public int ContadorDePrograma { get; set; }

        public RegistradoresContexto()
        {
            AX = 0;
            BX = 0;
            CX = 0;
            DX = 0;
            ContadorDePrograma = 0;
        }
    }
}
