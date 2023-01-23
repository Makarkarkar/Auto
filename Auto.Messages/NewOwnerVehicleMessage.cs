namespace Auto.Messages;

public class NewOwnerVehicleMessage
{
    public string ModelCode { get; set; }
    public int Year { get; set; }
    public string Registration { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }

    public NewOwnerVehicleMessage(){}
    public NewOwnerVehicleMessage(NewOwnerMessage owner, string modelCode, int year, string registration)
    {
        this.ModelCode = modelCode;
        this.Year = year;
        this.Registration = registration;
        this.Name = owner.Name;
        this.Surname = owner.Surname;
        this.Email = owner.Email;
        this.Age = owner.Age;
    }
    
}