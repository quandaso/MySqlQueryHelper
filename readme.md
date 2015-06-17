#MySql Query Helper (for .NET/c#)

**-Installation**

	- Download and install MySql Connector at https://dev.mysql.com/downloads/connector/net/6.9.html
	- Add MySql.Data as a reference

**-SELECT statement:**

	var db = new MySqlQueryHelper("Server=localhost;Port=3306;Database=test;Uid=root;Pwd=;charset=utf8;Convert Zero Datetime=True");
    var users = db.Select("SELECT * FROM `users`");

    foreach (var u in users)
    {
		Console.WriteLine(u["id"]);
        Console.WriteLine(u["email"]);
    }
	// Using parameters
	var users = db.Select("SELECT * FROM `users` WHERE `id`=@id", new {id = 1});
	Console.WriteLine(users[0]["id"]);

**-SELECT COUNT result**

	int userCount = db.Count("SELECT COUNT(*) FROM `users`");

**-INSERT/UPDATE/DELETE records**

	int effectedRow = db.Insert("INSERT INTO `users`(`username`,`email`) VALUES(@username,@email)", new {username = "abc", email="abc@abc.com" );
	effectedRow = db.Update("UPDATE `users` SET `email`=@email WHERE `id`=@id", new {id = 1, email="test@abc.com");
	effectedRow = db.Delete("DELETE FROM `users` WHERE `id`=@id", new {id = 1 );

**-Using transaction**

	var transaction = db.Connection.BeginTransaction();  
	int effectedRow1 = db.Update("UPDATE `users` SET `email`=@email WHERE `id`=@id", new {id = 1, email="test@abc.com");
	int effectedRow2 = db.Update("UPDATE `profiles` SET `company`=@company WHERE `user_id`=@userId", new {userId = 1, company="abc");
	
	if (effectedRow1 > 0 && effectedRow2 > 0)
	{
		transaction.Commit();	
	} else 
	{
		transaction.Rollback();
	}

	
