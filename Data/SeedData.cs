using System;
using SportCenter.Models;

namespace SportCenter.Data
{
    public static class SeedData
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Trenerlarni qo'shish
            var trainers = new[]
            {
                new Trainer { FirstName = "Jamshid", LastName = "Karimov", Age = 35, MonthlyFee = 1500000 },
                new Trainer { FirstName = "Dilshod", LastName = "Rahimov", Age = 28, MonthlyFee = 1200000 },
                new Trainer { FirstName = "Sherzod", LastName = "Toshmatov", Age = 32, MonthlyFee = 1400000 },
                new Trainer { FirstName = "Rustam", LastName = "Qodirov", Age = 30, MonthlyFee = 1300000 },
                new Trainer { FirstName = "Jahongir", LastName = "Azimov", Age = 29, MonthlyFee = 1250000 }
            };

            foreach (var trainer in trainers)
            {
                if (!context.Trainers.Any(t => t.FirstName == trainer.FirstName && t.LastName == trainer.LastName))
                {
                    context.Trainers.Add(trainer);
                }
            }
            context.SaveChanges();

            // A'zolarni qo'shish
            var members = new[]
            {
                new Member { FirstName = "Abror", LastName = "Solijonov", PhoneNumber = "+998901234567", DateOfBirth = new DateTime(1995, 5, 15), Address = "Chilonzor tumani", RegistrationDate = DateTime.Now, IsActive = true },
                new Member { FirstName = "Sardor", LastName = "Aliyev", PhoneNumber = "+998902345678", DateOfBirth = new DateTime(1998, 8, 20), Address = "Yunusobod tumani", RegistrationDate = DateTime.Now, IsActive = true },
                new Member { FirstName = "Umid", LastName = "Rahmonov", PhoneNumber = "+998903456789", DateOfBirth = new DateTime(1993, 3, 10), Address = "Mirzo Ulug'bek tumani", RegistrationDate = DateTime.Now, IsActive = true },
                new Member { FirstName = "Zafar", LastName = "Usmonov", PhoneNumber = "+998904567890", DateOfBirth = new DateTime(1997, 11, 25), Address = "Yashnobod tumani", RegistrationDate = DateTime.Now, IsActive = true },
                new Member { FirstName = "Bobur", LastName = "Kamolov", PhoneNumber = "+998905678901", DateOfBirth = new DateTime(1996, 7, 8), Address = "Shayxontohur tumani", RegistrationDate = DateTime.Now, IsActive = true },
                new Member { FirstName = "Aziz", LastName = "Mahmudov", PhoneNumber = "+998906789012", DateOfBirth = new DateTime(1994, 9, 30), Address = "Olmazor tumani", RegistrationDate = DateTime.Now, IsActive = true },
                new Member { FirstName = "Jasur", LastName = "Qodirov", PhoneNumber = "+998907890123", DateOfBirth = new DateTime(1999, 4, 12), Address = "Sergeli tumani", RegistrationDate = DateTime.Now, IsActive = true },
                new Member { FirstName = "Farrux", LastName = "Tojiyev", PhoneNumber = "+998908901234", DateOfBirth = new DateTime(1992, 12, 5), Address = "Mirobod tumani", RegistrationDate = DateTime.Now, IsActive = true },
                new Member { FirstName = "Jahongir", LastName = "Saidov", PhoneNumber = "+998909012345", DateOfBirth = new DateTime(1991, 6, 18), Address = "Bektemir tumani", RegistrationDate = DateTime.Now, IsActive = true },
                new Member { FirstName = "Akmal", LastName = "Yusupov", PhoneNumber = "+998900123456", DateOfBirth = new DateTime(1990, 2, 28), Address = "Uchtepa tumani", RegistrationDate = DateTime.Now, IsActive = true }
            };

            foreach (var member in members)
            {
                if (!context.Members.Any(m => m.PhoneNumber == member.PhoneNumber))
                {
                    // Tasodifiy trener biriktirish
                    var randomTrainer = context.Trainers.OrderBy(r => Guid.NewGuid()).First();
                    member.TrainerId = randomTrainer.Id;
                    context.Members.Add(member);
                }
            }
            context.SaveChanges();
        }
    }
}
