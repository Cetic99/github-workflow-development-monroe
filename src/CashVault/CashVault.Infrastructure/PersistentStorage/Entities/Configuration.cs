using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace CashVault.Infrastructure.PersistentStorage
{
    public class Configuration
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Key { get; set; }
        [Required]
        public string Value { get; set; }

        public Configuration(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public JsonDocument GetJson()
        {
            return JsonDocument.Parse(Value);
        }
    }
}
