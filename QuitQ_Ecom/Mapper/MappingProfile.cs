using AutoMapper;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Models;

namespace QuitQ_Ecom.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Simple DTO ↔ Model mappings
            CreateMap<Gender, GenderDTO>().ReverseMap();
            CreateMap<User, UserDTO>().ReverseMap();

            // Explicitly define both directions for ProductDetail and Image
            CreateMap<ProductDetail, ProductDetailDTO>();
            CreateMap<ProductDetailDTO, ProductDetail>();

            CreateMap<Image, ImageDTO>();
            CreateMap<ImageDTO, Image>();
            CreateMap<Store, StoreDTO>().ReverseMap();
            CreateMap<Category, CategoryDTO>().ReverseMap();

            // Assuming SubCategory and Product models exist for these DTOs
            CreateMap<SubCategory, SubCategoryDTO>().ReverseMap();
            CreateMap<WishList, WishListDTO>().ReverseMap();
            CreateMap<Cart, CartDTO>();
            CreateMap<CartDTO, Cart>()
               .ForMember(dest => dest.Product, opt => opt.Ignore());

            // The new, explicit mapping for Product -> ProductDTO
            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName))
                .ForMember(dest => dest.ProductImage, opt => opt.MapFrom(src => src.ProductImage))
                .ForMember(dest => dest.StoreId, opt => opt.MapFrom(src => src.StoreId))
                .ForMember(dest => dest.ProductStatusId, opt => opt.MapFrom(src => src.ProductStatusId))
                .ForMember(dest => dest.BrandId, opt => opt.MapFrom(src => src.BrandId))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.SubCategoryId, opt => opt.MapFrom(src => src.SubCategoryId))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.ProductDetails, opt => opt.MapFrom(src => src.ProductDetails))
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images))
                .ForMember(dest => dest.ProductImageFile, opt => opt.Ignore())
                .ForMember(dest => dest.sellerName, opt => opt.Ignore())
                .ForMember(dest => dest.BrandName, opt => opt.Ignore())
                .ForMember(dest => dest.StoreName, opt => opt.Ignore())
                .ForMember(dest => dest.CategoryName, opt => opt.Ignore())
                .ForMember(dest => dest.SubCategoryName, opt => opt.Ignore());

            // Your ProductDTO -> Product mapping (which was already explicit and safe)
            CreateMap<ProductDTO, Product>()
               .ForMember(dest => dest.Brand, opt => opt.Ignore())
               .ForMember(dest => dest.Category, opt => opt.Ignore())
               .ForMember(dest => dest.ProductStatus, opt => opt.Ignore())
               .ForMember(dest => dest.Store, opt => opt.Ignore())
               .ForMember(dest => dest.SubCategory, opt => opt.Ignore())
               .ForMember(dest => dest.Carts, opt => opt.Ignore())
               .ForMember(dest => dest.Images, opt => opt.Ignore())
               .ForMember(dest => dest.ProductDetails, opt => opt.Ignore())
               .ForMember(dest => dest.OrderItems, opt => opt.Ignore())
               .ForMember(dest => dest.WishLists, opt => opt.Ignore());

            CreateMap<BrandDTO, Brand>()
            .ForMember(dest => dest.BrandLogo, opt => opt.MapFrom(src => src.BrandLogo));

            // Mapping from Model to DTO
            CreateMap<Brand, BrandDTO>()
                 .ForMember(dest => dest.BrandLogoImg, opt => opt.Ignore()); // Ignore file during mapping

            // Add these two lines for State and City mappings
            CreateMap<State, StateDTO>().ReverseMap();
            CreateMap<City, CityDTO>().ReverseMap();
            CreateMap<UserAddress, UserAddressDTO>().ReverseMap();
            CreateMap<Order, OrderDTO>().ReverseMap();
            CreateMap<OrderItem, OrderItemDTO>().ReverseMap();
            CreateMap<Payment, PaymentDTO>().ReverseMap();
            CreateMap<Shipper, ShipperDTO>().ReverseMap();
        }
    }
}