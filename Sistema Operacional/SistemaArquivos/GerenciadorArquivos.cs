using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sistema_Operacional.Utilidades;

namespace Sistema_Operacional.SistemaArquivos
{
    public class GerenciadorArquivos
    {
        private List<Bloco> DiscoVirtual;
        public Arquivo DiretorioRaiz { get; private set; }
        private int TotalBlocos => DiscoVirtual.Count;

        // Cada bloco armazena até 512 bytes de dados
        private const int TamanhoBloco = 512;

        public GerenciadorArquivos(int totalBlocos)
        {
            DiscoVirtual = new List<Bloco>();
            for (int i = 0; i < totalBlocos; i++)
                DiscoVirtual.Add(new Bloco { Id = i });

            DiretorioRaiz = new Arquivo("ROOT", true);
            Console.WriteLine($"Sistema de Arquivos iniciado: {totalBlocos} blocos x {TamanhoBloco} bytes = {totalBlocos * TamanhoBloco / 1024} KB de disco virtual.");
        }

        // Cria um novo arquivo no diretório pai com o conteúdo fornecido
        public bool CriarArquivo(string nome, Arquivo diretorioPai, string conteudo)
        {
            if (diretorioPai == null || !diretorioPai.EhDiretorio)
            {
                Console.WriteLine("Erro: Diretório pai inválido.");
                return false;
            }

            if (diretorioPai.Filhos.Any(f => f.Nome == nome && !f.EhDiretorio))
            {
                Console.WriteLine($"Erro: Arquivo '{nome}' já existe neste diretório.");
                return false;
            }

            string dados = conteudo ?? string.Empty;
            int tamanhoBytes = Encoding.UTF8.GetByteCount(dados);
            int blocosNecessarios = Math.Max(1, (int)Math.Ceiling((double)tamanhoBytes / TamanhoBloco));

            List<int> blocosAlocados = AlocarBlocos(blocosNecessarios);
            if (blocosAlocados == null)
            {
                Console.WriteLine($"Erro: Espaço insuficiente no disco. Necessário: {blocosNecessarios} blocos, Disponível: {GetBlocosLivres()} blocos.");
                return false;
            }

            EscreverConteudoNoBlocos(blocosAlocados, dados);

            string extensao = nome.Contains('.') ? nome.Substring(nome.LastIndexOf('.') + 1) : "";
            var arquivo = new Arquivo(nome)
            {
                Extensao = extensao,
                TamanhoBytes = tamanhoBytes,
                BlocoInicial = blocosAlocados[0],
                DiretorioPai = diretorioPai,
                EhDiretorio = false
            };

            diretorioPai.Filhos.Add(arquivo);
            Logger.Registrar($"ARQUIVO CRIADO: '{nome}' em '{diretorioPai.Nome}' ({tamanhoBytes} bytes, {blocosNecessarios} blocos alocados)");
            Console.WriteLine($"Arquivo '{nome}' criado com sucesso ({tamanhoBytes} bytes, {blocosNecessarios} bloco(s) alocado(s)).");
            return true;
        }

        // Cria um subdiretório dentro do diretório pai
        public bool CriarDiretorio(string nome, Arquivo diretorioPai)
        {
            if (diretorioPai == null || !diretorioPai.EhDiretorio)
            {
                Console.WriteLine("Erro: Diretório pai inválido.");
                return false;
            }

            if (diretorioPai.Filhos.Any(f => f.Nome == nome && f.EhDiretorio))
            {
                Console.WriteLine($"Erro: Diretório '{nome}' já existe.");
                return false;
            }

            var dir = new Arquivo(nome, true) { DiretorioPai = diretorioPai };
            diretorioPai.Filhos.Add(dir);
            Logger.Registrar($"DIRETÓRIO CRIADO: '{nome}' em '{diretorioPai.Nome}'");
            Console.WriteLine($"Diretório '{nome}' criado com sucesso.");
            return true;
        }

        // Lê e retorna o conteúdo de um arquivo
        public string LerArquivo(string nome, Arquivo diretorioPai)
        {
            var arquivo = BuscarArquivo(nome, diretorioPai);
            if (arquivo == null)
            {
                Console.WriteLine($"Erro: Arquivo '{nome}' não encontrado.");
                return null;
            }
            if (arquivo.EhDiretorio)
            {
                Console.WriteLine($"Erro: '{nome}' é um diretório, não um arquivo.");
                return null;
            }

            string conteudo = LerConteudoDosBlocos(arquivo.BlocoInicial);
            Logger.Registrar($"ARQUIVO LIDO: '{nome}' ({arquivo.TamanhoBytes} bytes)");
            return conteudo;
        }

        // Sobrescreve o conteúdo de um arquivo existente
        public bool EscreverArquivo(string nome, Arquivo diretorioPai, string novoConteudo)
        {
            var arquivo = BuscarArquivo(nome, diretorioPai);
            if (arquivo == null)
            {
                Console.WriteLine($"Erro: Arquivo '{nome}' não encontrado.");
                return false;
            }
            if (arquivo.EhDiretorio)
            {
                Console.WriteLine($"Erro: '{nome}' é um diretório.");
                return false;
            }

            // Libera os blocos antigos antes de realocar
            LiberarCadeiaBlocos(arquivo.BlocoInicial);

            string dados = novoConteudo ?? string.Empty;
            int tamanhoBytes = Encoding.UTF8.GetByteCount(dados);
            int blocosNecessarios = Math.Max(1, (int)Math.Ceiling((double)tamanhoBytes / TamanhoBloco));

            List<int> novos = AlocarBlocos(blocosNecessarios);
            if (novos == null)
            {
                Console.WriteLine($"Erro: Espaço insuficiente para escrita ({blocosNecessarios} blocos necessários).");
                arquivo.TamanhoBytes = 0;
                arquivo.BlocoInicial = -1;
                return false;
            }

            EscreverConteudoNoBlocos(novos, dados);
            arquivo.BlocoInicial = novos[0];
            arquivo.TamanhoBytes = tamanhoBytes;

            Logger.Registrar($"ARQUIVO ATUALIZADO: '{nome}' ({tamanhoBytes} bytes, {blocosNecessarios} blocos)");
            Console.WriteLine($"Arquivo '{nome}' atualizado com sucesso ({tamanhoBytes} bytes).");
            return true;
        }

        // Remove um arquivo ou diretório vazio
        public bool DeletarArquivo(string nome, Arquivo diretorioPai)
        {
            var arquivo = BuscarArquivo(nome, diretorioPai);
            if (arquivo == null)
            {
                Console.WriteLine($"Erro: '{nome}' não encontrado neste diretório.");
                return false;
            }

            if (arquivo.EhDiretorio && arquivo.Filhos.Count > 0)
            {
                Console.WriteLine($"Erro: Diretório '{nome}' não está vazio. Delete os arquivos internos primeiro.");
                return false;
            }

            if (!arquivo.EhDiretorio && arquivo.BlocoInicial >= 0)
                LiberarCadeiaBlocos(arquivo.BlocoInicial);

            diretorioPai.Filhos.Remove(arquivo);
            Logger.Registrar($"DELETADO: '{nome}' de '{diretorioPai.Nome}'");
            Console.WriteLine($"'{nome}' deletado com sucesso.");
            return true;
        }

        // Lista o conteúdo de um diretório
        public void ListarDiretorio(Arquivo diretorio)
        {
            if (diretorio == null || !diretorio.EhDiretorio)
            {
                Console.WriteLine("Erro: Não é um diretório válido.");
                return;
            }

            Console.WriteLine($"\n=== Conteúdo de '{diretorio.Nome}' ===");
            if (diretorio.Filhos.Count == 0)
            {
                Console.WriteLine("(diretório vazio)");
                Console.WriteLine();
                return;
            }

            Console.WriteLine($"{"Tipo",-7} {"Nome",-22} {"Tamanho",-14} {"Bloco Inicial"}");
            Console.WriteLine(new string('-', 60));

            foreach (var item in diretorio.Filhos.OrderBy(f => !f.EhDiretorio).ThenBy(f => f.Nome))
            {
                string tipo = item.EhDiretorio ? "<DIR>" : "<ARQ>";
                string tamanho = item.EhDiretorio ? "-" : $"{item.TamanhoBytes} bytes";
                string bloco = item.EhDiretorio ? "-" : $"#{item.BlocoInicial}";
                Console.WriteLine($"{tipo,-7} {item.Nome,-22} {tamanho,-14} {bloco}");
            }
            Console.WriteLine();
        }

        // Exibe estatísticas do disco virtual
        public void MostrarStatusDisco()
        {
            int ocupados = DiscoVirtual.Count(b => b.Ocupado);
            int livres = TotalBlocos - ocupados;
            float percentual = TotalBlocos > 0 ? (float)ocupados / TotalBlocos * 100 : 0;

            Console.WriteLine("=== STATUS DO DISCO VIRTUAL ===");
            Console.WriteLine($"Total de blocos : {TotalBlocos} ({TamanhoBloco} bytes cada)");
            Console.WriteLine($"Blocos ocupados : {ocupados} ({percentual:F1}%)");
            Console.WriteLine($"Blocos livres   : {livres}");
            Console.WriteLine($"Capacidade total: {(long)TotalBlocos * TamanhoBloco / 1024.0:F1} KB");
            Console.WriteLine($"Espaço usado    : {(long)ocupados * TamanhoBloco / 1024.0:F1} KB");
            Console.WriteLine($"Espaço livre    : {(long)livres * TamanhoBloco / 1024.0:F1} KB");
            Console.WriteLine();
        }

        // Encontra arquivo pelo nome no diretório informado
        public Arquivo BuscarArquivo(string nome, Arquivo diretorioPai)
        {
            return diretorioPai?.Filhos.FirstOrDefault(f => f.Nome == nome);
        }

        public int GetBlocosLivres() => DiscoVirtual.Count(b => !b.Ocupado);
        public int GetBlocosOcupados() => DiscoVirtual.Count(b => b.Ocupado);

        // --- Métodos privados ---

        private List<int> AlocarBlocos(int quantidade)
        {
            var livres = DiscoVirtual.Where(b => !b.Ocupado).Take(quantidade).ToList();
            if (livres.Count < quantidade) return null;

            foreach (var b in livres) b.Ocupado = true;
            return livres.Select(b => b.Id).ToList();
        }

        private void EscreverConteudoNoBlocos(List<int> blocos, string conteudo)
        {
            for (int i = 0; i < blocos.Count; i++)
            {
                int inicio = i * TamanhoBloco;
                int fim = Math.Min(inicio + TamanhoBloco, conteudo.Length);

                DiscoVirtual[blocos[i]].Dados = inicio < conteudo.Length
                    ? conteudo.Substring(inicio, fim - inicio)
                    : string.Empty;

                DiscoVirtual[blocos[i]].ProximoBloco = (i < blocos.Count - 1) ? blocos[i + 1] : -1;
            }
        }

        private string LerConteudoDosBlocos(int blocoInicial)
        {
            if (blocoInicial < 0 || blocoInicial >= TotalBlocos) return string.Empty;

            var sb = new StringBuilder();
            int atual = blocoInicial;
            int seguranca = 0;

            while (atual != -1 && seguranca++ < TotalBlocos)
            {
                sb.Append(DiscoVirtual[atual].Dados ?? string.Empty);
                atual = DiscoVirtual[atual].ProximoBloco;
            }

            return sb.ToString();
        }

        private void LiberarCadeiaBlocos(int blocoInicial)
        {
            if (blocoInicial < 0 || blocoInicial >= TotalBlocos) return;

            int atual = blocoInicial;
            int seguranca = 0;

            while (atual != -1 && seguranca++ < TotalBlocos)
            {
                int prox = DiscoVirtual[atual].ProximoBloco;
                DiscoVirtual[atual].Ocupado = false;
                DiscoVirtual[atual].Dados = null;
                DiscoVirtual[atual].ProximoBloco = -1;
                atual = prox;
            }
        }
    }
}
