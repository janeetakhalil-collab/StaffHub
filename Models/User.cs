namespace StaffDb.Models;

public class User
{
    public int Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;
}

//CREATE TABLE Users
//(
    //Id INT IDENTITY(1,1) PRIMARY KEY,
    //Username NVARCHAR(MAX) NOT NULL,
   // PasswordHash NVARCHAR(MAX) NOT NULL
//);