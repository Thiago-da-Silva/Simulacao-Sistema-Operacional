using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sistema_Operacional.Enums;

namespace Sistema_Operacional
{
    public class Processo
    {
        public string Nome { get; set; }
        public int Id { get; set; }
        public int Prioridade { get; set; }
        public List<Thread> Threads { get; set; } = new List<Thread>();
        public Estados Estado { get; set; } = Estados.Criado;
        public DateTime TempoChegada { get; set; }
        //public float MemoriaUtilizada { get; set; } = 0;
        public int TempoDeExecucaoTotal { get; private set; }
        public int TempoExecutado { get; set; } = 0;
        public bool Terminou => TempoExecutado >= TempoDeExecucaoTotal;

        // Contexto da CPU (Registradores + Contador de Programa).
        public RegistradoresContexto ContextoCPU { get; set; }

        // Tabela de arquivos abertos por este processo.
        public List<string> TabelaArquivosAbertos { get; private set; }

        // Tabela de páginas que mapeia memória lógica para física.
        public TabelaPaginas TabelaDePaginas { get; private set; }

        public Processo(string nome, int id, int prioridade)
        {
            Nome = nome;
            Id = id;
            Prioridade = prioridade;
            TempoChegada = DateTime.Now;
            TempoDeExecucaoTotal = new Random().Next(500, 2001);
            ContextoCPU = new RegistradoresContexto();
            TabelaArquivosAbertos = new List<string>();
            TabelaDePaginas = new TabelaPaginas(this.Id);
        }

        public bool AdicionarThread(float memoriaThread)
        {
            try
            {
                var novaThread = new Thread(memoriaThread, this.Threads.Count + 1, this);
                this.Threads.Add(novaThread);

                Console.WriteLine($"Thread adicionada ao processo {this.Nome} (ID: {this.Id}). Total de threads: {this.Threads.Count}, Memoria Lógica da Thread: {memoriaThread}MB");
                Console.WriteLine($"Memoria total (páginas) do processo: {this.TabelaDePaginas.TotalPaginas()} páginas");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao adicionar thread: {ex.Message}");
                return false;
            }
        }

        public void ListarThreads()
        {
            if (Threads.Count == 0)
            {
                Console.WriteLine("Nenhuma thread encontrada.");
                return;
            }

            Console.WriteLine($"Memoria física alocada: {this.TabelaDePaginas.TotalPaginas()} páginas");
            Console.WriteLine($"Memoria lógica total: {CalcularMemoriaTotal():F2}MB");

            Console.WriteLine("Threads:");
            foreach (var thread in Threads)
            {
                Console.WriteLine($"Thread ID: {thread.Id} | Memoria Lógica: {thread.MemoriaUtilizada}MB | Estado: {thread.Estado}");
            }
            Console.WriteLine();
        }

        public Thread FinalizarThread(int id)
        {
            try
            {
                Thread thread = this.Threads.FirstOrDefault(t => t.Id == id);
                if (thread == null)
                {
                    Console.WriteLine($"Thread com ID {id} não encontrada no processo {this.Nome} (ID: {this.Id}).");
                    return null;
                }

                // A memória será liberada pelo SistemaOperacional
                // this.MemoriaUtilizada -= thread.MemoriaUtilizada; // REMOVIDO

                thread.Estado = Enums.Estados.Finalizado;
                this.Threads.Remove(thread);

                Console.WriteLine($"Thread com ID {id} finalizada no processo {this.Nome} (ID: {this.Id}).");
                Console.WriteLine($"Total de threads restantes: {this.Threads.Count}");
                return thread; // Retorna a thread para o SO saber quanta memória liberar
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao finalizar a thread: {ex.Message}");
                return null;
            }
        }

        public float CalcularMemoriaTotal()
        {
            return Threads.Sum(t => t.MemoriaUtilizada);
        }
    }
}