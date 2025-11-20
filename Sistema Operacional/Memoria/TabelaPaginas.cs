using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_Operacional.Memoria
{
    public class TabelaPaginas
    {
        public int ProcessoId { get; private set; }

        private Dictionary<int, int> Mapeamento { get; set; }
        private int contadorPaginasLogicas = 0;

        private TLB TLB { get; set; }

        public int TotalAcessos { get; private set; } = 0;
        public int TotalPageFaults { get; private set; } = 0;

        public TabelaPaginas(int processoId, int capacidadeTLB = 16)
        {
            ProcessoId = processoId;
            Mapeamento = new Dictionary<int, int>();
            TLB = new TLB(capacidadeTLB);
        }

        public List<int> RegistrarAlocacao(List<int> indicesFrames)
        {
            List<int> paginasLogicasCriadas = new List<int>();

            foreach (var frameIndex in indicesFrames)
            {
                int idLogico = contadorPaginasLogicas++;
                Mapeamento.Add(idLogico, frameIndex);
                paginasLogicasCriadas.Add(idLogico);

                TLB.Adicionar(idLogico, frameIndex);
            }

            return paginasLogicasCriadas;
        }

        public int TraduzirEndereco(int paginaLogica)
        {
            TotalAcessos++;

            if (TLB.TentarObter(paginaLogica, out int frameFisico))
            {
                return frameFisico;
            }

            if (Mapeamento.ContainsKey(paginaLogica))
            {
                frameFisico = Mapeamento[paginaLogica];
                TLB.Adicionar(paginaLogica, frameFisico);
                return frameFisico;
            }

            TotalPageFaults++;
            return -1;
        }

        public List<int> LiberarPaginasEspecificas(List<int> paginasLogicas)
        {
            var framesLiberados = new List<int>();

            foreach (int paginaLogica in paginasLogicas)
            {
                if (Mapeamento.ContainsKey(paginaLogica))
                {
                    framesLiberados.Add(Mapeamento[paginaLogica]);
                    Mapeamento.Remove(paginaLogica);
                    TLB.Remover(paginaLogica);
                }
            }
            return framesLiberados;
        }

        public List<int> ObterTodosFrames()
        {
            return Mapeamento.Values.ToList();
        }

        public int TotalPaginas()
        {
            return Mapeamento.Count;
        }

        public void MostrarEstatisticas()
        {
            Console.WriteLine($"=== ESTATÍSTICAS DE MEMÓRIA - Processo {ProcessoId} ===");
            Console.WriteLine($"Total de páginas alocadas: {TotalPaginas()}");
            Console.WriteLine($"Total de acessos à memória: {TotalAcessos}");
            Console.WriteLine($"Total de Page Faults: {TotalPageFaults}");
            
            if (TotalAcessos > 0)
            {
                double taxaPageFault = (double)TotalPageFaults / TotalAcessos * 100.0;
                Console.WriteLine($"Taxa de Page Fault: {taxaPageFault:F2}%");
            }

            Console.WriteLine();
            TLB.MostrarEstatisticas();
        }

        public TLB GetTLB()
        {
            return TLB;
        }
    }
}