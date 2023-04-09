using MongoDB.Bson;
using MongoDB.Driver;

namespace LB165_App;

internal class Program
{
    static void Main(string[] args)
    {
        // Verbindung zur MongoDB-Instanz herstellen
        MongoClient client = new MongoClient("mongodb://localhost:27017");
        IMongoDatabase database = client.GetDatabase("LB165");
        IMongoCollection<BsonDocument> games = database.GetCollection<BsonDocument>("games");

        // Create-Operation: Ein neues Spiel einfügen
        var data = new BsonDocument {
            {"appid", 12345},
            {"name", "Testspiel"},
            {"positive", 100},
            {"negative", 10}
        };
        games.InsertOne(data);
        Console.WriteLine("--------------------------------------------");
        Console.WriteLine("Ein neues Spiel wurde hinzugefügt. (appid: 12345)");
        Console.WriteLine("--------------------------------------------");
        
        // Read-Operation: Spiele mit mehr als 100.000 positiven Bewertungen finden
        var filter = Builders<BsonDocument>.Filter.Gt("positive", 100000);
        var result = games.Find(filter).ToList();
        Console.WriteLine($"Spiele mit mehr als 100.000 positiven Bewertungen: {result.Count}");
        Console.WriteLine("--------------------------------------------");

        // Update-Operation: Die Anzahl der positiven Bewertungen für ein Spiel erhöhen
        var updateFilter = Builders<BsonDocument>.Filter.Eq("appid", 12345);
        var update = Builders<BsonDocument>.Update.Inc("positive", 11);
        var projection = Builders<BsonDocument>.Projection.Include("name").Include("appid").Include("positive").Exclude("_id"); 
        var foundFirstDocument = games.Find(updateFilter).FirstOrDefault();
        
        if (foundFirstDocument != null)
        {
            Console.WriteLine("Name: " + foundFirstDocument["name"]);
            Console.WriteLine("AppID: " + foundFirstDocument["appid"]);
            Console.WriteLine("Positive Bewertungen: " + foundFirstDocument["positive"]);
            Console.WriteLine("--------------------------------------------");
        }
        else
        {
            Console.WriteLine("Kein Dokument mit der angegebenen AppID gefunden.");
        }
        games.UpdateOne(updateFilter, update);
        Console.WriteLine("Beim Dokument mit der appid 12345 wurden die positiven Bewertungen um 11 erhöht");
        Console.WriteLine("--------------------------------------------");
        var foundSecondDocument = games.Find(updateFilter).Project(projection).FirstOrDefault();
        if (foundSecondDocument != null)
        {
            Console.WriteLine("Name: " + foundSecondDocument["name"]);
            Console.WriteLine("AppID: " + foundSecondDocument["appid"]);
            Console.WriteLine("Positive Bewertungen: " + foundSecondDocument["positive"]);
            Console.WriteLine("--------------------------------------------");
        }
        else
        {
            Console.WriteLine("Kein Dokument mit der angegebenen AppID gefunden.");
            Console.WriteLine("--------------------------------------------");
        }

        // Delete-Operation: Ein Spiel löschen
        var deleteFilter = Builders<BsonDocument>.Filter.Lt("appid", 12345);
        games.DeleteOne(deleteFilter);
        Console.WriteLine("Das Spiel mit der AppID 12345 wurde gelöscht.");
        Console.WriteLine("--------------------------------------------");
    }
}