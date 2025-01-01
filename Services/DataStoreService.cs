using System.Diagnostics;
using Newtonsoft.Json;
using SofyTrender.Models;

namespace SofyTrender.Services
{
    internal class DataStoreService
    {
        const string PlaylistFileName = "playlists.json";
        const string CredentialsFileName = "credentials.json";

        public static async Task<CredentialsData> LoadCredentials()
        {
            var result = await DoLoad<CredentialsData>(CredentialsFileName);
            return result;
        }

        public static void SaveCredentials(CredentialsData data)
        {
            DoSave(data, CredentialsFileName);
        }

        public static async Task<PlaylistSaveData[]> LoadPlaylists()
        {
            var result = await DoLoad<PlaylistSaveData[]>(PlaylistFileName);
            return result;
        }

        public static void SavePlaylists(PlaylistSaveData[] data)
        {
            DoSave(data, PlaylistFileName);
        }

        static async Task<T> DoLoad<T>(string fileName)
        {
            T rv = default;
            try
            {
                string path = Path.Combine(FileSystem.AppDataDirectory, fileName);
                if (!File.Exists(path))
                {
                    return rv;
                }
                using (StreamReader sr = File.OpenText(path))
                {
                    var text = await sr.ReadToEndAsync();
                    rv = JsonConvert.DeserializeObject<T>(text);
                    return rv;
                }
            } catch (FileNotFoundException ex)
            {
                Debug.WriteLine($"{ex.Message}");
            }
            return rv;
        }

        static void DoSave(Object data, string fileName)
        {
            try
            {
                string path = Path.Combine(FileSystem.AppDataDirectory, fileName);
                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.WriteLine(JsonConvert.SerializeObject(data));
                }
                Debug.WriteLine("saved to: " + path);
            } catch (FileNotFoundException ex)
            {
                Debug.WriteLine($"{ex.Message}");
            }
        }
    }
}
