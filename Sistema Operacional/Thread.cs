using Sistema_Operacional.Enums;
using System;
using System.Collections.Generic; // Necessário para Stack
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_Operacional
{
    public class Thread
    {
        public float MemoriaUtilizada { get; set; }
        public Estados Estado { get; set; } = Estados.Criado;
        public int Id { get; set; }

        // --- INÍCIO DOS CAMPOS DO TCB (Req 3.2) ---

        /// <summary>
        /// Referência ao processo pai ao qual esta thread pertence.
        /// </summary>
        public Processo ProcessoPai { get; private set; }

        /// <summary>
        /// Pilha lógica simulada para a thread (ex: para chamadas de função).
        /// </summary>
        public Stack<string> PilhaLogica { get; private set; }

        // --- FIM DOS CAMPOS DO TCB ---

        public Thread(float memoriaUtilizada, int id, Processo processoPai)
        {
            MemoriaUtilizada = memoriaUtilizada;
            Id = id;

            // Inicializa os campos do TCB
            ProcessoPai = processoPai;
            PilhaLogica = new Stack<string>();
        }

        public void PausarThread()
        {
            if (this.Estado == Estados.Executando || this.Estado == Estados.Pronto)
            {
                this.Estado = Estados.Bloqueado;
                Console.WriteLine($"Thread com ID {this.Id} pausada.");
            }
            else
            {
                Console.WriteLine($"Thread com ID {this.Id} não está em um estado que permite pausa.");
            }
        }

        public void RetomarThread()
        {
            if (this.Estado == Estados.Bloqueado)
            {
                this.Estado = Estados.Pronto;
                Console.WriteLine($"Thread com ID {this.Id} retomada.");
            }
            else
            {
                Console.WriteLine($"Thread com ID {this.Id} não está em um estado que permite retomada.");
            }
        }
    }
}