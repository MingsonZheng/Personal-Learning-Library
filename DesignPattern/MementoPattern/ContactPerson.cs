namespace MementoPattern
{
    /// <summary>
    /// 联系人
    /// </summary>
    public class ContactPerson
    {
        public string Name { get; set; }

        public string PhoneNumber { get; set; }

        public ContactPerson(string name, string phoneNumber)
        {
            Name = name;
            PhoneNumber = phoneNumber;
        }
    }
}