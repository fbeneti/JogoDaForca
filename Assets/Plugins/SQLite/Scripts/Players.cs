using SQLite4Unity3d;

public class Players  {

	[PrimaryKey, AutoIncrement]
	public int Id { get; set; }
	public string Username { get; set; }
	public string Email { get; set; }
	public string Password { get; set; }
	public int Avatar { get; set; }
	public int Victories { get; set; }
	public int Losses { get; set; }
	public int Diamonds { get; set; }

	public override string ToString ()
	{
		return string.Format ("[Players: Id={0}, Userame={1}, Email={2}, Password={3}, Avatar={4}, Victories={5}, Losses={6}, Diamonds={7}]", Id, Username, Email, Password, Avatar, Victories, Losses, Diamonds);
	}
}
