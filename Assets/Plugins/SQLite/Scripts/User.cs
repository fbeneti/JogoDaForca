public class User
{
	public int Id { get; set; }
	public string Username { get; set; }
	public string Email { get; set; }
	public string Password { get; set; }
	public int Permission { get; set; }

	public override string ToString ()
	{
		return string.Format ("[User: Id={0}, Userame={1}, Email={2}, Password={3}, Permission={4}]", Id, Username, Email, Password, Permission);
	}
}