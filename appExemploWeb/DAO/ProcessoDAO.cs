using appExemploWeb.Configs;
using AppWebExemplo.Configs;
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

                processo.Numero = DAOHelper.GetString(reader, "numero_pro");
                processo.Interessado = DAOHelper.GetString(reader, "interessado_pro");
                processo.Assunto = DAOHelper.GetString(reader, "assunto_pro");
                processo.Descricao = DAOHelper.GetString(reader, "descricao_pro");
                processo.Situacao = DAOHelper.IsNull(reader, "situacao_pro")
                    ? "Aberto"
                    : DAOHelper.GetString(reader, "situacao_pro");

                DateTime? dataBanco = DAOHelper.GetDateTime(reader, "data_pro");
                processo.Data = dataBanco.HasValue
                    ? DateOnly.FromDateTime(dataBanco.Value)
                    : null;

                lista.Add(processo);
            }

            return lista;
        }

        public Processo? ObterPorId(int id)
        {
            using var con = _conexao.GetConnection();
            const string sql = @"SELECT id_pro, numero_pro, data_pro, interessado_pro,
                                        assunto_pro, descricao_pro, situacao_pro
                                 FROM processos
                                 WHERE id_pro = @id";

            using var comando = con.CreateCommand();
            comando.CommandText = sql;
            comando.Parameters.AddWithValue("@id", id);

            using var reader = comando.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            DateTime? dataBanco = DAOHelper.GetDateTime(reader, "data_pro");

            return new Processo
            {
                Id = reader.GetInt32(reader.GetOrdinal("id_pro")),
                Numero = DAOHelper.GetString(reader, "numero_pro"),
                Data = dataBanco.HasValue ? DateOnly.FromDateTime(dataBanco.Value) : null,
                Interessado = DAOHelper.GetString(reader, "interessado_pro"),
                Assunto = DAOHelper.GetString(reader, "assunto_pro"),
                Descricao = DAOHelper.GetString(reader, "descricao_pro"),
                Situacao = DAOHelper.IsNull(reader, "situacao_pro")
                    ? "Aberto"
                    : DAOHelper.GetString(reader, "situacao_pro")
            };
        }

        public void Excluir(int id)
        {
            using var con = _conexao.GetConnection();
            const string sql = "DELETE FROM processos WHERE id_pro = @id";

            using var comando = con.CreateCommand();
            comando.CommandText = sql;
            comando.Parameters.AddWithValue("@id", id);
            comando.ExecuteNonQuery();
        }

        public void Atualizar(Processo processo)
        {
            using var con = _conexao.GetConnection();
            const string sql = @"UPDATE processos
                                 SET numero_pro = @numero,
                                     data_pro = @data,
                                     interessado_pro = @interessado,
                                     assunto_pro = @assunto,
                                     descricao_pro = @descricao,
                                     situacao_pro = @situacao
                                 WHERE id_pro = @id";

            using var comando = con.CreateCommand();
            comando.CommandText = sql;
            comando.Parameters.AddWithValue("@numero", processo.Numero ?? string.Empty);
            comando.Parameters.AddWithValue("@data", processo.Data.HasValue
                ? processo.Data.Value.ToDateTime(TimeOnly.MinValue)
                : DBNull.Value);
            comando.Parameters.AddWithValue("@interessado", processo.Interessado ?? string.Empty);
            comando.Parameters.AddWithValue("@assunto", processo.Assunto ?? string.Empty);
            comando.Parameters.AddWithValue("@descricao", processo.Descricao ?? string.Empty);
            comando.Parameters.AddWithValue("@situacao", processo.Situacao ?? "Aberto");
            comando.Parameters.AddWithValue("@id", processo.Id);
            comando.ExecuteNonQuery();
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
