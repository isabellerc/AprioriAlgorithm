using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        // Menu para seleção do dataset
        Console.WriteLine("Qual dataset você quer avaliar?");
        Console.WriteLine("1 - Dataset 1");
        Console.WriteLine("2 - Dataset 2");
        Console.WriteLine("3 - Dataset 3");
        int choice = int.Parse(Console.ReadLine());

        // Caminho completo dos arquivos de dataset
        string filePath = choice switch
        {
            1 => @"C:\Users\isabe\OneDrive\Área de Trabalho\AprioriAlgorithm\AprioriAlgorithm\AprioriAlgorithm\Data\data2.csv",
            2 => @"C:\Users\isabe\OneDrive\Área de Trabalho\AprioriAlgorithm\AprioriAlgorithm\AprioriAlgorithm\Data\data3.csv",
            3 => @"C:\Users\isabe\OneDrive\Área de Trabalho\AprioriAlgorithm\AprioriAlgorithm\AprioriAlgorithm\Data\data6.csv",
            _ => throw new ArgumentException("Escolha inválida.")
        };

        // Carregar transações do arquivo
        List<List<string>> transactions = LoadDataset(filePath);

        // Obter os itens únicos
        var uniqueItems = GetUniqueItems(transactions);

        // Exibir itens únicos
        Console.WriteLine("Itens Únicos:");
        foreach (var item in uniqueItems)
        {
            Console.WriteLine(item);
        }

        // Gerar dados binários
        var binaryData = GetBinaryData(transactions, uniqueItems);

        // Exibir dados binários
        Console.WriteLine("\nDados Binários:");
        foreach (var binary in binaryData)
        {
            Console.WriteLine(string.Join(", ", binary));
        }

        // Definir o suporte e confiança mínima
        double minSupport = 0.5;
        double minConfidence50 = 0.5;
        double minConfidence75 = 0.75;

        // Gerar regras de associação com suporte 50% e confiança 50%
        var associationRules50 = GenerateAssociationRules(binaryData, uniqueItems, minSupport, minConfidence50);

        // Exibir as regras de associação com suporte 50% e confiança 50%
        Console.WriteLine("\nRegras com suporte 50% e confiança 50%:");
        foreach (var rule in associationRules50)
        {
            Console.WriteLine($"Antecedente: {string.Join(", ", rule.Item1)} -> Consequente: {string.Join(", ", rule.Item2)} | Suporte: {rule.Item3}, Confiança: {rule.Item4}");
        }

        // Gerar regras de associação com suporte 50% e confiança 75%
        var associationRules75 = GenerateAssociationRules(binaryData, uniqueItems, minSupport, minConfidence75);

        // Exibir as regras de associação com suporte 50% e confiança 75%
        Console.WriteLine("\nRegras com suporte 50% e confiança 75%:");
        foreach (var rule in associationRules75)
        {
            Console.WriteLine($"Antecedente: {string.Join(", ", rule.Item1)} -> Consequente: {string.Join(", ", rule.Item2)} | Suporte: {rule.Item3}, Confiança: {rule.Item4}");
        }
    }

    static List<List<string>> LoadDataset(string filePath)
    {
        var transactions = new List<List<string>>();
        try
        {
            foreach (var line in File.ReadAllLines(filePath))
            {
                var items = line.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                transactions.Add(items);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erro ao carregar o dataset: " + ex.Message);
        }
        return transactions;
    }

    // Função para obter os itens únicos
    static List<string> GetUniqueItems(List<List<string>> transactions)
    {
        return transactions.SelectMany(t => t).Distinct().ToList();
    }

    // Função para gerar os dados binários
    static List<List<int>> GetBinaryData(List<List<string>> transactions, List<string> uniqueItems)
    {
        var binaryData = new List<List<int>>();
        foreach (var transaction in transactions)
        {
            var binaryTransaction = uniqueItems.Select(item => transaction.Contains(item) ? 1 : 0).ToList();
            binaryData.Add(binaryTransaction);
        }
        return binaryData;
    }

    // Função para calcular o suporte
    static double CalculateSupport(List<List<int>> binaryData, List<int> itemset)
    {
        int totalTransactions = binaryData.Count;
        int count = binaryData.Count(transaction => itemset.All(item => transaction[item] == 1));
        return (double)count / totalTransactions;
    }

    // Função para gerar as regras de associação
    static List<Tuple<List<string>, List<string>, double, double>> GenerateAssociationRules(
        List<List<int>> binaryData,
        List<string> uniqueItems,
        double minSupport,
        double minConfidence)
    {
        var rules = new List<Tuple<List<string>, List<string>, double, double>>();

        // Gerar regras de associação para todos os subconjuntos de itens
        for (int i = 0; i < uniqueItems.Count; i++)
        {
            for (int j = i + 1; j < uniqueItems.Count; j++)
            {
                var antecedent = new List<string> { uniqueItems[i] };
                var consequent = new List<string> { uniqueItems[j] };

                // Calcular o suporte
                var itemset = new List<int> { i, j };
                double support = CalculateSupport(binaryData, itemset);

                if (support >= minSupport)
                {
                    // Calcular a confiança
                    double confidence = support / CalculateSupport(binaryData, new List<int> { i });

                    // Adicionar regra se a confiança for maior ou igual ao mínimo
                    if (confidence >= minConfidence)
                    {
                        rules.Add(new Tuple<List<string>, List<string>, double, double>(
                            antecedent,
                            consequent,
                            support,
                            confidence
                        ));
                    }
                }
            }
        }

        return rules;
    }
}
