using System;

namespace QuitQ_Ecom.Exceptions
{
    // Exception for when a city is not found
    public class CityNotFoundException : Exception
    {
        public CityNotFoundException(string message) : base(message) { }
        public CityNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }

    // Exception for when a state is not found
    public class StateNotFoundException : Exception
    {
        public StateNotFoundException(string message) : base(message) { }
        public StateNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }

    // Exception for when an error occurs while getting all cities
    public class GetAllCitiesException : Exception
    {
        public GetAllCitiesException(string message) : base(message) { }
        public GetAllCitiesException(string message, Exception innerException) : base(message, innerException) { }
    }

    // Exception for when an error occurs while adding a city
    public class AddCityException : Exception
    {
        public AddCityException(string message) : base(message) { }
        public AddCityException(string message, Exception innerException) : base(message, innerException) { }
    }

    // Exception for when an error occurs while deleting a city
    public class DeleteCityException : Exception
    {
        public DeleteCityException(string message) : base(message) { }
        public DeleteCityException(string message, Exception innerException) : base(message, innerException) { }
    }

    // Exception for when an error occurs while updating a city's state
    public class UpdateCityStateException : Exception
    {
        public UpdateCityStateException(string message) : base(message) { }
        public UpdateCityStateException(string message, Exception innerException) : base(message, innerException) { }
    }
}