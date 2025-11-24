using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sistema_Operacional.Enums;
using Sistema_Operacional.Memoria;
using Sistema_Operacional.Utilidades;

namespace Sistema_Operacional.Modelos
{
    public class Processo
    {
        public string Nome { get; set; }
        public int Id { get; set; }
        public int Prioridade { get; set; }
        public List<Thread> Threads { get; set; } = new List<Thread>();
        public Estados Estado { get; set; } = Estados.Criado;
        public DateTime TempoChegada { get; set; }
        
        private int tempoDeExecucaoTotal;
        public int TempoDeExecucaoTotal 
        { 
            get => tempoDeExecucaoTotal;
            private set => tempoDeExecucaoTotal = value;
        }
        
        public int TempoExecutado { get; set; } = 0;
        public bool Terminou => TempoExecutado >= TempoDeExecucaoTotal;

        public DateTime? TempoPrimeiraExecucao { get; set; } = null;
        public DateTime? TempoFinalizacao { get; set; } = null;
        public TimeSpan TempoDeEspera { get; set; } = TimeSpan.Zero;

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
            tempoDeExecucaoTotal = AleatorioSistema.Next(500, 2001);
            ContextoCPU = new RegistradoresContexto();
            TabelaArquivosAbertos = new List<string>();
            TabelaDePaginas = new TabelaPaginas(this.Id);
        }

        public bool AdicionarThread(float memoriaThread, List<int> paginasLogicas)
        {
            try
            {
                var novaThread = new Thread(memoriaThread, this.Threads.Count + 1, this);

                // Associa as páginas alocadas à thread
                if (paginasLogicas != null)
                {
                    novaThread.PaginasLogicasAlocadas = paginasLogicas;
                }

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
