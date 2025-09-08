using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_Operacional.Enums
{
    public enum Estados
    {
        Criado, // Processo recém-criado, ainda não está pronto para execução.
        Finalizado, // Processo terminou sua execução e liberou os recursos.
        Pronto, // Processo está carregado na memória, aguardando a CPU.
        Bloqueado, // Processo está bloqueado esperando algum evento (ex: I/O).
        Executando, // Processo em execução, usando a CPU.
        ProntoSuspenso, // Processo pronto, mas suspenso pelo sistema (ex: falta de memória).
        EsperaSuspensa, // Processo bloqueado, mas também suspenso (aguarda evento e retomada).

    }
}
