﻿using System.ComponentModel.DataAnnotations;

namespace DreamyShop.Common.Exceptions
{
    public enum ErrorCodes
    {
        [Display(Description = "Credentials Is Invalid!")]
        CredentialsInvalid = 1,

        [Display(Description = "An Unknown Error Occured!")]
        OopsSomethingHapped,

        [Display(Description = "Access Token Is InValid!")]
        AccessTokenIsInValid,

        [Display(Description = "Access Token Is Misssing!")]
        AccessTokenIsMissing,

        [Display(Description = "Unauthenticated To Access This Data!")]
        UserIsUnauthenticated,

        [Display(Description = "Unauthorized To Access This Data!")]
        UserIsUnauthorized,

        [Display(Description = "Duplicated Data Found!")]
        DuplicatedData,

        [Display(Description = "User Is Not Owner Of The Data Entry!")]
        UserIsNotOwner,

        [Display(Description = "Data Entry Cannot Be Found!")]
        DataEntryIsNotExisted,

        [Display(Description = "Data Entry Is Not Valid!")]
        DataEntryIsNotValid,

        [Display(Description = "Data Validation Is Failed!")]
        FailedValidationData,

        [Display(Description = "Cannot Create Booking In The Past!")]
        CannotCreateBookingInThePast,

        [Display(Description = "Data Already In Use And Cannot Be Deleted!")]
        DataAlreadyInUseAndCannotBeDeleted,

        [Display(Description = "Cannot Delete Order Because Belong To Service!")]
        CannotDeleteOrderBecauseBelongToService,

        [Display(Description = "Cannot Delete!")]
        DeleteFailed,

        [Display(Description = "Upload Failed")]
        UploadFailed,
    }
}