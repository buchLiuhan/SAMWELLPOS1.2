using SAMWELLPOS.MVVM.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace SAMWELLPOS.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection? _db;

        // This runs every time we want to talk to the DB to ensure it's "awake"
        private async Task Init()
        {
            if (_db is not null)
                return;

            // Secure local path on the mobile device
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "SamwellPOS.db3");

            _db = new SQLiteAsyncConnection(dbPath);

            // This is the "Magic Line" that reads your UserModel and builds the table
            await _db.CreateTableAsync<UserModel>();
        }

        // --- USER OPERATIONS ---

        public async Task<int> AddUser(UserModel user)
        {
            await Init();
            return await _db!.InsertAsync(user);
        }

        public async Task<List<UserModel>> GetUsers()
        {
            await Init();
            return await _db!.Table<UserModel>().ToListAsync();
        }

        public async Task<UserModel> GetUserByEmail(string email)
        {
            await Init();
            return await _db!.Table<UserModel>().Where(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task<int> UpdateUser(UserModel user)
        {
            await Init();
            return await _db!.UpdateAsync(user);
        }

        public async Task<int> DeleteUser(UserModel user)
        {
            await Init();
            return await _db!.DeleteAsync(user);
        }
    }
}

