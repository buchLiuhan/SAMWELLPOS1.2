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
            await _db.CreateTableAsync<ProductModel>(); // ← add this line here
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

        public async Task<UserModel> GetUserByUsername(string username)
        {
            await Init();
            return await _db!.Table<UserModel>()
                .Where(u => u.Username == username)
                .FirstOrDefaultAsync();
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

        public async Task<UserModel> GetUserById(int id)
        {
            await Init();
            return await _db!.Table<UserModel>().Where(u => u.Id == id).FirstOrDefaultAsync();
        }

        // --- PRODUCT OPERATIONS ---

        public async Task<int> AddProduct(ProductModel product)
        {
            await Init();
            return await _db!.InsertAsync(product);
        }

        public async Task<List<ProductModel>> GetProducts()
        {
            await Init();
            return await _db!.Table<ProductModel>().ToListAsync();
        }

        public async Task<ProductModel> GetProductById(int id)
        {
            await Init();
            return await _db!.Table<ProductModel>().Where(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<int> UpdateProduct(ProductModel product)
        {
            await Init();
            return await _db!.UpdateAsync(product);
        }

        public async Task<int> DeleteProduct(ProductModel product)
        {
            await Init();
            return await _db!.DeleteAsync(product);
        }
    }
}

