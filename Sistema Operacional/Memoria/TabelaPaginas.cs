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

        // Mapeia (Página Lógica, Índice da Moldura Física)
        private Dictionary<int, int> Mapeamento { get; set; }
        private int contadorPaginasLogicas = 0;

        public TabelaPaginas(int processoId)
        {
            ProcessoId = processoId;
            Mapeamento = new Dictionary<int, int>();
        }
        public List<int> RegistrarAlocacao(List<int> indicesFrames)
        {
            List<int> paginasLogicasCriadas = new List<int>();

            foreach (var frameIndex in indicesFrames)
            {
                // Simplesmente usa o contador incremental para gerar novos IDs lógicos
                int idLogico = contadorPaginasLogicas++;
                Mapeamento.Add(idLogico, frameIndex);
                paginasLogicasCriadas.Add(idLogico);
            }

            return paginasLogicasCriadas;
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