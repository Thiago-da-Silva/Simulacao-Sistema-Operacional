using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Generic;
using System.Linq;

namespace Sistema_Operacional.SistemaArquivos
{
    public class GerenciadorArquivos
    {
        private List<Bloco> DiscoVirtual;
        public Arquivo DiretorioRaiz { get; private set; }

        public GerenciadorArquivos(int totalBlocos)
        {
            DiscoVirtual = new List<Bloco>();
            for (int i = 0; i < totalBlocos; i++) DiscoVirtual.Add(new Bloco { Id = i });

            DiretorioRaiz = new Arquivo("ROOT", true);
        }
        public bool CriarArquivo(string nome, Arquivo diretorioPai, string conteudo)
        {
            return true;
        }

        public void ListarDiretorio(Arquivo diretorio)
        {
            if (!diretorio.EhDiretorio) return;
            foreach (var arq in diretorio.Filhos)
            {
                string tipo = arq.EhDiretorio ? "<DIR>" : "<ARQ>";
                System.Console.WriteLine($"{tipo} \t {arq.Nome} \t {arq.TamanhoBytes} bytes");
            }
        }
    }
}