// Benchmarks automatizados para queries críticas usando Dapper
using Xunit;
using System.Diagnostics;
using Dapper;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DapperBenchmarkTests
{
    private const string ConnectionString = "Data Source=localhost;Initial Catalog=PDV;Integrated Security=True";
    private const int Iterations = 50; // Ajuste para volume representativo

    private static string ResultReportPath => "core/tests/DapperBenchmarkResults.txt";

    [Fact(DisplayName = "Benchmark Produtos - SELECT *")]
    public void Benchmark_Produtos_SelectAll()
    {
        var query = "SELECT * FROM Produtos";
        var result = RunBenchmark(query, "Produtos");
        Assert.True(result.P95 < 500, $"Consulta Produtos P95={result.P95}ms excede o limite recomendado.");
        AppendResult(result);
    }

    [Fact(DisplayName = "Benchmark Estoque - SELECT *")]
    public void Benchmark_Estoque_SelectAll()
    {
        var query = "SELECT * FROM Estoques";
        var result = RunBenchmark(query, "Estoque");
        Assert.True(result.P95 < 500, $"Consulta Estoque P95={result.P95}ms excede o limite recomendado.");
        AppendResult(result);
    }

    [Fact(DisplayName = "Benchmark Vendas - SELECT *")]
    public void Benchmark_Vendas_SelectAll()
    {
        var query = "SELECT * FROM Vendas";
        var result = RunBenchmark(query, "Venda");
        Assert.True(result.P95 < 500, $"Consulta Vendas P95={result.P95}ms excede o limite recomendado.");
        AppendResult(result);
    }

    [Fact(DisplayName = "Benchmark Venda + ItensVenda JOIN")]
    public void Benchmark_Venda_ItensVenda_Join()
    {
        var query = @"
            SELECT v.*, i.*
            FROM Vendas v
            INNER JOIN ItensVenda i ON i.VendaId = v.Id";
        var result = RunBenchmark(query, "Venda+ItensVenda JOIN");
        Assert.True(result.P95 < 800, $"Consulta Venda+ItensVenda JOIN P95={result.P95}ms excede o limite recomendado.");
        AppendResult(result);
    }

    private BenchmarkResult RunBenchmark(string query, string label)
    {
        var timings = new List<long>();
        int rowCount = 0;

        using var connection = new SqlConnection(ConnectionString);
        connection.Open();

        for (int i = 0; i < Iterations; i++)
        {
            var sw = Stopwatch.StartNew();
            var result = connection.Query(query).ToList();
            sw.Stop();
            timings.Add(sw.ElapsedMilliseconds);
            if (i == 0) rowCount = result.Count;
        }

        timings.Sort();
        var avg = timings.Average();
        var p95 = timings[(int)(Iterations * 0.95) - 1];
        var max = timings.Max();
        var min = timings.Min();
        var throughput = Iterations / (timings.Sum() / 1000.0);

        return new BenchmarkResult
        {
            Label = label,
            Iterations = Iterations,
            RowCount = rowCount,
            Avg = avg,
            P95 = p95,
            Min = min,
            Max = max,
            Throughput = throughput,
            Timestamp = DateTime.Now
        };
    }

    private void AppendResult(BenchmarkResult result)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"[{result.Timestamp:yyyy-MM-dd HH:mm:ss}] Benchmark: {result.Label}");
        sb.AppendLine($"  Iterações: {result.Iterations}");
        sb.AppendLine($"  Rows (primeira execução): {result.RowCount}");
        sb.AppendLine($"  Média: {result.Avg:F2}ms | P95: {result.P95}ms | Min: {result.Min}ms | Max: {result.Max}ms");
        sb.AppendLine($"  Throughput: {result.Throughput:F2} ops/s");
        sb.AppendLine();

        System.IO.File.AppendAllText(ResultReportPath, sb.ToString());
    }

    private class BenchmarkResult
    {
        public string Label { get; set; }
        public int Iterations { get; set; }
        public int RowCount { get; set; }
        public double Avg { get; set; }
        public long P95 { get; set; }
        public long Min { get; set; }
        public long Max { get; set; }
        public double Throughput { get; set; }
        public DateTime Timestamp { get; set; }
    }
}

// Instruções:
// - Execute os testes com volume realista de dados nas tabelas.
// - Os resultados serão salvos em core/tests/DapperBenchmarkResults.txt.
// - Analise P95, throughput e outliers para identificar gargalos.
// - Sugestões de otimização: criar índices, revisar joins, paginar queries, evitar SELECT * em produção.