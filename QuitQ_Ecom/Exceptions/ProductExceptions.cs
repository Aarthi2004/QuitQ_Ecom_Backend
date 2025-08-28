using System;

namespace QuitQ_Ecom.Repository.Exceptions
{
    public class AddProductException : Exception
    {
        public AddProductException(string message) : base(message) { }
    }

    public class GetProductsBySubCategoryException : Exception
    {
        public GetProductsBySubCategoryException(string message) : base(message) { }
    }

    public class GetProductByIdException : Exception
    {
        public GetProductByIdException(string message) : base(message) { }
    }

    public class UpdateProductException : Exception
    {
        public UpdateProductException(string message) : base(message) { }
    }

    public class DeleteProductException : Exception
    {
        public DeleteProductException(string message) : base(message) { }
    }

    public class CheckProductQuantityException : Exception
    {
        public CheckProductQuantityException(string message) : base(message) { }
    }

    public class UpdateProductQuantityException : Exception
    {
        public UpdateProductQuantityException(string message) : base(message) { }
    }

    public class SearchProductException : Exception
    {
        public SearchProductException(string message) : base(message) { }
    }

    public class GetAllProductsException : Exception
    {
        public GetAllProductsException(string message) : base(message) { }
    }

    public class GetAllProductsByStoreIdException : Exception
    {
        public GetAllProductsByStoreIdException(string message) : base(message) { }
    }

    public class FilterProductsException : Exception
    {
        public FilterProductsException(string message) : base(message) { }
    }
}