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

        public Processo(string nome, int id, int prioridade)
        {
            Nome = nome;
            Id = id;
            Prioridade = prioridade;
        }

        public void AdicionarThread()
        {
            int randomMemoria = new Random().Next(1, 3);
            this.Threads.Add(new Thread(randomMemoria, this.Threads.Count + 1)); // Adiciona um randomizador para o valor da memoria
            Console.WriteLine($"Thread adicionada ao processo {this.Nome} (ID: {this.Id}). Total de threads: {this.Threads.Count}, Memoria Utilizada: {randomMemoria}");
        }

        public void ListarThreads()
        {
            foreach (var thread in Threads)
            {
                Console.WriteLine($"Thread ID: {thread.Id}\nMemoria Utilizada: {thread.MemoriaUtilizada}\nEstado: {thread.Estado}\n");
            }
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
                thread.Estado = Enums.Estados.Finalizado;
                this.Threads.Remove(thread);
                Console.WriteLine($"Thread com ID {id} finalizada no processo {this.Nome} (ID: {this.Id}). Total de threads restantes: {this.Threads.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao finalizar a thread: {ex.Message}");
            }
        }

        public void PausarThread(int id)
        {
            try
            {
                Thread thread = this.Threads.FirstOrDefault(t => t.Id == id);
                if (thread == null)
                {
                    Console.WriteLine($"Thread com ID {id} não encontrada no processo {this.Nome} (ID: {this.Id}).");
                    return;
                }
                thread.Estado = Enums.Estados.Bloqueado;
                Console.WriteLine($"Thread com ID {id} pausada no processo {this.Nome} (ID: {this.Id}).");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao pausar a thread: {ex.Message}");
            }
        }

        public void RetomarThread(int id)
        {
            try
            {
                Thread thread = this.Threads.FirstOrDefault(t => t.Id == id);
                if (thread == null)
                {
                    Console.WriteLine($"Thread com ID {id} não encontrada no processo {this.Nome} (ID: {this.Id}).");
                    return;
                }
                if (thread.Estado != Enums.Estados.Bloqueado)
                {
                    Console.WriteLine($"Thread com ID {id} não está pausada no processo {this.Nome} (ID: {this.Id}).");
                    return;
                }
                thread.Estado = Enums.Estados.Executando;
                Console.WriteLine($"Thread com ID {id} retomada no processo {this.Nome} (ID: {this.Id}).");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao retomar a thread: {ex.Message}");
            }
        }
    }
}
