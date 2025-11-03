using System.Security.Cryptography;
using System.Text;

namespace Highdmin.Services
{
    public interface IPasswordHashService
    {
        string HashPassword(string password, string salt);
        string GenerateSalt();
        bool VerifyPassword(string password, string hash, string salt);
    }

    public class PasswordHashService : IPasswordHashService
    {
        public string HashPassword(string password, string salt)
        {
            Console.WriteLine("Hashing password for user...");
            Console.WriteLine("Password: " + password);
            
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("La contraseña no puede estar vacía", nameof(password));
            Console.WriteLine("Salt: " + salt);
            if (string.IsNullOrEmpty(salt))
                throw new ArgumentException("El salt no puede estar vacío", nameof(salt));
            Console.WriteLine("Generating hash...");
            using var sha256 = SHA256.Create();
            var saltedPassword = password + salt;
            var saltedPasswordBytes = Encoding.UTF8.GetBytes(saltedPassword);
            var hashBytes = sha256.ComputeHash(saltedPasswordBytes);
            Console.WriteLine("Hash generated: " + Convert.ToBase64String(hashBytes));
            return Convert.ToBase64String(hashBytes);
        }

        public string GenerateSalt()
        {
            using var rng = RandomNumberGenerator.Create();
            var saltBytes = new byte[32];
            rng.GetBytes(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }

        public bool VerifyPassword(string password, string hash, string salt)
        {
           if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash) || string.IsNullOrEmpty(salt))
                return false;

            var computedHash = HashPassword(password, salt);

            // Convertir ambos a bytes antes de comparar
            var computedBytes = Encoding.UTF8.GetBytes(computedHash);
            var storedBytes = Encoding.UTF8.GetBytes(hash);

            // Comparación segura en tiempo constante
            return CryptographicOperations.FixedTimeEquals(computedBytes, storedBytes);
        }
    }
}