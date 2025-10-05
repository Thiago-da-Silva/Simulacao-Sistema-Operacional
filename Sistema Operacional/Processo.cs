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
        public string Nome {  get; set; }
        public int Id { get; set; }
        public int Prioridade {  get; set; }
        public List<Thread> Threads { get; set; } = new List<Thread>();
        public Estados Estado { get; set; } = Estados.Criado;
        public DateTime TempoChegada { get; set; }
        public float MemoriaUtilizada { get; set; } = 0;
        public int TempoDeExecucaoTotal { get; private set; } // Em unidade de tempo (ex: ms)
        public int TempoExecutado { get; set; } = 0;
        public bool Terminou => TempoExecutado >= TempoDeExecucaoTotal;

        public Processo(string nome, int id, int prioridade)
        {
            Nome = nome;
            Id = id;
            Prioridade = prioridade;
            TempoChegada = DateTime.Now;
            // Simula um "burst time" aleatório para o processo.
            TempoDeExecucaoTotal = new Random().Next(500, 2001);
        }

        public bool AdicionarThread(float memoriaThread)
        {
            try
            {
                var novaThread = new Thread(memoriaThread, this.Threads.Count + 1);
                this.Threads.Add(novaThread);
                this.MemoriaUtilizada += memoriaThread;
                
                Console.WriteLine($"Thread adicionada ao processo {this.Nome} (ID: {this.Id}). Total de threads: {this.Threads.Count}, Memoria da Thread: {memoriaThread}MB");
                Console.WriteLine($"Memoria total do processo: {this.MemoriaUtilizada}MB");
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

            Console.WriteLine($"Memoria total do processo: {MemoriaUtilizada}MB");
            Console.WriteLine("Threads:");
            foreach (var thread in Threads)
            {
                Console.WriteLine($"Thread ID: {thread.Id} | Memoria: {thread.MemoriaUtilizada}MB | Estado: {thread.Estado}");
            }
            Console.WriteLine();
        }

        public void FinalizarThread(int id)
        {
            try
            {
                Thread thread = this.Threads.FirstOrDefault(t => t.Id == id);
                if (thread == null)
                {
                    Console.WriteLine($"Thread com ID {id} não encontrada no processo {this.Nome} (ID: {this.Id}).");
                    return;
                }
                
                // Remove a memória da thread do total do processo
                this.MemoriaUtilizada -= thread.MemoriaUtilizada;
                
                thread.Estado = Enums.Estados.Finalizado;
                this.Threads.Remove(thread);
                
                Console.WriteLine($"Thread com ID {id} finalizada no processo {this.Nome} (ID: {this.Id}). Memoria liberada: {thread.MemoriaUtilizada}MB");
                Console.WriteLine($"Total de threads restantes: {this.Threads.Count} | Memoria total do processo: {this.MemoriaUtilizada}MB");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao finalizar a thread: {ex.Message}");
            }
        }

        public float CalcularMemoriaTotal()
        {
            return Threads.Sum(t => t.MemoriaUtilizada);
        }

        //public void PausarThread(int id)
        //{
        //    try
        //    {
        //        Thread thread = this.Threads.FirstOrDefault(t => t.Id == id);
        //        if (thread == null)
        //        {
        //            Console.WriteLine($"Thread com ID {id} não encontrada no processo {this.Nome} (ID: {this.Id}).");
        //            return;
        //        }
        //        thread.Estado = Enums.Estados.Bloqueado;
        //        Console.WriteLine($"Thread com ID {id} pausada no processo {this.Nome} (ID: {this.Id}).");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Erro ao pausar a thread: {ex.Message}");
        //    }
        //}

        //public void RetomarThread(int id)
        //{
        //    try
        //    {
        //        Thread thread = this.Threads.FirstOrDefault(t => t.Id == id);
        //        if (thread == null)
        //        {
        //            Console.WriteLine($"Thread com ID {id} não encontrada no processo {this.Nome} (ID: {this.Id}).");
        //            return;
        //        }
        //        if (thread.Estado != Enums.Estados.Bloqueado)
        //        {
        //            Console.WriteLine($"Thread com ID {id} não está pausada no processo {this.Nome} (ID: {this.Id}).");
        //            return;
        //        }
        //        thread.Estado = Enums.Estados.Executando;
        //        Console.WriteLine($"Thread com ID {id} retomada no processo {this.Nome} (ID: {this.Id}).");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Erro ao retomar a thread: {ex.Message}");
        //    }
        //}
    }
}
