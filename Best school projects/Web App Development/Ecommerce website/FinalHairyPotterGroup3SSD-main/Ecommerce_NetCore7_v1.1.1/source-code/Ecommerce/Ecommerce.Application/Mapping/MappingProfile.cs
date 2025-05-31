using AutoMapper;
using Ecommerce.Application.Dto;
using Ecommerce.Application.Handlers.Categories.Commands;
using Ecommerce.Application.Handlers.Colors.Commands;
using Ecommerce.Application.Handlers.Configuration.Commands;
using Ecommerce.Application.Handlers.ContactQueries.Commands;
using Ecommerce.Application.Handlers.Customers.Commands;
using Ecommerce.Application.Handlers.DeliveryMethods.Commands;
using Ecommerce.Application.Handlers.OrderStatusValues.Commands;
using Ecommerce.Application.Handlers.ProductReviews.Commands;
using Ecommerce.Application.Handlers.Products.Commands;
using Ecommerce.Application.Handlers.Sizes.Commands;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Identity.Entities;
using Ecommerce.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Application.Mapping;
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        #region Category
        CreateMap<CreateCategoryCommand, Category>();
        CreateMap<UpdateCategoryCommand, Category>();
        CreateMap<UpdateCategoryCommand, CategoryDto>().ReverseMap();
        CreateMap<CategoryDto, Category>().ReverseMap();
        #endregion

        #region Product
        CreateMap<CreateProductCommand, Product>();
        CreateMap<ProductForEditDto, Product>();
        CreateMap<ProductDto, Product>().ReverseMap();
        #endregion

        #region Color
        CreateMap<CreateColorCommand, Color>();
        CreateMap<UpdateColorCommand, Color>();
        CreateMap<UpdateColorCommand, ColorDto>().ReverseMap();
        CreateMap<ColorDto, Color>().ReverseMap();
        #endregion

        #region Size
        CreateMap<CreateSizeCommand, Size>();
        CreateMap<UpdateSizeCommand, Size>();
        CreateMap<UpdateSizeCommand, SizeDto>().ReverseMap();
        CreateMap<SizeDto, Size>().ReverseMap();
        #endregion

        #region OrderStatusValue
        CreateMap<CreateOrderStatusValueCommand, OrderStatusValue>();
        CreateMap<UpdateOrderStatusValueCommand, OrderStatusValue>();
        CreateMap<UpdateOrderStatusValueCommand, OrderStatusValueDto>().ReverseMap();
        CreateMap<OrderStatusValueDto, OrderStatusValue>().ReverseMap();
        #endregion

        #region DeliveryMethod
        CreateMap<CreateDeliveryMethodCommand, DeliveryMethod>();
        CreateMap<UpdateDeliveryMethodCommand, DeliveryMethod>();
        CreateMap<UpdateDeliveryMethodCommand, DeliveryMethodDto>().ReverseMap();
        CreateMap<DeliveryMethodDto, DeliveryMethod>().ReverseMap();
        #endregion

        #region Configuration
        CreateMap<GeneralConfigurationDto, GeneralConfiguration>().ReverseMap();
        CreateMap<UpdateGeneralConfigurationCommand, GeneralConfiguration>().ReverseMap();

        CreateMap<SocialProfileDto, SocialProfile>().ReverseMap();
        CreateMap<UpdateSocialConfigurationCommand, SocialProfileDto>().ReverseMap();

        CreateMap<HeaderSliderDto, HeaderSlider>().ReverseMap();
        CreateMap<DealOfTheDayDto, DealOfTheDay>().ReverseMap();
        CreateMap<BannerDto, Banner>().ReverseMap();
        CreateMap<UpdateDealOfTheDayConfigCommand, DealOfTheDayDto>().ReverseMap();

        CreateMap<FeatureProductConfigurationDto, FeatureProductConfiguration>().ReverseMap();

        CreateMap<StockConfigurationDto, StockConfiguration>().ReverseMap();
        CreateMap<UpdateStockConfigurationCommand, StockConfigurationDto>().ReverseMap();

        CreateMap<TopCategoriesConfiguration, TopCategoriesConfigurationDto>().ReverseMap();

        CreateMap<BasicSeoConfiguration, BasicSeoConfigurationDto>().ReverseMap();

        CreateMap<SmtpConfiguration, SmtpConfigurationDto>().ReverseMap();
        CreateMap<UpdateSmtpConfigurationCommand, SmtpConfigurationDto>().ReverseMap();

        CreateMap<SecurityConfiguration, SecurityConfigurationDto>().ReverseMap();
        CreateMap<UpdateSecurityConfigurationCommand, SecurityConfigurationDto>().ReverseMap();

        CreateMap<AdvancedConfiguration, AdvancedConfigurationDto>().ReverseMap();
        CreateMap<UpdateAdvancedConfigurationCommand, AdvancedConfigurationDto>().ReverseMap();
        #endregion

        CreateMap<ContactQueryDto, ContactQuery>().ReverseMap();
        CreateMap<CreateContactQueryCommand, ContactQueryDto>().ReverseMap();
        CreateMap<CreateContactQueryCommand, ContactQuery>().ReverseMap();

        CreateMap<GalleryDto, Gallery>().ReverseMap();

        CreateMap<OrderDto, Order>().ReverseMap();
        CreateMap<OrderDetailsDto, OrderDetails>().ReverseMap();
        CreateMap<OrderStatusDto, OrderStatus>().ReverseMap();

        CreateMap<CreateProductReviewByCustomerCommand, CustomerReview>();
        CreateMap<CreateReplyForCustomerReviewCommand, CustomerReview>();
        CreateMap<ProductReviewDto, CustomerReview>().ReverseMap();

        CreateMap<ApplicationUser, EditProfileDto>().ReverseMap();

        CreateMap<IdentityRole, RoleDto>();
        CreateMap<IdentityRole, AddEditRoleDto>().ReverseMap();

        CreateMap<StockDto, Stock>().ReverseMap();

        CreateMap<ApplicationUser, UserDto>();
        CreateMap<ApplicationUser, AddEditUserDto>().ReverseMap();
        CreateMap<IdentityRole, ManageRoleDto>();

        CreateMap<CustomerDto, Customer>().ReverseMap();
        CreateMap<UpdateCustomerCommand, CustomerDto>().ReverseMap();
        CreateMap<UpdateCustomerCommand, Customer>();
        //CreateMap<User, CustomerRegisterDto>().ReverseMap();

        CreateMap<ApplicationUser, UserDto>()
            .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Customer)).ReverseMap();


    }
}