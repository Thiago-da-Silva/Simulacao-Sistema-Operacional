using Sistema_Operacional.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_Operacional.Escalonamento
{
    public interface IEscalonador
    {
        int QuantidadeProcessosNaFila { get; }

        void AdicionarProcesso(Processo processo);

        Processo ObterProximoProcesso();
        bool RemoverProcessoDaFila(int processoId);
        void ExibirInformacoesFila();
    }
}
