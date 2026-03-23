using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace SAMWELLPOS.MVVM.Models
{
    [Table("Users")]
    public class UserModel
    {
        // PrimaryKey: The unique ID for the database
        // AutoIncrement: SQLite will handle the numbering (1, 2, 3...) for us
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // Unique: Ensures no two users can use the same email
        [Unique]
        public string? Email { get; set; }

        public string? FullName { get; set; }

        public string? Password { get; set; }

        // We'll use this to differentiate "Admin" and "Cashier"
        public string? Role { get; set; }

        // This is the "Account Approved" toggle we designed in the UI
        public bool IsApproved { get; set; }

        // Optional: Store the path to the profile picture we added earlier
        public string? ProfilePicturePath { get; set; }

        [Ignore]
        public string Initials
        {
            get
            {
                if (string.IsNullOrWhiteSpace(FullName)) return "?";
                var parts = FullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return parts.Length >= 2
                    ? $"{parts[0][0]}{parts[1][0]}".ToUpper()
                    : parts[0][0].ToString().ToUpper();
            }
        }

        [Ignore]
        public Color AvatarColor => Role == "Admin"
        ? Color.FromArgb("#0f4c5c")  // LoginColor
        : Color.FromArgb("#586F7C"); // SecondaryColor

    }


}
