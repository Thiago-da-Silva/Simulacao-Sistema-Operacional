using System;
using System.Collections.Generic;
using System.Linq;

namespace Sistema_Operacional.Memoria
{
    public class TLB
    {
        private Dictionary<int, int> Cache { get; set; }
        private int CapacidadeMaxima { get; set; }
        private Queue<int> OrdemAcesso { get; set; }

        public int TotalHits { get; private set; } = 0;
        public int TotalMisses { get; private set; } = 0;

        public TLB(int capacidade = 16)
        {
            CapacidadeMaxima = capacidade;
            Cache = new Dictionary<int, int>();
            OrdemAcesso = new Queue<int>();
        }

        public bool TentarObter(int paginaLogica, out int frameFisico)
        {
            if (Cache.ContainsKey(paginaLogica))
            {
                TotalHits++;
                frameFisico = Cache[paginaLogica];
                return true;
            }

            TotalMisses++;
            frameFisico = -1;
            return false;
        }

        public void Adicionar(int paginaLogica, int frameFisico)
        {
            if (Cache.ContainsKey(paginaLogica))
            {
                Cache[paginaLogica] = frameFisico;
                return;
            }

            if (Cache.Count >= CapacidadeMaxima)
            {
                int paginaRemover = OrdemAcesso.Dequeue();
                Cache.Remove(paginaRemover);
            }

            Cache[paginaLogica] = frameFisico;
            OrdemAcesso.Enqueue(paginaLogica);
        }

        public void Remover(int paginaLogica)
        {
            if (Cache.ContainsKey(paginaLogica))
            {
                Cache.Remove(paginaLogica);
            }
        }

        public void Limpar()
        {
            Cache.Clear();
            OrdemAcesso.Clear();
        }

        public double CalcularTaxaAcerto()
        {
            int total = TotalHits + TotalMisses;
            if (total == 0) return 0.0;
            return (double)TotalHits / total * 100.0;
        }

        public void MostrarEstatisticas()
        {
            int total = TotalHits + TotalMisses;
            double taxaAcerto = CalcularTaxaAcerto();

            Console.WriteLine("=== ESTATÍSTICAS DA TLB ===");
            Console.WriteLine($"Capacidade: {CapacidadeMaxima} entradas");
            Console.WriteLine($"Entradas em uso: {Cache.Count}");
            Console.WriteLine($"Total de acessos: {total}");
            Console.WriteLine($"Hits: {TotalHits}");
            Console.WriteLine($"Misses: {TotalMisses}");
            Console.WriteLine($"Taxa de acerto: {taxaAcerto:F2}%");
            Console.WriteLine();
        }
    }
}
