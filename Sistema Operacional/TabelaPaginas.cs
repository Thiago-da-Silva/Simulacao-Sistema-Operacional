using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_Operacional
{
    public class TabelaPaginas
    {
        public int ProcessoId { get; private set; }

        // Mapeia (Página Lógica, Índice da Moldura Física)
        private Dictionary<int, int> Mapeamento { get; set; }
        private int proximaPaginaLogica = 0;

        public TabelaPaginas(int processoId)
        {
            ProcessoId = processoId;
            Mapeamento = new Dictionary<int, int>();
        }

        // Registra novos frames alocados para este processo
        public void RegistrarAlocacao(List<int> indicesFrames)
        {
            foreach (var frameIndex in indicesFrames)
            {
                Mapeamento.Add(proximaPaginaLogica, frameIndex);
                proximaPaginaLogica++;
            }
        }

        // Libera as 'N' últimas páginas alocadas (útil ao finalizar threads)
        public List<int> LiberarPaginasRecentes(int quantidade)
        {
            var framesLiberados = new List<int>();
            if (quantidade > Mapeamento.Count)
                quantidade = Mapeamento.Count;

            // Libera da última página para a primeira (LIFO)
            for (int i = 0; i < quantidade; i++)
            {
                int paginaParaRemover = proximaPaginaLogica - 1;
                if (Mapeamento.ContainsKey(paginaParaRemover))
                {
                    framesLiberados.Add(Mapeamento[paginaParaRemover]);
                    Mapeamento.Remove(paginaParaRemover);
                    proximaPaginaLogica--;
                }
            }
            return framesLiberados;
        }

        // Obtém todos os frames usados por este processo (para finalizar)
        public List<int> ObterTodosFrames()
        {
            return Mapeamento.Values.ToList();
        }

        public int TotalPaginas()
        {
            return Mapeamento.Count;
        }
    }
}