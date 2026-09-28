using appExemploWeb.Configs;
using AppWebExemplo.Model;
using System.Data;

namespace appExemploWeb.DAO
{
    public class ProcessoDAO
    {
        private readonly Conexao _conexao;

        public ProcessoDAO(Conexao conexao)
        {
            _conexao = conexao;
        }

        public List<Processo> Listar()
        {
            var lista = new List<Processo>();

            using var con = _conexao.GetConnection();
            if (con.State != ConnectionState.Open)
            {
                con.Open();
            }

            string sql = @"SELECT id_pro, numero_pro, data_pro, interessado_pro, 
                                  assunto_pro, descricao_pro, situacao_pro 
                           FROM processos";

            using var cmd = con.CreateCommand();
            cmd.CommandText = sql;

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var processo = new Processo();

                // Leitura do ID (Geralmente nunca é nulo)
                processo.Id = reader.GetInt32(reader.GetOrdinal("id_pro"));

                // Leitura de Strings tratando NULO
                processo.Numero = reader.IsDBNull(reader.GetOrdinal("numero_pro"))
                    ? string.Empty
                    : reader.GetString(reader.GetOrdinal("numero_pro"));

                processo.Interessado = reader.IsDBNull(reader.GetOrdinal("interessado_pro"))
                    ? string.Empty
                    : reader.GetString(reader.GetOrdinal("interessado_pro"));

                processo.Assunto = reader.IsDBNull(reader.GetOrdinal("assunto_pro"))
                    ? string.Empty
                    : reader.GetString(reader.GetOrdinal("assunto_pro"));

                processo.Descricao = reader.IsDBNull(reader.GetOrdinal("descricao_pro"))
                    ? string.Empty
                    : reader.GetString(reader.GetOrdinal("descricao_pro"));

                processo.Situacao = reader.IsDBNull(reader.GetOrdinal("situacao_pro"))
                    ? "Aberto"
                    : reader.GetString(reader.GetOrdinal("situacao_pro"));

                // Leitura de Data tratando NULO
                if (!reader.IsDBNull(reader.GetOrdinal("data_pro")))
                {
                    DateTime dataBanco = reader.GetDateTime(reader.GetOrdinal("data_pro"));
                    processo.Data = DateOnly.FromDateTime(dataBanco);
                }
                else
                {
                    processo.Data = null;
                }

                lista.Add(processo);
            }

            return lista;
        }

        public void Inserir(Processo processo)
        {
            using var con = _conexao.GetConnection();
            if (con.State != ConnectionState.Open)
            {
                con.Open();
            }

            string sql = @"INSERT INTO processos
                (numero_pro, data_pro, interessado_pro, assunto_pro, descricao_pro, situacao_pro)
                VALUES
                (@numero, @data, @interessado, @assunto, @descricao, @situacao)";

            using var comando = con.CreateCommand();
            comando.CommandText = sql;

            comando.Parameters.AddWithValue("@numero", processo.Numero ?? string.Empty);

            // Trata envio da data se ela estiver nula
            object valorData = processo.Data.HasValue
                ? processo.Data.Value.ToDateTime(TimeOnly.MinValue)
                : DBNull.Value;
            comando.Parameters.AddWithValue("@data", valorData);

            comando.Parameters.AddWithValue("@interessado", processo.Interessado ?? string.Empty);
            comando.Parameters.AddWithValue("@assunto", processo.Assunto ?? string.Empty);
            comando.Parameters.AddWithValue("@descricao", processo.Descricao ?? string.Empty);
            comando.Parameters.AddWithValue("@situacao", processo.Situacao ?? "Aberto");

            comando.ExecuteNonQuery();
        }
    }
}