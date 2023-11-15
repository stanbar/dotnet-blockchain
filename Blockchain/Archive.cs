using System.Text.Json;

namespace Blockchain;

public class Archive<Tx>
where Tx : ITransaction
{

    public readonly string archivePath;
    public Archive(string archivePath)
    {
        this.archivePath = archivePath;
        Directory.CreateDirectory(archivePath);
    }

    public bool IsEmpty()
    {
        return Directory.GetFiles(archivePath).Length == 0;
    }
    public IEnumerable<Block<Tx>> LoadBlocksFromFile()
    {
        foreach (var file in Directory.GetFiles(archivePath).OrderBy(x => x))
        {
            string json = File.ReadAllText(file);
            var block = JsonSerializer.Deserialize<Block<Tx>>(json) ?? throw new Exception($"Failed to deserialize block {file}");
            yield return block;
        }
    }
    public void SaveBlockToFile(Block<Tx> block, long height)
    {
        string fileName = Path.Combine(archivePath, $"{height}-{block.ToHash()}.json");
        string json = JsonSerializer.Serialize(block, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(fileName, json);
    }
}