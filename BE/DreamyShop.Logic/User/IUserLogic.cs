﻿using DreamyShop.Common.Results;
using DreamyShop.Domain.Shared.Dtos;
using DreamyShop.Domain.Shared.Dtos.User;
using DreamyShop.Logic.Conditions;

namespace DreamyShop.Logic.User
{
    public interface IUserLogic
    {
        Task<ApiResult<PageResult<UserDto>>> GetAllUser(PagingRequest pagingRequest);
        Task<ApiResult<bool>> UpdateUser(int userId, UserUpdateDto userUpdateDto);
        Task<ApiResult<IList<UserDto>>> Search(SearchUserCondition condition);
        Task<ApiResult<bool>> DeleteUser(string userId);
    }
}